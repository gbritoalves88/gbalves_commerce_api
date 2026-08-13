// Infrastructure/Payment/MercadoPagoConfiguration.cs
namespace GBAlves.Commerce.Infrastructure.Payment;

public class MercadoPagoConfiguration
{
    public string AccessToken { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public bool UseSandbox { get; set; } = true;
}