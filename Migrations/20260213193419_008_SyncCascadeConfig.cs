using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class _008_SyncCascadeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingFsaRules_Branches_BranchId",
                table: "ShippingFsaRules");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingFsaRules_ShippingRegions_ShippingRegionId",
                table: "ShippingFsaRules");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingGroups_Branches_BranchId",
                table: "ShippingGroups");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingFsaRules_Branches_BranchId",
                table: "ShippingFsaRules",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingFsaRules_ShippingRegions_ShippingRegionId",
                table: "ShippingFsaRules",
                column: "ShippingRegionId",
                principalTable: "ShippingRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingGroups_Branches_BranchId",
                table: "ShippingGroups",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingFsaRules_Branches_BranchId",
                table: "ShippingFsaRules");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingFsaRules_ShippingRegions_ShippingRegionId",
                table: "ShippingFsaRules");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingGroups_Branches_BranchId",
                table: "ShippingGroups");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingFsaRules_Branches_BranchId",
                table: "ShippingFsaRules",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingFsaRules_ShippingRegions_ShippingRegionId",
                table: "ShippingFsaRules",
                column: "ShippingRegionId",
                principalTable: "ShippingRegions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingGroups_Branches_BranchId",
                table: "ShippingGroups",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "BranchId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
