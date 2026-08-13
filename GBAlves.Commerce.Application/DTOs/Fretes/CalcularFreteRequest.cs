// Application/DTOs/Fretes/CalcularFreteRequest.cs
namespace GBAlves.Commerce.Application.DTOs.Fretes;

public record CalcularFreteRequest(
    string CepDestino,
    List<FreteItemRequest> Itens
);

public record FreteItemRequest(
    Guid ProdutoId,
    int Quantidade
);