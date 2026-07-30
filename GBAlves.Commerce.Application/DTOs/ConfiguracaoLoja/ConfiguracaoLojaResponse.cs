// Application/DTOs/ConfiguracaoLoja/ConfiguracaoLojaResponse.cs
namespace GBAlves.Commerce.Application.DTOs.ConfiguracaoLoja;

public record ConfiguracaoLojaResponse(
    Guid Id,
    string NomeLoja,
    string? Logo,
    string? Favicon,
    string CorPrimaria,
    string CorSecundaria,
    string? Whatsapp,
    string? Instagram,
    string? Facebook,
    string? Email,
    string? SeoTitulo,
    string? SeoDescricao
);