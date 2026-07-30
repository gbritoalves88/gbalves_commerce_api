// Application/DTOs/Produtos/AtualizarProdutoRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Produtos;

public record AtualizarProdutoRequest(
    string Nome,
    string? Descricao,
    decimal Preco,
    decimal? PrecoOriginal,
    string? ImagemUrl,
    int Estoque,
    bool Destaque,
    Guid? CategoriaId
);