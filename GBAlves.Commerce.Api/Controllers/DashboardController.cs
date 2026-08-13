// Api/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Application.DTOs;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]  // Admin e Operador
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService service, ILogger<DashboardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// ✅ OPERADOR/ADMIN - Obter resumo do dashboard
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> ObterResumo()
    {
        try
        {
            var resumo = await _service.ObterResumo();
            return Ok(resumo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter resumo do dashboard");
            return StatusCode(500, new { erro = "Erro interno ao carregar dashboard." });
        }
    }
}