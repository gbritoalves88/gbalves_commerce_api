// Application/DTOs/Produtos/CriarProdutoRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Produtos;

public record CriarProdutoRequest(
    string Nome,
    string? Descricao,
    decimal Preco,
    decimal? PrecoOriginal,
    string? ImagemUrl,
    int Estoque,
    bool Destaque,
    Guid? CategoriaId
);