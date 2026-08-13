// Application/DTOs/Pagamentos/CriarCheckoutRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Pagamentos;

public record CriarCheckoutRequest(
    Guid PedidoId,
    string MetodoPagamento,        // "pix", "mastercard", "visa"
    string EmailComprador,
    string? NomeComprador = null,
    string? CpfComprador = null,
    string? Token = null,          // Token do cartão (SDK frontend)
    int? Parcelas = 1              // Número de parcelas
);

// NOVO CONTRATO: separa status, qrCode e qrCodeBase64
public record CriarCheckoutResponse(
    string PagamentoId,
    string Status,                 // "approved", "pending", "rejected", "cancelled"
    string? QrCode = null,         // QR Code (apenas PIX pendente)
    string? QrCodeBase64 = null    // QR Code em base64 (apenas PIX pendente)
);