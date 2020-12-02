using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class DbUpdateDBs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AmountToBeWon",
                table: "Challenges",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "GameHostPoints",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GameSummary",
                table: "Challenges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGameHostDone",
                table: "Challenges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUserChallengedDone",
                table: "Challenges",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeGameHostEnded",
                table: "Challenges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeUserChallengeEnded",
                table: "Challenges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserChallengedPoints",
                table: "Challenges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountToBeWon",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "GameHostPoints",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "GameSummary",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "IsGameHostDone",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "IsUserChallengedDone",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "TimeGameHostEnded",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "TimeUserChallengeEnded",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "UserChallengedPoints",
                table: "Challenges");
        }
    }
}
