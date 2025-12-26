using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class replaceClient_secretForPreferenceIdOnPaymentTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Preference_Id",
                table: "PaymentTokens",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nombreSocio",
                table: "PaymentTokens",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "PaymentTokens",
                columns: new[] { "Id", "FechaExpiracion", "IdSocio", "Preference_Id", "anio", "monto", "nombreSocio", "semestre", "usado" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateOnly(2026, 1, 25), 1, null, 2025, 2500.00m, "Juan Pérez", 1, false },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateOnly(2026, 1, 25), 1, null, 2025, 2500.00m, "Juan Pérez", 2, false },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateOnly(2026, 1, 25), 2, null, 2025, 2500.00m, "María Gómez", 1, false },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateOnly(2026, 1, 25), 2, null, 2025, 2500.00m, "María Gómez", 2, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "PaymentTokens",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DropColumn(
                name: "Preference_Id",
                table: "PaymentTokens");

            migrationBuilder.DropColumn(
                name: "nombreSocio",
                table: "PaymentTokens");
        }
    }
}
