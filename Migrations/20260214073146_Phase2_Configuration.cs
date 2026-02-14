using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class Phase2_Configuration : Migration
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
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    MachineType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Machines_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonWorkingDayOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovedByUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonWorkingDayOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonWorkingDayOverrides_AspNetUsers_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonWorkingDayOverrides_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonWorkingDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonWorkingDays_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickPackStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickPackStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickPackStations_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    IsOvernight = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftTemplates_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingGroups_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRegions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MaxWeightLbs = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trucks_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineOperatorAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                    PostalCodePrefix = table.Column<string>(type: "TEXT", nullable: false),
                    RegionId = table.Column<int>(type: "INTEGER", nullable: true),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFsaRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_ShippingGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ShippingGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_ShippingRegions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "ShippingRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachineOperatorAssignments_MachineId",
                table: "MachineOperatorAssignments",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineOperatorAssignments_UserId",
                table: "MachineOperatorAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_BranchId",
                table: "Machines",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_NonWorkingDayOverrides_ApprovedByUserId",
                table: "NonWorkingDayOverrides",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NonWorkingDayOverrides_BranchId",
                table: "NonWorkingDayOverrides",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_NonWorkingDays_BranchId",
                table: "NonWorkingDays",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PickPackStations_BranchId",
                table: "PickPackStations",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftTemplates_BranchId",
                table: "ShiftTemplates",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_BranchId",
                table: "ShippingFsaRules",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_GroupId",
                table: "ShippingFsaRules",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_RegionId",
                table: "ShippingFsaRules",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingGroups_BranchId",
                table: "ShippingGroups",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRegions_BranchId",
                table: "ShippingRegions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_BranchId",
                table: "Trucks",
                column: "BranchId");
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
