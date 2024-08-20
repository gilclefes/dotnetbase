using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class orderreconadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "391b8bd7-36ef-4e69-aa9a-bd2ec84b9a61");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "a6fc90f2-84f4-4433-9111-6d15b1eb28cd");

            migrationBuilder.AddColumn<bool>(
                name: "is_yabo_charge",
                table: "charges",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "order_recons",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    partner_user_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    total_order_amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    yabo_share = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    partner_share = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    ordec_completion_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_recons", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_recons_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "477ccee7-d304-4d4f-aab0-13eb54e2c369", null, "Role", "Admin", "ADMIN" },
                    { "9acd4f76-493d-48ea-ba8d-a8fb26c2968c", null, "Role", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_order_recons_order_id",
                table: "order_recons",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_recons");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "477ccee7-d304-4d4f-aab0-13eb54e2c369");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "9acd4f76-493d-48ea-ba8d-a8fb26c2968c");

            migrationBuilder.DropColumn(
                name: "is_yabo_charge",
                table: "charges");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "391b8bd7-36ef-4e69-aa9a-bd2ec84b9a61", null, "Role", "Admin", "ADMIN" },
                    { "a6fc90f2-84f4-4433-9111-6d15b1eb28cd", null, "Role", "User", "USER" }
                });
        }
    }
}
