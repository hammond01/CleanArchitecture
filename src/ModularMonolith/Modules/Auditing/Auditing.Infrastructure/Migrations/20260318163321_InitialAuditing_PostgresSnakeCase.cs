using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auditing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuditing_PostgresSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auditing");

            migrationBuilder.CreateTable(
                name: "audit_log_entries",
                schema: "auditing",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    user_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    action = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    object_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    log = table.Column<string>(type: "text", nullable: false),
                    created_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log_entries", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_entries_action",
                schema: "auditing",
                table: "audit_log_entries",
                column: "action");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_entries_user_id_created_date_time",
                schema: "auditing",
                table: "audit_log_entries",
                columns: new[] { "user_id", "created_date_time" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log_entries",
                schema: "auditing");
        }
    }
}
