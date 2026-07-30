// Api/Controllers/ProdutosController.cs
using Microsoft.AspNetCore.Mvc;
using GBAlves.Commerce.Application.DTOs.Produtos;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Listar todos os produtos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterTodos()
    {
        var produtos = await _produtoService.ObterTodos();
        return Ok(produtos);
    }

    /// <summary>
    /// Obter produtos em destaque
    /// </summary>
    [HttpGet("destaques")]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterDestaques()
    {
        var produtos = await _produtoService.ObterDestaques();
        return Ok(produtos);
    }

    /// <summary>
    /// Obter produtos por categoria
    /// </summary>
    [HttpGet("categoria/{slug}")]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterPorCategoria(string slug)
    {
        var produtos = await _produtoService.ObterPorCategoria(slug);
        return Ok(produtos);
    }

    /// <summary>
    /// Obter produto por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(Guid id)
    {
        var produto = await _produtoService.ObterPorId(id);
        if (produto is null) return NotFound();
        return Ok(produto);
    }

    /// <summary>
    /// Criar produto
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProdutoResponse>> Criar([FromBody] CriarProdutoRequest request)
    {
        var produto = await _produtoService.Criar(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    /// <summary>
    /// Atualizar produto
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProdutoResponse>> Atualizar(Guid id, [FromBody] AtualizarProdutoRequest request)
    {
        var produto = await _produtoService.Atualizar(id, request);
        if (produto is null) return NotFound();
        return Ok(produto);
    }

    /// <summary>
    /// Excluir produto (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        var resultado = await _produtoService.Excluir(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}