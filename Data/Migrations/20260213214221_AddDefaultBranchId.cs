using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMetalsFulfillment.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultBranchId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultBranchId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchMemberships_UserId",
                table: "UserBranchMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchClaims_UserId",
                table: "UserBranchClaims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchClaims_AspNetUsers_UserId",
                table: "UserBranchClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBranchMemberships_AspNetUsers_UserId",
                table: "UserBranchMemberships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchClaims_AspNetUsers_UserId",
                table: "UserBranchClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBranchMemberships_AspNetUsers_UserId",
                table: "UserBranchMemberships");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchMemberships_UserId",
                table: "UserBranchMemberships");

            migrationBuilder.DropIndex(
                name: "IX_UserBranchClaims_UserId",
                table: "UserBranchClaims");

            migrationBuilder.DropColumn(
                name: "DefaultBranchId",
                table: "AspNetUsers");
        }
    }
}
