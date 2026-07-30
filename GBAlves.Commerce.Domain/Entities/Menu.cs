// Domain/Entities/Menu.cs
namespace GBAlves.Commerce.Domain.Entities;

public class Menu
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Localizacao { get; set; } = string.Empty; // 'header', 'footer'
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<ItemMenu> Itens { get; set; } = new List<ItemMenu>();
}