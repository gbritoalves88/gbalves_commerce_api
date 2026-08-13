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
    private readonly ILogger<FreteController> _logger;

    public FreteController(IFreteService freteService, ILogger<FreteController> logger)
    {
        _freteService = freteService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ PÚBLICO - Calcular frete (qualquer pessoa pode consultar)
    /// </summary>
    [HttpPost("calcular")]
    [AllowAnonymous]
    public async Task<ActionResult<List<FreteResponse>>> CalcularFrete([FromBody] CalcularFreteRequest request)
    {
        try
        {
            var opcoes = await _freteService.CalcularFreteAsync(request);
            
            _logger.LogInformation("Frete calculado: CEP {Cep}, {Quantidade} opções", 
                request.CepDestino, opcoes.Count);
            
            return Ok(opcoes);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Dados inválidos para frete: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao calcular frete para CEP {Cep}", request.CepDestino);
            return StatusCode(500, new { erro = "Erro interno ao calcular frete." });
        }
    }
}