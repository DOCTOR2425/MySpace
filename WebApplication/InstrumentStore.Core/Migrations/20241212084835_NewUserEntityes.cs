using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NewUserEntityes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMail",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "User");

            migrationBuilder.AddColumn<Guid>(
                name: "UserRegistrInfoId",
                table: "User",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserRegistrInfos",
                columns: table => new
                {
                    UserRegistrInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRegistrInfos", x => x.UserRegistrInfoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_UserRegistrInfoId",
                table: "User",
                column: "UserRegistrInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserRegistrInfos_UserRegistrInfoId",
                table: "User",
                column: "UserRegistrInfoId",
                principalTable: "UserRegistrInfos",
                principalColumn: "UserRegistrInfoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_UserRegistrInfos_UserRegistrInfoId",
                table: "User");

            migrationBuilder.DropTable(
                name: "UserRegistrInfos");

            migrationBuilder.DropIndex(
                name: "IX_User_UserRegistrInfoId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserRegistrInfoId",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "EMail",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
