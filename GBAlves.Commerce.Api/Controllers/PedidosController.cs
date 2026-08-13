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
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(IPedidoService pedidoService, AppDbContext context, ILogger<PedidosController> logger)
    {
        _pedidoService = pedidoService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// ✅ ADMIN - Listar todos os pedidos
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult<List<PedidoResponse>>> ObterTodos()
    {
        try
        {
            return Ok(await _pedidoService.ObterTodos());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar todos os pedidos");
            return StatusCode(500, new { erro = "Erro interno ao listar pedidos." });
        }
    }

    /// <summary>
    /// ✅ CLIENTE - Listar meus pedidos (validado por Claim)
    /// </summary>
    [HttpGet("meus")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<List<object>>> ObterMeusPedidos()
    {
        try
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioId)) 
                return Unauthorized(new { erro = "Usuário não autenticado" });

            var usuario = await _context.Usuarios
                .Include(u => u.Cliente)
                .FirstOrDefaultAsync(u => u.Id.ToString() == usuarioId);

            if (usuario?.Cliente is null) 
                return Ok(new List<object>());

            var pedidos = await _context.Pedidos
                .Where(p => p.ClienteId == usuario.Cliente.Id)
                .OrderByDescending(p => p.CriadoEm)
                .Select(p => new { p.Id, p.NumeroPedido, Status = p.Status.ToString(), p.Total, p.CriadoEm })
                .ToListAsync();

            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos do usuário");
            return StatusCode(500, new { erro = "Erro interno ao listar seus pedidos." });
        }
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
                return Unauthorized(new { erro = "Usuário não autenticado" });

            var pedido = await _pedidoService.Criar(request, Guid.Parse(usuarioId));
            _logger.LogInformation("Pedido criado: {Id} - #{Numero}", pedido.Id, pedido.NumeroPedido);
            return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
        }
        catch (InvalidOperationException ex)
        {
            // Erro de validação (produto, frete, estoque) - OK retornar
            _logger.LogWarning("Dados inválidos ao criar pedido: {Erro}", ex.Message);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, new { erro = "Erro interno ao criar pedido." });
        }
    }
    
    /// <summary>
    /// ✅ ADMIN - Verificar pagamento no Mercado Pago
    /// </summary>
    [HttpGet("{id:guid}/verificar-pagamento")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> VerificarPagamento(Guid id)
    {
        try
        {
            var result = await _pedidoService.VerificarPagamentoAsync(id);
            if (result is null) return NotFound(new { erro = "Pedido não encontrado" });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar pagamento do pedido {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao verificar pagamento." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Atualizar código de rastreio
    /// </summary>
    [HttpPut("{id:guid}/rastreio")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> AtualizarRastreio(Guid id, [FromBody] AtualizarRastreioRequest request)
    {
        try
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido is null) return NotFound(new { erro = "Pedido não encontrado" });

            pedido.CodigoRastreio = request.CodigoRastreio;
            pedido.Status = StatusPedido.Enviado;
            pedido.AtualizadoEm = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rastreio atualizado: Pedido #{Numero} - {Rastreio}", 
                pedido.NumeroPedido, request.CodigoRastreio);

            return Ok(new { mensagem = "Rastreio atualizado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar rastreio do pedido {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao atualizar rastreio." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Atualizar status do pedido
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> AtualizarStatus(Guid id, [FromBody] string novoStatus)
    {
        try
        {
            var result = await _pedidoService.AtualizarStatus(id, novoStatus);
            if (!result) return NotFound(new { erro = "Pedido não encontrado" });
            
            _logger.LogInformation("Status atualizado: Pedido {Id} → {Status}", id, novoStatus);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao atualizar status." });
        }
    }

    /// <summary>
    /// ✅ MISTO - Obter pedido por ID (admin vê qualquer, cliente só o seu)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Cliente")]
    public async Task<ActionResult<PedidoResponse>> ObterPorId(Guid id)
    {
        try
        {
            var pedido = await _pedidoService.ObterPorId(id);
            if (pedido is null) return NotFound(new { erro = "Pedido não encontrado" });

            // Cliente só pode ver seus próprios pedidos
            if (!User.IsInRole("Admin") && !User.IsInRole("Operador"))
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var usuario = await _context.Usuarios
                    .Include(u => u.Cliente)
                    .FirstOrDefaultAsync(u => u.Id.ToString() == usuarioId);

                if (usuario?.ClienteId != pedido.ClienteId)
                {
                    _logger.LogWarning("Tentativa de acesso não autorizado: Usuario {UsuarioId} → Pedido {PedidoId}", 
                        usuarioId, id);
                    return Forbid();
                }
            }

            return Ok(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao carregar pedido." });
        }
    }

    /// <summary>
    /// ✅ ADMIN - Cancelar pedido
    /// </summary>
    [HttpPost("{id:guid}/cancelar")]
    [Authorize(Policy = "Operador")]
    public async Task<ActionResult> CancelarPedido(Guid id)
    {
        try
        {
            var result = await _pedidoService.CancelarPedidoAsync(id);
            if (!result) return NotFound(new { erro = "Pedido não encontrado" });
            
            _logger.LogInformation("Pedido cancelado: {Id}", id);
            return Ok(new { mensagem = "Pedido cancelado e estoque restaurado" });
        }
        catch (InvalidOperationException ex)
        {
            // Erro de regra de negócio (pedido pago não pode cancelar) - OK retornar
            _logger.LogWarning("Tentativa de cancelar pedido pago: {Id}", id);
            return BadRequest(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar pedido {Id}", id);
            return StatusCode(500, new { erro = "Erro interno ao cancelar pedido." });
        }
    }
}