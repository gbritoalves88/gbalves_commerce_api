// Application/Services/ProdutoService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Produtos;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProdutoResponse>> ObterTodos()
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo)
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => MapToResponse(p))
            .ToListAsync();
    }

    public async Task<List<ProdutoResponse>> ObterDestaques()
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo && p.Destaque)
            .OrderByDescending(p => p.CriadoEm)
            .Take(8)
            .Select(p => MapToResponse(p))
            .ToListAsync();
    }

    public async Task<List<ProdutoResponse>> ObterPorCategoria(string slug)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.Ativo && p.Categoria!.Slug == slug)
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => MapToResponse(p))
            .ToListAsync();
    }

    public async Task<ProdutoResponse?> ObterPorId(Guid id)
    {
        var produto = await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);

        return produto is null ? null : MapToResponse(produto);
    }

    public async Task<ProdutoResponse> Criar(CriarProdutoRequest request)
    {
        var produto = new Produto
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Slug = GerarSlug(request.Nome),
            Descricao = request.Descricao,
            Preco = request.Preco,
            PrecoOriginal = request.PrecoOriginal,
            ImagemUrl = request.ImagemUrl,
            Estoque = request.Estoque,
            Destaque = request.Destaque,
            CategoriaId = request.CategoriaId,
            CriadoEm = DateTime.UtcNow
        };

        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();

        return MapToResponse(produto);
    }

    public async Task<ProdutoResponse?> Atualizar(Guid id, AtualizarProdutoRequest request)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return null;

        produto.Nome = request.Nome;
        produto.Slug = GerarSlug(request.Nome);
        produto.Descricao = request.Descricao;
        produto.Preco = request.Preco;
        produto.PrecoOriginal = request.PrecoOriginal;
        produto.ImagemUrl = request.ImagemUrl;
        produto.Estoque = request.Estoque;
        produto.Destaque = request.Destaque;
        produto.CategoriaId = request.CategoriaId;
        produto.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToResponse(produto);
    }

    public async Task<bool> Excluir(Guid id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto is null) return false;

        produto.Ativo = false; // Soft delete
        produto.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static ProdutoResponse MapToResponse(Produto p)
    {
        return new ProdutoResponse(
            p.Id,
            p.Nome,
            p.Slug,
            p.Descricao,
            p.Preco,
            p.PrecoOriginal,
            p.ImagemUrl,
            p.Estoque,
            p.Destaque,
            p.Ativo,
            p.Categoria?.Nome,
            p.CriadoEm
        );
    }

    private static string GerarSlug(string texto)
    {
        return texto
            .ToLower()
            .Replace(" ", "-")
            .Replace("--", "-");
    }
}