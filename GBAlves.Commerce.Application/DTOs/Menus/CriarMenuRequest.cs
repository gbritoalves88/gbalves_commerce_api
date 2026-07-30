// Application/DTOs/Menus/CriarMenuRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Menus;

public record CriarMenuRequest(
    string Nome,
    string Localizacao
);

public record CriarItemMenuRequest(
    string Nome,
    string? Slug,
    string? Url,
    string? Icone,
    Guid? ItemPaiId,
    int OrdemExibicao
);