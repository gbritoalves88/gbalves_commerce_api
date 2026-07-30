// Domain/Entities/ItemPedido.cs
namespace GBAlves.Commerce.Domain.Entities;

public class ItemPedido
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Pedido Pedido { get; set; } = null!;

    public Guid? ProdutoId { get; set; }
    public Produto? Produto { get; set; }

    // Snapshot do produto no momento da compra
    public string NomeProduto { get; set; } = string.Empty;
    public string? ImagemProduto { get; set; }
    public decimal Preco { get; set; }
    public int Quantidade { get; set; }
    public decimal Total { get; set; }
}