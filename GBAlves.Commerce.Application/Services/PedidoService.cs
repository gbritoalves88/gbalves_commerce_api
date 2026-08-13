using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using Microsoft.Extensions.Configuration;
using GBAlves.Commerce.Application.DTOs.Pedidos;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Domain.Enums;
using GBAlves.Commerce.Infrastructure.Data;
using GBAlves.Commerce.Application.DTOs.Fretes;

namespace GBAlves.Commerce.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<PedidoService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IFreteService _freteService;

    public PedidoService(AppDbContext context, IEmailService emailService, 
        ILogger<PedidoService> logger, IConfiguration configuration,
        IFreteService freteService)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
        _configuration = configuration;
        _freteService = freteService;
    }

    public async Task<List<PedidoResponse>> ObterTodos()
    {
        return await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .Include(p => p.Pagamentos)
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => MapToResponse(p))
            .ToListAsync();
    }

    public async Task<PedidoResponse?> ObterPorId(Guid id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .Include(p => p.Pagamentos)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido is null ? null : MapToResponse(pedido);
    }

    public async Task<PedidoResponse> Criar(CriarPedidoRequest request, Guid usuarioId)
    {
        // ========== 1. BUSCAR USUÁRIO AUTENTICADO ==========
        var usuario = await _context.Usuarios
            .Include(u => u.Cliente)
            .FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario is null)
            throw new InvalidOperationException("Usuário não encontrado");

        // ========== 2. VALIDAR E BUSCAR PRODUTOS ==========
        var produtosPedido = new List<(Produto Produto, int Quantidade)>();
        
        foreach (var itemRequest in request.Itens)
        {
            var produto = await _context.Produtos.FindAsync(itemRequest.ProdutoId);
            
            if (produto is null)
                throw new InvalidOperationException($"Produto {itemRequest.ProdutoId} não encontrado");
            
            if (produto.Estoque < itemRequest.Quantidade)
                throw new InvalidOperationException($"Produto '{produto.Nome}' indisponível. Estoque: {produto.Estoque}, Solicitado: {itemRequest.Quantidade}");

            produtosPedido.Add((produto, itemRequest.Quantidade));
        }

        // ========== 3. VALIDAR FRETE NO SERVIDOR ==========
        if (request.Frete < 0)
            throw new InvalidOperationException("Frete inválido!");

        if (!string.IsNullOrEmpty(request.Cep) && request.Frete > 0)
        {
            var fretesValidos = await _freteService.CalcularFreteAsync(new CalcularFreteRequest(
                request.Cep,
                produtosPedido.Select(p => new FreteItemRequest(p.Produto.Id, p.Quantidade)).ToList()
            ));

            var freteValido = fretesValidos.FirstOrDefault(f => 
                f.Valor == request.Frete && 
                f.Transportadora == request.Transportadora &&
                f.Nome == request.ServicoFrete
            );

            if (freteValido is null)
            {
                _logger.LogWarning("Frete inválido! UsuarioId: {UsuarioId}, Frete: {Frete}, Transportadora: {Transportadora}", 
                    usuarioId, request.Frete, request.Transportadora);
                throw new InvalidOperationException("Frete inválido! Recalcule o frete.");
            }
        }

        // ========== 4. OBTER OU CRIAR CLIENTE ==========
        var cliente = usuario.Cliente;

        if (cliente is null)
        {
            cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = request.ClienteTelefone,
                Cpf = request.ClienteCpf
            };
            _context.Clientes.Add(cliente);
            usuario.ClienteId = cliente.Id;
        }
        else
        {
            // Atualizar apenas se informado
            if (!string.IsNullOrWhiteSpace(request.ClienteTelefone))
                cliente.Telefone = request.ClienteTelefone;
            
            if (!string.IsNullOrWhiteSpace(request.ClienteCpf))
                cliente.Cpf = request.ClienteCpf;
        }

        // ========== 5. CRIAR PEDIDO ==========
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteId = cliente.Id,
            Status = StatusPedido.Criado,
            Cep = request.Cep,
            Rua = request.Rua,
            Numero = request.Numero,
            Complemento = request.Complemento,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Estado = request.Estado,
            Frete = request.Frete,
            Transportadora = request.Transportadora,
            ServicoFrete = request.ServicoFrete,
            Itens = new List<ItemPedido>()
        };

        // ========== 6. ADICIONAR ITENS E CALCULAR TOTAIS ==========
        decimal subtotal = 0;

        foreach (var (produto, quantidade) in produtosPedido)
        {
            var item = new ItemPedido
            {
                Id = Guid.NewGuid(),
                PedidoId = pedido.Id,
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                ImagemProduto = produto.ImagemUrl,
                Preco = produto.Preco,
                Quantidade = quantidade,
                Total = produto.Preco * quantidade
            };

            subtotal += item.Total;
            produto.Estoque -= quantidade;
            pedido.Itens.Add(item);
        }

        pedido.Subtotal = subtotal;
        pedido.Total = pedido.Subtotal + pedido.Frete;

        // ========== 7. SALVAR PEDIDO ==========
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        // ========== 8. ENVIAR EMAIL DE CONFIRMAÇÃO ==========
        try
        {
            await _emailService.EnviarConfirmacaoPedidoAsync(
                cliente.Email,
                pedido.NumeroPedido,
                pedido.Total
            );
            _logger.LogInformation("📧 Email enviado para {Email} - Pedido #{Numero}", 
                cliente.Email, pedido.NumeroPedido);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao enviar email de confirmação do pedido #{Numero}", 
                pedido.NumeroPedido);
        }

        return MapToResponse(pedido);
    }

    public async Task<bool> AtualizarStatus(Guid id, string novoStatus)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (pedido is null) return false;

        if (Enum.TryParse<StatusPedido>(novoStatus, true, out var status))
        {
            pedido.Status = status;
            pedido.AtualizadoEm = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Enviar email de atualização
            if (pedido.Cliente != null)
            {
                try
                {
                    await _emailService.EnviarStatusAtualizadoAsync(
                        pedido.Cliente.Email,
                        pedido.NumeroPedido,
                        status.ToString()
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"❌ ERRO EMAIL: {ex.Message}");
                    
                    if (ex.InnerException != null)
                        _logger.LogInformation($"   Inner: {ex.InnerException.Message}");
                    _logger.LogWarning(ex, "Erro ao enviar email de atualização");
                }
            }

            return true;
        }

        return false;
    }

    private static PedidoResponse MapToResponse(Pedido p)
    {
        var ultimoPagamento = p.Pagamentos?.OrderByDescending(pg => pg.CriadoEm).FirstOrDefault();

        return new PedidoResponse(
            p.Id,                                    // Guid Id
            p.ClienteId,                             // Guid? ClienteId
            p.NumeroPedido,                          // int NumeroPedido
            p.Status.ToString(),                     // string Status
            p.Subtotal,                              // decimal Subtotal
            p.Frete,                                 // decimal Frete
            p.Total,                                 // decimal Total
            ultimoPagamento?.Metodo.ToString(),      // string? FormaPagamento
            ultimoPagamento?.Status.ToString(),      // string? StatusPagamento
            p.Cliente?.Nome,                         // string? ClienteNome
            p.Cliente?.Email,                        // string? ClienteEmail
            p.Cliente?.Telefone,                     // string? ClienteTelefone
            p.Cep,                                   // string? Cep
            p.Rua,                                   // string? Rua
            p.Numero,                                // string? Numero
            p.Complemento,                           // string? Complemento
            p.Bairro,                                // string? Bairro
            p.Cidade,                                // string? Cidade
            p.Estado,                                // string? Estado
            p.CriadoEm,                              // DateTime CriadoEm
            p.Itens.Select(i => new ItemPedidoResponse(
                i.Id, i.NomeProduto, i.ImagemProduto,
                i.Preco, i.Quantidade, i.Total
            )).ToList(),                             // List<ItemPedidoResponse> Itens
            ultimoPagamento?.Id.ToString(),          // string? PagamentoId
            ultimoPagamento?.GatewayPagamentoId,     // string? GatewayPagamentoId
            p.CodigoRastreio,                        // string? CodigoRastreio
            p.Transportadora,                        // string? Transportadora
            p.ServicoFrete                           // string? ServicoFrete
        );
    }
    public async Task<bool> CancelarPedidoAsync(Guid id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .Include(p => p.Pagamentos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null) return false;

        // Não pode cancelar se já foi pago
        var temPagamentoAprovado = pedido.Pagamentos?
            .Any(p => p.Status == StatusPagamento.Aprovado) ?? false;

        if (temPagamentoAprovado)
            throw new InvalidOperationException("Pedido com pagamento aprovado não pode ser cancelado.");

        // Retorna produtos ao estoque
        foreach (var item in pedido.Itens)
        {
            var produto = await _context.Produtos.FindAsync(item.ProdutoId);
            if (produto is not null)
                produto.Estoque += item.Quantidade;
        }

        pedido.Status = StatusPedido.Cancelado;
        pedido.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<object?> VerificarPagamentoAsync(Guid pedidoId)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Pagamentos)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido is null) return null;

        var ultimoPagamento = pedido.Pagamentos?.OrderByDescending(p => p.CriadoEm).FirstOrDefault();
        if (ultimoPagamento is null) return new { status = "sem_pagamento" };

        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"]!;
        var client = new PaymentClient();
        var mpPayment = await client.GetAsync(long.Parse(ultimoPagamento.GatewayPagamentoId!));

        return new
        {
            status = mpPayment?.Status,
            detail = mpPayment?.StatusDetail,
            metodo = ultimoPagamento.Metodo.ToString(),
            valor = ultimoPagamento.Valor,
            gatewayId = ultimoPagamento.GatewayPagamentoId
        };
    }
}