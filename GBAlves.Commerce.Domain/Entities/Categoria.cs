// Domain/Entities/Categoria.cs
namespace GBAlves.Commerce.Domain.Entities;

public class Categoria : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ImagemUrl { get; set; }
    public bool Ativo { get; set; } = true;
    public int OrdemExibicao { get; set; }

    // Relacionamento
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}