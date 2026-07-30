// Application/Interfaces/IDashboardService.cs
namespace GBAlves.Commerce.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponse> ObterResumo();
}

public record DashboardResponse(
    int TotalPedidosHoje,
    decimal Faturamento,
    int TotalProdutos,
    int TotalClientes,
    List<PedidoRecenteResponse> UltimosPedidos
);

public record PedidoRecenteResponse(
    Guid Id,
    int NumeroPedido,
    string ClienteNome,
    decimal Total,
    string Status,
    DateTime CriadoEm
);