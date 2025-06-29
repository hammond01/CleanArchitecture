using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPEK51P217B652XJ9BEB6HV");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPEK51P7FCDKYK54XRXB3YJ");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPEK51PJMK278810XSA2MDZ");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPEK51PSVS88TQRWAQN0DN1");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QNG",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 887, DateTimeKind.Unspecified).AddTicks(4498), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QPX",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 887, DateTimeKind.Unspecified).AddTicks(4534), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QRS",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 887, DateTimeKind.Unspecified).AddTicks(4537), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QTU",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 887, DateTimeKind.Unspecified).AddTicks(4539), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QVW",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 887, DateTimeKind.Unspecified).AddTicks(4547), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryID", "CreatedDateTime", "Discontinued", "ProductName", "QuantityPerUnit", "ReorderLevel", "SupplierID", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JH179GH01234567890ABCDEF", "01JH179GGG9BN2V8SS9RG70QNG", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 892, DateTimeKind.Unspecified).AddTicks(8463), new TimeSpan(0, 7, 0, 0, 0)), false, "iPhone 15 Pro", "1 unit", (short)10, "01JH179GGZ7FAHZ0DNFYNZ19FG", 999.99m, (short)50, (short)0, null },
                    { "01JH179GH01234567890GHIJKL", "01JH179GGG9BN2V8SS9RG70QNG", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 892, DateTimeKind.Unspecified).AddTicks(8497), new TimeSpan(0, 7, 0, 0, 0)), false, "Samsung Galaxy S24", "1 unit", (short)5, "01JH179GGZ7FAHZ0DNFYNZ18YX", 899.99m, (short)30, (short)0, null },
                    { "01JH179GH01234567890MNOPQR", "01JH179GGG9BN2V8SS9RG70QPX", new DateTimeOffset(new DateTime(2025, 6, 26, 22, 48, 40, 892, DateTimeKind.Unspecified).AddTicks(8501), new TimeSpan(0, 7, 0, 0, 0)), false, "Wireless Charger", "1 unit", (short)20, "01JH179GGZ7FAHZ0DNFYNZ19FG", 49.99m, (short)100, (short)0, null }
                });

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "CreatedDateTime", "RegionDescription", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JYPETHSY2Z3V3R1K8RWQD9AE", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ho Chi Minh City", null },
                    { "01JYPETHSYG7WHV6Z8S451D92C", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ha Noi", null },
                    { "01JYPETHSYJCC3QFMW3EZ73XSW", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nha Trang", null },
                    { "01JYPETHSYXNCKNKZJEXZT6A0M", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Can Tho", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "01JH179GH01234567890ABCDEF");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "01JH179GH01234567890GHIJKL");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "01JH179GH01234567890MNOPQR");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPETHSY2Z3V3R1K8RWQD9AE");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPETHSYG7WHV6Z8S451D92C");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPETHSYJCC3QFMW3EZ73XSW");

            migrationBuilder.DeleteData(
                table: "Region",
                keyColumn: "Id",
                keyValue: "01JYPETHSYXNCKNKZJEXZT6A0M");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QNG",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 44, 38, 446, DateTimeKind.Unspecified).AddTicks(7030), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QPX",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 44, 38, 446, DateTimeKind.Unspecified).AddTicks(7081), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QRS",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 44, 38, 446, DateTimeKind.Unspecified).AddTicks(7083), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QTU",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 44, 38, 446, DateTimeKind.Unspecified).AddTicks(7086), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QVW",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 44, 38, 446, DateTimeKind.Unspecified).AddTicks(7088), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Region",
                columns: new[] { "Id", "CreatedDateTime", "RegionDescription", "UpdatedDateTime" },
                values: new object[,]
                {
                    { "01JYPEK51P217B652XJ9BEB6HV", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ho Chi Minh City", null },
                    { "01JYPEK51P7FCDKYK54XRXB3YJ", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Nha Trang", null },
                    { "01JYPEK51PJMK278810XSA2MDZ", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Can Tho", null },
                    { "01JYPEK51PSVS88TQRWAQN0DN1", new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Ha Noi", null }
                });
        }
    }
}
