// Application/DTOs/Menus/MenuResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Menus;

public record MenuResponse(
    Guid Id,
    string Nome,
    string Localizacao,
    List<ItemMenuResponse> Itens
);

public record ItemMenuResponse(
    Guid Id,
    string Nome,
    string? Slug,
    string? Url,
    string? Icone,
    int OrdemExibicao,
    List<ItemMenuResponse> SubItens
);