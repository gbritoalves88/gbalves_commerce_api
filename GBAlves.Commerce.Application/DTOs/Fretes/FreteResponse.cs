// Application/DTOs/Fretes/FreteResponse.cs
namespace GBAlves.Commerce.Application.DTOs.Fretes;

public record FreteResponse(
    string Id,
    string Nome,
    string Transportadora,
    decimal Valor,
    int PrazoEntrega
);