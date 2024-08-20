using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class providerrating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "9eae4cd6-68e6-4cc4-a455-c92a80f0551e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "dfada0e6-7d7e-4d81-8550-55422431fe90");

            migrationBuilder.AddColumn<decimal>(
                name: "rating",
                table: "service_providers",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "rating",
                table: "partners",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "3eafc146-f4af-4f23-9aa2-54dc40f75097", null, "Role", "Admin", "ADMIN" },
                    { "ef7a087b-4046-4a91-a04a-ef5a88ec65c6", null, "Role", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "3eafc146-f4af-4f23-9aa2-54dc40f75097");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "ef7a087b-4046-4a91-a04a-ef5a88ec65c6");

            migrationBuilder.DropColumn(
                name: "rating",
                table: "service_providers");

            migrationBuilder.DropColumn(
                name: "rating",
                table: "partners");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "9eae4cd6-68e6-4cc4-a455-c92a80f0551e", null, "Role", "Admin", "ADMIN" },
                    { "dfada0e6-7d7e-4d81-8550-55422431fe90", null, "Role", "User", "USER" }
                });
        }
    }
}
