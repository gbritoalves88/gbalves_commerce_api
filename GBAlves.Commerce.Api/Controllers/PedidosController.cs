// Api/Controllers/PedidosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Pedidos;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly AppDbContext _context;

    public PedidosController(IPedidoService pedidoService, AppDbContext context)
    {
        _pedidoService = pedidoService;
        _context = context;
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

    [HttpGet("meus")]
    [Authorize]
    public async Task<ActionResult<List<object>>> ObterMeusPedidos()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var usuario = await _context.Usuarios
            .Include(u => u.Cliente)
            .FirstOrDefaultAsync(u => u.Id.ToString() == usuarioId);

        if (usuario?.Cliente is null) return Ok(new List<object>());

        var pedidos = await _context.Pedidos
            .Where(p => p.ClienteId == usuario.Cliente.Id)
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => new
            {
                p.Id,
                p.NumeroPedido,
                Status = p.Status.ToString(),
                p.Total,
                p.CriadoEm
            })
            .ToListAsync();

        return Ok(pedidos);
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