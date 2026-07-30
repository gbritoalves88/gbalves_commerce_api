// Api/Controllers/MenusController.cs
using Microsoft.AspNetCore.Mvc;
using GBAlves.Commerce.Application.DTOs.Menus;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MenuResponse>>> ObterTodos()
    {
        return Ok(await _menuService.ObterTodos());
    }

    [HttpGet("{localizacao}")]
    public async Task<ActionResult<MenuResponse>> ObterPorLocalizacao(string localizacao)
    {
        var menu = await _menuService.ObterPorLocalizacao(localizacao);
        if (menu is null) return NotFound();
        return Ok(menu);
    }

    [HttpPost]
    public async Task<ActionResult<MenuResponse>> CriarMenu([FromBody] CriarMenuRequest request)
    {
        return Created("", await _menuService.CriarMenu(request));
    }

    [HttpPost("{menuId:guid}/itens")]
    public async Task<ActionResult<ItemMenuResponse>> AdicionarItem(Guid menuId, [FromBody] CriarItemMenuRequest request)
    {
        return Created("", await _menuService.AdicionarItem(menuId, request));
    }

    [HttpDelete("itens/{itemId:guid}")]
    public async Task<ActionResult> RemoverItem(Guid itemId)
    {
        var result = await _menuService.RemoverItem(itemId);
        if (!result) return NotFound();
        return NoContent();
    }
}