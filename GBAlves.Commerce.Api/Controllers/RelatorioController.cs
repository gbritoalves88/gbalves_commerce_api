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
    private readonly ILogger<RelatorioController> _logger;

    public RelatorioController(IRelatorioService relatorioService, ILogger<RelatorioController> logger)
    {
        _relatorioService = relatorioService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ ADMIN/OPERADOR - Exportar relatório Excel
    /// </summary>
    [HttpGet("excel")]
    public async Task<ActionResult> ExportarExcel()
    {
        try
        {
            var excel = await _relatorioService.GerarExcelAsync();
            
            _logger.LogInformation("Relatório Excel gerado: {Arquivo}", 
                $"relatorio-{DateTime.Now:yyyyMMdd}.xlsx");
            
            return File(excel, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"relatorio-{DateTime.Now:yyyyMMdd}.xlsx");
        }
        catch (Exception ex)
        {
            // Erro inesperado - NÃO expor detalhes internos
            _logger.LogError(ex, "Erro ao gerar relatório Excel");
            return StatusCode(500, new { erro = "Erro interno ao gerar relatório. Tente novamente." });
        }
    }
}