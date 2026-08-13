// Application/Services/RelatorioService.cs
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Infrastructure.Data;
using GBAlves.Commerce.Application.Interfaces;

namespace GBAlves.Commerce.Application.Services;

public class RelatorioService : IRelatorioService
{
    private readonly AppDbContext _context;

    public RelatorioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GerarExcelAsync()
    {
        using var workbook = new XLWorkbook();

        // Aba 1 - Vendas
        var abaVendas = workbook.Worksheets.Add("Vendas");
        await GerarAbaVendas(abaVendas);

        // Aba 2 - Produtos
        var abaProdutos = workbook.Worksheets.Add("Produtos");
        await GerarAbaProdutos(abaProdutos);

        // Aba 3 - Clientes
        var abaClientes = workbook.Worksheets.Add("Clientes");
        await GerarAbaClientes(abaClientes);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    private async Task GerarAbaVendas(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Relatório de Vendas - GBAlves Commerce";
        ws.Range("A1:F1").Merge().Style.Font.Bold = true;
        ws.Cell(2, 1).Value = $"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}";

        var pedidos = await _context.Pedidos
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        // Cabeçalho
        ws.Cell(4, 1).Value = "Pedido";
        ws.Cell(4, 2).Value = "Data";
        ws.Cell(4, 3).Value = "Cliente";
        ws.Cell(4, 4).Value = "Total";
        ws.Cell(4, 5).Value = "Status";
        ws.Cell(4, 6).Value = "Pagamento";
        ws.Range("A4:F4").Style.Font.Bold = true;

        int row = 5;
        foreach (var p in pedidos)
        {
            var pagamento = await _context.Pagamentos
                .FirstOrDefaultAsync(pg => pg.PedidoId == p.Id);

            ws.Cell(row, 1).Value = $"#{p.NumeroPedido}";
            ws.Cell(row, 2).Value = p.CriadoEm.ToString("dd/MM/yyyy");
            ws.Cell(row, 3).Value = p.Cliente?.Nome ?? "-";
            ws.Cell(row, 4).Value = p.Total;
            ws.Cell(row, 5).Value = p.Status.ToString();
            ws.Cell(row, 6).Value = pagamento?.Metodo.ToString() ?? "-";
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private async Task GerarAbaProdutos(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Relatório de Produtos";

        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();

        ws.Cell(3, 1).Value = "Produto";
        ws.Cell(3, 2).Value = "Categoria";
        ws.Cell(3, 3).Value = "Preço";
        ws.Cell(3, 4).Value = "Estoque";
        ws.Cell(3, 5).Value = "Destaque";
        ws.Range("A3:E3").Style.Font.Bold = true;

        int row = 4;
        foreach (var p in produtos)
        {
            ws.Cell(row, 1).Value = p.Nome;
            ws.Cell(row, 2).Value = p.Categoria?.Nome ?? "-";
            ws.Cell(row, 3).Value = p.Preco;
            ws.Cell(row, 4).Value = p.Estoque;
            ws.Cell(row, 5).Value = p.Destaque ? "Sim" : "Não";
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private async Task GerarAbaClientes(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "Relatório de Clientes";

        var clientes = await _context.Clientes
            .Include(c => c.Pedidos)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();

        ws.Cell(3, 1).Value = "Nome";
        ws.Cell(3, 2).Value = "Email";
        ws.Cell(3, 3).Value = "Telefone";
        ws.Cell(3, 4).Value = "Total Pedidos";
        ws.Cell(3, 5).Value = "Total Gasto";
        ws.Range("A3:E3").Style.Font.Bold = true;

        int row = 4;
        foreach (var c in clientes)
        {
            ws.Cell(row, 1).Value = c.Nome;
            ws.Cell(row, 2).Value = c.Email;
            ws.Cell(row, 3).Value = c.Telefone ?? "-";
            ws.Cell(row, 4).Value = c.Pedidos.Count;
            ws.Cell(row, 5).Value = c.Pedidos.Sum(p => p.Total);
            row++;
        }

        ws.Columns().AdjustToContents();
    }
}