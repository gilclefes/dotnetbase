using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class subscriptiontableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "0ff8d256-77b7-4326-9d7e-c0e1eccbd688");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "775577ca-fab5-434e-9e11-3a3294ff7fa1");

            migrationBuilder.AddColumn<bool>(
                name: "is_favorite",
                table: "subscription_plan_prices",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "subscription_plan_benefits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    subscription_id = table.Column<int>(type: "int", nullable: false),
                    benefit = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rank = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_plan_benefits", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscription_plan_benefits_subscription_plans_subscription_pl",
                        column: x => x.subscription_id,
                        principalTable: "subscription_plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "da50f7e8-325c-4a9a-879e-eb0d84c8f84b", null, "Role", "User", "USER" },
                    { "db202950-0248-48fa-8cde-d002ef633b94", null, "Role", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plan_benefits_benefit",
                table: "subscription_plan_benefits",
                column: "benefit");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plan_benefits_subscription_id",
                table: "subscription_plan_benefits",
                column: "subscription_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_plan_benefits");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "da50f7e8-325c-4a9a-879e-eb0d84c8f84b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "db202950-0248-48fa-8cde-d002ef633b94");

            migrationBuilder.DropColumn(
                name: "is_favorite",
                table: "subscription_plan_prices");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "0ff8d256-77b7-4326-9d7e-c0e1eccbd688", null, "Role", "Admin", "ADMIN" },
                    { "775577ca-fab5-434e-9e11-3a3294ff7fa1", null, "Role", "User", "USER" }
                });
        }
    }
}
