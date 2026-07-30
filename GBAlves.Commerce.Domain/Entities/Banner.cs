// Domain/Entities/Banner.cs
namespace GBAlves.Commerce.Domain.Entities;

public class Banner
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string ImagemUrl { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? TextoBotao { get; set; }
    public int OrdemExibicao { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}