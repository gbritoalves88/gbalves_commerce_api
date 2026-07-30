// Application/Services/ConfiguracaoLojaService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.ConfiguracaoLoja;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class ConfiguracaoLojaService : IConfiguracaoLojaService
{
    private readonly AppDbContext _context;

    public ConfiguracaoLojaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConfiguracaoLojaResponse> Obter()
    {
        var config = await _context.ConfiguracaoLoja.FirstOrDefaultAsync();

        if (config is null)
        {
            config = new Domain.Entities.ConfiguracaoLoja
            {
                Id = Guid.NewGuid(),
                NomeLoja = "GBAlves Commerce",
                CorPrimaria = "#1a237e",
                CorSecundaria = "#ff4081"
            };
            _context.ConfiguracaoLoja.Add(config);
            await _context.SaveChangesAsync();
        }

        return MapToResponse(config);
    }

    public async Task<ConfiguracaoLojaResponse> Atualizar(AtualizarConfiguracaoLojaRequest request)
    {
        var config = await _context.ConfiguracaoLoja.FirstOrDefaultAsync();

        if (config is null)
        {
            config = new Domain.Entities.ConfiguracaoLoja { Id = Guid.NewGuid() };
            _context.ConfiguracaoLoja.Add(config);
        }

        config.NomeLoja = request.NomeLoja;
        config.Logo = request.Logo;
        config.Favicon = request.Favicon;
        config.CorPrimaria = request.CorPrimaria;
        config.CorSecundaria = request.CorSecundaria;
        config.Whatsapp = request.Whatsapp;
        config.Instagram = request.Instagram;
        config.Facebook = request.Facebook;
        config.Email = request.Email;
        config.SeoTitulo = request.SeoTitulo;
        config.SeoDescricao = request.SeoDescricao;
        config.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToResponse(config);
    }

    private static ConfiguracaoLojaResponse MapToResponse(Domain.Entities.ConfiguracaoLoja c)
    {
        return new ConfiguracaoLojaResponse(
            c.Id, c.NomeLoja, c.Logo, c.Favicon,
            c.CorPrimaria, c.CorSecundaria,
            c.Whatsapp, c.Instagram, c.Facebook,
            c.Email, c.SeoTitulo, c.SeoDescricao
        );
    }
}