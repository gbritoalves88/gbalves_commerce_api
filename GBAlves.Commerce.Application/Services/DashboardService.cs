// Application/Services/DashboardService.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse> ObterResumo()
    {
        var hoje = DateTime.UtcNow.Date;

        var totalPedidosHoje = await _context.Pedidos
            .CountAsync(p => p.CriadoEm.Date == hoje);

        var faturamento = await _context.Pedidos
            .Where(p => p.Status == Domain.Enums.StatusPedido.Pago)
            .SumAsync(p => p.Total);

        var totalProdutos = await _context.Produtos
            .CountAsync(p => p.Ativo);

        var totalClientes = await _context.Clientes.CountAsync();

        var ultimosPedidos = await _context.Pedidos
            .OrderByDescending(p => p.CriadoEm)
            .Take(5)
            .Select(p => new PedidoRecenteResponse(
                p.Id, p.NumeroPedido, p.Cliente!.Nome,
                p.Total, p.Status.ToString(), p.CriadoEm
            ))
            .ToListAsync();

        return new DashboardResponse(
            totalPedidosHoje, faturamento, totalProdutos,
            totalClientes, ultimosPedidos
        );
    }
}