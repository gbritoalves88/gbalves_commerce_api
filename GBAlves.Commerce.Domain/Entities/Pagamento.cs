// Domain/Entities/Pagamento.cs
using GBAlves.Commerce.Domain.Enums;

namespace GBAlves.Commerce.Domain.Entities;

public class Pagamento
{
    public Guid Id { get; set; }
    public Guid PedidoId { get; set; }
    public Pedido Pedido { get; set; } = null!;
    public string Gateway { get; set; } = "mercadopago";
    public string? GatewayPagamentoId { get; set; }
    public MetodoPagamento Metodo { get; set; }
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;
    public decimal Valor { get; set; }
    public DateTime? ConfirmadoEm { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}