// Application/Services/PagamentoService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using GBAlves.Commerce.Application.DTOs.Pagamentos;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Domain.Enums;
using GBAlves.Commerce.Infrastructure.Data;
using MercadoPago.Client.Payment;
using MercadoPago.Config;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace GBAlves.Commerce.Application.Services;

public class PagamentoService : IPagamentoService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PagamentoService> _logger;

    public PagamentoService(AppDbContext context, IConfiguration configuration, ILogger<PagamentoService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    private void ConfigurarToken()
    {
        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"]!;
    }

    // ========== NOVO: Validação de assinatura do webhook ==========
    public bool ValidarAssinaturaWebhook(WebhookRequest request, string? signature, string? requestId)
    {
        // Se não tem assinatura, rejeita
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(requestId))
        {
            _logger.LogWarning("Webhook sem assinatura ou request-id");
            return false;
        }

        var webhookSecret = _configuration["MercadoPago:WebhookSecret"] 
                ?? Environment.GetEnvironmentVariable("MERCADOPAGO_WEBHOOK_SECRET");
        if (string.IsNullOrEmpty(webhookSecret))
        {
            _logger.LogError("WebhookSecret não configurado!");
            return false;
        }

        // Template oficial do Mercado Pago
        var parts = new List<string>();
        
        if (!string.IsNullOrEmpty(request.Id))
            parts.Add($"id:{request.Id}");
        
        parts.Add($"request-id:{requestId}");
        
        if (request.Timestamp.HasValue)
            parts.Add($"ts:{request.Timestamp.Value:yyyy-MM-dd'T'HH:mm:ss}");
        
        var template = string.Join(";", parts);

        _logger.LogDebug("Validando assinatura. Template: {Template}", template);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(template));
        var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

        var isValid = computedSignature == signature.ToLowerInvariant();
        
        if (!isValid)
            _logger.LogWarning("Assinatura inválida! Esperada: {Expected}, Recebida: {Received}", 
                computedSignature, signature);

        return isValid;
    }

    public async Task<CriarCheckoutResponse> CriarCheckoutAsync(CriarCheckoutRequest request)
    {
        ConfigurarToken();

        var pedido = await _context.Pedidos
            .Include(p => p.Itens)
            .Include(p => p.Pagamentos)
            .FirstOrDefaultAsync(p => p.Id == request.PedidoId);

        if (pedido is null)
            throw new Exception("Pedido não encontrado");

        // ========== IDEMPOTÊNCIA: Verificar se já existe pagamento ativo ==========
        var pagamentoExistente = pedido.Pagamentos?
            .Where(p => p.Status == StatusPagamento.Pendente || 
                        p.Status == StatusPagamento.Aprovado)
            .OrderByDescending(p => p.CriadoEm)
            .FirstOrDefault();

        if (pagamentoExistente is not null)
        {
            _logger.LogWarning("Pedido {PedidoId} já possui pagamento ativo! PagamentoId: {PagamentoId}, Status: {Status}", 
                pedido.Id, pagamentoExistente.Id, pagamentoExistente.Status);

            // Se for PIX pendente, retorna o QR Code existente
            if (pagamentoExistente.Metodo == MetodoPagamento.Pix && 
                pagamentoExistente.Status == StatusPagamento.Pendente)
            {
                var clientExistente = new PaymentClient();
                var mpPagamentoExistente = await clientExistente.GetAsync(
                    long.Parse(pagamentoExistente.GatewayPagamentoId!));
                
                var qrCode = mpPagamentoExistente?.PointOfInteraction?.TransactionData?.QrCode ?? "";
                var qrCodeBase64 = mpPagamentoExistente?.PointOfInteraction?.TransactionData?.QrCodeBase64 ?? "";
                
                if (!string.IsNullOrEmpty(qrCode) || !string.IsNullOrEmpty(qrCodeBase64))
                {
                    return new CriarCheckoutResponse(
                        pagamentoExistente.Id.ToString(),
                        "pending",
                        qrCode,
                        qrCodeBase64
                    );
                }
            }

            // Retorna status atual
            return new CriarCheckoutResponse(
                pagamentoExistente.Id.ToString(),
                pagamentoExistente.Status.ToString().ToLowerInvariant()
            );
        }

        // ========== CRIAR NOVO PAGAMENTO ==========
        var paymentRequest = new PaymentCreateRequest
        {
            TransactionAmount = pedido.Total,
            Description = $"Pedido #{pedido.NumeroPedido}",
            PaymentMethodId = request.MetodoPagamento,
            DateOfExpiration = DateTime.UtcNow.AddHours(2),
            ExternalReference = pedido.Id.ToString(),
            Payer = new PaymentPayerRequest
            {
                Email = request.EmailComprador,
                FirstName = request.NomeComprador ?? "Comprador"
            }
        };

        if (request.MetodoPagamento != "pix" && !string.IsNullOrEmpty(request.Token))
        {
            paymentRequest.Token = request.Token;
            paymentRequest.Installments = request.Parcelas ?? 1;
        }

        _logger.LogInformation("📤 Enviando pagamento: {Metodo} | Valor: {Valor:C}", 
            request.MetodoPagamento, pedido.Total);

        var client = new PaymentClient();
        var payment = await client.CreateAsync(paymentRequest);

        _logger.LogInformation("📥 Resposta: {Status} - {Detalhe}", 
            payment.Status, payment.StatusDetail);

        var metodo = request.MetodoPagamento == "pix" ? MetodoPagamento.Pix : MetodoPagamento.CartaoCredito;

        var pagamento = new Pagamento
        {
            Id = Guid.NewGuid(),
            PedidoId = pedido.Id,
            Gateway = "mercadopago",
            GatewayPagamentoId = payment.Id.ToString(),
            Metodo = metodo,
            Status = MapearStatusMercadoPago(payment.Status),
            Valor = pedido.Total,
            ConfirmadoEm = payment.Status == "approved" ? DateTime.UtcNow : null
        };

        _context.Pagamentos.Add(pagamento);

        if (payment.Status == "approved")
            pedido.Status = StatusPedido.Preparando;

        await _context.SaveChangesAsync();
        await RegistrarEventoAsync(pagamento.Id, "PagamentoCriado", new { payment.Id, payment.Status });

        // ========== NOVO CONTRATO ==========
        var status = payment.Status?.ToLowerInvariant() ?? "pending";
        var qrCodePix = payment.PointOfInteraction?.TransactionData?.QrCode ?? "";
        var qrCodeBase64Pix = payment.PointOfInteraction?.TransactionData?.QrCodeBase64 ?? "";

        return new CriarCheckoutResponse(
            pagamento.Id.ToString(),
            status,
            qrCodePix,
            qrCodeBase64Pix
        );
    }

    public async Task ConfirmarPagamentoAsync(string pagamentoId)
    {
        ConfigurarToken();

        _logger.LogInformation("🔍 Buscando pagamento: {PagamentoId}", pagamentoId);

        var pagamento = await _context.Pagamentos
            .FirstOrDefaultAsync(p => p.Id.ToString() == pagamentoId || p.GatewayPagamentoId == pagamentoId);

        if (pagamento is null) 
        { 
            _logger.LogWarning("Pagamento não encontrado: {PagamentoId}", pagamentoId); 
            return; 
        }

        _logger.LogInformation("✅ Gateway ID: {GatewayId}", pagamento.GatewayPagamentoId);

        var client = new PaymentClient();
        var mpPayment = await client.GetAsync(long.Parse(pagamento.GatewayPagamentoId!));

        _logger.LogInformation("📥 Status MP: {Status}", mpPayment?.Status);

        // ========== CORRIGIDO: Mapeamento correto de status ==========
        var novoStatus = MapearStatusMercadoPago(mpPayment?.Status);
        
        if (novoStatus != pagamento.Status)
        {
            pagamento.Status = novoStatus;
            
            if (novoStatus == StatusPagamento.Aprovado)
            {
                pagamento.ConfirmadoEm = DateTime.UtcNow;
                
                var pedido = await _context.Pedidos.FindAsync(pagamento.PedidoId);
                if (pedido is not null)
                {
                    pedido.Status = StatusPedido.Preparando;
                    _logger.LogInformation("📦 Pedido #{Numero} → Preparando", pedido.NumeroPedido);
                }
                
                await _context.SaveChangesAsync();
                await RegistrarEventoAsync(pagamento.Id, "PagamentoAprovado", new { Id = mpPayment.Id, Status = "approved" });
                _logger.LogInformation("🎉 Pagamento APROVADO!");
            }
            else if (novoStatus == StatusPagamento.Recusado)
            {
                await _context.SaveChangesAsync();
                await RegistrarEventoAsync(pagamento.Id, "PagamentoRecusado", new { Id = mpPayment.Id, Status = mpPayment.Status });
                _logger.LogWarning("❌ Pagamento RECUSADO: {Detalhe}", mpPayment?.StatusDetail);
            }
            else
            {
                await _context.SaveChangesAsync();
                await RegistrarEventoAsync(pagamento.Id, "PagamentoAtualizado", new { Id = mpPayment?.Id, Status = mpPayment?.Status });
                _logger.LogInformation("🔄 Status atualizado: {Status}", novoStatus);
            }
        }
    }

    public async Task CancelarPagamentoAsync(string pagamentoId)
    {
        ConfigurarToken();

        var pagamento = await _context.Pagamentos.FindAsync(Guid.Parse(pagamentoId));
        if (pagamento is null) return;

        var client = new PaymentClient();
        await client.CancelAsync(long.Parse(pagamento.GatewayPagamentoId!));

        pagamento.Status = StatusPagamento.Cancelado;
        await _context.SaveChangesAsync();
        await RegistrarEventoAsync(pagamento.Id, "PagamentoCancelado", null);
        _logger.LogInformation("🚫 Pagamento cancelado");
    }

    public async Task<StatusPagamentoResponse> ObterStatusAsync(string pagamentoId)
    {
        var pagamento = await _context.Pagamentos.FindAsync(Guid.Parse(pagamentoId));
        if (pagamento is null) throw new Exception("Pagamento não encontrado");

        return new StatusPagamentoResponse(pagamento.Id.ToString(), pagamento.Status.ToString(),
            pagamento.Metodo.ToString(), pagamento.Valor, pagamento.ConfirmadoEm);
    }

    // ========== MÉTODO AUXILIAR: Mapeamento de status ==========
    private StatusPagamento MapearStatusMercadoPago(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "approved" => StatusPagamento.Aprovado,
            "pending" => StatusPagamento.Pendente,
            "in_process" => StatusPagamento.Pendente,
            "rejected" => StatusPagamento.Recusado,
            "cancelled" => StatusPagamento.Cancelado,
            "refunded" => StatusPagamento.Reembolsado,
            "charged_back" => StatusPagamento.Estornado,
            _ => StatusPagamento.Pendente
        };
    }

    // ========== CORRIGIDO: Async + Logger ==========
    private async Task RegistrarEventoAsync(Guid pagamentoId, string evento, object? dados)
    {
        _context.EventosPagamento.Add(new EventoPagamento
        {
            Id = Guid.NewGuid(),
            PagamentoId = pagamentoId,
            Evento = evento,
            Dados = dados is not null ? JsonSerializer.Serialize(dados) : null
        });
        await _context.SaveChangesAsync();
    }
}