using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class codegenerationupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "da50f7e8-325c-4a9a-879e-eb0d84c8f84b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "db202950-0248-48fa-8cde-d002ef633b94");

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "service_providers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "partners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "code",
                table: "clients",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "code_generators",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    entity_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    entity_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_code_generators", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "3c0cf02b-f7f2-4f07-8fa1-67964208c87a", null, "Role", "User", "USER" },
                    { "6144d022-a72f-4142-80f6-b209efcd73fc", null, "Role", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "code_generators");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "3c0cf02b-f7f2-4f07-8fa1-67964208c87a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "6144d022-a72f-4142-80f6-b209efcd73fc");

            migrationBuilder.DropColumn(
                name: "code",
                table: "service_providers");

            migrationBuilder.DropColumn(
                name: "code",
                table: "partners");

            migrationBuilder.DropColumn(
                name: "code",
                table: "clients");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "da50f7e8-325c-4a9a-879e-eb0d84c8f84b", null, "Role", "User", "USER" },
                    { "db202950-0248-48fa-8cde-d002ef633b94", null, "Role", "Admin", "ADMIN" }
                });
        }
    }
}
