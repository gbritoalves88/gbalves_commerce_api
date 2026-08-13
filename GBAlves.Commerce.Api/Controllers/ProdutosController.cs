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
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IProdutoService produtoService, ILogger<ProdutosController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ PÚBLICO - Listar produtos (loja)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterTodos([FromQuery] string? busca = null)
    {
        try
        {
            var produtos = string.IsNullOrWhiteSpace(busca)
                ? await _produtoService.ObterTodos()
                : await _produtoService.Buscar(busca);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos. Busca: {Busca}", busca);
            return StatusCode(500, new { erro = "Erro interno ao listar produtos." });
        }
    }

    /// <summary>
    /// ✅ PÚBLICO - Produtos em destaque (home)
    /// </summary>
    [HttpGet("destaques")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterDestaques()
    {
        try
        {
            var produtos = await _produtoService.ObterDestaques();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos em destaque");
            return StatusCode(500, new { erro = "Erro interno ao carregar destaques." });
        }
    }

    /// <summary>
    /// ✅ PÚBLICO - Produtos por categoria
    /// </summary>
    [HttpGet("categoria/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterPorCategoria(string slug)
    {
        try
        {
            var produtos = await _produtoService.ObterPorCategoria(slug);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos da categoria {Slug}", slug);
            return StatusCode(500, new { erro = "Erro interno ao carregar categoria." });
        }
    }

    /// <summary>
    /// ✅ PÚBLICO - Produto por slug (detalhe)
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProdutoResponse>> ObterPorSlug(string slug)
    {
        try
        {
            var produto = await _produtoService.ObterPorSlug(slug);
            if (produto is null) return NotFound(new { erro = "Produto não encontrado" });
            return Ok(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por slug: {Slug}", slug);
            return StatusCode(500, new { erro = "Erro interno ao carregar produto." });
        }
    }

    /// <summary>
    /// ✅ PÚBLICO - Produto por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(Guid id)
    {
        try
        {
            var produto = await _produtoService.ObterPorId(id);
            if (produto is null) return NotFound(new { erro = "Produto não encontrado" });
            return Ok(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por ID: {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao carregar produto." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Listar todos os produtos (inclusive inativos)
    /// </summary>
    [HttpGet("admin")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<List<ProdutoResponse>>> ObterTodosAdmin()
    {
        try
        {
            var produtos = await _produtoService.ObterTodosAdmin();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos admin");
            return StatusCode(500, new { erro = "Erro interno ao listar produtos." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Criar produto
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ProdutoResponse>> Criar([FromBody] CriarProdutoRequest request)
    {
        try
        {
            var produto = await _produtoService.Criar(request);
            _logger.LogInformation("Produto criado: {Id} - {Nome}", produto.Id, produto.Nome);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao criar produto: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return StatusCode(500, new { erro = "Erro interno ao criar produto." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Atualizar produto
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ProdutoResponse>> Atualizar(Guid id, [FromBody] AtualizarProdutoRequest request)
    {
        try
        {
            var produto = await _produtoService.Atualizar(id, request);
            if (produto is null) return NotFound(new { erro = "Produto não encontrado" });
            _logger.LogInformation("Produto atualizado: {Id}", id);
            return Ok(produto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao atualizar produto {Id}: {Erro}", id, ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao atualizar produto." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Excluir produto
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        try
        {
            var resultado = await _produtoService.Excluir(id);
            if (!resultado) return NotFound(new { erro = "Produto não encontrado" });
            _logger.LogInformation("Produto excluído: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir produto {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao excluir produto." });
        }
    }
}