// Domain/Entities/ItemMenu.cs
namespace GBAlves.Commerce.Domain.Entities;

public class ItemMenu
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public Menu Menu { get; set; } = null!;
    public string Nome { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Url { get; set; }
    public string? Icone { get; set; }
    public Guid? ItemPaiId { get; set; }
    public ItemMenu? ItemPai { get; set; }
    public int OrdemExibicao { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<ItemMenu> SubItens { get; set; } = new List<ItemMenu>();
}