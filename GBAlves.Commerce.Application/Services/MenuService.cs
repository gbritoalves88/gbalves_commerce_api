// Application/Services/MenuService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Menus;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class MenuService : IMenuService
{
    private readonly AppDbContext _context;

    public MenuService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuResponse>> ObterTodos()
    {
        var menus = await _context.Menus
            .Include(m => m.Itens.Where(i => i.ItemPaiId == null))
                .ThenInclude(i => i.SubItens)
            .OrderBy(m => m.Nome)
            .ToListAsync();

        return menus.Select(MapMenuToResponse).ToList();
    }

    public async Task<MenuResponse?> ObterPorLocalizacao(string localizacao)
    {
        var menu = await _context.Menus
            .Include(m => m.Itens.Where(i => i.Ativo && i.ItemPaiId == null))
                .ThenInclude(i => i.SubItens.Where(s => s.Ativo))
            .FirstOrDefaultAsync(m => m.Ativo && m.Localizacao == localizacao);

        return menu is null ? null : MapMenuToResponse(menu);
    }

    public async Task<MenuResponse?> ObterPorId(Guid id)
    {
        var menu = await _context.Menus
            .Include(m => m.Itens.Where(i => i.ItemPaiId == null))
                .ThenInclude(i => i.SubItens)
            .FirstOrDefaultAsync(m => m.Id == id);

        return menu is null ? null : MapMenuToResponse(menu);
    }

    public async Task<MenuResponse> CriarMenu(CriarMenuRequest request)
    {
        var menu = new Menu
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Localizacao = request.Localizacao
        };

        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();

        return MapMenuToResponse(menu);
    }

    public async Task<ItemMenuResponse> AdicionarItem(Guid menuId, CriarItemMenuRequest request)
    {
        var item = new ItemMenu
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            Nome = request.Nome,
            Slug = request.Slug,
            Url = request.Url,
            Icone = request.Icone,
            ItemPaiId = request.ItemPaiId,
            OrdemExibicao = request.OrdemExibicao,
            CategoriaId = request.CategoriaId
        };

        _context.ItensMenu.Add(item);
        await _context.SaveChangesAsync();

        return MapItemToResponse(item);
    }

    public async Task<bool> RemoverItem(Guid itemId)
    {
        var item = await _context.ItensMenu.FindAsync(itemId);
        if (item is null) return false;
        item.Ativo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleItem(Guid itemId)
    {
        var item = await _context.ItensMenu.FindAsync(itemId);
        if (item is null) return false;
        item.Ativo = !item.Ativo;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleMenu(Guid menuId)
    {
        var menu = await _context.Menus.FindAsync(menuId);
        if (menu is null) return false;
        menu.Ativo = !menu.Ativo;
        await _context.SaveChangesAsync();
        return true;
    }

    private static MenuResponse MapMenuToResponse(Menu menu)
    {
        return new MenuResponse(
            menu.Id,
            menu.Nome,
            menu.Localizacao,
            menu.Ativo,
            menu.Itens.Select(MapItemToResponse).ToList()
        );
    }

    private static ItemMenuResponse MapItemToResponse(ItemMenu item)
    {
        return new ItemMenuResponse(
            item.Id,
            item.Nome,
            item.Slug,
            item.Url,
            item.Icone,
            item.OrdemExibicao,
            item.Ativo,
            item.SubItens.Select(MapItemToResponse).ToList()
        );
    }
}