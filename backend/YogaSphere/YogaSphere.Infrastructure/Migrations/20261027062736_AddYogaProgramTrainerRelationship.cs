using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YogaSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYogaProgramTrainerRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_YogaPrograms_TrainerId",
                table: "YogaPrograms",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_YogaPrograms_TrainerProfiles_TrainerId",
                table: "YogaPrograms",
                column: "TrainerId",
                principalTable: "TrainerProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YogaPrograms_TrainerProfiles_TrainerId",
                table: "YogaPrograms");

            migrationBuilder.DropIndex(
                name: "IX_YogaPrograms_TrainerId",
                table: "YogaPrograms");
        }
    }
}
