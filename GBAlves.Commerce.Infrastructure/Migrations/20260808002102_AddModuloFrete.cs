using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GBAlves.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloFrete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Altura",
                table: "produtos",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Comprimento",
                table: "produtos",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Largura",
                table: "produtos",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Peso",
                table: "produtos",
                type: "numeric(10,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "codigo_rastreio",
                table: "pedidos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_envio",
                table: "pedidos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "servico_frete",
                table: "pedidos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transportadora",
                table: "pedidos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Altura", table: "produtos");
            migrationBuilder.DropColumn(name: "Comprimento", table: "produtos");
            migrationBuilder.DropColumn(name: "Largura", table: "produtos");
            migrationBuilder.DropColumn(name: "Peso", table: "produtos");
            migrationBuilder.DropColumn(name: "codigo_rastreio", table: "pedidos");
            migrationBuilder.DropColumn(name: "data_envio", table: "pedidos");
            migrationBuilder.DropColumn(name: "servico_frete", table: "pedidos");
            migrationBuilder.DropColumn(name: "transportadora", table: "pedidos");
        }
    }
}