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

    public CheckoutController(IPagamentoService pagamentoService)
    {
        _pagamentoService = pagamentoService;
    }

    /// <summary>
    //✅ CLIENTE - Criar checkout de pagamento
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CriarCheckoutResponse>> CriarCheckout([FromBody] CriarCheckoutRequest request)
    {
        var response = await _pagamentoService.CriarCheckoutAsync(request);
        return Ok(response);
    }
}