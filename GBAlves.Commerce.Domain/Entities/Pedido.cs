// Domain/Entities/Pedido.cs
using GBAlves.Commerce.Domain.Enums;

namespace GBAlves.Commerce.Domain.Entities;

public class Pedido : EntidadeBase
{
    public int NumeroPedido { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.Criado;
    public decimal Subtotal { get; set; }
    public decimal Frete { get; set; }
    public decimal Total { get; set; }

    // Endereço
    public string? Cep { get; set; }
    public string? Rua { get; set; }
    public string? Numero { get; set; }
    public string? Complemento { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Transportadora { get; set; }
    public string? ServicoFrete { get; set; }
    public string? CodigoRastreio { get; set; }
    public DateTime? DataEnvio { get; set; }

    // Observação
    public string? Observacao { get; set; }

    // FK
    public Guid? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}