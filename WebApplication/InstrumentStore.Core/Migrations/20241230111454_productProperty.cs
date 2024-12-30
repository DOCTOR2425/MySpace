using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class productProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductType_ProductTypeId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductArchive_ProductType_ProductTypeId",
                table: "ProductArchive");

            migrationBuilder.DropTable(
                name: "ProductType");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "ProductArchive",
                newName: "ProductTypeProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductArchive_ProductTypeId",
                table: "ProductArchive",
                newName: "IX_ProductArchive_ProductTypeProductCategoryId");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "Product",
                newName: "ProductCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ProductTypeId",
                table: "Product",
                newName: "IX_Product_ProductCategoryId");

            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.ProductCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ProductProperty",
                columns: table => new
                {
                    ProductPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProperty", x => x.ProductPropertyId);
                    table.ForeignKey(
                        name: "FK_ProductProperty_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "ProductCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "ProductPropertyValue",
                columns: table => new
                {
                    ProductPropertyValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPropertyValue", x => x.ProductPropertyValueId);
                    table.ForeignKey(
                        name: "FK_ProductPropertyValue_ProductProperty_ProductPropertyId",
                        column: x => x.ProductPropertyId,
                        principalTable: "ProductProperty",
                        principalColumn: "ProductPropertyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPropertyValue_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperty_ProductCategoryId",
                table: "ProductProperty",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPropertyValue_ProductId",
                table: "ProductPropertyValue",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPropertyValue_ProductPropertyId",
                table: "ProductPropertyValue",
                column: "ProductPropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductCategory_ProductCategoryId",
                table: "Product",
                column: "ProductCategoryId",
                principalTable: "ProductCategory",
                principalColumn: "ProductCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductArchive_ProductCategory_ProductTypeProductCategoryId",
                table: "ProductArchive",
                column: "ProductTypeProductCategoryId",
                principalTable: "ProductCategory",
                principalColumn: "ProductCategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductCategory_ProductCategoryId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductArchive_ProductCategory_ProductTypeProductCategoryId",
                table: "ProductArchive");

            migrationBuilder.DropTable(
                name: "ProductPropertyValue");

            migrationBuilder.DropTable(
                name: "ProductProperty");

            migrationBuilder.DropTable(
                name: "ProductCategory");

            migrationBuilder.RenameColumn(
                name: "ProductTypeProductCategoryId",
                table: "ProductArchive",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductArchive_ProductTypeProductCategoryId",
                table: "ProductArchive",
                newName: "IX_ProductArchive_ProductTypeId");

            migrationBuilder.RenameColumn(
                name: "ProductCategoryId",
                table: "Product",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ProductCategoryId",
                table: "Product",
                newName: "IX_Product_ProductTypeId");

            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    ProductTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.ProductTypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductType_ProductTypeId",
                table: "Product",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductArchive_ProductType_ProductTypeId",
                table: "ProductArchive",
                column: "ProductTypeId",
                principalTable: "ProductType",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
