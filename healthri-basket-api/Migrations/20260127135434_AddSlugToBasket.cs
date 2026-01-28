using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace healthri_basket_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToBasket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Slug column (nullable first to allow data migration)
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Baskets",
                type: "text",
                nullable: true);

            // Generate slugs from existing basket names
            // For duplicate slugs within same userId, add suffix -1, -2, etc.
            migrationBuilder.Sql(@"
                WITH BasketSlugs AS (
                    SELECT 
                        ""Id"",
                        ""UserId"",
                        LOWER(REGEXP_REPLACE(REGEXP_REPLACE(""Name"", '[^a-zA-Z0-9\\s-]', '', 'g'), '[\\s-]+', '-', 'g')) AS base_slug,
                        ROW_NUMBER() OVER (PARTITION BY ""UserId"", LOWER(REGEXP_REPLACE(REGEXP_REPLACE(""Name"", '[^a-zA-Z0-9\\s-]', '', 'g'), '[\\s-]+', '-', 'g')) ORDER BY ""CreatedAt"") - 1 AS dup_count
                    FROM ""Baskets""
                    WHERE ""Slug"" IS NULL
                )
                UPDATE ""Baskets""
                SET ""Slug"" = CASE 
                    WHEN bs.dup_count = 0 THEN bs.base_slug
                    ELSE bs.base_slug || '-' || bs.dup_count
                END
                FROM BasketSlugs bs
                WHERE ""Baskets"".""Id"" = bs.""Id"";
            ");

            // Make Slug non-nullable after data migration
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Baskets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Create unique index
            migrationBuilder.CreateIndex(
                name: "IX_Baskets_UserId_Slug",
                table: "Baskets",
                columns: new[] { "UserId", "Slug" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Baskets_UserId_Slug",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Baskets");
        }
    }
}
