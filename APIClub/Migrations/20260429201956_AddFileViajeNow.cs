using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APIClub.Migrations
{
    public partial class AddFileViajeNow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ---------------------------
            // TABLA FileViajes               
            // ---------------------------
            migrationBuilder.CreateTable(
                name: "FileViajes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),

                    NumeroFile = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),

                    ViajeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileViajes", x => x.Id);

                    table.ForeignKey(
                        name: "FK_FileViajes_Viajes_ViajeId",
                        column: x => x.ViajeId,
                        principalTable: "Viajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileViajes_ViajeId",
                table: "FileViajes",
                column: "ViajeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileViajes_NumeroFile_ViajeId",
                table: "FileViajes",
                columns: new[] { "NumeroFile", "ViajeId" },
                unique: true);

            // ---------------------------
            // COLUMNA EN Inscriptos
            // ---------------------------
            migrationBuilder.AddColumn<int>(
                name: "FileViajeId",
                table: "Inscriptos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscriptos_FileViajeId",
                table: "Inscriptos",
                column: "FileViajeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptos_FileViajes_FileViajeId",
                table: "Inscriptos",
                column: "FileViajeId",
                principalTable: "FileViajes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptos_FileViajes_FileViajeId",
                table: "Inscriptos");

            migrationBuilder.DropIndex(
                name: "IX_Inscriptos_FileViajeId",
                table: "Inscriptos");

            migrationBuilder.DropColumn(
                name: "FileViajeId",
                table: "Inscriptos");

            migrationBuilder.DropTable(
                name: "FileViajes");
        }
    }
}
