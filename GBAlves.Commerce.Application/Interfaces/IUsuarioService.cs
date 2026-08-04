// Application/Interfaces/IUsuarioService.cs
using GBAlves.Commerce.Application.DTOs.Usuarios;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioResponse>> ObterTodos();
    Task<UsuarioResponse?> ObterPorId(Guid id);
    Task<UsuarioResponse> Criar(CriarUsuarioRequest request);
    Task<bool> Excluir(Guid id);
    Task<UsuarioResponse?> Atualizar(Guid id, AtualizarUsuarioRequest request);
}