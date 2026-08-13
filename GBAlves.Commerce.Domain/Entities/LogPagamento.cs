// Domain/Entities/LogPagamento.cs
namespace GBAlves.Commerce.Domain.Entities;

public class LogPagamento
{
    public Guid Id { get; set; }
    public Guid PagamentoId { get; set; }
    public Pagamento Pagamento { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string? Conteudo { get; set; } // JSON
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}