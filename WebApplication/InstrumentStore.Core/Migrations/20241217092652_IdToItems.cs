using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class IdToItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaidOrderItemId",
                table: "PaidOrderItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CartItemId",
                table: "CartItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PaidOrderItem",
                table: "PaidOrderItem",
                column: "PaidOrderItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem",
                column: "CartItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PaidOrderItem",
                table: "PaidOrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "PaidOrderItemId",
                table: "PaidOrderItem");

            migrationBuilder.DropColumn(
                name: "CartItemId",
                table: "CartItem");
        }
    }
}
