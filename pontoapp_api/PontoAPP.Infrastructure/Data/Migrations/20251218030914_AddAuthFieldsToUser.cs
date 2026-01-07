using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PontoAPP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                schema: "public",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                schema: "public",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                schema: "public",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAt",
                schema: "public",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Platform = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    OsVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BiometricCapable = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PushToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_devices_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_devices_DeviceId",
                schema: "public",
                table: "devices",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_devices_DeviceId_UserId",
                schema: "public",
                table: "devices",
                columns: new[] { "DeviceId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_devices_TenantId",
                schema: "public",
                table: "devices",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_devices_UserId",
                schema: "public",
                table: "devices",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAt",
                schema: "public",
                table: "users");
        }
    }
}
