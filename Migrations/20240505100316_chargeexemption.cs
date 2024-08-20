using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class chargeexemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "15c28a33-2219-4e22-b905-29d376f76605");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "d42f295a-814a-4ff2-83d3-ee50a42cf05a");

            migrationBuilder.CreateTable(
                name: "subscription_plan_charge_exemptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    charge_id = table.Column<int>(type: "int", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscription_plan_charge_exemptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscription_plan_charge_exemptions_charges_charge_id",
                        column: x => x.charge_id,
                        principalTable: "charges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_subscription_plan_charge_exemptions_subscription_plans_subsc",
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
                    { "391b8bd7-36ef-4e69-aa9a-bd2ec84b9a61", null, "Role", "Admin", "ADMIN" },
                    { "a6fc90f2-84f4-4433-9111-6d15b1eb28cd", null, "Role", "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plan_charge_exemptions_charge_id",
                table: "subscription_plan_charge_exemptions",
                column: "charge_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscription_plan_charge_exemptions_subscription_id",
                table: "subscription_plan_charge_exemptions",
                column: "subscription_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_plan_charge_exemptions");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "391b8bd7-36ef-4e69-aa9a-bd2ec84b9a61");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "a6fc90f2-84f4-4433-9111-6d15b1eb28cd");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "15c28a33-2219-4e22-b905-29d376f76605", null, "Role", "User", "USER" },
                    { "d42f295a-814a-4ff2-83d3-ee50a42cf05a", null, "Role", "Admin", "ADMIN" }
                });
        }
    }
}
