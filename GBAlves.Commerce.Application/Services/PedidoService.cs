// Application/Services/PedidoService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Pedidos;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Domain.Enums;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly AppDbContext _context;

    public PedidoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PedidoResponse>> ObterTodos()
    {
        return await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => MapToResponse(p))
            .ToListAsync();
    }

    public async Task<PedidoResponse?> ObterPorId(Guid id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido is null ? null : MapToResponse(pedido);
    }

    public async Task<PedidoResponse> Criar(CriarPedidoRequest request)
    {
        // Criar ou encontrar cliente
        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Email == request.ClienteEmail);

        if (cliente is null)
        {
            cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = request.ClienteNome,
                Email = request.ClienteEmail,
                Telefone = request.ClienteTelefone,
                Cpf = request.ClienteCpf
            };
            _context.Clientes.Add(cliente);
        }

        // Criar pedido
        var pedido = new Pedido
        {
            Id = Guid.NewGuid(),
            ClienteId = cliente.Id,
            Status = StatusPedido.Pendente,
            Cep = request.Cep,
            Rua = request.Rua,
            Numero = request.Numero,
            Complemento = request.Complemento,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Estado = request.Estado,
            FormaPagamento = request.FormaPagamento,
            StatusPagamento = "pendente",
            Itens = new List<ItemPedido>()
        };

        decimal subtotal = 0;

        foreach (var itemRequest in request.Itens)
        {
            var produto = await _context.Produtos.FindAsync(itemRequest.ProdutoId);
            if (produto is null || produto.Estoque < itemRequest.Quantidade)
                throw new Exception($"Produto {itemRequest.ProdutoId} indisponível");

            var item = new ItemPedido
            {
                Id = Guid.NewGuid(),
                PedidoId = pedido.Id,
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                ImagemProduto = produto.ImagemUrl,
                Preco = produto.Preco,
                Quantidade = itemRequest.Quantidade,
                Total = produto.Preco * itemRequest.Quantidade
            };

            subtotal += item.Total;
            produto.Estoque -= itemRequest.Quantidade;
            pedido.Itens.Add(item);
        }

        pedido.Subtotal = subtotal;
        pedido.Frete = subtotal >= 199 ? 0 : 15;
        pedido.Total = pedido.Subtotal + pedido.Frete;

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        return MapToResponse(pedido);
    }

    public async Task<bool> AtualizarStatus(Guid id, string novoStatus)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido is null) return false;

        if (Enum.TryParse<StatusPedido>(novoStatus, true, out var status))
        {
            pedido.Status = status;
            pedido.AtualizadoEm = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    private static PedidoResponse MapToResponse(Pedido p)
    {
        return new PedidoResponse(
            p.Id,
            p.NumeroPedido,
            p.Status.ToString(),
            p.Subtotal,
            p.Frete,
            p.Total,
            p.FormaPagamento,
            p.StatusPagamento,
            p.Cliente?.Nome,
            p.CriadoEm,
            p.Itens.Select(i => new ItemPedidoResponse(
                i.Id, i.NomeProduto, i.ImagemProduto,
                i.Preco, i.Quantidade, i.Total
            )).ToList()
        );
    }
}