using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dotnetbase.Migrations
{
    /// <inheritdoc />
    public partial class servicetableapprovalupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_charges_charge_categories_charge_category_id",
                table: "charges");

            migrationBuilder.DropForeignKey(
                name: "fk_client_subscriptions_subscription_plans_subscription_plan_id",
                table: "client_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "fk_order_promo_codes_promo_codes_promo_code_id",
                table: "order_promo_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_services_service_categories_service_category_id",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "fk_services_service_types_service_type_id",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_benefits_subscription_plans_subscription_pl",
                table: "subscription_plan_benefits");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_charges_subscription_plans_subscription_pla",
                table: "subscription_plan_charges");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_prices_subscription_plans_subscription_plan",
                table: "subscription_plan_prices");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_services_subscription_plans_subscription_pl",
                table: "subscription_plan_services");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plans_periods_period_id",
                table: "subscription_plans");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "25fcee81-ff15-4ef4-be0e-269ac9eb8f6f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "69696d67-d34f-4059-8e14-8acd37aa81f0");

            migrationBuilder.AddColumn<string>(
                name: "approved_by",
                table: "services",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "approved_date",
                table: "services",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "pending",
                table: "services",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "AspNetRoles",
                type: "varchar(13)",
                maxLength: 13,
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
                    { "15c28a33-2219-4e22-b905-29d376f76605", null, "Role", "User", "USER" },
                    { "d42f295a-814a-4ff2-83d3-ee50a42cf05a", null, "Role", "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_charges_charge_categories_category_id",
                table: "charges",
                column: "category_id",
                principalTable: "charge_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_subscriptions_subscription_plans_subscription_id",
                table: "client_subscriptions",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_promo_codes_promo_codes_promo_id",
                table: "order_promo_codes",
                column: "promo_id",
                principalTable: "promo_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_services_service_categories_category_id",
                table: "services",
                column: "category_id",
                principalTable: "service_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_services_service_types_type_id",
                table: "services",
                column: "type_id",
                principalTable: "service_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_benefits_subscription_plans_subscription_id",
                table: "subscription_plan_benefits",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_charges_subscription_plans_subscription_id",
                table: "subscription_plan_charges",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_prices_subscription_plans_subscription_id",
                table: "subscription_plan_prices",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_services_subscription_plans_subscription_id",
                table: "subscription_plan_services",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plans_periods_order_period_id",
                table: "subscription_plans",
                column: "order_period_id",
                principalTable: "periods",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_charges_charge_categories_category_id",
                table: "charges");

            migrationBuilder.DropForeignKey(
                name: "fk_client_subscriptions_subscription_plans_subscription_id",
                table: "client_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "fk_order_promo_codes_promo_codes_promo_id",
                table: "order_promo_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_services_service_categories_category_id",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "fk_services_service_types_type_id",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_benefits_subscription_plans_subscription_id",
                table: "subscription_plan_benefits");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_charges_subscription_plans_subscription_id",
                table: "subscription_plan_charges");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_prices_subscription_plans_subscription_id",
                table: "subscription_plan_prices");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plan_services_subscription_plans_subscription_id",
                table: "subscription_plan_services");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_plans_periods_order_period_id",
                table: "subscription_plans");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "15c28a33-2219-4e22-b905-29d376f76605");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "id",
                keyValue: "d42f295a-814a-4ff2-83d3-ee50a42cf05a");

            migrationBuilder.DropColumn(
                name: "approved_by",
                table: "services");

            migrationBuilder.DropColumn(
                name: "approved_date",
                table: "services");

            migrationBuilder.DropColumn(
                name: "pending",
                table: "services");

            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "AspNetRoles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(13)",
                oldMaxLength: 13)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "id", "concurrency_stamp", "discriminator", "name", "normalized_name" },
                values: new object[,]
                {
                    { "25fcee81-ff15-4ef4-be0e-269ac9eb8f6f", null, "Role", "Admin", "ADMIN" },
                    { "69696d67-d34f-4059-8e14-8acd37aa81f0", null, "Role", "User", "USER" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_charges_charge_categories_charge_category_id",
                table: "charges",
                column: "category_id",
                principalTable: "charge_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_subscriptions_subscription_plans_subscription_plan_id",
                table: "client_subscriptions",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_order_promo_codes_promo_codes_promo_code_id",
                table: "order_promo_codes",
                column: "promo_id",
                principalTable: "promo_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_services_service_categories_service_category_id",
                table: "services",
                column: "category_id",
                principalTable: "service_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_services_service_types_service_type_id",
                table: "services",
                column: "type_id",
                principalTable: "service_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_benefits_subscription_plans_subscription_pl",
                table: "subscription_plan_benefits",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_charges_subscription_plans_subscription_pla",
                table: "subscription_plan_charges",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_prices_subscription_plans_subscription_plan",
                table: "subscription_plan_prices",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plan_services_subscription_plans_subscription_pl",
                table: "subscription_plan_services",
                column: "subscription_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_plans_periods_period_id",
                table: "subscription_plans",
                column: "order_period_id",
                principalTable: "periods",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
