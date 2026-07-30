// Application/DTOs/Categorias/CategoriaResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Categorias;

public record CategoriaResponse(
    Guid Id,
    string Nome,
    string Slug,
    string? ImagemUrl,
    int OrdemExibicao,
    bool Ativo,
    int TotalProdutos
);