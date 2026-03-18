using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auditing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuditing_PostgresBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auditing");

            migrationBuilder.CreateTable(
                name: "AuditLogEntries",
                schema: "auditing",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Action = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ObjectId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Log = table.Column<string>(type: "text", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_Action",
                schema: "auditing",
                table: "AuditLogEntries",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_UserId_CreatedDateTime",
                schema: "auditing",
                table: "AuditLogEntries",
                columns: new[] { "UserId", "CreatedDateTime" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogEntries",
                schema: "auditing");
        }
    }
}
