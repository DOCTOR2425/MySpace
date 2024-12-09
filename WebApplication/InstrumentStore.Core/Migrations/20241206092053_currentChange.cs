using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class currentChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Brand");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Brand");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Brand",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
