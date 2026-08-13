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

public record AtualizarUsuarioRequest(
    string Nome,
    string Email,
    string? Telefone,
    string? Cpf
);