// Application/DTOs/Produtos/CriarProdutoRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Produtos;

// Application/DTOs/Produtos/CriarProdutoRequest.cs
public record CriarProdutoRequest(
    string Nome,
    string? Descricao,
    decimal Preco,
    decimal? PrecoOriginal,
    string? ImagemUrl,
    int Estoque,
    bool Destaque,
    Guid? CategoriaId,
    decimal Peso = 0.3m,
    decimal Altura = 10,
    decimal Largura = 15,
    decimal Comprimento = 20
);