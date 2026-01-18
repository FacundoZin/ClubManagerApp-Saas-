using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIClub.Migrations
{
    /// <inheritdoc />
    public partial class addUsers : Migration
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
                    PrecioAlquiler = table.Column<int>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreLote = table.Column<string>(type: "TEXT", nullable: false),
                    CalleNorte = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CalleSur = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CalleEste = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CalleOeste = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MontoCuota",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MontoCuotaFija = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MontoCuota", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    nombreSocio = table.Column<string>(type: "TEXT", nullable: false),
                    IdSocio = table.Column<int>(type: "INTEGER", nullable: false),
                    anio = table.Column<int>(type: "INTEGER", nullable: false),
                    semestre = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaExpiracion = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    monto = table.Column<decimal>(type: "TEXT", nullable: false),
                    usado = table.Column<bool>(type: "INTEGER", nullable: false),
                    Preference_Id = table.Column<string>(type: "TEXT", nullable: true),
                    PaymentStatus = table.Column<string>(type: "TEXT", nullable: true),
                    StatusDetail = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreUsuario = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Socios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Dni = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true),
                    Direcccion = table.Column<string>(type: "TEXT", nullable: true),
                    Localidad = table.Column<string>(type: "TEXT", nullable: true),
                    PreferenciaDePago = table.Column<int>(type: "INTEGER", nullable: false),
                    LoteId = table.Column<int>(type: "INTEGER", nullable: true),
                    FechaAsociacion = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    IsActivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaDeBaja = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Socios_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

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
                name: "Cuotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FormaDePago = table.Column<int>(type: "INTEGER", nullable: false),
                    Anio = table.Column<int>(type: "INTEGER", nullable: false),
                    Semestre = table.Column<int>(type: "INTEGER", nullable: false),
                    SocioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuotas_Socios_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservasSalones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", nullable: false),
                    FechaAlquiler = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsCancelled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SocioId = table.Column<int>(type: "INTEGER", nullable: false),
                    SalonId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasSalones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasSalones_Salones_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservasSalones_Socios_SocioId",
                        column: x => x.SocioId,
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

            migrationBuilder.CreateTable(
                name: "pagoReservaSalon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReservaSalonId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pagoReservaSalon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pagoReservaSalon_ReservasSalones_ReservaSalonId",
                        column: x => x.ReservaSalonId,
                        principalTable: "ReservasSalones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Articulos",
                columns: new[] { "Id", "Nombre", "PrecioAlquiler" },
                values: new object[,]
                {
                    { 1, "Silla de Ruedas", 10500 },
                    { 2, "Andador", 8000 },
                    { 3, "Muletas", 5000 },
                    { 4, "Bastón", 4500 }
                });

            migrationBuilder.InsertData(
                table: "Lotes",
                columns: new[] { "Id", "CalleEste", "CalleNorte", "CalleOeste", "CalleSur", "NombreLote" },
                values: new object[,]
                {
                    { 1, "Belgrano", "Mitre", "Urquiza", "San Martín", "Lote A" },
                    { 2, "Salta", "Av. Pellegrini", "Jujuy", "Bv. Oroño", "Lote B" },
                    { 3, "Lavalle", "Av. Corrientes", "Sarmiento", "Florida", "Lote C" }
                });

            migrationBuilder.InsertData(
                table: "MontoCuota",
                columns: new[] { "Id", "FechaActualizacion", "MontoCuotaFija" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2500.00m });

            migrationBuilder.InsertData(
                table: "PaymentTokens",
                columns: new[] { "Id", "FechaExpiracion", "IdSocio", "PaymentStatus", "Preference_Id", "StatusDetail", "anio", "monto", "nombreSocio", "semestre", "usado" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateOnly(2026, 2, 17), 1, null, null, null, 2025, 2500.00m, "Juan Pérez", 1, false },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateOnly(2026, 2, 17), 1, null, null, null, 2025, 2500.00m, "Juan Pérez", 2, false },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateOnly(2026, 2, 17), 2, null, null, null, 2025, 2500.00m, "María Gómez", 1, false },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateOnly(2026, 2, 17), 2, null, null, null, 2025, 2500.00m, "María Gómez", 2, false }
                });

            migrationBuilder.InsertData(
                table: "Salones",
                columns: new[] { "Id", "Direccion", "Name" },
                values: new object[,]
                {
                    { 1, "Calle Falsa 123", "Salón Central" },
                    { 2, "Av. Siempre Viva 742", "Salón Norte" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "FechaCreacion", "NombreUsuario", "PasswordHash", "Rol", "UltimoAcceso" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin", "$2a$11$70MiU/uEc.4l6c/ZJT1r4.853AC/g6hGV9IzsQb.u46dTMAVK6o7u", 1, null });

            migrationBuilder.InsertData(
                table: "Socios",
                columns: new[] { "Id", "Apellido", "Direcccion", "Dni", "FechaAsociacion", "FechaDeBaja", "IsActivo", "Localidad", "LoteId", "Nombre", "PreferenciaDePago", "Telefono" },
                values: new object[,]
                {
                    { 1, "Pérez", "Mitre 100", "12345678", new DateOnly(2020, 5, 10), null, true, "Rosario", 1, "Juan", 0, "341-1234567" },
                    { 2, "Gómez", "San Martín 200", "87654321", new DateOnly(2021, 3, 15), null, true, "Córdoba", 2, "María", 2, "341-7654321" },
                    { 3, "Ruiz", "Belgrano 500", "11223344", new DateOnly(2022, 1, 10), null, true, "Rosario", 3, "Carlos", 1, "341-9988776" }
                });

            migrationBuilder.InsertData(
                table: "Cuotas",
                columns: new[] { "Id", "Anio", "FechaPago", "FormaDePago", "Monto", "Semestre", "SocioId" },
                values: new object[,]
                {
                    { 1, 2024, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2500.00m, 1, 1 },
                    { 2, 2024, new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2500.00m, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "ReservasSalones",
                columns: new[] { "Id", "FechaAlquiler", "Importe", "IsCancelled", "SalonId", "SocioId", "Titulo", "TotalPagado" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000.00m, false, 1, 1, "fiesta de 15 cele", 4000.00m },
                    { 2, new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 7000.00m, false, 2, 2, "baile abuelos", 7000.00m },
                    { 3, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 6000.00m, false, 1, 3, "Cumpleaños de Carlos", 0.00m },
                    { 4, new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 8000.00m, false, 2, 1, "Reunión Familiar", 3000.00m }
                });

            migrationBuilder.InsertData(
                table: "alquileresArticulos",
                columns: new[] { "Id", "FechaAlquiler", "Finalizado", "IdSocio", "Observaciones" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Alquiler por 3 días" },
                    { 2, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "Préstamo semanal" },
                    { 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Rehabilitación post operación" },
                    { 4, new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, 2, "Alquiler viejo ya cerrado" },
                    { 5, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 3, "Alquiler Activo para Test" }
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
                    { 6, 4, 1, 1 },
                    { 7, 5, 1, 1 }
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

            migrationBuilder.InsertData(
                table: "pagoReservaSalon",
                columns: new[] { "Id", "FechaPago", "ReservaSalonId", "monto" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2000.00m },
                    { 2, new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2000.00m },
                    { 3, new DateTime(2026, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3000.00m },
                    { 4, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2000.00m },
                    { 5, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2000.00m },
                    { 6, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 3000.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_alquileresArticulos_IdSocio",
                table: "alquileresArticulos",
                column: "IdSocio");

            migrationBuilder.CreateIndex(
                name: "IX_Cuotas_SocioId",
                table: "Cuotas",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemALquiler_AlquilerId",
                table: "ItemALquiler",
                column: "AlquilerId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemALquiler_ArticuloId",
                table: "ItemALquiler",
                column: "ArticuloId");

            migrationBuilder.CreateIndex(
                name: "IX_pagoReservaSalon_ReservaSalonId",
                table: "pagoReservaSalon",
                column: "ReservaSalonId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosAlquilerDeArticulos_IdAlquiler_Anio_Mes",
                table: "PagosAlquilerDeArticulos",
                columns: new[] { "IdAlquiler", "Anio", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservasSalones_SalonId",
                table: "ReservasSalones",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasSalones_SocioId",
                table: "ReservasSalones",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Socios_LoteId",
                table: "Socios",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cuotas");

            migrationBuilder.DropTable(
                name: "ItemALquiler");

            migrationBuilder.DropTable(
                name: "MontoCuota");

            migrationBuilder.DropTable(
                name: "pagoReservaSalon");

            migrationBuilder.DropTable(
                name: "PagosAlquilerDeArticulos");

            migrationBuilder.DropTable(
                name: "PaymentTokens");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Articulos");

            migrationBuilder.DropTable(
                name: "ReservasSalones");

            migrationBuilder.DropTable(
                name: "alquileresArticulos");

            migrationBuilder.DropTable(
                name: "Salones");

            migrationBuilder.DropTable(
                name: "Socios");

            migrationBuilder.DropTable(
                name: "Lotes");
        }
    }
}
