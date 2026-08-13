// Application/Services/EmailService.cs
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Infrastructure.Data;

namespace GBAlves.Commerce.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public EmailService(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    private async Task<(string storeName, string storeEmail, string? storePhone, string? storeLogo)> ObterConfiguracaoLojaAsync()
    {
        var config = await _context.ConfiguracaoLoja.FirstOrDefaultAsync();

        return (
            config?.NomeLoja ?? "GBAlves Commerce",
            config?.Email ?? _configuration["Email:From"] ?? "contato@gbalves.com",
            config?.Whatsapp,
            config?.Logo
        );
    }

    public async Task EnviarConfirmacaoPedidoAsync(string email, int numeroPedido, decimal total)
    {
        var (storeName, _, storePhone, storeLogo) = await ObterConfiguracaoLojaAsync();

        var headerHtml = !string.IsNullOrEmpty(storeLogo)
            ? $"<div style='text-align:center;padding:20px;background:white;'>" +
              $"<img src='{storeLogo}' alt='{storeName}' style='max-width:180px;max-height:80px;'>" +
              $"</div>"
            : $"<div style='background:#1a237e;padding:24px;text-align:center;'>" +
              $"<h1 style='color:white;margin:0;font-size:20px;'>{storeName}</h1></div>";

        var corpo = $@"<div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
            {headerHtml}
            <div style='padding:24px;background:white;'>
                <h2>Pedido Confirmado! 🎉</h2>
                <p>Seu pedido <strong>#{numeroPedido}</strong> foi recebido.</p>
                <p style='font-size:22px;color:#1a237e;font-weight:bold;'>Total: R$ {total:N2}</p>
                <p>Acompanhe pelo site.</p>
                {(storePhone != null ? $"<p>WhatsApp: {storePhone}</p>" : "")}
            </div>
            <div style='background:#f5f5f5;padding:12px;text-align:center;font-size:11px;color:#999;'>
                {storeName} - Todos os direitos reservados
            </div></div>";

        await EnviarEmailAsync(email, $"Pedido #{numeroPedido} Confirmado - {storeName}", corpo);
    }

    public async Task EnviarStatusAtualizadoAsync(string email, int numeroPedido, string status)
    {
        var (storeName, _, storePhone, storeLogo) = await ObterConfiguracaoLojaAsync();

        var headerHtml = !string.IsNullOrEmpty(storeLogo)
            ? $"<div style='text-align:center;padding:20px;background:white;'>" +
              $"<img src='{storeLogo}' alt='{storeName}' style='max-width:180px;max-height:80px;'>" +
              $"</div>"
            : $"<div style='background:#1a237e;padding:24px;text-align:center;'>" +
              $"<h1 style='color:white;margin:0;font-size:20px;'>{storeName}</h1></div>";

        var corpo = $@"<div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
            {headerHtml}
            <div style='padding:24px;background:white;'>
                <h2>Status Atualizado 📦</h2>
                <p>Pedido <strong>#{numeroPedido}</strong></p>
                <p style='font-size:18px;color:#1a237e;font-weight:bold;'>Status: {status}</p>
                {(storePhone != null ? $"<p>WhatsApp: {storePhone}</p>" : "")}
            </div>
            <div style='background:#f5f5f5;padding:12px;text-align:center;font-size:11px;color:#999;'>
                {storeName} - Todos os direitos reservados
            </div></div>";

        await EnviarEmailAsync(email, $"Pedido #{numeroPedido} - {status}", corpo);
    }

    private async Task EnviarEmailAsync(string email, string assunto, string corpo)
    {
        using var smtp = new SmtpClient
        {
            Host = _configuration["Email:Host"] ?? "smtp.gmail.com",
            Port = int.Parse(_configuration["Email:Port"] ?? "587"),
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                _configuration["Email:Username"] ?? "",
                _configuration["Email:Password"] ?? ""
            )
        };

        using var mensagem = new MailMessage
        {
            From = new MailAddress(_configuration["Email:From"] ?? "contato@gbalves.com"),
            Subject = assunto,
            Body = corpo,
            IsBodyHtml = true
        };
        mensagem.To.Add(email);

        await smtp.SendMailAsync(mensagem);
    }
}