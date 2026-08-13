// Application/DTOs/Auth/LoginResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Auth;

public record LoginResponse(
    string Token,
    string Id, 
    string Nome, 
    string Email,
    string NivelAcesso,
    string? Telefone,   // ← adicionar
    string? Cpf  );