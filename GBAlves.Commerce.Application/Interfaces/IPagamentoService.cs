// Application/Interfaces/IPagamentoService.cs
using GBAlves.Commerce.Application.DTOs.Pagamentos;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IPagamentoService
{
    Task<CriarCheckoutResponse> CriarCheckoutAsync(CriarCheckoutRequest request);
    Task ConfirmarPagamentoAsync(string pagamentoId);
    Task CancelarPagamentoAsync(string pagamentoId);
    Task<StatusPagamentoResponse> ObterStatusAsync(string pagamentoId);
    bool ValidarAssinaturaWebhook(WebhookRequest request, string? signature, string? requestId);
}