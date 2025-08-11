using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySteamWPF.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHiddenToUserGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGames_Users_UserId1",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_UserId1",
                table: "UserGames");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserGames");

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "UserGames",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "UserGames");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserGames",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_UserId1",
                table: "UserGames",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGames_Users_UserId1",
                table: "UserGames",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
