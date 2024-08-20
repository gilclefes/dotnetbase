using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class servicetableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "19be55bf-5cdf-42a1-b6b9-0ded99b11d56");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "1e65c9a7-c2c3-4565-9df9-7b1f103a8ccb");

            migrationBuilder.AddColumn<string>(
                name: "how_to",
                table: "services",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "25fcee81-ff15-4ef4-be0e-269ac9eb8f6f", null, "Role", "Admin", "ADMIN" },
                    { "69696d67-d34f-4059-8e14-8acd37aa81f0", null, "Role", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "25fcee81-ff15-4ef4-be0e-269ac9eb8f6f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "69696d67-d34f-4059-8e14-8acd37aa81f0");

            migrationBuilder.DropColumn(
                name: "how_to",
                table: "services");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "19be55bf-5cdf-42a1-b6b9-0ded99b11d56", null, "Role", "Admin", "ADMIN" },
                    { "1e65c9a7-c2c3-4565-9df9-7b1f103a8ccb", null, "Role", "User", "USER" }
                });
        }
    }
}
