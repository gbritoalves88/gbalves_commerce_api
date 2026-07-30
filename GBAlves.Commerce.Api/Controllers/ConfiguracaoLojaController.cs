// Api/Controllers/ConfiguracaoLojaController.cs
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<ActionResult<ConfiguracaoLojaResponse>> Obter()
    {
        var config = await _service.Obter();
        return Ok(config);
    }

    [HttpPut]
    public async Task<ActionResult<ConfiguracaoLojaResponse>> Atualizar([FromBody] AtualizarConfiguracaoLojaRequest request)
    {
        var config = await _service.Atualizar(request);
        return Ok(config);
    }
}