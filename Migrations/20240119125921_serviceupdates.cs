using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class serviceupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "3c0cf02b-f7f2-4f07-8fa1-67964208c87a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "6144d022-a72f-4142-80f6-b209efcd73fc");

            migrationBuilder.AddColumn<decimal>(
                name: "min_order_value",
                table: "services",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "promo_codes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "bb8ec0d2-65fa-4c7c-9cff-3c034ab3bbc6", null, "Role", "User", "USER" },
                    { "ed1f3c16-bd14-412e-915c-ddf74b86390b", null, "Role", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "bb8ec0d2-65fa-4c7c-9cff-3c034ab3bbc6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "ed1f3c16-bd14-412e-915c-ddf74b86390b");

            migrationBuilder.DropColumn(
                name: "min_order_value",
                table: "services");

            migrationBuilder.DropColumn(
                name: "description",
                table: "promo_codes");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "3c0cf02b-f7f2-4f07-8fa1-67964208c87a", null, "Role", "User", "USER" },
                    { "6144d022-a72f-4142-80f6-b209efcd73fc", null, "Role", "Admin", "ADMIN" }
                });
        }
    }
}
