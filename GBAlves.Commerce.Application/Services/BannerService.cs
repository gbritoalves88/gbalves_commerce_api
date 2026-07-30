// Application/Services/BannerService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Banners;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class BannerService : IBannerService
{
    private readonly AppDbContext _context;

    public BannerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BannerResponse>> ObterTodos()
    {
        return await _context.Banners
            .OrderBy(b => b.OrdemExibicao)
            .Select(b => MapToResponse(b))
            .ToListAsync();
    }

    public async Task<List<BannerResponse>> ObterAtivos()
    {
        return await _context.Banners
            .Where(b => b.Ativo)
            .OrderBy(b => b.OrdemExibicao)
            .Select(b => MapToResponse(b))
            .ToListAsync();
    }

    public async Task<BannerResponse?> ObterPorId(Guid id)
    {
        var banner = await _context.Banners.FindAsync(id);
        return banner is null ? null : MapToResponse(banner);
    }

    public async Task<BannerResponse> Criar(CriarBannerRequest request)
    {
        var banner = new Banner
        {
            Id = Guid.NewGuid(),
            Titulo = request.Titulo,
            Subtitulo = request.Subtitulo,
            ImagemUrl = request.ImagemUrl,
            Link = request.Link,
            TextoBotao = request.TextoBotao,
            OrdemExibicao = request.OrdemExibicao,
            CriadoEm = DateTime.UtcNow
        };

        _context.Banners.Add(banner);
        await _context.SaveChangesAsync();
        return MapToResponse(banner);
    }

    public async Task<BannerResponse?> Atualizar(Guid id, CriarBannerRequest request)
    {
        var banner = await _context.Banners.FindAsync(id);
        if (banner is null) return null;

        banner.Titulo = request.Titulo;
        banner.Subtitulo = request.Subtitulo;
        banner.ImagemUrl = request.ImagemUrl;
        banner.Link = request.Link;
        banner.TextoBotao = request.TextoBotao;
        banner.OrdemExibicao = request.OrdemExibicao;

        await _context.SaveChangesAsync();
        return MapToResponse(banner);
    }

    public async Task<bool> Excluir(Guid id)
    {
        var banner = await _context.Banners.FindAsync(id);
        if (banner is null) return false;

        banner.Ativo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static BannerResponse MapToResponse(Banner b)
    {
        return new BannerResponse(
            b.Id, b.Titulo, b.Subtitulo, b.ImagemUrl,
            b.Link, b.TextoBotao, b.OrdemExibicao, b.Ativo
        );
    }
}