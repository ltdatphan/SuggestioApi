using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SuggestioApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRatingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c668784c-75df-4984-a59f-3c199d6fec4b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f78f3d34-4a47-4e25-80d3-b12b1056fce0");

            migrationBuilder.AlterColumn<float>(
                name: "Rating",
                table: "Items",
                type: "real",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4ff87720-069a-40e9-a8bb-21db443a06fe", null, "Admin", "ADMIN" },
                    { "c10c6aed-adb6-40cd-8918-0fecef3897e6", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4ff87720-069a-40e9-a8bb-21db443a06fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c10c6aed-adb6-40cd-8918-0fecef3897e6");

            migrationBuilder.AlterColumn<short>(
                name: "Rating",
                table: "Items",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "c668784c-75df-4984-a59f-3c199d6fec4b", null, "User", "USER" },
                    { "f78f3d34-4a47-4e25-80d3-b12b1056fce0", null, "Admin", "ADMIN" }
                });
        }
    }
}
