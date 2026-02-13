using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class _002_ShippingConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRegions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingRegions_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ShippingRegionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingGroups_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingGroups_ShippingRegions_ShippingRegionId",
                        column: x => x.ShippingRegionId,
                        principalTable: "ShippingRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingFsaRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    FsaPrefix = table.Column<string>(type: "nchar(3)", maxLength: 3, nullable: false),
                    ShippingRegionId = table.Column<int>(type: "int", nullable: false),
                    ShippingGroupId = table.Column<int>(type: "int", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingFsaRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_ShippingGroups_ShippingGroupId",
                        column: x => x.ShippingGroupId,
                        principalTable: "ShippingGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShippingFsaRules_ShippingRegions_ShippingRegionId",
                        column: x => x.ShippingRegionId,
                        principalTable: "ShippingRegions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_BranchId",
                table: "ShippingFsaRules",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_ShippingGroupId",
                table: "ShippingFsaRules",
                column: "ShippingGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingFsaRules_ShippingRegionId",
                table: "ShippingFsaRules",
                column: "ShippingRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingGroups_BranchId",
                table: "ShippingGroups",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingGroups_ShippingRegionId",
                table: "ShippingGroups",
                column: "ShippingRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRegions_BranchId",
                table: "ShippingRegions",
                column: "BranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingFsaRules");

            migrationBuilder.DropTable(
                name: "ShippingGroups");

            migrationBuilder.DropTable(
                name: "ShippingRegions");
        }
    }
}
