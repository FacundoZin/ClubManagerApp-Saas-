using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class updateNullabilidadInVarianteViaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoDeButaca",
                table: "VariantesViaje",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Regimen",
                table: "VariantesViaje",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$LDLGC9.fiYAlSuwt6f//Fel7Vf3JxNn.pGJrMW5c2svp1lz4ps022");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoDeButaca",
                table: "VariantesViaje",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Regimen",
                table: "VariantesViaje",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$DAGAk6PTu4fGhD0MuPeeYeGvPjFrtYsy39sp94w7K9mYcPLFE8X32");
        }
    }
}
