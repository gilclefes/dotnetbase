using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class orderassingmentupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "bb8ec0d2-65fa-4c7c-9cff-3c034ab3bbc6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "ed1f3c16-bd14-412e-915c-ddf74b86390b");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "service_providers",
                type: "varchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "assigned_status",
                table: "order_assignments",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_status_changed",
                table: "order_assignments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "clients",
                type: "varchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "d3aa419e-4404-4b6c-a773-9fe315512ec7", null, "Role", "User", "USER" },
                    { "fafa51ab-c7f9-4bb2-a504-e6fff56aa947", null, "Role", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "d3aa419e-4404-4b6c-a773-9fe315512ec7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "fafa51ab-c7f9-4bb2-a504-e6fff56aa947");

            migrationBuilder.DropColumn(
                name: "assigned_status",
                table: "order_assignments");

            migrationBuilder.DropColumn(
                name: "date_status_changed",
                table: "order_assignments");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "service_providers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "last_name",
                table: "clients",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldMaxLength: 450)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "bb8ec0d2-65fa-4c7c-9cff-3c034ab3bbc6", null, "Role", "User", "USER" },
                    { "ed1f3c16-bd14-412e-915c-ddf74b86390b", null, "Role", "Admin", "ADMIN" }
                });
        }
    }
}
