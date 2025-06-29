using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManager.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixCategorySeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3319), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QPX",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3368), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QRS",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3370), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QTU",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3372), new TimeSpan(0, 7, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: "01JH179GGG9BN2V8SS9RG70QVW",
                column: "CreatedDateTime",
                value: new DateTimeOffset(new DateTime(2025, 6, 26, 22, 41, 11, 374, DateTimeKind.Unspecified).AddTicks(3374), new TimeSpan(0, 7, 0, 0, 0)));

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
        }
    }
}
