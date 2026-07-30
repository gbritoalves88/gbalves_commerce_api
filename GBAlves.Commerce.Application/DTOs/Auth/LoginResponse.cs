// Application/DTOs/Auth/LoginResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Auth;

public record LoginResponse(string Token, string Nome, string Email, string NivelAcesso);