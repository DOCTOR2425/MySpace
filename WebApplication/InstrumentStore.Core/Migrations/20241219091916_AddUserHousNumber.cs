using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddUserHousNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HouseNumber",
                table: "UserAdresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HouseNumber",
                table: "UserAdresses");
        }
    }
}
