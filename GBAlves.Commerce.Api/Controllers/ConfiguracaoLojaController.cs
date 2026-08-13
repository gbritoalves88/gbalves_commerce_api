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

    public ConfiguracaoLojaController(IConfiguracaoLojaService service)
    {
        _service = service;
    }

    /// <summary>
    //✅ PÚBLICO - Obter configurações da loja (cores, nome, logo)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ConfiguracaoLojaResponse>> Obter()
    {
        var config = await _service.Obter();
        return Ok(config);
    }

    /// <summary>
    //✅ ADMIN - Atualizar configurações da loja
    /// </summary>
    [HttpPut]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<ConfiguracaoLojaResponse>> Atualizar([FromBody] AtualizarConfiguracaoLojaRequest request)
    {
        var config = await _service.Atualizar(request);
        return Ok(config);
    }
}