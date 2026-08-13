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

    public BannersController(IBannerService bannerService)
    {
        _bannerService = bannerService;
    }

    /// <summary>
    //✅ PÚBLICO - Listar banners (admin)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<List<BannerResponse>>> ObterTodos()
    {
        var banners = await _bannerService.ObterTodos();
        return Ok(banners);
    }

    /// <summary>
    //✅ PÚBLICO - Banners ativos (loja)
    /// </summary>
    [HttpGet("ativos")]
    [AllowAnonymous]
    public async Task<ActionResult<List<BannerResponse>>> ObterAtivos()
    {
        var banners = await _bannerService.ObterAtivos();
        return Ok(banners);
    }

    /// <summary>
    //✅ ADMIN - Obter banner por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<BannerResponse>> ObterPorId(Guid id)
    {
        var banner = await _bannerService.ObterPorId(id);
        if (banner is null) return NotFound();
        return Ok(banner);
    }

    /// <summary>
    //✅ ADMIN - Criar banner
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<BannerResponse>> Criar([FromBody] CriarBannerRequest request)
    {
        var banner = await _bannerService.Criar(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = banner.Id }, banner);
    }

    /// <summary>
    //✅ ADMIN - Atualizar banner
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<BannerResponse>> Atualizar(Guid id, [FromBody] CriarBannerRequest request)
    {
        var banner = await _bannerService.Atualizar(id, request);
        if (banner is null) return NotFound();
        return Ok(banner);
    }

    /// <summary>
    //✅ ADMIN - Excluir banner
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult> Excluir(Guid id)
    {
        var resultado = await _bannerService.Excluir(id);
        if (!resultado) return NotFound();
        return NoContent();
    }
}