using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class changeDecimalToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrecioAlquiler",
                value: 10500);

            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 2,
                column: "PrecioAlquiler",
                value: 8000);

            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 3,
                column: "PrecioAlquiler",
                value: 5000);

            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 4,
                column: "PrecioAlquiler",
                value: 4500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 1,
                column: "PrecioAlquiler",
                value: 1500.00m);

            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 2,
                column: "PrecioAlquiler",
                value: 800.00m);

            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 3,
                column: "PrecioAlquiler",
                value: 500.00m);

            migrationBuilder.UpdateData(
                table: "Articulos",
                keyColumn: "Id",
                keyValue: 4,
                column: "PrecioAlquiler",
                value: 300.00m);
        }
    }
}
