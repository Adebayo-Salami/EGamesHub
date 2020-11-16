using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class updatess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "GameHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameHistories_UserId",
                table: "GameHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameHistories_Users_UserId",
                table: "GameHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameHistories_Users_UserId",
                table: "GameHistories");

            migrationBuilder.DropIndex(
                name: "IX_GameHistories_UserId",
                table: "GameHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GameHistories");
        }
    }
}
