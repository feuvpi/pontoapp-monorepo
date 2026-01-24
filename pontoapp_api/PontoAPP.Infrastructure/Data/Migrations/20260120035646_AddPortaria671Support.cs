using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PontoAPP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPortaria671Support : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_email",
                schema: "public",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_tenants_email",
                schema: "public",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "EditReason",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "EditedAt",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                schema: "public",
                table: "time_records");

            migrationBuilder.RenameColumn(
                name: "CompanyDocument",
                schema: "public",
                table: "tenants",
                newName: "InscricaoEstadual");

            migrationBuilder.AlterColumn<bool>(
                name: "MustChangePassword",
                schema: "public",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "cpf",
                schema: "public",
                table: "users",
                type: "character(11)",
                fixedLength: true,
                maxLength: 11,
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "pis",
                schema: "public",
                table: "users",
                type: "character(11)",
                fixedLength: true,
                maxLength: 11,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "public",
                table: "time_records",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                schema: "public",
                table: "time_records",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                schema: "public",
                table: "time_records",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdjustment",
                schema: "public",
                table: "time_records",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "NSR",
                schema: "public",
                table: "time_records",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<Guid>(
                name: "OriginalTimeRecordId",
                schema: "public",
                table: "time_records",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureHash",
                schema: "public",
                table: "time_records",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                schema: "public",
                table: "time_records",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEI",
                schema: "public",
                table: "tenants",
                type: "character(12)",
                fixedLength: true,
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cnpj",
                schema: "public",
                table: "tenants",
                type: "character(14)",
                fixedLength: true,
                maxLength: 14,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TenantNSRCounters",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentNSR = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantNSRCounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantNSRCounters_tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "public",
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeRecordAdjustments",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalRecordId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdjustmentRecordId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalRecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewRecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewType = table.Column<int>(type: "integer", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RequestedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeRecordAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeRecordAdjustments_time_records_AdjustmentRecordId",
                        column: x => x.AdjustmentRecordId,
                        principalSchema: "public",
                        principalTable: "time_records",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TimeRecordAdjustments_time_records_OriginalRecordId",
                        column: x => x.OriginalRecordId,
                        principalSchema: "public",
                        principalTable: "time_records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeRecordAdjustments_users_ApproverId",
                        column: x => x.ApproverId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TimeRecordAdjustments_users_RequesterId",
                        column: x => x.RequesterId,
                        principalSchema: "public",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkSchedules",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    WeeklyHours = table.Column<decimal>(type: "numeric", nullable: false),
                    MondayStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MondayEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    MondayBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    TuesdayStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    TuesdayEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    TuesdayBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    WednesdayStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    WednesdayEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    WednesdayBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    ThursdayStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ThursdayEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ThursdayBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    FridayStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    FridayEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    FridayBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    SaturdayStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    SaturdayEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    SaturdayBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    SundayStart = table.Column<TimeSpan>(type: "interval", nullable: true),
                    SundayEnd = table.Column<TimeSpan>(type: "interval", nullable: true),
                    SundayBreakMinutes = table.Column<int>(type: "integer", nullable: false),
                    ToleranceMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_cpf",
                schema: "public",
                table: "users",
                column: "cpf");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "public",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_users_pis",
                schema: "public",
                table: "users",
                column: "pis",
                filter: "\"pis\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_time_records_OriginalTimeRecordId",
                schema: "public",
                table: "time_records",
                column: "OriginalTimeRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_time_records_tenant_nsr",
                schema: "public",
                table: "time_records",
                columns: new[] { "TenantId", "NSR" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_cnpj",
                schema: "public",
                table: "tenants",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantNSRCounters_TenantId",
                schema: "public",
                table: "TenantNSRCounters",
                column: "TenantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeRecordAdjustments_AdjustmentRecordId",
                schema: "public",
                table: "TimeRecordAdjustments",
                column: "AdjustmentRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeRecordAdjustments_ApproverId",
                schema: "public",
                table: "TimeRecordAdjustments",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeRecordAdjustments_OriginalRecordId",
                schema: "public",
                table: "TimeRecordAdjustments",
                column: "OriginalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeRecordAdjustments_RequesterId",
                schema: "public",
                table: "TimeRecordAdjustments",
                column: "RequesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_time_records_time_records_OriginalTimeRecordId",
                schema: "public",
                table: "time_records",
                column: "OriginalTimeRecordId",
                principalSchema: "public",
                principalTable: "time_records",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            

            // Criar stored procedure para gerar NSR atômico
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION get_next_nsr(p_tenant_id UUID)
            RETURNS BIGINT AS $$
            DECLARE
                v_nsr BIGINT;
            BEGIN
                UPDATE TenantNSRCounters 
                SET ""CurrentNSR"" = ""CurrentNSR"" + 1,
                    ""UpdatedAt"" = NOW()
                WHERE ""TenantId"" = p_tenant_id
                RETURNING ""CurrentNSR"" INTO v_nsr;
                
                IF NOT FOUND THEN
                    INSERT INTO TenantNSRCounters (""Id"", ""TenantId"", ""CurrentNSR"", ""UpdatedAt"", ""CreatedAt"")
                    VALUES (gen_random_uuid(), p_tenant_id, 1, NOW(), NOW())
                    RETURNING ""CurrentNSR"" INTO v_nsr;
                END IF;
                
                RETURN v_nsr;
            END;
            $$ LANGUAGE plpgsql;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_time_records_time_records_OriginalTimeRecordId",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropTable(
                name: "TenantNSRCounters",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TimeRecordAdjustments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WorkSchedules",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_users_cpf",
                schema: "public",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_email",
                schema: "public",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_pis",
                schema: "public",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_time_records_OriginalTimeRecordId",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropIndex(
                name: "IX_time_records_tenant_nsr",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropIndex(
                name: "IX_tenants_cnpj",
                schema: "public",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "cpf",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "pis",
                schema: "public",
                table: "users");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "IsAdjustment",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "NSR",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "OriginalTimeRecordId",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "SignatureHash",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                schema: "public",
                table: "time_records");

            migrationBuilder.DropColumn(
                name: "CEI",
                schema: "public",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "cnpj",
                schema: "public",
                table: "tenants");

            migrationBuilder.RenameColumn(
                name: "InscricaoEstadual",
                schema: "public",
                table: "tenants",
                newName: "CompanyDocument");

            migrationBuilder.AlterColumn<bool>(
                name: "MustChangePassword",
                schema: "public",
                table: "users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "public",
                table: "time_records",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditReason",
                schema: "public",
                table: "time_records",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                schema: "public",
                table: "time_records",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                schema: "public",
                table: "time_records",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                schema: "public",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_email",
                schema: "public",
                table: "tenants",
                column: "email");
        }
    }
}
