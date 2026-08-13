// Application/Interfaces/IRelatorioService.cs
namespace GBAlves.Commerce.Application.Interfaces;

public interface IRelatorioService
{
    Task<byte[]> GerarExcelAsync();
}