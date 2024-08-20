using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class AuthorizeNetTrackerFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "df16d903-1e2f-4e43-8298-01341af91ef6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "f14678cf-c370-4e73-9854-918b95467d22");

            migrationBuilder.AlterColumn<string>(
                name: "shipping_address_id",
                table: "authorize_net_customer_profiles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "customer_profile_id",
                table: "authorize_net_customer_profiles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "card_payment_profile_id",
                table: "authorize_net_customer_profiles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "bank_account_payment_profile_id",
                table: "authorize_net_customer_profiles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "default_payment_profile_id",
                table: "authorize_net_customer_profiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "af9aecdf-eb7c-4b32-8bd8-345f6989bdf7", null, "Role", "User", "USER" },
                    { "f6b89de8-dd62-4f34-a698-ace4b5ddafbd", null, "Role", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "af9aecdf-eb7c-4b32-8bd8-345f6989bdf7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "f6b89de8-dd62-4f34-a698-ace4b5ddafbd");

            migrationBuilder.DropColumn(
                name: "default_payment_profile_id",
                table: "authorize_net_customer_profiles");

            migrationBuilder.AlterColumn<int>(
                name: "shipping_address_id",
                table: "authorize_net_customer_profiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "customer_profile_id",
                table: "authorize_net_customer_profiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "card_payment_profile_id",
                table: "authorize_net_customer_profiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "bank_account_payment_profile_id",
                table: "authorize_net_customer_profiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "df16d903-1e2f-4e43-8298-01341af91ef6", null, "Role", "User", "USER" },
                    { "f14678cf-c370-4e73-9854-918b95467d22", null, "Role", "Admin", "ADMIN" }
                });
        }
    }
}
