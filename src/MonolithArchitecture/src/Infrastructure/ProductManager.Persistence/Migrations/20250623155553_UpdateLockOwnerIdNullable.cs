using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLockOwnerIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierID",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG5KSSWRZGEX41YT9K");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QNG");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGGHZARD5R4YMAA2CZ4");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGGPX0ZMTGSQTXTHH8B");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGGY50TV4V86N3NZXJV");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JH179GGY3YJGNZ5R9K3NSNCJ");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JH179GGY9167SY5TNYDDYATN");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JH179GGYETDAC820A28BXYZV");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JH179GGYKXDXBDP9PABPZQRN");

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: "01JH179GGZ32TC9HH3THJMR6X4");

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: "01JH179GGZ7FAHZ0DNFYNZ19FG");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Revoked",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Expires",
                table: "RefreshToken",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierID",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryID",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Locks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryName", "CreatedDateTime", "Description", "Picture", "PictureLink", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JYER1FST68X27T1H8WWGSAH4", "Accessories", new DateTimeOffset(new DateTime(2025, 6, 23, 22, 55, 49, 946, DateTimeKind.Unspecified).AddTicks(1241), new TimeSpan(0, 7, 0, 0, 0)), "Category for accessories such as cases, chargers, cables", null, null, null },
                    { "01JYER1FSTBWDGAST458E79ANV", "SIM Cards", new DateTimeOffset(new DateTime(2025, 6, 23, 22, 55, 49, 946, DateTimeKind.Unspecified).AddTicks(1245), new TimeSpan(0, 7, 0, 0, 0)), "Category for SIM cards and promotional plans", null, null, null },
                    { "01JYER1FSTEZF8ATS6DGKBE81X", "Repair Services", new DateTimeOffset(new DateTime(2025, 6, 23, 22, 55, 49, 946, DateTimeKind.Unspecified).AddTicks(1248), new TimeSpan(0, 7, 0, 0, 0)), "Category for professional phone repair services", null, null, null },
                    { "01JYER1FSTT4G7BNPCQMATHZ1V", "Extended Warranty", new DateTimeOffset(new DateTime(2025, 6, 23, 22, 55, 49, 946, DateTimeKind.Unspecified).AddTicks(1290), new TimeSpan(0, 7, 0, 0, 0)), "Category for extended warranty packages", null, null, null },
                    { "01JYER1FSTWGRCA3DCJQCJ56B6", "Mobile Phones", new DateTimeOffset(new DateTime(2025, 6, 23, 22, 55, 49, 946, DateTimeKind.Unspecified).AddTicks(1205), new TimeSpan(0, 7, 0, 0, 0)), "Category for the latest mobile phones", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "CreatedDateTime", "RegionDescription", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JYER1FT00CZNRD48Z4C47R90", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ha Noi", null },
                    { "01JYER1FT053C4WXNYQRVXMMYE", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nha Trang", null },
                    { "01JYER1FT0A79PS2E15DSP2BB4", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ho Chi Minh City", null },
                    { "01JYER1FT0YEYN2PVT4EFY9ZS0", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Can Tho", null }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "City", "CompanyName", "ContactName", "ContactTitle", "Country", "CreatedDateTime", "Fax", "HomePage", "Phone", "PostalCode", "Region", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JYER1FT0KH7XRAQH2QBKZFCQ", "456 Mobile Blvd", "MobileCity", "Mobile Accessories Inc.", "Jane Smith", "CEO", "PhoneCountry", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "098-765-4322", "https://github.com/hammond01", "098-765-4321", "67890", "MobileRegion", null },
                    { "01JYER1FT0WT9XDZJBNTQZPGG1", "123 Tech Street", "TechCity", "Tech Supplies Co.", "John Doe", "Sales Manager", "Techland", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "123-456-7891", "https://github.com/hammond01", "123-456-7890", "12345", "TechRegion", null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierID",
                table: "Products",
                column: "SupplierID",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierID",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JYER1FST68X27T1H8WWGSAH4");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JYER1FSTBWDGAST458E79ANV");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JYER1FSTEZF8ATS6DGKBE81X");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JYER1FSTT4G7BNPCQMATHZ1V");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JYER1FSTWGRCA3DCJQCJ56B6");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYER1FT00CZNRD48Z4C47R90");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYER1FT053C4WXNYQRVXMMYE");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYER1FT0A79PS2E15DSP2BB4");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYER1FT0YEYN2PVT4EFY9ZS0");

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: "01JYER1FT0KH7XRAQH2QBKZFCQ");

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: "01JYER1FT0WT9XDZJBNTQZPGG1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Revoked",
                table: "RefreshToken",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Expires",
                table: "RefreshToken",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "SupplierID",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryID",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Locks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryName", "CreatedDateTime", "Description", "Picture", "PictureLink", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JH179GGG5KSSWRZGEX41YT9K", "Extended Warranty", new DateTimeOffset(new DateTime(2025, 1, 8, 3, 28, 55, 696, DateTimeKind.Unspecified).AddTicks(533), new TimeSpan(0, 7, 0, 0, 0)), "Category for extended warranty packages", null, null, null },
                    { "01JH179GGG9BN2V8SS9RG70QNG", "Mobile Phones", new DateTimeOffset(new DateTime(2025, 1, 8, 3, 28, 55, 696, DateTimeKind.Unspecified).AddTicks(487), new TimeSpan(0, 7, 0, 0, 0)), "Category for the latest mobile phones", null, null, null },
                    { "01JH179GGGHZARD5R4YMAA2CZ4", "Repair Services", new DateTimeOffset(new DateTime(2025, 1, 8, 3, 28, 55, 696, DateTimeKind.Unspecified).AddTicks(529), new TimeSpan(0, 7, 0, 0, 0)), "Category for professional phone repair services", null, null, null },
                    { "01JH179GGGPX0ZMTGSQTXTHH8B", "Accessories", new DateTimeOffset(new DateTime(2025, 1, 8, 3, 28, 55, 696, DateTimeKind.Unspecified).AddTicks(523), new TimeSpan(0, 7, 0, 0, 0)), "Category for accessories such as cases, chargers, cables", null, null, null },
                    { "01JH179GGGY50TV4V86N3NZXJV", "SIM Cards", new DateTimeOffset(new DateTime(2025, 1, 8, 3, 28, 55, 696, DateTimeKind.Unspecified).AddTicks(526), new TimeSpan(0, 7, 0, 0, 0)), "Category for SIM cards and promotional plans", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "CreatedDateTime", "RegionDescription", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JH179GGY3YJGNZ5R9K3NSNCJ", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ho Chi Minh City", null },
                    { "01JH179GGY9167SY5TNYDDYATN", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ha Noi", null },
                    { "01JH179GGYETDAC820A28BXYZV", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Can Tho", null },
                    { "01JH179GGYKXDXBDP9PABPZQRN", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nha Trang", null }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "City", "CompanyName", "ContactName", "ContactTitle", "Country", "CreatedDateTime", "Fax", "HomePage", "Phone", "PostalCode", "Region", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JH179GGZ32TC9HH3THJMR6X4", "456 Mobile Blvd", "MobileCity", "Mobile Accessories Inc.", "Jane Smith", "CEO", "PhoneCountry", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "098-765-4322", "https://github.com/hammond01", "098-765-4321", "67890", "MobileRegion", null },
                    { "01JH179GGZ7FAHZ0DNFYNZ19FG", "123 Tech Street", "TechCity", "Tech Supplies Co.", "John Doe", "Sales Manager", "Techland", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "123-456-7891", "https://github.com/hammond01", "123-456-7890", "12345", "TechRegion", null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryID",
                table: "Products",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierID",
                table: "Products",
                column: "SupplierID",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }
    }
}
