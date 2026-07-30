// Application/Services/CategoriaService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Categorias;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class CategoriaService : ICategoriaService
{
    private readonly AppDbContext _context;

    public CategoriaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoriaResponse>> ObterTodas()
    {
        return await _context.Categorias
            .Where(c => c.Ativo)
            .OrderBy(c => c.OrdemExibicao)
            .Select(c => new CategoriaResponse(
                c.Id, c.Nome, c.Slug, c.ImagemUrl, 
                c.OrdemExibicao, c.Ativo, c.Produtos.Count
            ))
            .ToListAsync();
    }

    public async Task<CategoriaResponse?> ObterPorId(Guid id)
    {
        var categoria = await _context.Categorias
            .Include(c => c.Produtos)
            .FirstOrDefaultAsync(c => c.Id == id);

        return categoria is null ? null : new CategoriaResponse(
            categoria.Id, categoria.Nome, categoria.Slug, 
            categoria.ImagemUrl, categoria.OrdemExibicao, 
            categoria.Ativo, categoria.Produtos.Count
        );
    }

    public async Task<CategoriaResponse> Criar(CriarCategoriaRequest request)
    {
        var categoria = new Categoria
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Slug = GerarSlug(request.Nome),
            ImagemUrl = request.ImagemUrl,
            OrdemExibicao = request.OrdemExibicao,
            CriadoEm = DateTime.UtcNow
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();

        return new CategoriaResponse(
            categoria.Id, categoria.Nome, categoria.Slug,
            categoria.ImagemUrl, categoria.OrdemExibicao,
            categoria.Ativo, 0
        );
    }

    public async Task<CategoriaResponse?> Atualizar(Guid id, AtualizarCategoriaRequest request)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null) return null;

        categoria.Nome = request.Nome;
        categoria.Slug = GerarSlug(request.Nome);
        categoria.ImagemUrl = request.ImagemUrl;
        categoria.OrdemExibicao = request.OrdemExibicao;
        categoria.Ativo = request.Ativo;
        categoria.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new CategoriaResponse(
            categoria.Id, categoria.Nome, categoria.Slug,
            categoria.ImagemUrl, categoria.OrdemExibicao,
            categoria.Ativo, categoria.Produtos.Count
        );
    }

    public async Task<bool> Excluir(Guid id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null) return false;

        categoria.Ativo = false;
        categoria.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static string GerarSlug(string texto)
    {
        return texto.ToLower().Replace(" ", "-").Replace("--", "-");
    }
}