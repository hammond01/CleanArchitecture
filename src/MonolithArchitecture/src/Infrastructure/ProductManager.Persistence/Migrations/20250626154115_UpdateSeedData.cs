using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryName", "CreatedDateTime", "Description", "Picture", "PictureLink", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JH179GGG9BN2V8SS9RG70QNG", "Mobile Phones", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3319), new TimeSpan(0, 7, 0, 0, 0)), "Category for the latest mobile phones", null, null, null },
                    { "01JH179GGG9BN2V8SS9RG70QPX", "Accessories", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3368), new TimeSpan(0, 7, 0, 0, 0)), "Category for accessories such as cases, chargers, cables", null, null, null },
                    { "01JH179GGG9BN2V8SS9RG70QRS", "SIM Cards", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3370), new TimeSpan(0, 7, 0, 0, 0)), "Category for SIM cards and promotional plans", null, null, null },
                    { "01JH179GGG9BN2V8SS9RG70QTU", "Repair Services", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3372), new TimeSpan(0, 7, 0, 0, 0)), "Category for professional phone repair services", null, null, null },
                    { "01JH179GGG9BN2V8SS9RG70QVW", "Extended Warranty", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3374), new TimeSpan(0, 7, 0, 0, 0)), "Category for extended warranty packages", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "CreatedDateTime", "RegionDescription", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JYPECTTP1FJ7P4ZFBCR7C5MP", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Can Tho", null },
                    { "01JYPECTTPC5AH6E1QCBZ4FW5M", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ho Chi Minh City", null },
                    { "01JYPECTTPNJERJD030WPY529C", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nha Trang", null },
                    { "01JYPECTTPV5FC08HYWCNXXRHK", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ha Noi", null }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "City", "CompanyName", "ContactName", "ContactTitle", "Country", "CreatedDateTime", "Fax", "HomePage", "Phone", "PostalCode", "Region", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JH179GGZ7FAHZ0DNFYNZ18YX", "123 Tech Street", "TechCity", "Tech Supplies Co.", "John Doe", "Sales Manager", "Techland", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "123-456-7891", "https://github.com/hammond01", "123-456-7890", "12345", "TechRegion", null },
                    { "01JH179GGZ7FAHZ0DNFYNZ19FG", "456 Mobile Blvd", "MobileCity", "Mobile Accessories Inc.", "Jane Smith", "CEO", "PhoneCountry", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "098-765-4322", "https://github.com/hammond01", "098-765-4321", "67890", "MobileRegion", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QNG");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QPX");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QRS");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QTU");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QVW");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPECTTP1FJ7P4ZFBCR7C5MP");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPECTTPC5AH6E1QCBZ4FW5M");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPECTTPNJERJD030WPY529C");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPECTTPV5FC08HYWCNXXRHK");

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: "01JH179GGZ7FAHZ0DNFYNZ18YX");

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: "01JH179GGZ7FAHZ0DNFYNZ19FG");

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
        }
    }
}
