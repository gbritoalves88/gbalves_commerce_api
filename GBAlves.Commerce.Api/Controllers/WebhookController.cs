// Api/Controllers/WebhookController.cs
using Microsoft.AspNetCore.Mvc;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Application.DTOs.Pagamentos;
using Microsoft.AspNetCore.Authorization;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class WebhookController : ControllerBase
{
    private readonly IPagamentoService _pagamentoService;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(IPagamentoService pagamentoService, ILogger<WebhookController> logger)
    {
        _pagamentoService = pagamentoService;
        _logger = logger;
    }

    /// <summary>
    /// ✅ PÚBLICO - Webhook do Mercado Pago
    /// 🔒 Valida assinatura x-signature para garantir autenticidade
    /// </summary>
    [HttpPost("pagamento")]
    [AllowAnonymous]
    public async Task<ActionResult> ReceberNotificacao(
        [FromBody] WebhookRequest request,
        [FromHeader(Name = "x-signature")] string? signature,
        [FromHeader(Name = "x-request-id")] string? requestId)
    {
        _logger.LogInformation(
            "Webhook recebido: Id={Id}, Topic={Topic}, RequestId={RequestId}", 
            request.Id, request.Topic, requestId);

        // 🔒 Validar assinatura do Mercado Pago
        if (!_pagamentoService.ValidarAssinaturaWebhook(request, signature, requestId))
        {
            _logger.LogWarning("⚠️ Webhook com assinatura inválida! Id={Id}", request.Id);
            return Unauthorized(); // Retorna 401 para assinatura inválida
        }

        try
        {
            await _pagamentoService.ConfirmarPagamentoAsync(request.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar webhook Id={Id}", request.Id);
            return Ok(); // Retorna 200 mesmo com erro pra não floodar o MP
        }
    }
}