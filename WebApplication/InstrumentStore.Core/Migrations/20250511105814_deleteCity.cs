using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class deleteCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddress_City_CityId",
                table: "DeliveryAddress");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryAddress_CityId",
                table: "DeliveryAddress");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "DeliveryAddress");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "DeliveryAddress",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "DeliveryAddress");

            migrationBuilder.AddColumn<Guid>(
                name: "CityId",
                table: "DeliveryAddress",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.CityId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddress_CityId",
                table: "DeliveryAddress",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddress_City_CityId",
                table: "DeliveryAddress",
                column: "CityId",
                principalTable: "City",
                principalColumn: "CityId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
