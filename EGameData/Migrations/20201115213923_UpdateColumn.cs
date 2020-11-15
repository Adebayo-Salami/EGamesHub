using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class UpdateColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Bingos");

            migrationBuilder.DropColumn(
                name: "PendingWithdrawalAmount",
                table: "Bingos");

            migrationBuilder.RenameColumn(
                name: "TotalAmountWithdrawn",
                table: "Bingos",
                newName: "TotalAmountWon");

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PendingWithdrawalAmount",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmountWon",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WithdrawableAmount",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PendingWithdrawalAmount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalAmountWon",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WithdrawableAmount",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TotalAmountWon",
                table: "Bingos",
                newName: "TotalAmountWithdrawn");

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "Bingos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PendingWithdrawalAmount",
                table: "Bingos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
