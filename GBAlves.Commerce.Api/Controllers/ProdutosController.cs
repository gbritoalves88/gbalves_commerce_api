// Api/Controllers/ProdutosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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
    //✅ PÚBLICO - Listar produtos (loja)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterTodos([FromQuery] string? busca = null)
    {
        var produtos = string.IsNullOrWhiteSpace(busca)
            ? await _produtoService.ObterTodos()
            : await _produtoService.Buscar(busca);
        return Ok(produtos);
    }

    /// <summary>
    //✅ PÚBLICO - Produtos em destaque (home)
    /// </summary>
    [HttpGet("destaques")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterDestaques()
    {
        var produtos = await _produtoService.ObterDestaques();
        return Ok(produtos);
    }

    /// <summary>
    //✅ PÚBLICO - Produtos por categoria
    /// </summary>
    [HttpGet("categoria/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterPorCategoria(string slug)
    {
        var produtos = await _produtoService.ObterPorCategoria(slug);
        return Ok(produtos);
    }

    /// <summary>
    //✅ PÚBLICO - Produto por slug (detalhe)
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProdutoResponse>> ObterPorSlug(string slug)
    {
        var produto = await _produtoService.ObterPorSlug(slug);
        if (produto is null) return NotFound();
        return Ok(produto);
    }

    /// <summary>
    //✅ PÚBLICO - Produto por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(Guid id)
    {
        var produto = await _produtoService.ObterPorId(id);
        if (produto is null) return NotFound();
        return Ok(produto);
    }

    /// <summary>
    //✅ ADMIN - Listar todos os produtos (inclusive inativos)
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterTodosAdmin()
    {
        var produtos = await _produtoService.ObterTodosAdmin();
        return Ok(produtos);
    }

    /// <summary>
    //✅ ADMIN - Criar produto
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ProdutoResponse>> Criar([FromBody] CriarProdutoRequest request)
    {
        var produto = await _produtoService.Criar(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    /// <summary>
    //✅ ADMIN - Atualizar produto
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ProdutoResponse>> Atualizar(Guid id, [FromBody] AtualizarProdutoRequest request)
    {
        var produto = await _produtoService.Atualizar(id, request);
        if (produto is null) return NotFound();
        return Ok(produto);
    }

    /// <summary>
    //✅ ADMIN - Excluir produto
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        var resultado = await _produtoService.Excluir(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}