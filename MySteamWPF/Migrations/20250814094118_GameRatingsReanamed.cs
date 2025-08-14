using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySteamWPF.Migrations
{
    /// <inheritdoc />
    public partial class GameRatingsReanamed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRatings_Games_GameId",
                table: "GameRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_GameRatings_Users_UserId",
                table: "GameRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameRatings",
                table: "GameRatings");

            migrationBuilder.RenameTable(
                name: "GameRatings",
                newName: "GameRating");

            migrationBuilder.RenameIndex(
                name: "IX_GameRatings_UserId",
                table: "GameRating",
                newName: "IX_GameRating_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameRating",
                table: "GameRating",
                columns: new[] { "GameId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameRating_Games_GameId",
                table: "GameRating",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameRating_Users_UserId",
                table: "GameRating",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRating_Games_GameId",
                table: "GameRating");

            migrationBuilder.DropForeignKey(
                name: "FK_GameRating_Users_UserId",
                table: "GameRating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameRating",
                table: "GameRating");

            migrationBuilder.RenameTable(
                name: "GameRating",
                newName: "GameRatings");

            migrationBuilder.RenameIndex(
                name: "IX_GameRating_UserId",
                table: "GameRatings",
                newName: "IX_GameRatings_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameRatings",
                table: "GameRatings",
                columns: new[] { "GameId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GameRatings_Games_GameId",
                table: "GameRatings",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameRatings_Users_UserId",
                table: "GameRatings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
