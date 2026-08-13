// Api/Controllers/CategoriasController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.DTOs.Categorias;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
    {
        _categoriaService = categoriaService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ PÚBLICO - Listar categorias (loja e admin)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoriaResponse>>> ObterTodas()
    {
        try
        {
            var categorias = await _categoriaService.ObterTodas();
            return Ok(categorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar categorias");
            return StatusCode(500, new { erro = "Erro interno ao listar categorias." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Obter categoria por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<CategoriaResponse>> ObterPorId(Guid id)
    {
        try
        {
            var categoria = await _categoriaService.ObterPorId(id);
            if (categoria is null) return NotFound(new { erro = "Categoria não encontrada" });
            return Ok(categoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter categoria {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao carregar categoria." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Criar categoria
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CategoriaResponse>> Criar([FromBody] CriarCategoriaRequest request)
    {
        try
        {
            var categoria = await _categoriaService.Criar(request);
            _logger.LogInformation("Categoria criada: {Id} - {Nome}", categoria.Id, categoria.Nome);
            return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao criar categoria: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar categoria");
            return StatusCode(500, new { erro = "Erro interno ao criar categoria." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Atualizar categoria
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CategoriaResponse>> Atualizar(Guid id, [FromBody] AtualizarCategoriaRequest request)
    {
        try
        {
            var categoria = await _categoriaService.Atualizar(id, request);
            if (categoria is null) return NotFound(new { erro = "Categoria não encontrada" });
            _logger.LogInformation("Categoria atualizada: {Id}", id);
            return Ok(categoria);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao atualizar categoria {Id}: {Erro}", id, ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar categoria {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao atualizar categoria." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Excluir categoria
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        try
        {
            var resultado = await _categoriaService.Excluir(id);
            if (!resultado) return NotFound(new { erro = "Categoria não encontrada" });
            _logger.LogInformation("Categoria excluída: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir categoria {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao excluir categoria." });
        }
    }
}