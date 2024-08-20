using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class AuthorizeNetTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "8b64e928-3d61-470c-84fc-288717f40eab");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "a09fd984-cb38-4c84-ba48-95d36ce8c660");

            migrationBuilder.CreateTable(
                name: "authorize_net_customer_profiles",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    card_payment_profile_id = table.Column<int>(type: "int", nullable: true),
                    bank_account_payment_profile_id = table.Column<int>(type: "int", nullable: true),
                    shipping_address_id = table.Column<int>(type: "int", nullable: true),
                    customer_profile_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_authorize_net_customer_profiles", x => x.client_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "df16d903-1e2f-4e43-8298-01341af91ef6", null, "Role", "User", "USER" },
                    { "f14678cf-c370-4e73-9854-918b95467d22", null, "Role", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_authorize_net_customer_profiles_email",
                table: "authorize_net_customer_profiles",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authorize_net_customer_profiles");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "df16d903-1e2f-4e43-8298-01341af91ef6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "f14678cf-c370-4e73-9854-918b95467d22");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "8b64e928-3d61-470c-84fc-288717f40eab", null, "Role", "Admin", "ADMIN" },
                    { "a09fd984-cb38-4c84-ba48-95d36ce8c660", null, "Role", "User", "USER" }
                });
        }
    }
}
