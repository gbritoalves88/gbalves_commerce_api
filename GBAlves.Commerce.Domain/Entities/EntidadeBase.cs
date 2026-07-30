// Domain/Entities/EntidadeBase.cs
namespace GBAlves.Commerce.Domain.Entities;

public abstract class EntidadeBase
{
    public Guid Id { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}