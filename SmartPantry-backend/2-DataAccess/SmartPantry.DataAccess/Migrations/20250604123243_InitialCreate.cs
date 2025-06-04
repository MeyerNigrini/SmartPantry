using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPantry.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(255)", nullable: false),
                    Ingredients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodProduct_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MealSuggestion",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuggestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_FoodProduct_UserID",
                table: "FoodProduct",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MealSuggestion_UserID",
                table: "MealSuggestion",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_MealSuggestionFoodProduct_FoodProductID",
                table: "MealSuggestionFoodProduct",
                column: "FoodProductID");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealSuggestionFoodProduct");

            migrationBuilder.DropTable(
                name: "FoodProduct");

            migrationBuilder.DropTable(
                name: "MealSuggestion");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
