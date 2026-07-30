// Application/DTOs/Usuarios/UsuarioResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Usuarios;

public record UsuarioResponse(
    Guid Id,
    string Nome,
    string Email,
    string NivelAcesso,
    bool Ativo,
    DateTime CriadoEm
);