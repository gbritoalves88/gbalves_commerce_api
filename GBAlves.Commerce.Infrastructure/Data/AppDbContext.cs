// Infrastructure/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using GBAlves.Commerce.Domain.Entities;
using GBAlves.Commerce.Domain.Enums;

namespace GBAlves.Commerce.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ConfiguracaoLoja> ConfiguracaoLoja { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<ItemPedido> ItensPedido { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<ItemMenu> ItensMenu { get; set; }
    public DbSet<Banner> Banners { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfiguracaoLoja>(e =>
        {
            e.ToTable("configuracao_loja");
            e.HasKey(x => x.Id);
            e.Property(x => x.NomeLoja).HasColumnName("nome_loja").HasMaxLength(100);
            e.Property(x => x.CorPrimaria).HasColumnName("cor_primaria").HasMaxLength(7);
            e.Property(x => x.CorSecundaria).HasColumnName("cor_secundaria").HasMaxLength(7);
            e.Property(x => x.CorFundo).HasColumnName("cor_fundo").HasMaxLength(7);
            e.Property(x => x.CorTexto).HasColumnName("cor_texto").HasMaxLength(7);
            e.Property(x => x.BannerDesktop).HasColumnName("banner_desktop");
            e.Property(x => x.BannerMobile).HasColumnName("banner_mobile");
            e.Property(x => x.Whatsapp).HasColumnName("whatsapp").HasMaxLength(20);
            e.Property(x => x.SeoTitulo).HasColumnName("seo_titulo").HasMaxLength(60);
            e.Property(x => x.SeoDescricao).HasColumnName("seo_descricao").HasMaxLength(160);
            e.Property(x => x.SeoPalavras).HasColumnName("seo_palavras").HasMaxLength(200);
            e.Property(x => x.GoogleAnalytics).HasColumnName("google_analytics").HasMaxLength(50);
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200);
            e.Property(x => x.SenhaHash).HasColumnName("senha_hash").HasMaxLength(500);
            e.Property(x => x.NivelAcesso).HasColumnName("nivel_acesso").HasMaxLength(20);
            e.Property(x => x.Ativo).HasColumnName("ativo");
            e.Property(x => x.ClienteId).HasColumnName("cliente_id");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");

            e.HasOne(x => x.Cliente)
                .WithMany()
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Cliente>(e =>
        {
            e.ToTable("clientes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(200);
            e.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(20);
            e.Property(x => x.Cpf).HasColumnName("cpf").HasMaxLength(14);
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        });

        modelBuilder.Entity<Categoria>(e =>
        {
            e.ToTable("categorias");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100);
            e.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(100);
            e.Property(x => x.ImagemUrl).HasColumnName("imagem_url");
            e.Property(x => x.Ativo).HasColumnName("ativo");
            e.Property(x => x.OrdemExibicao).HasColumnName("ordem_exibicao");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        });

        modelBuilder.Entity<Produto>(e =>
        {
            e.ToTable("produtos");
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Slug).IsUnique();
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(200);
            e.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(200);
            e.Property(x => x.Descricao).HasColumnName("descricao");
            e.Property(x => x.Preco).HasColumnName("preco").HasColumnType("decimal(10,2)");
            e.Property(x => x.PrecoOriginal).HasColumnName("preco_original").HasColumnType("decimal(10,2)");
            e.Property(x => x.ImagemUrl).HasColumnName("imagem_url");
            e.Property(x => x.Estoque).HasColumnName("estoque");
            e.Property(x => x.Destaque).HasColumnName("destaque");
            e.Property(x => x.Ativo).HasColumnName("ativo");
            e.Property(x => x.CategoriaId).HasColumnName("categoria_id");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");

            e.HasOne(x => x.Categoria)
                .WithMany(x => x.Produtos)
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Pedido>(e =>
        {
            e.ToTable("pedidos");
            e.HasKey(x => x.Id);
            e.Property(x => x.NumeroPedido).HasColumnName("numero_pedido").UseSerialColumn();
            e.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
            e.Property(x => x.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(10,2)");
            e.Property(x => x.Frete).HasColumnName("frete").HasColumnType("decimal(10,2)");
            e.Property(x => x.Total).HasColumnName("total").HasColumnType("decimal(10,2)");
            e.Property(x => x.Cep).HasColumnName("cep").HasMaxLength(8);
            e.Property(x => x.Rua).HasColumnName("rua").HasMaxLength(200);
            e.Property(x => x.Numero).HasColumnName("numero").HasMaxLength(20);
            e.Property(x => x.Complemento).HasColumnName("complemento").HasMaxLength(100);
            e.Property(x => x.Bairro).HasColumnName("bairro").HasMaxLength(100);
            e.Property(x => x.Cidade).HasColumnName("cidade").HasMaxLength(100);
            e.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(2);
            e.Property(x => x.FormaPagamento).HasColumnName("forma_pagamento").HasMaxLength(20);
            e.Property(x => x.StatusPagamento).HasColumnName("status_pagamento").HasMaxLength(20);
            e.Property(x => x.Observacao).HasColumnName("observacao");
            e.Property(x => x.ClienteId).HasColumnName("cliente_id");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
            e.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");

            e.HasOne(x => x.Cliente)
                .WithMany(x => x.Pedidos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ItemPedido>(e =>
        {
            e.ToTable("itens_pedido");
            e.HasKey(x => x.Id);
            e.Property(x => x.PedidoId).HasColumnName("pedido_id");
            e.Property(x => x.ProdutoId).HasColumnName("produto_id");
            e.Property(x => x.NomeProduto).HasColumnName("nome_produto").HasMaxLength(200);
            e.Property(x => x.ImagemProduto).HasColumnName("imagem_produto");
            e.Property(x => x.Preco).HasColumnName("preco").HasColumnType("decimal(10,2)");
            e.Property(x => x.Quantidade).HasColumnName("quantidade");
            e.Property(x => x.Total).HasColumnName("total").HasColumnType("decimal(10,2)");

            e.HasOne(x => x.Pedido)
                .WithMany(x => x.Itens)
                .HasForeignKey(x => x.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Produto)
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Menu>(e =>
        {
            e.ToTable("menus");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100);
            e.Property(x => x.Localizacao).HasColumnName("localizacao").HasMaxLength(50);
            e.Property(x => x.Ativo).HasColumnName("ativo");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
        });

        modelBuilder.Entity<ItemMenu>(e =>
        {
            e.ToTable("itens_menu");
            e.HasKey(x => x.Id);
            e.Property(x => x.MenuId).HasColumnName("menu_id");
            e.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100);
            e.Property(x => x.Slug).HasColumnName("slug").HasMaxLength(100);
            e.Property(x => x.Url).HasColumnName("url");
            e.Property(x => x.Icone).HasColumnName("icone").HasMaxLength(50);
            e.Property(x => x.ItemPaiId).HasColumnName("item_pai_id");
            e.Property(x => x.OrdemExibicao).HasColumnName("ordem_exibicao");
            e.Property(x => x.Ativo).HasColumnName("ativo");
            e.Property(x => x.CategoriaId).HasColumnName("categoria_id");

            e.HasOne(x => x.Menu)
                .WithMany(x => x.Itens)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.ItemPai)
                .WithMany(x => x.SubItens)
                .HasForeignKey(x => x.ItemPaiId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Categoria)
                .WithMany()
                .HasForeignKey(x => x.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Banner>(e =>
        {
            e.ToTable("banners");
            e.HasKey(x => x.Id);
            e.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(100);
            e.Property(x => x.Subtitulo).HasColumnName("subtitulo").HasMaxLength(200);
            e.Property(x => x.ImagemUrl).HasColumnName("imagem_url");
            e.Property(x => x.Link).HasColumnName("link");
            e.Property(x => x.TextoBotao).HasColumnName("texto_botao").HasMaxLength(50);
            e.Property(x => x.OrdemExibicao).HasColumnName("ordem_exibicao");
            e.Property(x => x.Ativo).HasColumnName("ativo");
            e.Property(x => x.CriadoEm).HasColumnName("criado_em");
        });
    }
}