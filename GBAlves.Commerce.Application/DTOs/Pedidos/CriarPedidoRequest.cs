// Application/DTOs/Pedidos/CriarPedidoRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Pedidos;

public record CriarPedidoRequest(
    string? ClienteTelefone,    // Opcional - para atualizar
    string? ClienteCpf,         // Opcional - para atualizar
    string? Cep,
    string? Rua,
    string? Numero,
    string? Complemento,
    string? Bairro,
    string? Cidade,
    string? Estado,
    decimal Frete,
    string? Transportadora,
    string? ServicoFrete,
    List<ItemPedidoRequest> Itens
);

public record ItemPedidoRequest(
    Guid ProdutoId,
    int Quantidade
);