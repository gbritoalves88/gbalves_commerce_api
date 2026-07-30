// Application/DTOs/Categorias/AtualizarCategoriaRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Categorias;

public record AtualizarCategoriaRequest(
    string Nome,
    string? ImagemUrl,
    int OrdemExibicao,
    bool Ativo
);