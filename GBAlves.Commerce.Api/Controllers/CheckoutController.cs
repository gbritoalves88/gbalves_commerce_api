// Api/Controllers/CheckoutController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GBAlves.Commerce.Application.DTOs.Pagamentos;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Cliente")]  // Qualquer usuário logado
public class CheckoutController : ControllerBase
{
    private readonly IPagamentoService _pagamentoService;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(IPagamentoService pagamentoService, ILogger<CheckoutController> logger)
    {
        _pagamentoService = pagamentoService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ CLIENTE - Criar checkout de pagamento
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CriarCheckoutResponse>> CriarCheckout([FromBody] CriarCheckoutRequest request)
    {
        try
        {
            var response = await _pagamentoService.CriarCheckoutAsync(request);
            
            _logger.LogInformation("Checkout criado: Pedido {PedidoId} → Pagamento {PagamentoId}", 
                request.PedidoId, response.PagamentoId);
            
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            // Erro de validação (pedido não encontrado, frete inválido, pagamento duplicado)
            _logger.LogWarning("Erro de validação no checkout: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar checkout para pedido {PedidoId}", request.PedidoId);
            return StatusCode(500, new { erro = "Erro interno ao processar pagamento." });
        }
    }
}