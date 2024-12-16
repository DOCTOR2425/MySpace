using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NewCartStruct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryMethod_DeliveryMethodId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_UserId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_PaidOrderItem_Order_PaidOrderId",
                table: "PaidOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "PaidOrder");

            migrationBuilder.RenameIndex(
                name: "IX_Order_UserId",
                table: "PaidOrder",
                newName: "IX_PaidOrder_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PaymentMethodId",
                table: "PaidOrder",
                newName: "IX_PaidOrder_PaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DeliveryMethodId",
                table: "PaidOrder",
                newName: "IX_PaidOrder_DeliveryMethodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaidOrder",
                table: "PaidOrder",
                column: "PaidOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaidOrder_DeliveryMethod_DeliveryMethodId",
                table: "PaidOrder",
                column: "DeliveryMethodId",
                principalTable: "DeliveryMethod",
                principalColumn: "DeliveryMethodId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaidOrder_PaymentMethod_PaymentMethodId",
                table: "PaidOrder",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaidOrder_User_UserId",
                table: "PaidOrder",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaidOrderItem_PaidOrder_PaidOrderId",
                table: "PaidOrderItem",
                column: "PaidOrderId",
                principalTable: "PaidOrder",
                principalColumn: "PaidOrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaidOrder_DeliveryMethod_DeliveryMethodId",
                table: "PaidOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_PaidOrder_PaymentMethod_PaymentMethodId",
                table: "PaidOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_PaidOrder_User_UserId",
                table: "PaidOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_PaidOrderItem_PaidOrder_PaidOrderId",
                table: "PaidOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PaidOrder",
                table: "PaidOrder");

            migrationBuilder.RenameTable(
                name: "PaidOrder",
                newName: "Order");

            migrationBuilder.RenameIndex(
                name: "IX_PaidOrder_UserId",
                table: "Order",
                newName: "IX_Order_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PaidOrder_PaymentMethodId",
                table: "Order",
                newName: "IX_Order_PaymentMethodId");

            migrationBuilder.RenameIndex(
                name: "IX_PaidOrder_DeliveryMethodId",
                table: "Order",
                newName: "IX_Order_DeliveryMethodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "PaidOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryMethod_DeliveryMethodId",
                table: "Order",
                column: "DeliveryMethodId",
                principalTable: "DeliveryMethod",
                principalColumn: "DeliveryMethodId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PaymentMethod_PaymentMethodId",
                table: "Order",
                column: "PaymentMethodId",
                principalTable: "PaymentMethod",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_UserId",
                table: "Order",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PaidOrderItem_Order_PaidOrderId",
                table: "PaidOrderItem",
                column: "PaidOrderId",
                principalTable: "Order",
                principalColumn: "PaidOrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
