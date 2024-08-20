using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class modelupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "af9aecdf-eb7c-4b32-8bd8-345f6989bdf7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "f6b89de8-dd62-4f34-a698-ace4b5ddafbd");

            migrationBuilder.AddColumn<string>(
                name: "contact_first_name",
                table: "partners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "contact_last_name",
                table: "partners",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "current_subscription_detail_id",
                table: "client_subscriptions",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "15a97257-dde1-448e-9b79-3c4614e90c52", null, "Role", "User", "USER" },
                    { "454dd9f3-adf9-4215-bf51-3df49b6155a4", null, "Role", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "15a97257-dde1-448e-9b79-3c4614e90c52");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "454dd9f3-adf9-4215-bf51-3df49b6155a4");

            migrationBuilder.DropColumn(
                name: "contact_first_name",
                table: "partners");

            migrationBuilder.DropColumn(
                name: "contact_last_name",
                table: "partners");

            migrationBuilder.DropColumn(
                name: "current_subscription_detail_id",
                table: "client_subscriptions");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "af9aecdf-eb7c-4b32-8bd8-345f6989bdf7", null, "Role", "User", "USER" },
                    { "f6b89de8-dd62-4f34-a698-ace4b5ddafbd", null, "Role", "Admin", "ADMIN" }
                });
        }
    }
}
