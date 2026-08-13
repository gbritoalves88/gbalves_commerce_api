// Application/DTOs/Pedidos/PedidoResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Pedidos;

public record PedidoResponse(
    Guid Id,
    Guid? ClienteId,
    int NumeroPedido,
    string Status,
    decimal Subtotal,
    decimal Frete,
    decimal Total,
    string? FormaPagamento,
    string? StatusPagamento,
    string? ClienteNome,
    string? ClienteEmail,       // ← novo
    string? ClienteTelefone,    // ← novo
    string? Cep,                // ← novo
    string? Rua,                // ← novo
    string? Numero,             // ← novo
    string? Complemento,        // ← novo
    string? Bairro,             // ← novo
    string? Cidade,             // ← novo
    string? Estado,             // ← novo
    DateTime CriadoEm,
    List<ItemPedidoResponse> Itens,
    string? PagamentoId = null,
    string? GatewayPagamentoId = null,
    string? CodigoRastreio = null,
    string? Transportadora = null,
    string? ServicoFrete = null
);
public record ItemPedidoResponse(
    Guid Id,
    string NomeProduto,
    string? ImagemProduto,
    decimal Preco,
    int Quantidade,
    decimal Total
);