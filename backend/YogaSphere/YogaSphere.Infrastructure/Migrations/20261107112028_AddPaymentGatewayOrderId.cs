using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YogaSphere.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentGatewayOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayOrderId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GatewayOrderId",
                table: "Payments");
        }
    }
}
