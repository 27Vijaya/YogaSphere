using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YogaSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerProfileUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrainerProfiles_UserId",
                table: "TrainerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProfiles_UserId",
                table: "CustomerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProfiles_Users_UserId",
                table: "CustomerProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerProfiles_Users_UserId",
                table: "TrainerProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProfiles_Users_UserId",
                table: "CustomerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerProfiles_Users_UserId",
                table: "TrainerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_TrainerProfiles_UserId",
                table: "TrainerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CustomerProfiles_UserId",
                table: "CustomerProfiles");
        }
    }
}
