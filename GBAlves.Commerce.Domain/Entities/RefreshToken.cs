namespace GBAlves.Commerce.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public DateTime CriadoEm { get; set; }
    public bool Revogado { get; set; }
    
    // Navegação
    public Usuario Usuario { get; set; } = null!;
    
    public bool EstaAtivo => !Revogado && ExpiraEm > DateTime.UtcNow;
}