// Application/DTOs/Produtos/ProdutoResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Produtos;

public record ProdutoResponse(
    Guid Id,
    string Nome,
    string Slug,
    string? Descricao,
    decimal Preco,
    decimal? PrecoOriginal,
    string? ImagemUrl,
    int Estoque,
    bool Destaque,
    bool Ativo,
    string? CategoriaNome,
    DateTime CriadoEm
);