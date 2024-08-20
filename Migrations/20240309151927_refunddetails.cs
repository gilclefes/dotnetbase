using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class refunddetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "179c1e8e-1c64-4086-8562-ac465709d01b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "afea8e64-2a1b-41df-891b-c96f5c741efb");

            migrationBuilder.CreateTable(
                name: "order_refunds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    extransaciton_id = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    refund_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    refund_retry = table.Column<int>(type: "int", nullable: false),
                    last_retry_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    refund_status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    refund_ex_transaction_id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    refund_reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_refunds", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_refunds_clients_client_id",
                        column: x => x.order_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_refunds_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "order_refund_details",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    order_refund_id = table.Column<int>(type: "int", nullable: false),
                    request_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    refund_status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    response_details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_refund_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_refund_details_order_refunds_order_refund_id",
                        column: x => x.order_refund_id,
                        principalTable: "order_refunds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "1af92d29-1a1d-40eb-87fc-15ea047c5fe1", null, "Role", "User", "USER" },
                    { "733d7815-08cf-449f-95c7-97af9da24bd8", null, "Role", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_order_refund_details_order_refund_id",
                table: "order_refund_details",
                column: "order_refund_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_refunds_order_id",
                table: "order_refunds",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_refund_details");

            migrationBuilder.DropTable(
                name: "order_refunds");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "1af92d29-1a1d-40eb-87fc-15ea047c5fe1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "733d7815-08cf-449f-95c7-97af9da24bd8");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "179c1e8e-1c64-4086-8562-ac465709d01b", null, "Role", "Admin", "ADMIN" },
                    { "afea8e64-2a1b-41df-891b-c96f5c741efb", null, "Role", "User", "USER" }
                });
        }
    }
}
