using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KitapOkumaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitiFixUserBookModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBooks_ApplicationUsers_UserId1",
                table: "UserBooks");

            migrationBuilder.DropIndex(
                name: "IX_UserBooks_UserId1",
                table: "UserBooks");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserBooks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserBooks_UserId1",
                table: "UserBooks",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBooks_ApplicationUsers_UserId1",
                table: "UserBooks",
                column: "UserId1",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
