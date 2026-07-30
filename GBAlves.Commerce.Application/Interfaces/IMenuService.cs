// Application/Interfaces/IMenuService.cs
using GBAlves.Commerce.Application.DTOs.Menus;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IMenuService
{
    Task<List<MenuResponse>> ObterTodos();
    Task<MenuResponse?> ObterPorLocalizacao(string localizacao);
    Task<MenuResponse> CriarMenu(CriarMenuRequest request);
    Task<ItemMenuResponse> AdicionarItem(Guid menuId, CriarItemMenuRequest request);
    Task<bool> RemoverItem(Guid itemId);
}