using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.Enums;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.ModuloGestionCobradores.Models;
using APIClub.Domain.ModuloGestionCuotas.Models;
using APIClub.Domain.ModuloGestionViajes.Models;
using APIClub.Domain.ReservasSalones.Models;
using APIClub.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Data
{
    public class DatabaseSeeder : IDataSeeder
    {
        private readonly AppDbcontext _appDbcontext;
        private readonly Random _random = new Random();

        public DatabaseSeeder(AppDbcontext context)
        {
            _appDbcontext = context;
        }

        public async Task seedTestDataAsync()
        {
            Console.WriteLine(">>> DatabaseSeeder starting...");

            // 1. Limpiar datos existentes
            Console.WriteLine(">>> Clearing existing data using Raw SQL...");

            try
            {
                // En PostgreSQL, desactivar triggers de FKs temporalmente o usar TRUNCATE CASCADE
                await _appDbcontext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';");

                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Cuotas\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RegistroCobradores\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PagosAlquilerDeArticulos\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ItemALquiler\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"alquileresArticulos\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"pagoReservaSalon\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"ReservasSalones\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Socios\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Articulos\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Lotes\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Salones\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"MontoCuota\" RESTART IDENTITY CASCADE;");

                await _appDbcontext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Error clearing data: {ex.Message}");
                throw;
            }

            Console.WriteLine(">>> Seeding data...");

            // 2. Sembrar Salones
            var salones = new List<Salon>
            {
                new Salon { Name = "Salón de Usos Múltiples (SUM)", Direccion = "Av. Principal 123, Planta Baja" },
                new Salon { Name = "Quincho Familiar 'Los Abuelos'", Direccion = "Calle Secundaria 456, Sector Norte" },
                new Salon { Name = "Terraza 'Vista Alegre'", Direccion = "Av. Costanera 789, Piso 2" }
            };
            await _appDbcontext.Salones.AddRangeAsync(salones);

            // 3. Sembrar Lotes
            var lotes = new List<Lote>
            {
                new Lote { NombreLote = "Zona Centro", CalleNorte = "Belgrano", CalleSur = "San Martín", CalleEste = "Mitre", CalleOeste = "Rivadavia" },
                new Lote { NombreLote = "Barrio Norte", CalleNorte = "9 de Julio", CalleSur = "Suipacha", CalleEste = "Esmeralda", CalleOeste = "Maipú" },
                new Lote { NombreLote = "Altos de la Villa", CalleNorte = "Pueyrredón", CalleSur = "Larrea", CalleEste = "Azcuénaga", CalleOeste = "Junín" },
                new Lote { NombreLote = "Barrio Sur", CalleNorte = "Moreno", CalleSur = "Piedras", CalleEste = "Chile", CalleOeste = "Independencia" }
            };
            await _appDbcontext.Lotes.AddRangeAsync(lotes);

            // 4. Sembrar Artículos
            var articulos = new List<Articulo>
            {
                new Articulo { Nombre = "Tablón 2mts", PrecioAlquiler = 1500 },
                new Articulo { Nombre = "Silla Plástica Blanca", PrecioAlquiler = 300 },
                new Articulo { Nombre = "Mantel Rectangular Blanco", PrecioAlquiler = 800 },
                new Articulo { Nombre = "Juego de Vajilla (10 pers)", PrecioAlquiler = 2500 },
                new Articulo { Nombre = "Equipo de Sonido Portátil", PrecioAlquiler = 5000 },
                new Articulo { Nombre = "Parrilla Móvil", PrecioAlquiler = 3500 }
            };
            await _appDbcontext.Articulos.AddRangeAsync(articulos);

            // 5. Sembrar MontoCuota
            var montosCuota = new List<MontoCuota>
            {
                new MontoCuota { MontoCuotaFija = 3000.00m, FechaActualizacion = new DateTime(2023, 1, 1) },
                new MontoCuota { MontoCuotaFija = 5000.00m, FechaActualizacion = new DateTime(2024, 7, 1) },
                new MontoCuota { MontoCuotaFija = 8000.00m, FechaActualizacion = new DateTime(2025, 7, 1) }
            };
            await _appDbcontext.MontoCuota.AddRangeAsync(montosCuota);

            // 6. Sembrar Socios (Pool grande para analíticas)
            var nombres = new[] { "Juan", "Ana", "Carlos", "Marta", "Roberto", "Lucía", "Pedro", "Elena", "Ricardo", "Silvia", "Jorge", "Claudia", "Diego", "Patricia", "Miguel", "Sonia", "Raúl", "Beatriz", "Oscar", "Alicia" };
            var apellidos = new[] { "García", "López", "Martínez", "Rodríguez", "Sánchez", "Pérez", "Gómez", "Fernández", "Díaz", "Álvarez", "Torres", "Ruiz", "Romero", "Herrera", "Castro", "Medina", "Pereyra", "González", "Vázquez", "Luna" };

            var socios = new List<Socio>();
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var fechaInicioSocio = new DateOnly(2023, 1, 1);
            var totalDias = (hoy.ToDateTime(TimeOnly.MinValue) - fechaInicioSocio.ToDateTime(TimeOnly.MinValue)).Days;

            // Generar un conjunto predefinido de 50 direcciones para forzar la coincidencia de domicilios (grupos familiares)
            var direccionesPool = new List<string>();
            for (int d = 0; d < 50; d++)
            {
                direccionesPool.Add($"Calle {_random.Next(1, 40)} {_random.Next(100, 1500)}");
            }

            for (int i = 0; i < 350; i++)
            {
                var nombre = nombres[_random.Next(nombres.Length)];
                var apellido = apellidos[_random.Next(apellidos.Length)];
                var fechaAsociacion = fechaInicioSocio.AddDays(_random.Next(totalDias));

                DateTime fechaBajaDt = fechaAsociacion.ToDateTime(TimeOnly.MinValue).AddMonths(_random.Next(6, 24));
                DateOnly? fechaBaja = null;
                bool isActivo = true;

                // Simular bajas (~15%)
                if (i % 7 == 0 && fechaBajaDt < DateTime.Today)
                {
                    fechaBaja = DateOnly.FromDateTime(fechaBajaDt);
                    isActivo = false;
                }

                // Generar mayormente con preferencia Cobrador (~75%) para tener suficientes deudores por lote
                var preferencia = _random.Next(100) < 75 ? MetodosDePago.Cobrador : (MetodosDePago)_random.Next(1, 3);

                // Asignar una dirección del pool para asegurar que varios socios vivan en el mismo lugar
                var direccionElegida = direccionesPool[_random.Next(direccionesPool.Count)];

                socios.Add(new Socio
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Dni = (20000000 + i * 1543).ToString(),
                    Telefono = "11" + _random.Next(40000000, 69999999).ToString(),
                    Direcccion = direccionElegida,
                    Localidad = "Pilar",
                    PreferenciaDePago = preferencia, // Cobrador, LinkDePago, Sede
                    FechaAsociacion = fechaAsociacion,
                    IsActivo = isActivo,
                    FechaDeBaja = fechaBaja,
                    Lote = lotes[_random.Next(lotes.Count)]
                });
            }
            await _appDbcontext.Socios.AddRangeAsync(socios);
            await _appDbcontext.SaveChangesAsync(); // Guardamos para tener los IDs

            // 7. Sembrar Cuotas e Historial de Cobradores
            var cuotas = new List<Cuota>();
            var registrosCobradores = new List<RegistroCobrador>();

            foreach (var socio in socios)
            {
                // Generar cuotas desde su alta hasta hoy o su baja
                var fechaFin = socio.FechaDeBaja ?? hoy;
                var currentYear = socio.FechaAsociacion.Year;
                var currentSemestre = socio.FechaAsociacion.Month <= 6 ? 1 : 2;

                while (currentYear < fechaFin.Year || (currentYear == fechaFin.Year && (currentSemestre == 1 || (currentSemestre == 2 && fechaFin.Month >= 7))))
                {
                    // Determinar monto según el año/semestre
                    decimal monto = 3000;
                    if (currentYear == 2024 && currentSemestre == 2) monto = 5000;
                    if (currentYear >= 2025) monto = 8000;

                    // Decidir si pagó (80% probabilidad si es activo, 100% si es baja previa)
                    bool pago = socio.IsActivo ? (_random.Next(100) < 85) : true;

                    if (pago)
                    {
                        var mesPago = currentSemestre == 1 ? _random.Next(1, 7) : _random.Next(7, 13);
                        var fechaPago = new DateOnly(currentYear, mesPago, _random.Next(1, 28));

                        if (fechaPago <= hoy)
                        {
                            var cuota = new Cuota
                            {
                                SocioId = socio.Id,
                                Anio = currentYear,
                                Semestre = currentSemestre,
                                Monto = monto,
                                FechaPago = fechaPago,
                                FormaDePago = socio.PreferenciaDePago
                            };
                            cuotas.Add(cuota);

                            if (socio.PreferenciaDePago == MetodosDePago.Cobrador)
                            {
                                registrosCobradores.Add(new RegistroCobrador
                                {
                                    FechaCobro = fechaPago,
                                    NombreSocio = $"{socio.Nombre} {socio.Apellido}",
                                    MontoCobrado = (int)monto,
                                    IdCobrador = _random.Next(1, 4)
                                });
                            }
                        }
                    }

                    // Avanzar al siguiente semestre
                    if (currentSemestre == 1) { currentSemestre = 2; }
                    else { currentSemestre = 1; currentYear++; }

                    // No pasarnos del año actual real (2026 en este caso)
                    if (currentYear > 2026) break;
                }
            }
            await _appDbcontext.Cuotas.AddRangeAsync(cuotas);
            await _appDbcontext.RegistroCobradores.AddRangeAsync(registrosCobradores);

            // 8. Sembrar Alquileres de Artículos
            var alquileres = new List<Alquiler>();
            for (int i = 0; i < 60; i++)
            {
                var socioAlquiler = socios[_random.Next(socios.Count)];
                var fechaAlquiler = fechaInicioSocio.AddDays(_random.Next(totalDias));
                var finalizado = fechaAlquiler.AddDays(7) < hoy;

                var nuevoAlquiler = new Alquiler
                {
                    IdSocio = socioAlquiler.Id,
                    FechaAlquiler = fechaAlquiler,
                    Observaciones = $"Alquiler para evento social de {socioAlquiler.Apellido}",
                    Finalizado = finalizado,
                    Items = new List<ItemAlquiler>()
                };

                // Agregar 1-3 articulos
                int numArt = _random.Next(1, 4);
                int totalAlq = 0;
                for (int j = 0; j < numArt; j++)
                {
                    var art = articulos[_random.Next(articulos.Count)];
                    var cant = _random.Next(5, 20);
                    nuevoAlquiler.Items.Add(new ItemAlquiler { Articulo = art, Cantidad = cant });
                    totalAlq += (int)art.PrecioAlquiler * cant;
                }

                if (finalizado)
                {
                    nuevoAlquiler.HistorialDePagos = new List<PagoAlquilerDeArticulos>
                    {
                        new PagoAlquilerDeArticulos
                        {
                            Monto = totalAlq,
                            Anio = fechaAlquiler.Year,
                            Mes = fechaAlquiler.Month
                        }
                    };
                }
                alquileres.Add(nuevoAlquiler);
            }
            await _appDbcontext.alquileresArticulos.AddRangeAsync(alquileres);

            // 9. Sembrar Reservas de Salones
            var reservas = new List<ReservaSalon>();
            for (int i = 0; i < 40; i++)
            {
                var socioReserva = socios[_random.Next(socios.Count)];
                var salon = salones[_random.Next(salones.Count)];
                var fechaReserva = fechaInicioSocio.AddDays(_random.Next(totalDias + 90)); // Algunas a futuro
                var importe = _random.Next(20, 80) * 1000;
                var isCancelled = _random.Next(100) < 10; // 10% canceladas

                var r = new ReservaSalon
                {
                    SocioId = socioReserva.Id,
                    Salon = salon,
                    Titulo = $"Evento {socioReserva.Apellido} - {i}",
                    FechaAlquiler = fechaReserva,
                    Importe = importe,
                    IsCancelled = isCancelled,
                    TotalPagado = 0,
                    historialPagos = new List<PagoReservaSalon>()
                };

                if (!isCancelled)
                {
                    // Pagos parciales o totales
                    if (fechaReserva < hoy.AddDays(-5))
                    {
                        // Total pagado
                        r.TotalPagado = importe;
                        r.historialPagos.Add(new PagoReservaSalon
                        {
                            monto = importe,
                            FechaPago = fechaReserva.AddDays(-15)
                        });
                    }
                    else if (fechaReserva < hoy.AddDays(30))
                    {
                        // Pago parcial (Seña)
                        var sena = importe / 2;
                        r.TotalPagado = sena;
                        r.historialPagos.Add(new PagoReservaSalon
                        {
                            monto = sena,
                            FechaPago = hoy.AddDays(-10)
                        });
                    }
                }
                reservas.Add(r);
            }
            await _appDbcontext.ReservasSalones.AddRangeAsync(reservas);

            await _appDbcontext.SaveChangesAsync();
            Console.WriteLine(">>> DatabaseSeeder finished successfully!");
        }

        public async Task seedSociosExistentes()
        {
            // Intentar buscar el archivo en varias ubicaciones
            string fileName = "padron_procesado.csv.csv";
            string[] possiblePaths = new[]
            {
                // 1. En el directorio actual (ideal para VPS/Docker donde copiamos el archivo junto al binario)
                Path.Combine(Directory.GetCurrentDirectory(), fileName),
                
                // 2. Un nivel arriba (ideal para desarrollo local: ./APIClub -> ../padron...)
                Path.Combine(Directory.GetCurrentDirectory(), "..", fileName),
                
                // 3. Ruta absoluta hardcodeada (fallback desarrollo local especifico)
                @"c:\Repositorio\SistemaClubAbuelos\padron_procesado.csv.csv"
            };

            string filePath = possiblePaths.FirstOrDefault(p => System.IO.File.Exists(p));

            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine($">>> Error: No se encontró el archivo CSV en ninguna de las rutas esperadas:");
                foreach (var p in possiblePaths) Console.WriteLine($"   - {p}");
                return;
            }

            Console.WriteLine($">>> CSV encontrado en: {filePath}");
            Console.WriteLine(">>> Seeding Socios Existentes...");

            // 1. Crear o buscar Lote "Sin Lote"
            var sinLote = await _appDbcontext.Lotes.FirstOrDefaultAsync(l => l.NombreLote == "Sin Lote");
            if (sinLote == null)
            {
                sinLote = new Lote
                {
                    NombreLote = "Sin Lote",
                    CalleNorte = "-",
                    CalleSur = "-",
                    CalleEste = "-",
                    CalleOeste = "-"
                };
                await _appDbcontext.Lotes.AddAsync(sinLote);
                await _appDbcontext.SaveChangesAsync();
            }

            var lines = await System.IO.File.ReadAllLinesAsync(filePath);
            var sociosNuevos = new List<Socio>();

            // Ignorar cabecera (fila 1)
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = ParseCsvLine(line);
                if (parts.Length < 4) continue;

                // 0: Nombre y Apellido
                string rawName = parts[0].Trim().Trim('"');
                string apellido = rawName;
                string nombre = "";

                if (rawName.Contains(','))
                {
                    var splitName = rawName.Split(',');
                    apellido = splitName[0].Trim();
                    if (splitName.Length > 1) nombre = splitName[1].Trim();
                }
                else
                {
                    // Heuristica: Primera palabra es apellido, resto nombre
                    var splitSpace = rawName.Split(' ', 2);
                    apellido = splitSpace[0].Trim();
                    if (splitSpace.Length > 1) nombre = splitSpace[1].Trim();
                }

                // 1: Direccion
                string direccion = parts[1].Trim().Trim('"');

                // 2: Telefono
                string? telefono = parts[2].Trim().Trim('"');
                if (string.IsNullOrEmpty(telefono)) telefono = null;

                // 3: DNI
                string dni = parts[3].Trim().Trim('"');
                if (dni.Length < 8) dni = dni.PadLeft(8, '0');

                var socio = new Socio
                {
                    Nombre = nombre,
                    Apellido = apellido,
                    Dni = dni,
                    Telefono = telefono,
                    Direcccion = direccion,
                    Localidad = "San Francisco",
                    PreferenciaDePago = MetodosDePago.Cobrador,
                    Lote = sinLote,
                    FechaAsociacion = DateOnly.FromDateTime(DateTime.Now),
                    IsActivo = true
                };

                sociosNuevos.Add(socio);
            }

            await _appDbcontext.Socios.AddRangeAsync(sociosNuevos);
            await _appDbcontext.SaveChangesAsync();
            Console.WriteLine($">>> {sociosNuevos.Count} Socios Existentes insertados.");
        }

        public async Task seedViajesAsync()
        {
            Console.WriteLine(">>> Seeding Viajes, Variantes e Inscriptos...");

            // 1. Limpiar datos existentes de viajes (opcional, pero recomendado para pruebas limpias)
            try
            {
                await _appDbcontext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'replica';");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Inscriptos\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"VariantesViaje\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Viajes\" RESTART IDENTITY CASCADE;");
                await _appDbcontext.Database.ExecuteSqlRawAsync("SET session_replication_role = 'origin';");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Warning: Could not truncate travel tables: {ex.Message}");
            }

            var socios = await _appDbcontext.Socios.Take(10).ToListAsync();
            if (!socios.Any())
            {
                Console.WriteLine(">>> Error: No hay socios cargados para inscribir en viajes. Corra el seeder de socios primero.");
                return;
            }

            // 2. Crear Viajes
            var viajes = new List<Viaje>
            {
                new Viaje
                {
                    Titulo = "Escapada a Carlos Paz",
                    Dias = 4,
                    Noches = 3,
                    Fechasalida = DateOnly.FromDateTime(DateTime.Today.AddMonths(2)),
                    PorcentajeComision = 10,
                    VentasParaLiberado = 20,
                    ValorBase = 85000,
                    Variantes = new List<VarianteViaje>
                    {
                        new VarianteViaje
                        {
                            NombreVariante = "Bus Semi-Cama + Media Pensión",
                            ValorViaje = 85000,
                            ValorSeña = 20000,
                            Regimen = RegimenViaje.MediaPension,
                            TipoDeButaca = "Semi-Cama"
                        },
                        new VarianteViaje
                        {
                            NombreVariante = "Bus Cama + Pensión Completa",
                            ValorViaje = 105000,
                            ValorSeña = 30000,
                            Regimen = RegimenViaje.PensionCompleta,
                            TipoDeButaca = "Cama"
                        }
                    }
                },
                new Viaje
                {
                    Titulo = "Merlo San Luis - Relax",
                    Dias = 5,
                    Noches = 4,
                    Fechasalida = DateOnly.FromDateTime(DateTime.Today.AddMonths(3)),
                    PorcentajeComision = 12,
                    VentasParaLiberado = 15,
                    ValorBase = 120000,
                    Variantes = new List<VarianteViaje>
                    {
                        new VarianteViaje
                        {
                            NombreVariante = "Standard",
                            ValorViaje = 120000,
                            ValorSeña = 40000,
                            Regimen = RegimenViaje.MediaPension,
                            TipoDeButaca = "Semi-Cama"
                        }
                    }
                }
            };

            await _appDbcontext.Viajes.AddRangeAsync(viajes);
            await _appDbcontext.SaveChangesAsync();

            // 3. Inscribir algunos socios en las variantes
            var inscriptos = new List<InscriptoViaje>();
            var todasLasVariantes = viajes.SelectMany(v => v.Variantes).ToList();

            foreach (var variante in todasLasVariantes)
            {
                // Inscribir 2-3 socios por variante
                var numInscriptos = _random.Next(2, 4);
                for (int i = 0; i < numInscriptos; i++)
                {
                    var socio = socios[_random.Next(socios.Count)];

                    // Evitar duplicados simples para el seed
                    if (inscriptos.Any(ins => ins.SocioId == socio.Id && ins.VarianteViajeId == variante.Id)) continue;

                    var montoAbonado = _random.Next(0, (int)variante.ValorViaje / 2);
                    inscriptos.Add(new InscriptoViaje
                    {
                        VarianteViajeId = variante.Id,
                        SocioId = socio.Id,
                        montoAbonado = montoAbonado,
                        MontoPendiente = variante.ValorViaje - montoAbonado,
                        cancelado = false
                    });
                }
            }

            await _appDbcontext.Inscriptos.AddRangeAsync(inscriptos);
            await _appDbcontext.SaveChangesAsync();

            Console.WriteLine($">>> Seeding Viajes finished: {viajes.Count} viajes y {inscriptos.Count} inscriptos creados.");
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            string currentValue = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentValue);
                    currentValue = "";
                }
                else
                {
                    currentValue += c;
                }
            }
            result.Add(currentValue);
            return result.ToArray();
        }
    }
}
