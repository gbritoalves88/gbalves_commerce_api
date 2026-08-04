// Application/Services/UsuarioService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Usuarios;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UsuarioResponse>> ObterTodos()
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioResponse(u.Id, u.Nome, u.Email, u.NivelAcesso, u.Ativo, u.CriadoEm))
            .ToListAsync();
    }

    public async Task<UsuarioResponse?> ObterPorId(Guid id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        return u is null ? null : new UsuarioResponse(u.Id, u.Nome, u.Email, u.NivelAcesso, u.Ativo, u.CriadoEm);
    }

    public async Task<UsuarioResponse> Criar(CriarUsuarioRequest request)
    {
        var nivel = string.IsNullOrWhiteSpace(request.NivelAcesso) ? "cliente" : request.NivelAcesso;

        var cliente = new Cliente
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Email = request.Email,
            Telefone = request.Telefone,
            Cpf = request.Cpf
        };
        _context.Clientes.Add(cliente);

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            NivelAcesso = nivel,
            ClienteId = cliente.Id,
            CriadoEm = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.NivelAcesso, usuario.Ativo, usuario.CriadoEm);
    }
    public async Task<bool> Excluir(Guid id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null) return false;

        usuario.Ativo = false;
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UsuarioResponse?> Atualizar(Guid id, AtualizarUsuarioRequest request)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario is null) return null;

        usuario.Nome = request.Nome;
        usuario.Email = request.Email;
        usuario.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, 
            usuario.NivelAcesso, usuario.Ativo, usuario.CriadoEm);
    }
}