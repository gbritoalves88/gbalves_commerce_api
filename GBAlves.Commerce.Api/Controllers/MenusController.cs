// Api/Controllers/MenusController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    //✅ PÚBLICO - Menus ativos para loja (header/footer)
    /// </summary>
    [HttpGet("{localizacao}")]
    [AllowAnonymous]
    public async Task<ActionResult<MenuResponse>> ObterPorLocalizacao(string localizacao)
    {
        var menu = await _menuService.ObterPorLocalizacao(localizacao);
        if (menu is null) return NotFound();
        return Ok(menu);
    }

    /// <summary>
    //✅ Listar todos os menus
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<MenuResponse>>> ObterTodosMenus()
    {
        return Ok(await _menuService.ObterTodos());
    }


    /// <summary>
    //✅ ADMIN - Obter menu por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<MenuResponse>> ObterPorId(Guid id)
    {
        var menu = await _menuService.ObterPorId(id);
        if (menu is null) return NotFound();
        return Ok(menu);
    }

    /// <summary>
    //✅ ADMIN - Criar menu
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<MenuResponse>> CriarMenu([FromBody] CriarMenuRequest request)
    {
        return Created("", await _menuService.CriarMenu(request));
    }

    /// <summary>
    //✅ ADMIN - Adicionar item ao menu
    /// </summary>
    [HttpPost("{menuId:guid}/itens")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ItemMenuResponse>> AdicionarItem(Guid menuId, [FromBody] CriarItemMenuRequest request)
    {
        return Created("", await _menuService.AdicionarItem(menuId, request));
    }

    /// <summary>
    //✅ ADMIN - Remover item do menu
    /// </summary>
    [HttpDelete("itens/{itemId:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> RemoverItem(Guid itemId)
    {
        var result = await _menuService.RemoverItem(itemId);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    //✅ ADMIN - Ativar/Desativar item do menu
    /// </summary>
    [HttpPatch("itens/{itemId:guid}/toggle")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> ToggleItem(Guid itemId)
    {
        var result = await _menuService.ToggleItem(itemId);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    //✅ ADMIN - Ativar/Desativar menu
    /// </summary>
    [HttpPatch("{menuId:guid}/toggle")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> ToggleMenu(Guid menuId)
    {
        var result = await _menuService.ToggleMenu(menuId);
        if (!result) return NotFound();
        return NoContent();
    }
}