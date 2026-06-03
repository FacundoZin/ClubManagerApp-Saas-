using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class RefactorizarModuloViajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "montoAbonado",
                table: "Inscriptos",
                newName: "MontoAbonado");

            migrationBuilder.RenameColumn(
                name: "cancelado",
                table: "Inscriptos",
                newName: "Cancelado");

            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Inscriptos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Inscriptos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroFile",
                table: "Inscriptos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Inscriptos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Migrate data from Socios before dropping SocioId
            migrationBuilder.Sql("UPDATE \"Inscriptos\" SET \"Nombre\" = s.\"Nombre\", \"Apellido\" = s.\"Apellido\", \"Telefono\" = COALESCE(s.\"Telefono\", ''), \"NumeroFile\" = 'LEGACY-' || \"Inscriptos\".\"Id\" FROM \"Socios\" s WHERE \"Inscriptos\".\"SocioId\" = s.\"Id\";");

            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptos_Socios_SocioId",
                table: "Inscriptos");

            migrationBuilder.DropIndex(
                name: "IX_Inscriptos_SocioId",
                table: "Inscriptos");

            migrationBuilder.DropColumn(
                name: "SocioId",
                table: "Inscriptos");

            migrationBuilder.CreateTable(
                name: "PagosInscriptosViaje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InscriptoViajeId = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NumeroRecibo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosInscriptosViaje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosInscriptosViaje_Inscriptos_InscriptoViajeId",
                        column: x => x.InscriptoViajeId,
                        principalTable: "Inscriptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Seed initial payment history for existing inscriptos
            migrationBuilder.Sql("INSERT INTO \"PagosInscriptosViaje\" (\"InscriptoViajeId\", \"Monto\", \"FechaPago\", \"NumeroRecibo\") SELECT \"Id\", \"MontoAbonado\", NOW(), 'ENTREGA-INICIAL' FROM \"Inscriptos\" WHERE \"MontoAbonado\" > 0;");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$RZb.Qa3jBHj9Rcu3d2b1OO1V89nUz4nSQh4MUvGq7ANNe6GuTb1UC");

            migrationBuilder.CreateIndex(
                name: "IX_PagosInscriptosViaje_InscriptoViajeId",
                table: "PagosInscriptosViaje",
                column: "InscriptoViajeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosInscriptosViaje");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Inscriptos");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Inscriptos");

            migrationBuilder.DropColumn(
                name: "NumeroFile",
                table: "Inscriptos");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Inscriptos");

            migrationBuilder.RenameColumn(
                name: "MontoAbonado",
                table: "Inscriptos",
                newName: "montoAbonado");

            migrationBuilder.RenameColumn(
                name: "Cancelado",
                table: "Inscriptos",
                newName: "cancelado");

            migrationBuilder.AddColumn<int>(
                name: "SocioId",
                table: "Inscriptos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$LDLGC9.fiYAlSuwt6f//Fel7Vf3JxNn.pGJrMW5c2svp1lz4ps022");

            migrationBuilder.CreateIndex(
                name: "IX_Inscriptos_SocioId",
                table: "Inscriptos",
                column: "SocioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptos_Socios_SocioId",
                table: "Inscriptos",
                column: "SocioId",
                principalTable: "Socios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
