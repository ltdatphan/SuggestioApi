using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SuggestioApi.Migrations
{
    /// <inheritdoc />
    public partial class DeployModelToFly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4ff87720-069a-40e9-a8bb-21db443a06fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c10c6aed-adb6-40cd-8918-0fecef3897e6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "62999c36-895e-4683-95f9-cd23657e2ad0", null, "Admin", "ADMIN" },
                    { "85147e87-81c1-4953-ba94-1b82fa86eb9a", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "62999c36-895e-4683-95f9-cd23657e2ad0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "85147e87-81c1-4953-ba94-1b82fa86eb9a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4ff87720-069a-40e9-a8bb-21db443a06fe", null, "Admin", "ADMIN" },
                    { "c10c6aed-adb6-40cd-8918-0fecef3897e6", null, "User", "USER" }
                });
        }
    }
}
