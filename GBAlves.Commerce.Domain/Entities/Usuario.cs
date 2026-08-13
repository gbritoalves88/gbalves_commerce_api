// Domain/Entities/Usuario.cs
using GBAlves.Commerce.Domain.Enums;

namespace GBAlves.Commerce.Domain.Entities;
public class Usuario : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public NivelAcesso NivelAcesso { get; set; } = NivelAcesso.Cliente;
    public bool Ativo { get; set; } = true;
    
    public Guid? ClienteId { get; set; }  // ← NOVO
    public Cliente? Cliente { get; set; }  // ← NOVO
}