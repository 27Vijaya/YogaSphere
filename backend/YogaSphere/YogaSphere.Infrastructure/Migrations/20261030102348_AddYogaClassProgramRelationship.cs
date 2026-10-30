using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YogaSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYogaClassProgramRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_YogaClasses_YogaProgramId",
                table: "YogaClasses",
                column: "YogaProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_YogaClasses_YogaPrograms_YogaProgramId",
                table: "YogaClasses",
                column: "YogaProgramId",
                principalTable: "YogaPrograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YogaClasses_YogaPrograms_YogaProgramId",
                table: "YogaClasses");

            migrationBuilder.DropIndex(
                name: "IX_YogaClasses_YogaProgramId",
                table: "YogaClasses");
        }
    }
}
