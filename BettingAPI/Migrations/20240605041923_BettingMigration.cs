using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BettingAPI.Migrations
{
    /// <inheritdoc />
    public partial class BettingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bets",
                columns: table => new
                {
                    BetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false),
                    TeamBetId = table.Column<int>(type: "int", nullable: false),
                    AmountBet = table.Column<double>(type: "float", nullable: false),
                    AmountWon = table.Column<double>(type: "float", nullable: false),
                    BetStatus = table.Column<int>(type: "int", nullable: false),
                    TeamWon = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConcludedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bets", x => x.BetId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bets_UserId",
                table: "Bets",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bets");
        }
    }
}
