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
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Baskets",
                type: "text",
                nullable: false);

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
