using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPantry.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFieldsOnFoodProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "FoodProduct");

            migrationBuilder.DropColumn(
                name: "Ingredients",
                table: "FoodProduct");

            migrationBuilder.AddColumn<string>(
                name: "Brands",
                table: "FoodProduct",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Categories",
                table: "FoodProduct",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brands",
                table: "FoodProduct");

            migrationBuilder.DropColumn(
                name: "Categories",
                table: "FoodProduct");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "FoodProduct",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ingredients",
                table: "FoodProduct",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
