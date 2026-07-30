// Application/DTOs/Pedidos/CriarPedidoRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Pedidos;

public record CriarPedidoRequest(
    string ClienteNome,
    string ClienteEmail,
    string? ClienteTelefone,
    string? ClienteCpf,
    string? Cep,
    string? Rua,
    string? Numero,
    string? Complemento,
    string? Bairro,
    string? Cidade,
    string? Estado,
    string FormaPagamento,
    List<ItemPedidoRequest> Itens
);

public record ItemPedidoRequest(
    Guid ProdutoId,
    int Quantidade
);