// Application/DTOs/Pedidos/PedidoResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Pedidos;

public record PedidoResponse(
    Guid Id,
    int NumeroPedido,
    string Status,
    decimal Subtotal,
    decimal Frete,
    decimal Total,
    string? FormaPagamento,
    string? StatusPagamento,
    string? ClienteNome,
    DateTime CriadoEm,
    List<ItemPedidoResponse> Itens
);

public record ItemPedidoResponse(
    Guid Id,
    string NomeProduto,
    string? ImagemProduto,
    decimal Preco,
    int Quantidade,
    decimal Total
);