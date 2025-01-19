using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstrumentStore.Domain.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryMethod",
                columns: table => new
                {
                    DeliveryMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryMethod", x => x.DeliveryMethodId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.PaymentMethodId);
                });

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
                name: "UserAdresses",
                columns: table => new
                {
                    UserAdressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HouseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Entrance = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flat = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAdresses", x => x.UserAdressId);
                });

            migrationBuilder.CreateTable(
                name: "UserRegistrInfos",
                columns: table => new
                {
                    UserRegistrInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRegistrInfos", x => x.UserRegistrInfoId);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "ProductCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "ProductArchive",
                columns: table => new
                {
                    ProductArchiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductArchive", x => x.ProductArchiveId);
                    table.ForeignKey(
                        name: "FK_ProductArchive_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductArchive_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductArchive_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "ProductCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductProperty",
                columns: table => new
                {
                    ProductPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRanged = table.Column<bool>(type: "bit", nullable: false),
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
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAdressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserRegistrInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_UserAdresses_UserAdressId",
                        column: x => x.UserAdressId,
                        principalTable: "UserAdresses",
                        principalColumn: "UserAdressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_UserRegistrInfos_UserRegistrInfoId",
                        column: x => x.UserRegistrInfoId,
                        principalTable: "UserRegistrInfos",
                        principalColumn: "UserRegistrInfoId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    CartItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.CartItemId);
                    table.ForeignKey(
                        name: "FK_CartItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItem_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaidOrder",
                columns: table => new
                {
                    PaidOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaidOrder", x => x.PaidOrderId);
                    table.ForeignKey(
                        name: "FK_PaidOrder_DeliveryMethod_DeliveryMethodId",
                        column: x => x.DeliveryMethodId,
                        principalTable: "DeliveryMethod",
                        principalColumn: "DeliveryMethodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaidOrder_PaymentMethod_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethod",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaidOrder_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaidOrderItem",
                columns: table => new
                {
                    PaidOrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaidOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaidOrderItem", x => x.PaidOrderItemId);
                    table.ForeignKey(
                        name: "FK_PaidOrderItem_PaidOrder_PaidOrderId",
                        column: x => x.PaidOrderId,
                        principalTable: "PaidOrder",
                        principalColumn: "PaidOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaidOrderItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ProductId",
                table: "CartItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_UserId",
                table: "CartItem",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidOrder_DeliveryMethodId",
                table: "PaidOrder",
                column: "DeliveryMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidOrder_PaymentMethodId",
                table: "PaidOrder",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidOrder_UserId",
                table: "PaidOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidOrderItem_PaidOrderId",
                table: "PaidOrderItem",
                column: "PaidOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidOrderItem_ProductId",
                table: "PaidOrderItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandId",
                table: "Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CountryId",
                table: "Product",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductCategoryId",
                table: "Product",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductArchive_BrandId",
                table: "ProductArchive",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductArchive_CountryId",
                table: "ProductArchive",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductArchive_ProductCategoryId",
                table: "ProductArchive",
                column: "ProductCategoryId");

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

            migrationBuilder.CreateIndex(
                name: "IX_User_UserAdressId",
                table: "User",
                column: "UserAdressId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserRegistrInfoId",
                table: "User",
                column: "UserRegistrInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "PaidOrderItem");

            migrationBuilder.DropTable(
                name: "ProductArchive");

            migrationBuilder.DropTable(
                name: "ProductPropertyValue");

            migrationBuilder.DropTable(
                name: "PaidOrder");

            migrationBuilder.DropTable(
                name: "ProductProperty");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "DeliveryMethod");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "ProductCategory");

            migrationBuilder.DropTable(
                name: "UserAdresses");

            migrationBuilder.DropTable(
                name: "UserRegistrInfos");
        }
    }
}
