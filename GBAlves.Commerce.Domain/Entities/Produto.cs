// Domain/Entities/Produto.cs
namespace GBAlves.Commerce.Domain.Entities;

public class Produto : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public decimal? PrecoOriginal { get; set; }
    public string? ImagemUrl { get; set; }
    public int Estoque { get; set; }
    public bool Destaque { get; set; }
    public bool Ativo { get; set; } = true;
    public decimal Peso { get; set; }           // kg
    public decimal Altura { get; set; }         // cm
    public decimal Largura { get; set; }        // cm  
    public decimal Comprimento { get; set; }    // cm

    // FK
    public Guid? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}