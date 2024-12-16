using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class tbl_orderToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_tbl_Order_tbl_OrderId",
                table: "OrderItem");

            migrationBuilder.DropTable(
                name: "tbl_Order");

            migrationBuilder.RenameColumn(
                name: "tbl_OrderId",
                table: "OrderItem",
                newName: "tbl_OrderOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_tbl_OrderId",
                table: "OrderItem",
                newName: "IX_OrderItem_tbl_OrderOrderId");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_DeliveryMethod_DeliveryMethodId",
                        column: x => x.DeliveryMethodId,
                        principalTable: "DeliveryMethod",
                        principalColumn: "DeliveryMethodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethod",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryMethodId",
                table: "Order",
                column: "DeliveryMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PaymentMethodId",
                table: "Order",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_tbl_OrderOrderId",
                table: "OrderItem",
                column: "tbl_OrderOrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_tbl_OrderOrderId",
                table: "OrderItem");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.RenameColumn(
                name: "tbl_OrderOrderId",
                table: "OrderItem",
                newName: "tbl_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_tbl_OrderOrderId",
                table: "OrderItem",
                newName: "IX_OrderItem_tbl_OrderId");

            migrationBuilder.CreateTable(
                name: "tbl_Order",
                columns: table => new
                {
                    tbl_OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Order", x => x.tbl_OrderId);
                    table.ForeignKey(
                        name: "FK_tbl_Order_DeliveryMethod_DeliveryMethodId",
                        column: x => x.DeliveryMethodId,
                        principalTable: "DeliveryMethod",
                        principalColumn: "DeliveryMethodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_Order_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethod",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_Order_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Order_DeliveryMethodId",
                table: "tbl_Order",
                column: "DeliveryMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Order_PaymentMethodId",
                table: "tbl_Order",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Order_UserId",
                table: "tbl_Order",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_tbl_Order_tbl_OrderId",
                table: "OrderItem",
                column: "tbl_OrderId",
                principalTable: "tbl_Order",
                principalColumn: "tbl_OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
