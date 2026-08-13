// Application/DTOs/Menus/MenuResponse.cs
public record MenuResponse(
    Guid Id,
    string Nome,
    string Localizacao,
    bool Ativo,                    // ← adicionar
    List<ItemMenuResponse> Itens
);

public record ItemMenuResponse(
    Guid Id,
    string Nome,
    string? Slug,
    string? Url,
    string? Icone,
    int OrdemExibicao,
    bool Ativo,                    // ← adicionar
    List<ItemMenuResponse> SubItens
);