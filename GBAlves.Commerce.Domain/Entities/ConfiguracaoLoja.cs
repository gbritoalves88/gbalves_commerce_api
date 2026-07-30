// Domain/Entities/ConfiguracaoLoja.cs
namespace GBAlves.Commerce.Domain.Entities;

public class ConfiguracaoLoja
{
    public Guid Id { get; set; }
    public string NomeLoja { get; set; } = "Minha Loja";
    public string? Logo { get; set; }
    public string? Favicon { get; set; }
    public string CorPrimaria { get; set; } = "#1a237e";
    public string CorSecundaria { get; set; } = "#ff4081";
    public string? CorFundo { get; set; }
    public string? CorTexto { get; set; }
    public string? BannerDesktop { get; set; }
    public string? BannerMobile { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Whatsapp { get; set; }
    public string? Instagram { get; set; }
    public string? Facebook { get; set; }
    public string? SeoTitulo { get; set; }
    public string? SeoDescricao { get; set; }
    public string? SeoPalavras { get; set; }
    public string? GoogleAnalytics { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}