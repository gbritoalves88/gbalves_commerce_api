// Application/Interfaces/ICategoriaService.cs
using GBAlves.Commerce.Application.DTOs.Categorias;

namespace GBAlves.Commerce.Application.Interfaces;

public interface ICategoriaService
{
    Task<List<CategoriaResponse>> ObterTodas();
    Task<CategoriaResponse?> ObterPorId(Guid id);
    Task<CategoriaResponse> Criar(CriarCategoriaRequest request);
    Task<CategoriaResponse?> Atualizar(Guid id, AtualizarCategoriaRequest request);
    Task<bool> Excluir(Guid id);
}