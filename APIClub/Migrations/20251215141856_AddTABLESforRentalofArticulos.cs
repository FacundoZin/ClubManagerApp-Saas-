using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class AddTABLESforRentalofArticulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Dni",
                table: "Socios",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "alquileresArticulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaAlquiler = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IdSocio = table.Column<int>(type: "INTEGER", nullable: false),
                    Finalizado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alquileresArticulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alquileresArticulos_Socios_IdSocio",
                        column: x => x.IdSocio,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemALquiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ArticuloId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlquilerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemALquiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemALquiler_Articulos_ArticuloId",
                        column: x => x.ArticuloId,
                        principalTable: "Articulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemALquiler_alquileresArticulos_AlquilerId",
                        column: x => x.AlquilerId,
                        principalTable: "alquileresArticulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagosAlquilerDeArticulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Anio = table.Column<int>(type: "INTEGER", nullable: false),
                    Mes = table.Column<int>(type: "INTEGER", nullable: false),
                    Monto = table.Column<int>(type: "INTEGER", nullable: false),
                    IdAlquiler = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosAlquilerDeArticulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosAlquilerDeArticulos_alquileresArticulos_IdAlquiler",
                        column: x => x.IdAlquiler,
                        principalTable: "alquileresArticulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "alquileresArticulos",
                columns: new[] { "Id", "FechaAlquiler", "Finalizado", "IdSocio", "Observaciones" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Alquiler por 3 días" },
                    { 2, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "Préstamo semanal" },
                    { 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Rehabilitación post operación" },
                    { 4, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "Alquiler viejo ya cerrado" }
                });

            migrationBuilder.InsertData(
                table: "ItemALquiler",
                columns: new[] { "Id", "AlquilerId", "ArticuloId", "Cantidad" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 1, 4, 2 },
                    { 3, 2, 2, 1 },
                    { 4, 3, 3, 2 },
                    { 5, 3, 4, 1 },
                    { 6, 4, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "PagosAlquilerDeArticulos",
                columns: new[] { "Id", "Anio", "IdAlquiler", "Mes", "Monto" },
                values: new object[,]
                {
                    { 1, 2025, 1, 1, 10500 },
                    { 2, 2025, 1, 2, 10500 },
                    { 3, 2025, 1, 3, 10500 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_alquileresArticulos_IdSocio",
                table: "alquileresArticulos",
                column: "IdSocio");

            migrationBuilder.CreateIndex(
                name: "IX_ItemALquiler_AlquilerId",
                table: "ItemALquiler",
                column: "AlquilerId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemALquiler_ArticuloId",
                table: "ItemALquiler",
                column: "ArticuloId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosAlquilerDeArticulos_IdAlquiler_Anio_Mes",
                table: "PagosAlquilerDeArticulos",
                columns: new[] { "IdAlquiler", "Anio", "Mes" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemALquiler");

            migrationBuilder.DropTable(
                name: "PagosAlquilerDeArticulos");

            migrationBuilder.DropTable(
                name: "alquileresArticulos");

            migrationBuilder.AlterColumn<string>(
                name: "Dni",
                table: "Socios",
                type: "TEXT",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20);
        }
    }
}
