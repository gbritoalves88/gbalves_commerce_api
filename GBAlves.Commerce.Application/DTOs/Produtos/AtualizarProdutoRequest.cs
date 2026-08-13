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
    Guid? CategoriaId,
    decimal Peso = 0.3m,
    decimal Altura = 10,
    decimal Largura = 15,
    decimal Comprimento = 20
);