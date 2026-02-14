using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class M000_Core : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TimezoneId = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "UTC")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultBranchId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Branches_DefaultBranchId",
                        column: x => x.DefaultBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LoadPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    TruckId = table.Column<string>(type: "TEXT", nullable: true),
                    DriverId = table.Column<string>(type: "TEXT", nullable: true),
                    PlannedDepartUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Version = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadPlans_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickingListHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    PickingListNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PrintDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ShipDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Buyer = table.Column<string>(type: "TEXT", nullable: true),
                    SalesRep = table.Column<string>(type: "TEXT", nullable: true),
                    ShipVia = table.Column<string>(type: "TEXT", nullable: true),
                    FobPoint = table.Column<string>(type: "TEXT", nullable: true),
                    SoldTo = table.Column<string>(type: "TEXT", nullable: true),
                    ShipTo = table.Column<string>(type: "TEXT", nullable: true),
                    TotalWeightLbs = table.Column<decimal>(type: "TEXT", nullable: false),
                    OrderInstructions = table.Column<string>(type: "TEXT", nullable: true),
                    ShippingMethod = table.Column<string>(type: "TEXT", nullable: true),
                    ShippingRegion = table.Column<string>(type: "TEXT", nullable: true),
                    ShippingGroup = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ImportedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingListHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingListHeaders_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    WeightLbs = table.Column<decimal>(type: "TEXT", nullable: false),
                    ItemCode = table.Column<string>(type: "TEXT", nullable: true),
                    Uom = table.Column<string>(type: "TEXT", nullable: true),
                    SourceType = table.Column<string>(type: "TEXT", nullable: true),
                    SourceRefId = table.Column<string>(type: "TEXT", nullable: true),
                    AllowedDestinationBranchId = table.Column<int>(type: "INTEGER", nullable: true),
                    SoftDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Version = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Branches_AllowedDestinationBranchId",
                        column: x => x.AllowedDestinationBranchId,
                        principalTable: "Branches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tags_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "AspNetUserPasskeys",
                columns: table => new
                {
                    CredentialId = table.Column<byte[]>(type: "BLOB", maxLength: 1024, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserPasskeys", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_AspNetUserPasskeys_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
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
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
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
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    EntityId = table.Column<string>(type: "TEXT", nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActorUserId = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditEvents_AspNetUsers_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditEvents_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchMemberships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchMemberships_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchRoles_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ErpShipmentRefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    ErpPackingListNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PickingListHeaderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipDateLocal = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErpShipmentRefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErpShipmentRefs_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ErpShipmentRefs_PickingListHeaders_PickingListHeaderId",
                        column: x => x.PickingListHeaderId,
                        principalTable: "PickingListHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoadPlanReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoadPlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    PickingListHeaderId = table.Column<int>(type: "INTEGER", nullable: false),
                    StopSequence = table.Column<int>(type: "INTEGER", nullable: false),
                    ReservationStatus = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadPlanReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadPlanReservations_LoadPlans_LoadPlanId",
                        column: x => x.LoadPlanId,
                        principalTable: "LoadPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadPlanReservations_PickingListHeaders_PickingListHeaderId",
                        column: x => x.PickingListHeaderId,
                        principalTable: "PickingListHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickingListLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PickingListHeaderId = table.Column<int>(type: "INTEGER", nullable: false),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    LineNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemCode = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    MaterialType = table.Column<string>(type: "TEXT", nullable: true),
                    Uom = table.Column<string>(type: "TEXT", nullable: true),
                    QtyOrdered = table.Column<int>(type: "INTEGER", nullable: false),
                    WeightOrderedLbs = table.Column<decimal>(type: "TEXT", nullable: false),
                    WidthIn = table.Column<decimal>(type: "TEXT", nullable: false),
                    LengthIn = table.Column<decimal>(type: "TEXT", nullable: false),
                    ReservedMaterialsJson = table.Column<string>(type: "TEXT", nullable: true),
                    LineInstructions = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingListLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingListLines_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickingListLines_PickingListHeaders_PickingListHeaderId",
                        column: x => x.PickingListHeaderId,
                        principalTable: "PickingListHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickPackTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    PickingListLineId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickPackTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickPackTasks_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickPackTasks_PickingListLines_PickingListLineId",
                        column: x => x.PickingListLineId,
                        principalTable: "PickingListLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    PickingListLineId = table.Column<int>(type: "INTEGER", nullable: true),
                    PlannedStartUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PlannedEndUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    MachineId = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_PickingListLines_PickingListLineId",
                        column: x => x.PickingListLineId,
                        principalTable: "PickingListLines",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserPasskeys_UserId",
                table: "AspNetUserPasskeys",
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
                name: "IX_AspNetUsers_DefaultBranchId",
                table: "AspNetUsers",
                column: "DefaultBranchId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_ActorUserId",
                table: "AuditEvents",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_BranchId",
                table: "AuditEvents",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchMemberships_BranchId",
                table: "BranchMemberships",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchMemberships_UserId_BranchId",
                table: "BranchMemberships",
                columns: new[] { "UserId", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchRoles_BranchId",
                table: "BranchRoles",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchRoles_UserId_BranchId_RoleName",
                table: "BranchRoles",
                columns: new[] { "UserId", "BranchId", "RoleName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErpShipmentRefs_BranchId_ErpPackingListNumber",
                table: "ErpShipmentRefs",
                columns: new[] { "BranchId", "ErpPackingListNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ErpShipmentRefs_ErpPackingListNumber",
                table: "ErpShipmentRefs",
                column: "ErpPackingListNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ErpShipmentRefs_PickingListHeaderId",
                table: "ErpShipmentRefs",
                column: "PickingListHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadPlanReservations_LoadPlanId_PickingListHeaderId",
                table: "LoadPlanReservations",
                columns: new[] { "LoadPlanId", "PickingListHeaderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadPlanReservations_PickingListHeaderId",
                table: "LoadPlanReservations",
                column: "PickingListHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadPlans_BranchId",
                table: "LoadPlans",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadPlans_PlannedDepartUtc",
                table: "LoadPlans",
                column: "PlannedDepartUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListHeaders_BranchId_PickingListNumber",
                table: "PickingListHeaders",
                columns: new[] { "BranchId", "PickingListNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingListHeaders_ShipDate",
                table: "PickingListHeaders",
                column: "ShipDate");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListLines_BranchId",
                table: "PickingListLines",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingListLines_PickingListHeaderId_LineNumber",
                table: "PickingListLines",
                columns: new[] { "PickingListHeaderId", "LineNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickPackTasks_BranchId",
                table: "PickPackTasks",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_PickPackTasks_PickingListLineId",
                table: "PickPackTasks",
                column: "PickingListLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AllowedDestinationBranchId",
                table: "Tags",
                column: "AllowedDestinationBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BranchId_TagNumber",
                table: "Tags",
                columns: new[] { "BranchId", "TagNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TagNumber",
                table: "Tags",
                column: "TagNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_BranchId",
                table: "WorkOrders",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_MachineId_PlannedStartUtc",
                table: "WorkOrders",
                columns: new[] { "MachineId", "PlannedStartUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_PickingListLineId",
                table: "WorkOrders",
                column: "PickingListLineId");
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
                name: "AspNetUserPasskeys");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditEvents");

            migrationBuilder.DropTable(
                name: "BranchMemberships");

            migrationBuilder.DropTable(
                name: "BranchRoles");

            migrationBuilder.DropTable(
                name: "ErpShipmentRefs");

            migrationBuilder.DropTable(
                name: "LoadPlanReservations");

            migrationBuilder.DropTable(
                name: "PickPackTasks");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "WorkOrders");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "LoadPlans");

            migrationBuilder.DropTable(
                name: "PickingListLines");

            migrationBuilder.DropTable(
                name: "PickingListHeaders");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
