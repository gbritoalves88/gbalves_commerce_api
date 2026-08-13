// Domain/Entities/EventoPagamento.cs
namespace GBAlves.Commerce.Domain.Entities;

public class EventoPagamento
{
    public Guid Id { get; set; }
    public Guid PagamentoId { get; set; }
    public Pagamento Pagamento { get; set; } = null!;
    public string Evento { get; set; } = string.Empty;
    public string? Dados { get; set; } // JSON
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}