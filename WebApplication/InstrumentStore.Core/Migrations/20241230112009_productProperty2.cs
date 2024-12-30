using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class productProperty2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductArchive_ProductCategory_ProductTypeProductCategoryId",
                table: "ProductArchive");

            migrationBuilder.RenameColumn(
                name: "ProductTypeProductCategoryId",
                table: "ProductArchive",
                newName: "ProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductArchive_ProductTypeProductCategoryId",
                table: "ProductArchive",
                newName: "IX_ProductArchive_ProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductArchive_ProductCategory_ProductCategoryId",
                table: "ProductArchive",
                column: "ProductCategoryId",
                principalTable: "ProductCategory",
                principalColumn: "ProductCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductArchive_ProductCategory_ProductCategoryId",
                table: "ProductArchive");

            migrationBuilder.RenameColumn(
                name: "ProductCategoryId",
                table: "ProductArchive",
                newName: "ProductTypeProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductArchive_ProductCategoryId",
                table: "ProductArchive",
                newName: "IX_ProductArchive_ProductTypeProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductArchive_ProductCategory_ProductTypeProductCategoryId",
                table: "ProductArchive",
                column: "ProductTypeProductCategoryId",
                principalTable: "ProductCategory",
                principalColumn: "ProductCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
