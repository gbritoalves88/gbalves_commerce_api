// Application/Interfaces/IPedidoService.cs
using GBAlves.Commerce.Application.DTOs.Pedidos;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IPedidoService
{
    Task<List<PedidoResponse>> ObterTodos();
    Task<PedidoResponse?> ObterPorId(Guid id);
    Task<PedidoResponse> Criar(CriarPedidoRequest request);
    Task<bool> AtualizarStatus(Guid id, string novoStatus);
}