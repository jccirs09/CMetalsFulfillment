using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchSettings",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    SetupCompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefaultPickPackStationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchSettings", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_BranchSettings_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventorySnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ImportedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImportedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SourceFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    MatchedRows = table.Column<int>(type: "int", nullable: false),
                    UnmatchedRows = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventorySnapshots_AspNetUsers_ImportedByUserId",
                        column: x => x.ImportedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventorySnapshots_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryStocks",
                columns: table => new
                {
                    InventoryStockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QuantityOnHand = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    WeightOnHand = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryStocks", x => x.InventoryStockId);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CoilRelationship = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PoundsPerSquareFoot = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CoilItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    MachineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    MachineType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.MachineId);
                    table.ForeignKey(
                        name: "FK_Machines_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "PickPackStations",
                columns: table => new
                {
                    PickPackStationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickPackStations", x => x.PickPackStationId);
                    table.ForeignKey(
                        name: "FK_PickPackStations_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftTemplates",
                columns: table => new
                {
                    ShiftTemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTimeLocal = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTimeLocal = table.Column<TimeSpan>(type: "time", nullable: false),
                    BreakRulesJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTemplates", x => x.ShiftTemplateId);
                    table.ForeignKey(
                        name: "FK_ShiftTemplates_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "UserBranchClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranchClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBranchClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBranchClaims_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBranchMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DefaultForUser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBranchMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBranchMemberships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBranchMemberships_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventorySnapshotLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SnapshotId = table.Column<int>(type: "int", nullable: false),
                    LineNo = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Mode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TagNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Correctable = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SnapshotLocation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountLocation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SnapshotValue = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CountValue = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Exception = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UOM = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    MatchStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MatchedItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventorySnapshotLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventorySnapshotLines_InventorySnapshots_SnapshotId",
                        column: x => x.SnapshotId,
                        principalTable: "InventorySnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventorySnapshotLines_Items_MatchedItemId",
                        column: x => x.MatchedItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MachineOperatorAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
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
                        principalColumn: "MachineId",
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
                    QtyOrdered = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "PickPackStationAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickPackStationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickPackStationAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickPackStationAssignments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickPackStationAssignments_PickPackStations_PickPackStationId",
                        column: x => x.PickPackStationId,
                        principalTable: "PickPackStations",
                        principalColumn: "PickPackStationId",
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySnapshotLines_MatchedItemId",
                table: "InventorySnapshotLines",
                column: "MatchedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySnapshotLines_SnapshotId",
                table: "InventorySnapshotLines",
                column: "SnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySnapshots_BranchId",
                table: "InventorySnapshots",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InventorySnapshots_ImportedByUserId",
                table: "InventorySnapshots",
                column: "ImportedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_BranchId",
                table: "InventoryStocks",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_BranchId_ItemCode",
                table: "Items",
                columns: new[] { "BranchId", "ItemCode" },
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
                name: "IX_Machines_BranchId",
                table: "Machines",
                column: "BranchId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PickPackStationAssignments_PickPackStationId",
                table: "PickPackStationAssignments",
                column: "PickPackStationId");

            migrationBuilder.CreateIndex(
                name: "IX_PickPackStationAssignments_UserId",
                table: "PickPackStationAssignments",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchClaims_BranchId",
                table: "UserBranchClaims",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchClaims_UserId",
                table: "UserBranchClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchMemberships_BranchId",
                table: "UserBranchMemberships",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchMemberships_UserId",
                table: "UserBranchMemberships",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BranchSettings");

            migrationBuilder.DropTable(
                name: "InventorySnapshotLines");

            migrationBuilder.DropTable(
                name: "InventoryStocks");

            migrationBuilder.DropTable(
                name: "MachineOperatorAssignments");

            migrationBuilder.DropTable(
                name: "PickingListEvents");

            migrationBuilder.DropTable(
                name: "PickingListLines");

            migrationBuilder.DropTable(
                name: "PickPackStationAssignments");

            migrationBuilder.DropTable(
                name: "ShiftTemplates");

            migrationBuilder.DropTable(
                name: "ShippingFsaRules");

            migrationBuilder.DropTable(
                name: "UserBranchClaims");

            migrationBuilder.DropTable(
                name: "UserBranchMemberships");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "InventorySnapshots");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "PickingLists");

            migrationBuilder.DropTable(
                name: "PickPackStations");

            migrationBuilder.DropTable(
                name: "ShippingGroups");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ShippingRegions");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
