// Api/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]  // Admin e Operador
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    /// <summary>
    //✅ OPERADOR/ADMIN - Obter resumo do dashboard
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> ObterResumo()
    {
        return Ok(await _service.ObterResumo());
    }
}