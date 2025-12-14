using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class AgregarArticulosOrtopedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PrecioAlquiler = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Articulos",
                columns: new[] { "Id", "Nombre", "PrecioAlquiler" },
                values: new object[,]
                {
                    { 1, "Silla de Ruedas", 1500.00m },
                    { 2, "Andador", 800.00m },
                    { 3, "Muletas", 500.00m },
                    { 4, "Bastón", 300.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articulos");
        }
    }
}
