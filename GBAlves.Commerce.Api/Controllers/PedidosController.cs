// Api/Controllers/PedidosController.cs
using Microsoft.AspNetCore.Mvc;
using GBAlves.Commerce.Application.DTOs.Pedidos;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PedidoResponse>>> ObterTodos()
    {
        return Ok(await _pedidoService.ObterTodos());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PedidoResponse>> ObterPorId(Guid id)
    {
        var pedido = await _pedidoService.ObterPorId(id);
        if (pedido is null) return NotFound();
        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<PedidoResponse>> Criar([FromBody] CriarPedidoRequest request)
    {
        var pedido = await _pedidoService.Criar(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult> AtualizarStatus(Guid id, [FromBody] string novoStatus)
    {
        var result = await _pedidoService.AtualizarStatus(id, novoStatus);
        if (!result) return NotFound();
        return NoContent();
    }
}