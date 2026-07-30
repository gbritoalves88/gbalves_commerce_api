// Application/Interfaces/IConfiguracaoLojaService.cs
using GBAlves.Commerce.Application.DTOs.ConfiguracaoLoja;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IConfiguracaoLojaService
{
    Task<ConfiguracaoLojaResponse> Obter();
    Task<ConfiguracaoLojaResponse> Atualizar(AtualizarConfiguracaoLojaRequest request);
}