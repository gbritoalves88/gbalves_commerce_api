// Domain/Entities/Cliente.cs
namespace GBAlves.Commerce.Domain.Entities;

public class Cliente : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Cpf { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}