using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class _005_PickingLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PickingLists",
                columns: table => new
                {
                    PickingListUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImportBranchId = table.Column<int>(type: "int", nullable: false),
                    PickingListNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShipToName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ShipToAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ShipToCity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShipToState = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ShipToZip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FOBPoint = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ShipDateLocal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingLists", x => x.PickingListUid);
                    table.ForeignKey(
                        name: "FK_PickingLists_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickingLists_Branches_ImportBranchId",
                        column: x => x.ImportBranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickingListEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickingListUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingListEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingListEvents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickingListEvents_PickingLists_PickingListUid",
                        column: x => x.PickingListUid,
                        principalTable: "PickingLists",
                        principalColumn: "PickingListUid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickingListLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickingListUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QtyOrdered = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UOM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FulfillmentKind = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AssignedPickPackStationId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingListLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingListLines_PickPackStations_AssignedPickPackStationId",
                        column: x => x.AssignedPickPackStationId,
                        principalTable: "PickPackStations",
                        principalColumn: "PickPackStationId");
                    table.ForeignKey(
                        name: "FK_PickingListLines_PickingLists_PickingListUid",
                        column: x => x.PickingListUid,
                        principalTable: "PickingLists",
                        principalColumn: "PickingListUid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickingListEvents_PickingListUid",
                table: "PickingListEvents",
                column: "PickingListUid");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListEvents_UserId",
                table: "PickingListEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListLines_AssignedPickPackStationId",
                table: "PickingListLines",
                column: "AssignedPickPackStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListLines_PickingListUid",
                table: "PickingListLines",
                column: "PickingListUid");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_CreatedByUserId",
                table: "PickingLists",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_ImportBranchId",
                table: "PickingLists",
                column: "ImportBranchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickingListEvents");

            migrationBuilder.DropTable(
                name: "PickingListLines");

            migrationBuilder.DropTable(
                name: "PickingLists");
        }
    }
}
