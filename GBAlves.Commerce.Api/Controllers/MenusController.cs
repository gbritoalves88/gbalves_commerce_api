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
    private readonly ILogger<MenusController> _logger;

    public MenusController(IMenuService menuService, ILogger<MenusController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ PÚBLICO - Menus ativos para loja (header/footer)
    /// </summary>
    [HttpGet("{localizacao}")]
    [AllowAnonymous]
    public async Task<ActionResult<MenuResponse>> ObterPorLocalizacao(string localizacao)
    {
        try
        {
            var menu = await _menuService.ObterPorLocalizacao(localizacao);
            if (menu is null) return NotFound(new { erro = "Menu não encontrado" });
            return Ok(menu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter menu por localização: {Localizacao}", localizacao);
            return StatusCode(500, new { erro = "Erro interno ao carregar menu." });
        }
    }

    /// <summary>
    /// ✅ PÚBLICO - Listar todos os menus
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<MenuResponse>>> ObterTodosMenus()
    {
        try
        {
            return Ok(await _menuService.ObterTodos());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar menus");
            return StatusCode(500, new { erro = "Erro interno ao listar menus." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Obter menu por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<MenuResponse>> ObterPorId(Guid id)
    {
        try
        {
            var menu = await _menuService.ObterPorId(id);
            if (menu is null) return NotFound(new { erro = "Menu não encontrado" });
            return Ok(menu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter menu {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao carregar menu." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Criar menu
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<MenuResponse>> CriarMenu([FromBody] CriarMenuRequest request)
    {
        try
        {
            var menu = await _menuService.CriarMenu(request);
            _logger.LogInformation("Menu criado: {Id} - {Nome}", menu.Id, menu.Nome);
            return Created("", menu);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao criar menu: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar menu");
            return StatusCode(500, new { erro = "Erro interno ao criar menu." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Adicionar item ao menu
    /// </summary>
    [HttpPost("{menuId:guid}/itens")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ItemMenuResponse>> AdicionarItem(Guid menuId, [FromBody] CriarItemMenuRequest request)
    {
        try
        {
            var item = await _menuService.AdicionarItem(menuId, request);
            _logger.LogInformation("Item adicionado ao menu {MenuId}: {ItemId}", menuId, item.Id);
            return Created("", item);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao adicionar item: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar item ao menu {MenuId}", menuId);
            return StatusCode(500, new { erro = "Erro interno ao adicionar item." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Remover item do menu
    /// </summary>
    [HttpDelete("itens/{itemId:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> RemoverItem(Guid itemId)
    {
        try
        {
            var result = await _menuService.RemoverItem(itemId);
            if (!result) return NotFound(new { erro = "Item não encontrado" });
            _logger.LogInformation("Item removido do menu: {ItemId}", itemId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover item {ItemId}", itemId);
            return StatusCode(500, new { erro = "Erro interno ao remover item." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Ativar/Desativar item do menu
    /// </summary>
    [HttpPatch("itens/{itemId:guid}/toggle")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> ToggleItem(Guid itemId)
    {
        try
        {
            var result = await _menuService.ToggleItem(itemId);
            if (!result) return NotFound(new { erro = "Item não encontrado" });
            _logger.LogInformation("Item toggle: {ItemId}", itemId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alternar item {ItemId}", itemId);
            return StatusCode(500, new { erro = "Erro interno ao alternar item." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Ativar/Desativar menu
    /// </summary>
    [HttpPatch("{menuId:guid}/toggle")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> ToggleMenu(Guid menuId)
    {
        try
        {
            var result = await _menuService.ToggleMenu(menuId);
            if (!result) return NotFound(new { erro = "Menu não encontrado" });
            _logger.LogInformation("Menu toggle: {MenuId}", menuId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alternar menu {MenuId}", menuId);
            return StatusCode(500, new { erro = "Erro interno ao alternar menu." });
        }
    }
}