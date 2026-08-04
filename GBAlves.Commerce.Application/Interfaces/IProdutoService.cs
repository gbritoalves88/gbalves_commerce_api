using GBAlves.Commerce.Application.DTOs.Produtos;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IProdutoService
{
    Task<List<ProdutoResponse>> ObterTodos();
    Task<List<ProdutoResponse>> ObterDestaques();
    Task<List<ProdutoResponse>> ObterPorCategoria(string slug);
    Task<ProdutoResponse?> ObterPorId(Guid id);
    Task<ProdutoResponse> Criar(CriarProdutoRequest request);
    Task<ProdutoResponse?> Atualizar(Guid id, AtualizarProdutoRequest request);
    Task<bool> Excluir(Guid id);
    Task<ProdutoResponse?> ObterPorSlug(string slug);
    Task<List<ProdutoResponse>> Buscar(string termo);
}