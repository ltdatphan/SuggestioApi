using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SuggestioApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3b9ea3e3-fad8-47b6-95c3-1b59e138d471");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "65c944b4-2659-4c29-8fd6-34d345add72b");

            migrationBuilder.RenameIndex(
                name: "IX_Items_ListId",
                table: "Items",
                newName: "IX_Item_ListId");

            migrationBuilder.RenameIndex(
                name: "IX_CuratedLists_OwnerId",
                table: "CuratedLists",
                newName: "IX_List_OwnerId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21e4daa0-183f-4955-b2e5-302d22297a5b", null, "User", "USER" },
                    { "5ab22398-f764-49f6-a4c2-10f90afd33d0", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21e4daa0-183f-4955-b2e5-302d22297a5b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ab22398-f764-49f6-a4c2-10f90afd33d0");

            migrationBuilder.RenameIndex(
                name: "IX_Item_ListId",
                table: "Items",
                newName: "IX_Items_ListId");

            migrationBuilder.RenameIndex(
                name: "IX_List_OwnerId",
                table: "CuratedLists",
                newName: "IX_CuratedLists_OwnerId");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3b9ea3e3-fad8-47b6-95c3-1b59e138d471", null, "Admin", "ADMIN" },
                    { "65c944b4-2659-4c29-8fd6-34d345add72b", null, "User", "USER" }
                });
        }
    }
}
