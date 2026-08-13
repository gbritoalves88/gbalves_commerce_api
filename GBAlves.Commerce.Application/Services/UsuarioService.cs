// Application/Services/UsuarioService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Usuarios;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Infrastructure.Data;
using GBAlves.Commerce.Domain.Enums;

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
            .Select(u => new UsuarioResponse(u.Id, u.Nome, u.Email, u.NivelAcesso.ToString(), u.Ativo, u.CriadoEm))
            .ToListAsync();
    }

    public async Task<UsuarioResponse?> ObterPorId(Guid id)
    {
        var u = await _context.Usuarios.FindAsync(id);
        return u is null ? null : new UsuarioResponse(u.Id, u.Nome, u.Email, u.NivelAcesso.ToString(), u.Ativo, u.CriadoEm);
    }

    public async Task<UsuarioResponse> Criar(CriarUsuarioRequest request)
    {
        // ========== VALIDAÇÃO DE SENHA FORTE ==========
        ValidarSenhaForte(request.Senha);

        // Sempre cria como Cliente - cadastro público
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
            NivelAcesso = NivelAcesso.Cliente,
            ClienteId = cliente.Id,
            CriadoEm = DateTime.UtcNow,
            Ativo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return new UsuarioResponse(
            usuario.Id, usuario.Nome, usuario.Email, 
            usuario.NivelAcesso.ToString(),
            usuario.Ativo, usuario.CriadoEm
        );
    }

    // ========== MÉTODO DE VALIDAÇÃO ==========
    private void ValidarSenhaForte(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("Senha é obrigatória!");

        if (senha.Length < 8)
            throw new ArgumentException("Senha deve ter no mínimo 8 caracteres!");

        if (senha.Length > 50)
            throw new ArgumentException("Senha muito longa! Máximo 50 caracteres.");

        if (!senha.Any(char.IsUpper))
            throw new ArgumentException("Senha deve conter pelo menos uma letra maiúscula!");

        if (!senha.Any(char.IsLower))
            throw new ArgumentException("Senha deve conter pelo menos uma letra minúscula!");

        if (!senha.Any(char.IsDigit))
            throw new ArgumentException("Senha deve conter pelo menos um número!");

        if (!senha.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException("Senha deve conter pelo menos um caractere especial (!@#$%...)!");

        // Não permitir espaços
        if (senha.Contains(' '))
            throw new ArgumentException("Senha não pode conter espaços!");
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
        // Buscar usuário com cliente
        var usuario = await _context.Usuarios
            .Include(u => u.Cliente)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (usuario is null) return null;

        // Atualizar dados do usuário
        usuario.Nome = request.Nome;
        usuario.Email = request.Email;
        usuario.AtualizadoEm = DateTime.UtcNow;

        // Atualizar dados do cliente (telefone e CPF)
        if (usuario.Cliente is not null)
        {
            if (!string.IsNullOrWhiteSpace(request.Telefone))
                usuario.Cliente.Telefone = request.Telefone;
            
            if (!string.IsNullOrWhiteSpace(request.Cpf))
                usuario.Cliente.Cpf = request.Cpf;
        }

        await _context.SaveChangesAsync();

        return new UsuarioResponse(
            usuario.Id, 
            usuario.Nome, 
            usuario.Email, 
            usuario.NivelAcesso.ToString(), 
            usuario.Ativo, 
            usuario.CriadoEm
        );
    }
}