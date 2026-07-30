// Application/Interfaces/IBannerService.cs
using GBAlves.Commerce.Application.DTOs.Banners;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IBannerService
{
    Task<List<BannerResponse>> ObterTodos();
    Task<List<BannerResponse>> ObterAtivos();
    Task<BannerResponse?> ObterPorId(Guid id);
    Task<BannerResponse> Criar(CriarBannerRequest request);
    Task<BannerResponse?> Atualizar(Guid id, CriarBannerRequest request);
    Task<bool> Excluir(Guid id);
}