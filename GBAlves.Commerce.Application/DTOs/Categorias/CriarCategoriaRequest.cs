// Application/DTOs/Categorias/CriarCategoriaRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Categorias;

public record CriarCategoriaRequest(
    string Nome,
    string? ImagemUrl,
    int OrdemExibicao
);