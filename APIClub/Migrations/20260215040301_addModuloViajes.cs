using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class addModuloViajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Viajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dias = table.Column<int>(type: "integer", nullable: false),
                    Noches = table.Column<int>(type: "integer", nullable: false),
                    Fechasalida = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PorcentajeComision = table.Column<decimal>(type: "numeric", nullable: false),
                    VentasParaLiberado = table.Column<int>(type: "integer", nullable: true),
                    ValorBase = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viajes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VariantesViaje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreVariante = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ValorViaje = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorSeña = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IdViaje = table.Column<int>(type: "integer", nullable: false),
                    Regimen = table.Column<int>(type: "integer", nullable: false),
                    TipoDeButaca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantesViaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantesViaje_Viajes_IdViaje",
                        column: x => x.IdViaje,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inscriptos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VarianteViajeId = table.Column<int>(type: "integer", nullable: false),
                    SocioId = table.Column<int>(type: "integer", nullable: false),
                    montoAbonado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MontoPendiente = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    cancelado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscriptos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscriptos_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inscriptos_VariantesViaje_VarianteViajeId",
                        column: x => x.VarianteViajeId,
                        principalTable: "VariantesViaje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$DAGAk6PTu4fGhD0MuPeeYeGvPjFrtYsy39sp94w7K9mYcPLFE8X32");

            migrationBuilder.CreateIndex(
                name: "IX_Inscriptos_SocioId",
                table: "Inscriptos",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscriptos_VarianteViajeId",
                table: "Inscriptos",
                column: "VarianteViajeId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantesViaje_IdViaje",
                table: "VariantesViaje",
                column: "IdViaje");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inscriptos");

            migrationBuilder.DropTable(
                name: "VariantesViaje");

            migrationBuilder.DropTable(
                name: "Viajes");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$.LHdrgabAcZ61XkEINzaIukiTuBaA94SHzAJo7d8rD8VjwkXDIovW");
        }
    }
}
