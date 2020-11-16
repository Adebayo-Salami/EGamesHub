using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class fowl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsWithdrawing",
                table: "Users",
                type: "bit",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "IsWithdrawing",
                table: "Users",
                type: "float",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
