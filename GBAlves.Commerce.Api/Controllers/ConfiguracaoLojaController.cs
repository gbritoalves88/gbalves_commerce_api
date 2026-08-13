// Api/Controllers/ConfiguracaoLojaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.DTOs.ConfiguracaoLoja;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/configuracao-loja")]
public class ConfiguracaoLojaController : ControllerBase
{
    private readonly IConfiguracaoLojaService _service;
    private readonly ILogger<ConfiguracaoLojaController> _logger;

    public ConfiguracaoLojaController(IConfiguracaoLojaService service, ILogger<ConfiguracaoLojaController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// ✅ PÚBLICO - Obter configurações da loja (cores, nome, logo)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ConfiguracaoLojaResponse>> Obter()
    {
        try
        {
            var config = await _service.Obter();
            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter configurações da loja");
            return StatusCode(500, new { erro = "Erro interno ao carregar configurações." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Atualizar configurações da loja
    /// </summary>
    [HttpPut]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ConfiguracaoLojaResponse>> Atualizar([FromBody] AtualizarConfiguracaoLojaRequest request)
    {
        try
        {
            var config = await _service.Atualizar(request);
            _logger.LogInformation("Configurações da loja atualizadas");
            return Ok(config);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos ao atualizar configurações: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar configurações da loja");
            return StatusCode(500, new { erro = "Erro interno ao atualizar configurações." });
        }
    }
}