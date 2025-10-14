using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace healthri_basket_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("1c5fa233-0c7c-4335-9ad8-0085caf32e20"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("ead26b2a-08a6-4eae-a95e-0dd29c0c3a3b"));

            migrationBuilder.DeleteData(
                table: "Items",
                keyColumn: "Id",
                keyValue: new Guid("ee365d89-d86a-48fe-a9d6-a5206f070ba3"));

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Collection of patient monitoring data of premature infants, ECG, CI and parameters as SpO2 and Temp. Half of the infants experienced a period of late onset sepsis during their hospital stay, the other half does not.", "NEOLOS - physiological measurements of preterm infants with and without late onset sepsis" },
                    { new Guid("11111111-1111-1111-1111-222222222222"), "Infectiepreventie van COVID-19 in ziekenhuizen - omgevingsstudie; COntrol of COVID-19 iN Hospitals - environmental study", "COntrol of COVID-19 iN Hospitals - environmental study" },
                    { new Guid("11111111-1111-1111-1111-333333333333"), "ctDNA on the way to implementation in the Netherlands (COIN)", "ctDNA on the way to implementation in the Netherlands (COIN)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Items",
                columns: new[] { "Id", "Description", "Title" },
                values: new object[,]
                {
                    { new Guid("1c5fa233-0c7c-4335-9ad8-0085caf32e20"), "ctDNA on the way to implementation in the Netherlands (COIN)", "ctDNA on the way to implementation in the Netherlands (COIN)" },
                    { new Guid("ead26b2a-08a6-4eae-a95e-0dd29c0c3a3b"), "Collection of patient monitoring data of premature infants, ECG, CI and parameters as SpO2 and Temp. Half of the infants experienced a period of late onset sepsis during their hospital stay, the other half does not.", "NEOLOS - physiological measurements of preterm infants with and without late onset sepsis" },
                    { new Guid("ee365d89-d86a-48fe-a9d6-a5206f070ba3"), "Infectiepreventie van COVID-19 in ziekenhuizen - omgevingsstudie; COntrol of COVID-19 iN Hospitals - environmental study", "COntrol of COVID-19 iN Hospitals - environmental study" }
                });
        }
    }
}
