using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class COrrectionOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderItem",
                newName: "tbl_OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_ProductId",
                table: "OrderItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_tbl_OrderId",
                table: "OrderItem",
                column: "tbl_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Product_ProductId",
                table: "OrderItem",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_tbl_Order_tbl_OrderId",
                table: "OrderItem",
                column: "tbl_OrderId",
                principalTable: "tbl_Order",
                principalColumn: "tbl_OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Product_ProductId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_tbl_Order_tbl_OrderId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_ProductId",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_tbl_OrderId",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "tbl_OrderId",
                table: "OrderItem",
                newName: "OrderId");
        }
    }
}
