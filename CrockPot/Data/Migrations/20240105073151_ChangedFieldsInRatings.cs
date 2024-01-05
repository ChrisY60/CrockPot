using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrockPot.Data.Migrations
{
    public partial class ChangedFieldsInRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "recipeId",
                table: "Rating",
                newName: "RecipeId");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "Rating",
                newName: "RatingValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipeId",
                table: "Rating",
                newName: "recipeId");

            migrationBuilder.RenameColumn(
                name: "RatingValue",
                table: "Rating",
                newName: "rating");
        }
    }
}
