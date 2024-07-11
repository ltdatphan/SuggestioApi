using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SuggestioApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFollowModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowedId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "215abd78-a3ac-4cfc-a678-090acadff47b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "517233db-0b30-4218-92ef-46c09dc41d2d");

            migrationBuilder.RenameColumn(
                name: "FollowedId",
                table: "Follows",
                newName: "TargetUserId");

            migrationBuilder.RenameColumn(
                name: "FollowerId",
                table: "Follows",
                newName: "CurrentUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowerId_FollowedId",
                table: "Follows",
                newName: "IX_Follows_CurrentUserId_TargetUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_FollowedId",
                table: "Follows",
                newName: "IX_Follows_TargetUserId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0389e673-3fa9-4577-8050-f7495a1d48c4", null, "User", "USER" },
                    { "f7355cda-ed71-44b6-a6e4-1764e3cd95a0", null, "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_CurrentUserId",
                table: "Follows",
                column: "CurrentUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_TargetUserId",
                table: "Follows",
                column: "TargetUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_CurrentUserId",
                table: "Follows");

            migrationBuilder.DropForeignKey(
                name: "FK_Follows_AspNetUsers_TargetUserId",
                table: "Follows");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0389e673-3fa9-4577-8050-f7495a1d48c4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f7355cda-ed71-44b6-a6e4-1764e3cd95a0");

            migrationBuilder.RenameColumn(
                name: "TargetUserId",
                table: "Follows",
                newName: "FollowedId");

            migrationBuilder.RenameColumn(
                name: "CurrentUserId",
                table: "Follows",
                newName: "FollowerId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_TargetUserId",
                table: "Follows",
                newName: "IX_Follows_FollowedId");

            migrationBuilder.RenameIndex(
                name: "IX_Follows_CurrentUserId_TargetUserId",
                table: "Follows",
                newName: "IX_Follows_FollowerId_FollowedId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "215abd78-a3ac-4cfc-a678-090acadff47b", null, "User", "USER" },
                    { "517233db-0b30-4218-92ef-46c09dc41d2d", null, "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowedId",
                table: "Follows",
                column: "FollowedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Follows_AspNetUsers_FollowerId",
                table: "Follows",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
