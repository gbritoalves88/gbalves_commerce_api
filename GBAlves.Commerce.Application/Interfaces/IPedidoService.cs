// Application/Interfaces/IPedidoService.cs
using GBAlves.Commerce.Application.DTOs.Pedidos;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IPedidoService
{
    Task<List<PedidoResponse>> ObterTodos();
    Task<PedidoResponse?> ObterPorId(Guid id);
    Task<PedidoResponse> Criar(CriarPedidoRequest request, Guid usuarioId);
    Task<bool> AtualizarStatus(Guid id, string novoStatus);
    Task<bool> CancelarPedidoAsync(Guid id);
    Task<object?> VerificarPagamentoAsync(Guid pedidoId);
}