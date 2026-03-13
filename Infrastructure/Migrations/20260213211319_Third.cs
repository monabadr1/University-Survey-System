using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_responses_users_UserId",
                table: "responses");

            migrationBuilder.DropColumn(
                name: "First_name",
                table: "responses");

            migrationBuilder.AddForeignKey(
                name: "FK_responses_users_UserId",
                table: "responses",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_responses_users_UserId",
                table: "responses");

            migrationBuilder.AddColumn<string>(
                name: "First_name",
                table: "responses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_responses_users_UserId",
                table: "responses",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserId");
        }
    }
}
