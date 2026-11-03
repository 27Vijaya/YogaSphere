using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YogaSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleYogaClassRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Schedules_YogaClassId",
                table: "Schedules",
                column: "YogaClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_YogaClasses_YogaClassId",
                table: "Schedules",
                column: "YogaClassId",
                principalTable: "YogaClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_YogaClasses_YogaClassId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_YogaClassId",
                table: "Schedules");
        }
    }
}
