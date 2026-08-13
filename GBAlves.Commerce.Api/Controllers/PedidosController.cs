// Api/Controllers/PedidosController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.DTOs.Pedidos;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Infrastructure.Data;
using GBAlves.Commerce.Domain.Enums;

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

    /// <summary>
    //✅ ADMIN - Listar todos os pedidos
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<List<PedidoResponse>>> ObterTodos()
    {
        return Ok(await _pedidoService.ObterTodos());
    }

    /// <summary>
    //✅ CLIENTE - Listar meus pedidos (validado por Claim)
    /// </summary>
    [HttpGet("meus")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<List<object>>> ObterMeusPedidos()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId)) return Unauthorized();

        var usuario = await _context.Usuarios
            .Include(u => u.Cliente)
            .FirstOrDefaultAsync(u => u.Id.ToString() == usuarioId);

        if (usuario?.Cliente is null) return Ok(new List<object>());

        var pedidos = await _context.Pedidos
            .Where(p => p.ClienteId == usuario.Cliente.Id)  // ← valida que é DO cliente
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => new { p.Id, p.NumeroPedido, Status = p.Status.ToString(), p.Total, p.CriadoEm })
            .ToListAsync();

        return Ok(pedidos);
    }

    /// <summary>
    /// ✅ CLIENTE - Criar pedido
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<PedidoResponse>> Criar([FromBody] CriarPedidoRequest request)
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioId))
                return Unauthorized();

            var pedido = await _pedidoService.Criar(request, Guid.Parse(usuarioId));
            return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
    
    /// <summary>
    //✅ ADMIN - Verificar pagamento no Mercado Pago
    /// </summary>
    [HttpGet("{id:guid}/verificar-pagamento")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> VerificarPagamento(Guid id)
    {
        var result = await _pedidoService.VerificarPagamentoAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    //✅ ADMIN - Atualizar código de rastreio
    /// </summary>
    [HttpPut("{id:guid}/rastreio")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> AtualizarRastreio(Guid id, [FromBody] AtualizarRastreioRequest request)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido is null) return NotFound();

        pedido.CodigoRastreio = request.CodigoRastreio;
        pedido.Status = StatusPedido.Enviado;
        pedido.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { mensagem = "Rastreio atualizado" });
    }

    /// <summary>
    //✅ ADMIN - Atualizar status do pedido
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> AtualizarStatus(Guid id, [FromBody] string novoStatus)
    {
        var result = await _pedidoService.AtualizarStatus(id, novoStatus);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    //✅ MISTO - Obter pedido por ID (admin vê qualquer, cliente só o seu)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<PedidoResponse>> ObterPorId(Guid id)
    {
        var pedido = await _pedidoService.ObterPorId(id);
        if (pedido is null) return NotFound();

        // Cliente só pode ver seus próprios pedidos
        if (!User.IsInRole("Admin") && !User.IsInRole("Operador"))
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.Id.ToString() == usuarioId);

            if (usuario?.ClienteId != pedido.ClienteId)
                return Forbid();
        }

        return Ok(pedido);
    }

    /// <summary>
    //✅ ADMIN - Cancelar pedido
    /// </summary>
    [HttpPost("{id:guid}/cancelar")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> CancelarPedido(Guid id)
    {
        try
        {
            var result = await _pedidoService.CancelarPedidoAsync(id);
            if (!result) return NotFound();
            return Ok(new { mensagem = "Pedido cancelado e estoque restaurado" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}