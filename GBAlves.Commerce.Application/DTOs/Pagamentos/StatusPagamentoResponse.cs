// Application/DTOs/Pagamentos/StatusPagamentoResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Pagamentos;

public record StatusPagamentoResponse(
    string PagamentoId,
    string Status,
    string Metodo,
    decimal Valor,
    DateTime? ConfirmadoEm
);