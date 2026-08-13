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

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    /// <summary>
    //✅ PÚBLICO - Listar categorias (loja e admin)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoriaResponse>>> ObterTodas()
    {
        var categorias = await _categoriaService.ObterTodas();
        return Ok(categorias);
    }

    /// <summary>
    //✅ ADMIN - Obter categoria por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<CategoriaResponse>> ObterPorId(Guid id)
    {
        var categoria = await _categoriaService.ObterPorId(id);
        if (categoria is null) return NotFound();
        return Ok(categoria);
    }

    /// <summary>
    //✅ ADMIN - Criar categoria
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CategoriaResponse>> Criar([FromBody] CriarCategoriaRequest request)
    {
        var categoria = await _categoriaService.Criar(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
    }

    /// <summary>
    //✅ ADMIN - Atualizar categoria
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CategoriaResponse>> Atualizar(Guid id, [FromBody] AtualizarCategoriaRequest request)
    {
        var categoria = await _categoriaService.Atualizar(id, request);
        if (categoria is null) return NotFound();
        return Ok(categoria);
    }

    /// <summary>
    //✅ ADMIN - Excluir categoria
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        var resultado = await _categoriaService.Excluir(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}