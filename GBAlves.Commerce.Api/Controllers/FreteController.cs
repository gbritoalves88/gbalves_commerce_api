// Api/Controllers/FreteController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.DTOs.Fretes;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreteController : ControllerBase
{
    private readonly IFreteService _freteService;

    public FreteController(IFreteService freteService)
    {
        _freteService = freteService;
    }

    /// <summary>
    //✅ PÚBLICO - Calcular frete (qualquer pessoa pode consultar)
    /// </summary>
    [HttpPost("calcular")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FreteResponse>>> CalcularFrete([FromBody] CalcularFreteRequest request)
    {
        var opcoes = await _freteService.CalcularFreteAsync(request);
        return Ok(opcoes);
    }
}