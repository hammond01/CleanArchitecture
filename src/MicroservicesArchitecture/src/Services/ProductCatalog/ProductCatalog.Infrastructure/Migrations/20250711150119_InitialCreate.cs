using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductCatalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Picture = table.Column<byte[]>(type: "image", nullable: true),
                    PictureLink = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ContactTitle = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    City = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    HomePage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SupplierId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    QuantityPerUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: true),
                    UnitsInStock = table.Column<short>(type: "smallint", nullable: true),
                    UnitsOnOrder = table.Column<short>(type: "smallint", nullable: true),
                    ReorderLevel = table.Column<short>(type: "smallint", nullable: true),
                    Discontinued = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "CreatedBy", "Description", "Picture", "PictureLink", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { "01JH179GGZ7FAHZ0DNFYNZ21BB", "Electronics", new DateTime(2025, 7, 11, 15, 1, 18, 1, DateTimeKind.Utc).AddTicks(2719), null, "Electronic devices and accessories", null, "https://example.com/electronics.jpg", null, null },
                    { "01JH179GGZ7FAHZ0DNFYNZ23DD", "Mobile Phones", new DateTime(2025, 7, 11, 15, 1, 18, 1, DateTimeKind.Utc).AddTicks(2721), null, "Smartphones and mobile accessories", null, "https://example.com/phones.jpg", null, null },
                    { "01JH179GGZ7FAHZ0DNFYNZ24EE", "Computers", new DateTime(2025, 7, 11, 15, 1, 18, 1, DateTimeKind.Utc).AddTicks(2721), null, "Laptops, desktops and computer accessories", null, "https://example.com/computers.jpg", null, null }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "SupplierId", "Address", "City", "CompanyName", "ContactName", "ContactTitle", "Country", "CreatedAt", "CreatedBy", "Fax", "HomePage", "Phone", "PostalCode", "Region", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { "01JH179GGZ7FAHZ0DNFYNZ18YX", "123 Tech Street", "TechCity", "Tech Supplies Co.", "John Doe", "Sales Manager", "Techland", new DateTime(2025, 7, 11, 15, 1, 18, 1, DateTimeKind.Utc).AddTicks(4291), null, "123-456-7891", "https://github.com/hammond01", "123-456-7890", "12345", "TechRegion", null, null },
                    { "01JH179GGZ7FAHZ0DNFYNZ19FG", "456 Mobile Blvd", "MobileCity", "Mobile Accessories Inc.", "Jane Smith", "CEO", "PhoneCountry", new DateTime(2025, 7, 11, 15, 1, 18, 1, DateTimeKind.Utc).AddTicks(4293), null, "098-765-4322", "https://github.com/hammond01", "098-765-4321", "67890", "MobileRegion", null, null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "CreatedBy", "Discontinued", "ProductName", "QuantityPerUnit", "ReorderLevel", "SupplierId", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { "01JH179GGZ7FAHZ0DNFYNZ20AA", "01JH179GGZ7FAHZ0DNFYNZ21BB", new DateTime(2025, 7, 11, 15, 1, 18, 1, DateTimeKind.Utc).AddTicks(1507), null, false, "Laptop Dell XPS 13", "1 unit", (short)10, "01JH179GGZ7FAHZ0DNFYNZ18YX", 1299.99m, (short)50, (short)0, null, null },
                    { "01JH179GGZ7FAHZ0DNFYNZ22CC", "01JH179GGZ7FAHZ0DNFYNZ23DD", new DateTime(2025, 7, 11, 15, 1, 18, 1, DateTimeKind.Utc).AddTicks(1510), null, false, "iPhone 15 Pro", "1 unit", (short)5, "01JH179GGZ7FAHZ0DNFYNZ19FG", 999.99m, (short)30, (short)20, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductName",
                table: "Products",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CompanyName",
                table: "Suppliers",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_PostalCode",
                table: "Suppliers",
                column: "PostalCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
