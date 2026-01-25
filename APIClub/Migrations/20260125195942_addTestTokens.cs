using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class addTestTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "FechaExpiracion", "anio", "monto" },
                values: new object[] { new DateOnly(2026, 2, 24), 2026, 5000.00m });

            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "FechaExpiracion",
                value: new DateOnly(2026, 2, 24));

            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "FechaExpiracion",
                value: new DateOnly(2026, 2, 24));

            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "FechaExpiracion",
                value: new DateOnly(2026, 2, 24));

            migrationBuilder.InsertData(
                table: "PaymentTokens",
                columns: new[] { "Id", "FechaExpiracion", "IdSocio", "PaymentStatus", "Preference_Id", "StatusDetail", "anio", "monto", "nombreSocio", "semestre", "usado" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), new DateOnly(2026, 2, 24), 3, null, null, null, 2026, 5000.00m, "Carlos Deudor", 1, false });

            migrationBuilder.InsertData(
                table: "Socios",
                columns: new[] { "Id", "Apellido", "Direcccion", "Dni", "FechaAsociacion", "FechaDeBaja", "IsActivo", "Localidad", "LoteId", "Nombre", "PreferenciaDePago", "Telefono" },
                values: new object[,]
                {
                    { 1, "Pérez", null, "111", new DateOnly(2024, 1, 1), null, true, null, null, "Juan", 1, null },
                    { 2, "Gómez", null, "222", new DateOnly(2025, 1, 1), null, true, null, null, "María", 1, null },
                    { 3, "Deudor", null, "333", new DateOnly(2024, 1, 1), null, true, null, null, "Carlos", 1, null }
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$2f1wsSKzPgrCrt1TineUGutCtrdpOQGw39goVm/wSHwgH9cmy02Jm");

            migrationBuilder.InsertData(
                table: "Cuotas",
                columns: new[] { "Id", "Anio", "FechaPago", "FormaDePago", "Monto", "Semestre", "SocioId" },
                values: new object[,]
                {
                    { 1, 2024, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2500m, 1, 1 },
                    { 2, 2024, new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2500m, 2, 1 },
                    { 3, 2025, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2500m, 1, 1 },
                    { 4, 2025, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2500m, 2, 1 },
                    { 5, 2025, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2500m, 1, 2 },
                    { 6, 2024, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2500m, 1, 3 },
                    { 7, 2024, new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2500m, 2, 3 },
                    { 8, 2025, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2500m, 1, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cuotas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Socios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Socios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Socios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "FechaExpiracion", "anio", "monto" },
                values: new object[] { new DateOnly(2026, 2, 23), 2025, 2500.00m });

            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "FechaExpiracion",
                value: new DateOnly(2026, 2, 23));

            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "FechaExpiracion",
                value: new DateOnly(2026, 2, 23));

            migrationBuilder.UpdateData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "FechaExpiracion",
                value: new DateOnly(2026, 2, 23));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$eiq0ojKVi/ogGtQ33UFfUOOlJrD2TATNOxzQ7xQkpFZZLwqTU.hZa");
        }
    }
}
