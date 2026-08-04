using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GBAlves.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteIdToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "cliente_id",
                table: "usuarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_cliente_id",
                table: "usuarios",
                column: "cliente_id");

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_clientes_cliente_id",
                table: "usuarios",
                column: "cliente_id",
                principalTable: "clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_clientes_cliente_id",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_cliente_id",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "cliente_id",
                table: "usuarios");
        }
    }
}