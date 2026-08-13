// Api/Controllers/RelatorioController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]  // Admin e Operador
public class RelatorioController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatorioController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    //✅ ADMIN/OPERADOR - Exportar relatório Excel
    /// </summary>
    [HttpGet("excel")]
    public async Task<ActionResult> ExportarExcel()
    {
        var excel = await _relatorioService.GerarExcelAsync();
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"relatorio-{DateTime.Now:yyyyMMdd}.xlsx");
    }
}