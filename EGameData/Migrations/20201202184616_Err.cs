using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class Err : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GameHostSaysGameIsOngoing",
                table: "Challenges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UserChallengedSaysGameIsOngoing",
                table: "Challenges",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameHostSaysGameIsOngoing",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "UserChallengedSaysGameIsOngoing",
                table: "Challenges");
        }
    }
}
