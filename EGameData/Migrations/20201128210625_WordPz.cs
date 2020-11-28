using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class WordPz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AmtUsedToPlayWordPuzzle",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmtUsedToPlayWordPuzzle",
                table: "Users");
        }
    }
}
