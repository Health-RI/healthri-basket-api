using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace healthri_basket_api.Migrations
{
    /// <inheritdoc />
    public partial class MakeItemIdStringAndDropItemMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Items_ItemId",
                table: "BasketItems");

            migrationBuilder.DropIndex(
                name: "IX_BasketItems_ItemId",
                table: "BasketItems");

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-222222222222"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-333333333333"));

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "TransactionLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Items",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ItemId",
                table: "BasketItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "TransactionLogs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Items",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "BasketItems",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Collection of patient monitoring data of premature infants, ECG, CI and parameters as SpO2 and Temp. Half of the infants experienced a period of late onset sepsis during their hospital stay, the other half does not.", "NEOLOS - physiological measurements of preterm infants with and without late onset sepsis" },
                    { new Guid("11111111-1111-1111-1111-222222222222"), "Infectiepreventie van COVID-19 in ziekenhuizen - omgevingsstudie; COntrol of COVID-19 iN Hospitals - environmental study", "COntrol of COVID-19 iN Hospitals - environmental study" },
                    { new Guid("11111111-1111-1111-1111-333333333333"), "ctDNA on the way to implementation in the Netherlands (COIN)", "ctDNA on the way to implementation in the Netherlands (COIN)" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_ItemId",
                table: "BasketItems",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Items_ItemId",
                table: "BasketItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
