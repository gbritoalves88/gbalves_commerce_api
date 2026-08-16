// Application/Services/MelhorEnvioFreteService.cs
using System.Text;
using System.Text.Json;
using GBAlves.Commerce.Application.DTOs.Fretes;
using GBAlves.Commerce.Application.Interfaces;
using GBAlves.Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GBAlves.Commerce.Application.Services;

public class MelhorEnvioFreteService : IFreteService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MelhorEnvioFreteService> _logger;

    public MelhorEnvioFreteService(
        AppDbContext context, 
        IConfiguration configuration,
        ILogger<MelhorEnvioFreteService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["MelhorEnvio:Token"]}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "GBAlves Commerce");
    }

    public async Task<List<FreteResponse>> CalcularFreteAsync(CalcularFreteRequest request)
    {
        var cepOrigem = _configuration["MelhorEnvio:CepOrigem"] ?? "01001000";

        var produtos = new List<object>();
        foreach (var item in request.Itens)
        {
            var produto = await _context.Produtos.FindAsync(item.ProdutoId);
            if (produto is null) continue;

            produtos.Add(new
            {
                width = produto.Largura > 0 ? produto.Largura : 15,
                height = produto.Altura > 0 ? produto.Altura : 10,
                length = produto.Comprimento > 0 ? produto.Comprimento : 20,
                weight = produto.Peso > 0 ? (float)produto.Peso : 0.3f,
                quantity = item.Quantidade
            });
        }

        if (!produtos.Any()) return new List<FreteResponse>();

        var payload = new { 
            from = new { postal_code = cepOrigem }, 
            to = new { postal_code = request.CepDestino }, 
            products = produtos 
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://melhorenvio.com.br/api/v2/me/shipment/calculate", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) 
        {
            _logger.LogWarning("Melhor Envio retornou erro: {Status} - {Body}", 
                response.StatusCode, responseBody);
            return new List<FreteResponse>();
        }

        var result = JsonSerializer.Deserialize<List<JsonElement>>(responseBody);
        if (result is null) return new List<FreteResponse>();

        var fretes = new List<FreteResponse>();
        foreach (var r in result)
        {
            try
            {
                if (r.TryGetProperty("error", out _)) continue;

                var id = r.GetProperty("id").ValueKind == JsonValueKind.Number
                    ? r.GetProperty("id").GetInt32().ToString()
                    : r.GetProperty("id").GetString()!;

                var nome = r.GetProperty("name").GetString()!;

                decimal valor = 0;
                if (r.TryGetProperty("price", out var p))
                    valor = p.ValueKind == JsonValueKind.String ? decimal.Parse(p.GetString()!) : p.GetDecimal();

                int prazo = 0;
                if (r.TryGetProperty("delivery_time", out var d))
                    prazo = d.ValueKind == JsonValueKind.String ? int.Parse(d.GetString()!) : d.GetInt32();

                string transportadora = "Desconhecido";
                if (r.TryGetProperty("company", out var c) && c.ValueKind == JsonValueKind.Object)
                    if (c.TryGetProperty("name", out var n)) transportadora = n.GetString()!;

                if (valor > 0)
                {
                    // ========== CORREÇÃO DO VALOR ==========
                    // Local: Melhor Envio retorna em CENTAVOS (ex: 2646 = R$ 26,46)
                    // Produção: Melhor Envio retorna em REAIS (ex: 14.33 = R$ 14,33)
                    // Se valor > 100, assume centavos e divide por 100
                    // Se valor < 100, assume reais e mantém
                    decimal valorCorrigido = valor > 100 ? valor / 100 : valor;
                    
                    _logger.LogInformation("Frete calculado: {Transportadora} {Nome} - Valor original: {ValorOriginal} → Corrigido: {ValorCorrigido}", 
                        transportadora, nome, valor, valorCorrigido);
                    
                    fretes.Add(new FreteResponse(id, nome, transportadora, valorCorrigido, prazo));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao processar opção de frete");
            }
        }

        return fretes;
    }
}