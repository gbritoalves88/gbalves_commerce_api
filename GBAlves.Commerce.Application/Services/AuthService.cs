// Application/Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using GBAlves.Commerce.Application.DTOs.Auth;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Cliente)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Ativo);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            return null;

        var token = GerarToken(usuario);

        return new LoginResponse(
            token,
            usuario.Id.ToString(),
            usuario.Nome,
            usuario.Email,
            usuario.NivelAcesso,
            usuario.Cliente?.Telefone,
            usuario.Cliente?.Cpf
        );
    }


    private string GerarToken(Domain.Entities.Usuario usuario)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Role, usuario.NivelAcesso)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}