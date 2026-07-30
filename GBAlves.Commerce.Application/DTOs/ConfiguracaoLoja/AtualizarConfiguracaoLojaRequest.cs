// Application/DTOs/ConfiguracaoLoja/AtualizarConfiguracaoLojaRequest.cs
namespace GBAlves.Commerce.Application.DTOs.ConfiguracaoLoja;

public record AtualizarConfiguracaoLojaRequest(
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