using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class orderrefunndsupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_refunds_clients_client_id",
                table: "order_refunds");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "1af92d29-1a1d-40eb-87fc-15ea047c5fe1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "733d7815-08cf-449f-95c7-97af9da24bd8");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "19be55bf-5cdf-42a1-b6b9-0ded99b11d56", null, "Role", "Admin", "ADMIN" },
                    { "1e65c9a7-c2c3-4565-9df9-7b1f103a8ccb", null, "Role", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_order_refunds_client_id",
                table: "order_refunds",
                column: "client_id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_refunds_clients_client_id",
                table: "order_refunds",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_refunds_clients_client_id",
                table: "order_refunds");

            migrationBuilder.DropIndex(
                name: "ix_order_refunds_client_id",
                table: "order_refunds");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "19be55bf-5cdf-42a1-b6b9-0ded99b11d56");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "1e65c9a7-c2c3-4565-9df9-7b1f103a8ccb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "1af92d29-1a1d-40eb-87fc-15ea047c5fe1", null, "Role", "User", "USER" },
                    { "733d7815-08cf-449f-95c7-97af9da24bd8", null, "Role", "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_order_refunds_clients_client_id",
                table: "order_refunds",
                column: "order_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
