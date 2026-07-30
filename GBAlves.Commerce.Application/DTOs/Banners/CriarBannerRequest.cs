// Application/DTOs/Banners/CriarBannerRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Banners;

public record CriarBannerRequest(
    string Titulo,
    string? Subtitulo,
    string ImagemUrl,
    string? Link,
    string? TextoBotao,
    int OrdemExibicao
);