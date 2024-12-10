using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceChatbot.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId1",
                table: "Payments",
                column: "OrderId1",
                unique: true,
                filter: "[OrderId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId1",
                table: "Payments",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "OrderID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId1",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId1",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "Payments");
        }
    }
}
