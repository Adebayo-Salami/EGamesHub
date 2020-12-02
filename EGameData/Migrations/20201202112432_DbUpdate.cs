using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class DbUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AmtUsedToPlayChallenge",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "TransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "BrainGameCategory",
                table: "Challenges",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BingoGameId",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrainGameQuestion1Id",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrainGameQuestion2Id",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrainGameQuestion3Id",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrainGameQuestion4Id",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrainGameQuestion5Id",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WordPuzzleId",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_BingoGameId",
                table: "Challenges",
                column: "BingoGameId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_BrainGameQuestion1Id",
                table: "Challenges",
                column: "BrainGameQuestion1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_BrainGameQuestion2Id",
                table: "Challenges",
                column: "BrainGameQuestion2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_BrainGameQuestion3Id",
                table: "Challenges",
                column: "BrainGameQuestion3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_BrainGameQuestion4Id",
                table: "Challenges",
                column: "BrainGameQuestion4Id");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_BrainGameQuestion5Id",
                table: "Challenges",
                column: "BrainGameQuestion5Id");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_WordPuzzleId",
                table: "Challenges",
                column: "WordPuzzleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Bingos_BingoGameId",
                table: "Challenges",
                column: "BingoGameId",
                principalTable: "Bingos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion1Id",
                table: "Challenges",
                column: "BrainGameQuestion1Id",
                principalTable: "BrainGameQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion2Id",
                table: "Challenges",
                column: "BrainGameQuestion2Id",
                principalTable: "BrainGameQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion3Id",
                table: "Challenges",
                column: "BrainGameQuestion3Id",
                principalTable: "BrainGameQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion4Id",
                table: "Challenges",
                column: "BrainGameQuestion4Id",
                principalTable: "BrainGameQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion5Id",
                table: "Challenges",
                column: "BrainGameQuestion5Id",
                principalTable: "BrainGameQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_WordPuzzles_WordPuzzleId",
                table: "Challenges",
                column: "WordPuzzleId",
                principalTable: "WordPuzzles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Bingos_BingoGameId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion1Id",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion2Id",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion3Id",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion4Id",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_BrainGameQuestions_BrainGameQuestion5Id",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_WordPuzzles_WordPuzzleId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_BingoGameId",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_BrainGameQuestion1Id",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_BrainGameQuestion2Id",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_BrainGameQuestion3Id",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_BrainGameQuestion4Id",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_BrainGameQuestion5Id",
                table: "Challenges");

            migrationBuilder.DropIndex(
                name: "IX_Challenges_WordPuzzleId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "AmtUsedToPlayChallenge",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "BingoGameId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "BrainGameQuestion1Id",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "BrainGameQuestion2Id",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "BrainGameQuestion3Id",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "BrainGameQuestion4Id",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "BrainGameQuestion5Id",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "WordPuzzleId",
                table: "Challenges");

            migrationBuilder.AlterColumn<int>(
                name: "BrainGameCategory",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
