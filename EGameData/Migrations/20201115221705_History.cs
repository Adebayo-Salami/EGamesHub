using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EGamesData.Migrations
{
    public partial class History : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundedById = table.Column<int>(type: "int", nullable: true),
                    UserFundedId = table.Column<int>(type: "int", nullable: true),
                    AmountFunded = table.Column<double>(type: "float", nullable: false),
                    DateFunded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionHistories_Users_FundedById",
                        column: x => x.FundedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionHistories_Users_UserFundedId",
                        column: x => x.UserFundedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_FundedById",
                table: "TransactionHistories",
                column: "FundedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_UserFundedId",
                table: "TransactionHistories",
                column: "UserFundedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionHistories");
        }
    }
}
