using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPantry.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecipeFoodProductTableAndRenameIngredientsJsonToIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeFoodProduct");

            migrationBuilder.RenameColumn(
                name: "IngredientsJson",
                table: "Recipe",
                newName: "Ingredients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ingredients",
                table: "Recipe",
                newName: "IngredientsJson");

            migrationBuilder.CreateTable(
                name: "RecipeFoodProduct",
                columns: table => new
                {
                    RecipeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodProductEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeFoodProduct", x => new { x.RecipeID, x.FoodProductID });
                    table.ForeignKey(
                        name: "FK_RecipeFoodProduct_FoodProduct_FoodProductEntityId",
                        column: x => x.FoodProductEntityId,
                        principalTable: "FoodProduct",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecipeFoodProduct_FoodProduct_FoodProductID",
                        column: x => x.FoodProductID,
                        principalTable: "FoodProduct",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecipeFoodProduct_Recipe_RecipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeFoodProduct_FoodProductEntityId",
                table: "RecipeFoodProduct",
                column: "FoodProductEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeFoodProduct_FoodProductID",
                table: "RecipeFoodProduct",
                column: "FoodProductID");
        }
    }
}
