using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class AddEditViajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PagosInscriptosViajeAudit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InscriptoViajeId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioNombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    MontoAnterior = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MontoNuevo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Diferencia = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosInscriptosViajeAudit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosInscriptosViajeAudit_Inscriptos_InscriptoViajeId",
                        column: x => x.InscriptoViajeId,
                        principalTable: "Inscriptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$x7NRaxkTgH7batkORsKtaeIzvyEr1hfsrn1tgo9DUvmvKwJXaDjKu");

            migrationBuilder.CreateIndex(
                name: "IX_PagosInscriptosViajeAudit_InscriptoViajeId",
                table: "PagosInscriptosViajeAudit",
                column: "InscriptoViajeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosInscriptosViajeAudit");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$RZb.Qa3jBHj9Rcu3d2b1OO1V89nUz4nSQh4MUvGq7ANNe6GuTb1UC");
        }
    }
}
