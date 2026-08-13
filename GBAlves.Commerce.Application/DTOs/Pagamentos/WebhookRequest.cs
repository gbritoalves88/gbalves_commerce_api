// Application/DTOs/Pagamentos/WebhookRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Pagamentos;

public record WebhookRequest(
    string Id,           // ID da notificação do Mercado Pago
    string? Topic,       // payment, merchant_order
    string? Action,      // payment.created, payment.updated
    DateTime? Timestamp  // Timestamp da notificação
);