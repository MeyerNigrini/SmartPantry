using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPantry.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMealSuggestionWithRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealSuggestionFoodProduct");

            migrationBuilder.DropTable(
                name: "MealSuggestion");

            migrationBuilder.CreateTable(
                name: "Recipe",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IngredientsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipe_User_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recipe_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecipeFoodProduct",
                columns: table => new
                {
                    RecipeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: true),
                    FoodProductEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                name: "IX_Recipe_UserEntityId",
                table: "Recipe",
                column: "UserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_UserID",
                table: "Recipe",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeFoodProduct_FoodProductEntityId",
                table: "RecipeFoodProduct",
                column: "FoodProductEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeFoodProduct_FoodProductID",
                table: "RecipeFoodProduct",
                column: "FoodProductID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeFoodProduct");

            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.CreateTable(
                name: "MealSuggestion",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SuggestionText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealSuggestion", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MealSuggestion_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MealSuggestionFoodProduct",
                columns: table => new
                {
                    MealSuggestionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FoodProductID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealSuggestionFoodProduct", x => new { x.MealSuggestionID, x.FoodProductID });
                    table.ForeignKey(
                        name: "FK_MealSuggestionFoodProduct_FoodProduct_FoodProductID",
                        column: x => x.FoodProductID,
                        principalTable: "FoodProduct",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MealSuggestionFoodProduct_MealSuggestion_MealSuggestionID",
                        column: x => x.MealSuggestionID,
                        principalTable: "MealSuggestion",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealSuggestion_UserID",
                table: "MealSuggestion",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MealSuggestionFoodProduct_FoodProductID",
                table: "MealSuggestionFoodProduct",
                column: "FoodProductID");
        }
    }
}
