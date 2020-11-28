using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class AgentCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferralCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WordPuzzles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Answers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordPuzzles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordPuzzles_Users_AddedById",
                        column: x => x.AddedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordPuzzles_AddedById",
                table: "WordPuzzles",
                column: "AddedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordPuzzles");

            migrationBuilder.DropColumn(
                name: "AgentCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReferralCode",
                table: "Users");
        }
    }
}
