// Application/Interfaces/IFreteService.cs
using GBAlves.Commerce.Application.DTOs.Fretes;

namespace GBAlves.Commerce.Application.Interfaces;

public interface IFreteService
{
    Task<List<FreteResponse>> CalcularFreteAsync(CalcularFreteRequest request);
}