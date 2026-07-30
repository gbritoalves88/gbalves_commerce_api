// Api/Controllers/CategoriasController.cs
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<ActionResult<List<CategoriaResponse>>> ObterTodas()
    {
        var categorias = await _categoriaService.ObterTodas();
        return Ok(categorias);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoriaResponse>> ObterPorId(Guid id)
    {
        var categoria = await _categoriaService.ObterPorId(id);
        if (categoria is null) return NotFound();
        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaResponse>> Criar([FromBody] CriarCategoriaRequest request)
    {
        var categoria = await _categoriaService.Criar(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoriaResponse>> Atualizar(Guid id, [FromBody] AtualizarCategoriaRequest request)
    {
        var categoria = await _categoriaService.Atualizar(id, request);
        if (categoria is null) return NotFound();
        return Ok(categoria);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        var resultado = await _categoriaService.Excluir(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}