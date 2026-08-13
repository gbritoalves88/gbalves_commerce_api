// Application/Interfaces/IEmailService.cs
namespace GBAlves.Commerce.Application.Interfaces;

public interface IEmailService
{
    Task EnviarConfirmacaoPedidoAsync(string email, int numeroPedido, decimal total);
    Task EnviarStatusAtualizadoAsync(string email, int numeroPedido, string status);
}