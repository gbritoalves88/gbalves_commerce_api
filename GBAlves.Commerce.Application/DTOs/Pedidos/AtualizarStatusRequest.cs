// Application/DTOs/Pedidos/AtualizarStatusRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Pedidos;

public record AtualizarStatusRequest(
    string NovoStatus
);