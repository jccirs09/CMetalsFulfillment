using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_Config : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    MachineCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MachineName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MachineType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultDurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonWorkingDayOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateLocal = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    OverrideType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuditEventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonWorkingDayOverrides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateLocal = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonWorkingDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickPackStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    StationCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    StationName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickPackStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShiftCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    StartLocalTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    EndLocalTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    BreakMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingFobMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    FobToken = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ShippingMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFobMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegionCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    RegionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRegions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    TruckCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MaxWeightLbs = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineOperatorAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    MachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    IsQualified = table.Column<bool>(type: "INTEGER", nullable: false),
                    QualifiedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    QualifiedByUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineOperatorAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineOperatorAssignments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MachineOperatorAssignments_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingFsaRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    FsaPrefix = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    RegionId = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFsaRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_ShippingGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ShippingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_ShippingRegions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "ShippingRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachineOperatorAssignments_BranchId_MachineId_UserId",
                table: "MachineOperatorAssignments",
                columns: new[] { "BranchId", "MachineId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineOperatorAssignments_MachineId",
                table: "MachineOperatorAssignments",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineOperatorAssignments_UserId",
                table: "MachineOperatorAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_BranchId_MachineCode",
                table: "Machines",
                columns: new[] { "BranchId", "MachineCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonWorkingDayOverrides_BranchId_DateLocal_OverrideType",
                table: "NonWorkingDayOverrides",
                columns: new[] { "BranchId", "DateLocal", "OverrideType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NonWorkingDays_BranchId_DateLocal",
                table: "NonWorkingDays",
                columns: new[] { "BranchId", "DateLocal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickPackStations_BranchId_StationCode",
                table: "PickPackStations",
                columns: new[] { "BranchId", "StationCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTemplates_BranchId_ShiftCode",
                table: "ShiftTemplates",
                columns: new[] { "BranchId", "ShiftCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFobMappings_BranchId_FobToken",
                table: "ShippingFobMappings",
                columns: new[] { "BranchId", "FobToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_BranchId_FsaPrefix_Priority",
                table: "ShippingFsaRules",
                columns: new[] { "BranchId", "FsaPrefix", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_GroupId",
                table: "ShippingFsaRules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_RegionId",
                table: "ShippingFsaRules",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingGroups_BranchId_GroupCode",
                table: "ShippingGroups",
                columns: new[] { "BranchId", "GroupCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRegions_BranchId_RegionCode",
                table: "ShippingRegions",
                columns: new[] { "BranchId", "RegionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_BranchId_TruckCode",
                table: "Trucks",
                columns: new[] { "BranchId", "TruckCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineOperatorAssignments");

            migrationBuilder.DropTable(
                name: "NonWorkingDayOverrides");

            migrationBuilder.DropTable(
                name: "NonWorkingDays");

            migrationBuilder.DropTable(
                name: "PickPackStations");

            migrationBuilder.DropTable(
                name: "ShiftTemplates");

            migrationBuilder.DropTable(
                name: "ShippingFobMappings");

            migrationBuilder.DropTable(
                name: "ShippingFsaRules");

            migrationBuilder.DropTable(
                name: "Trucks");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "ShippingGroups");

            migrationBuilder.DropTable(
                name: "ShippingRegions");
        }
    }
}
