// Api/Controllers/BannersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.DTOs.Banners;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannersController : ControllerBase
{
    private readonly IBannerService _bannerService;
    private readonly ILogger<BannersController> _logger;

    public BannersController(IBannerService bannerService, ILogger<BannersController> logger)
    {
        _bannerService = bannerService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ OPERADOR - Listar banners (admin)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<List<BannerResponse>>> ObterTodos()
    {
        try
        {
            var banners = await _bannerService.ObterTodos();
            return Ok(banners);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar banners");
            return StatusCode(500, new { erro = "Erro interno ao listar banners." });
        }
    }

    /// <summary>
    /// ✅ PÚBLICO - Banners ativos (loja)
    /// </summary>
    [HttpGet("ativos")]
    [AllowAnonymous]
    public async Task<ActionResult<List<BannerResponse>>> ObterAtivos()
    {
        try
        {
            var banners = await _bannerService.ObterAtivos();
            return Ok(banners);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar banners ativos");
            return StatusCode(500, new { erro = "Erro interno ao carregar banners." });
        }
    }

    /// <summary>
    /// ✅ OPERADOR - Obter banner por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<BannerResponse>> ObterPorId(Guid id)
    {
        try
        {
            var banner = await _bannerService.ObterPorId(id);
            if (banner is null) return NotFound(new { erro = "Banner não encontrado" });
            return Ok(banner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter banner {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao carregar banner." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Criar banner
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<BannerResponse>> Criar([FromBody] CriarBannerRequest request)
    {
        try
        {
            var banner = await _bannerService.Criar(request);
            _logger.LogInformation("Banner criado: {Id} - {Titulo}", banner.Id, banner.Titulo);
            return CreatedAtAction(nameof(ObterPorId), new { id = banner.Id }, banner);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao criar banner: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar banner");
            return StatusCode(500, new { erro = "Erro interno ao criar banner." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Atualizar banner
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<BannerResponse>> Atualizar(Guid id, [FromBody] CriarBannerRequest request)
    {
        try
        {
            var banner = await _bannerService.Atualizar(id, request);
            if (banner is null) return NotFound(new { erro = "Banner não encontrado" });
            _logger.LogInformation("Banner atualizado: {Id}", id);
            return Ok(banner);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao atualizar banner {Id}: {Erro}", id, ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar banner {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao atualizar banner." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Excluir banner
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        try
        {
            var resultado = await _bannerService.Excluir(id);
            if (!resultado) return NotFound(new { erro = "Banner não encontrado" });
            _logger.LogInformation("Banner excluído: {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir banner {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao excluir banner." });
        }
    }
}