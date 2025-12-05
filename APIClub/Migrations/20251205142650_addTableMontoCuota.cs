using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class addTableMontoCuota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Trimestre",
                table: "Cuotas",
                newName: "Semestre");

            migrationBuilder.CreateTable(
                name: "MontoCuota",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MontoCuotaFija = table.Column<decimal>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MontoCuota", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MontoCuota");

            migrationBuilder.RenameColumn(
                name: "Semestre",
                table: "Cuotas",
                newName: "Trimestre");
        }
    }
}
