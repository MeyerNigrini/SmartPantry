using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPantry.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBarcodeAndChangeCategoriesToCategoryInFoodProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "FoodProduct");

            migrationBuilder.RenameColumn(
                name: "Categories",
                table: "FoodProduct",
                newName: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "FoodProduct",
                newName: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "FoodProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
