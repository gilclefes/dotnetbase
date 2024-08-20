using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class taxupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "3eafc146-f4af-4f23-9aa2-54dc40f75097");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "ef7a087b-4046-4a91-a04a-ef5a88ec65c6");

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "orders",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "client_subscription_details",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "amount_type",
                table: "charges",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "city_taxes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tax_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tax_percentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    city_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_city_taxes", x => x.id);
                    table.ForeignKey(
                        name: "fk_city_taxes_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "179c1e8e-1c64-4086-8562-ac465709d01b", null, "Role", "Admin", "ADMIN" },
                    { "afea8e64-2a1b-41df-891b-c96f5c741efb", null, "Role", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_city_taxes_city_id",
                table: "city_taxes",
                column: "city_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "city_taxes");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "179c1e8e-1c64-4086-8562-ac465709d01b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "afea8e64-2a1b-41df-891b-c96f5c741efb");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "client_subscription_details");

            migrationBuilder.DropColumn(
                name: "amount_type",
                table: "charges");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "3eafc146-f4af-4f23-9aa2-54dc40f75097", null, "Role", "Admin", "ADMIN" },
                    { "ef7a087b-4046-4a91-a04a-ef5a88ec65c6", null, "Role", "User", "USER" }
                });
        }
    }
}
