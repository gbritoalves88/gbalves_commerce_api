// Application/DTOs/Banners/BannerResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Banners;

public record BannerResponse(
    Guid Id,
    string Titulo,
    string? Subtitulo,
    string ImagemUrl,
    string? Link,
    string? TextoBotao,
    int OrdemExibicao,
    bool Ativo
);