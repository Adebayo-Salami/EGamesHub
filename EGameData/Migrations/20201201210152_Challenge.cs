using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class Challenge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameType = table.Column<int>(type: "int", nullable: false),
                    BrainGameCategory = table.Column<int>(type: "int", nullable: false),
                    GameHostId = table.Column<int>(type: "int", nullable: true),
                    UserChallengedId = table.Column<int>(type: "int", nullable: true),
                    AmountToStaked = table.Column<double>(type: "float", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WinningUserId = table.Column<int>(type: "int", nullable: true),
                    ChallengeStatus = table.Column<int>(type: "int", nullable: false),
                    ChallengeLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsChallengeStarted = table.Column<bool>(type: "bit", nullable: false),
                    IsChallengeEnded = table.Column<bool>(type: "bit", nullable: false),
                    IsUserJoined = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Challenges_Users_GameHostId",
                        column: x => x.GameHostId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_Users_UserChallengedId",
                        column: x => x.UserChallengedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Challenges_Users_WinningUserId",
                        column: x => x.WinningUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_GameHostId",
                table: "Challenges",
                column: "GameHostId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_UserChallengedId",
                table: "Challenges",
                column: "UserChallengedId");

            migrationBuilder.CreateIndex(
                name: "IX_Challenges_WinningUserId",
                table: "Challenges",
                column: "WinningUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Challenges");
        }
    }
}
