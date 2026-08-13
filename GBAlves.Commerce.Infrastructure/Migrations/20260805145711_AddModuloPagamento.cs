using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GBAlves.Commerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "forma_pagamento",
                table: "pedidos");

            migrationBuilder.DropColumn(
                name: "status_pagamento",
                table: "pedidos");

            migrationBuilder.CreateTable(
                name: "pagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    pedido_id = table.Column<Guid>(type: "uuid", nullable: false),
                    gateway = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    gateway_pagamento_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    metodo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    confirmado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PedidoId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagamentos_pedidos_PedidoId1",
                        column: x => x.PedidoId1,
                        principalTable: "pedidos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_pagamentos_pedidos_pedido_id",
                        column: x => x.pedido_id,
                        principalTable: "pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "eventos_pagamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    pagamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    evento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dados = table.Column<string>(type: "jsonb", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos_pagamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_eventos_pagamento_pagamentos_pagamento_id",
                        column: x => x.pagamento_id,
                        principalTable: "pagamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "logs_pagamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    pagamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    conteudo = table.Column<string>(type: "jsonb", nullable: true),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs_pagamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_logs_pagamento_pagamentos_pagamento_id",
                        column: x => x.pagamento_id,
                        principalTable: "pagamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_eventos_pagamento_pagamento_id",
                table: "eventos_pagamento",
                column: "pagamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_logs_pagamento_pagamento_id",
                table: "logs_pagamento",
                column: "pagamento_id");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_pedido_id",
                table: "pagamentos",
                column: "pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_pagamentos_PedidoId1",
                table: "pagamentos",
                column: "PedidoId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "eventos_pagamento");

            migrationBuilder.DropTable(
                name: "logs_pagamento");

            migrationBuilder.DropTable(
                name: "pagamentos");

            migrationBuilder.AddColumn<string>(
                name: "forma_pagamento",
                table: "pedidos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_pagamento",
                table: "pedidos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
