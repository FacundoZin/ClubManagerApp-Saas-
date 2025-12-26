using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Domain.ReservasSalones.Models;
using APIClub.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace APIClub.Data
{
    public class AppDbcontext : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options)
            : base(options)
        {
        }

        public DbSet<ReservaSalon> ReservasSalones { get; set; }
        public DbSet<Cuota> Cuotas { get; set; }
        public DbSet<Salon> Salones { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<MontoCuota> MontoCuota { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Alquiler> alquileresArticulos { get; set; }  
        public DbSet<ItemAlquiler> ItemALquiler {  get; set; }
        public DbSet<PagoAlquilerDeArticulos> PagosAlquilerDeArticulos { get; set; }
        public DbSet<PaymentToken> PaymentTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------------
            // Configuraciones básicas
            // ---------------------------
            modelBuilder.Entity<Socio>(entity =>
            {
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Dni).HasMaxLength(20);
                entity.HasMany(s => s.HistorialCuotas)
                      .WithOne(c => c.Socio)
                      .HasForeignKey(c => c.SocioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Salon>(entity =>
            {
                entity.Property(s => s.Name).IsRequired().HasMaxLength(150);
                entity.Property(s => s.Direccion).HasMaxLength(250);
            });

            modelBuilder.Entity<MontoCuota>(entity =>
            {
                entity.Property(m => m.MontoCuotaFija).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Cuota>(entity =>
            {
                entity.Property(c => c.Monto).HasColumnType("decimal(18,2)");
                entity.Property(c => c.Anio);
                entity.Property(c => c.Semestre);
            });

            modelBuilder.Entity<ReservaSalon>(entity =>
            {
                entity.Property(r => r.Importe).HasColumnType("decimal(18,2)");
                entity.Property(r => r.TotalPagado).HasColumnType("decimal(18,2)");

                entity.HasOne(r => r.Socio)
                      .WithMany() 
                      .HasForeignKey(r => r.SocioId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Salon)
                      .WithMany()
                      .HasForeignKey(r => r.SalonId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<ReservaSalon>()
                .Property(a => a.FechaAlquiler)
                .HasConversion(
                    v => v.ToDateTime(new TimeOnly(0, 0)),
                    v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<Cuota>()
                .Property(c => c.FechaPago)
                .HasConversion(
                    v => v.ToDateTime(new TimeOnly(0, 0)),
                    v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(a => a.PrecioAlquiler).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.Property(a => a.Nombre)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(a => a.PrecioAlquiler)
                      .IsRequired();
            });

            modelBuilder.Entity<Alquiler>(entity =>
            {
                entity.Property(a => a.Observaciones)
                      .HasMaxLength(1000);

                entity.HasOne(a => a.Socio)
                      .WithMany()
                      .HasForeignKey(a => a.IdSocio)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(a => a.FechaAlquiler)
                .HasConversion(v => v.ToDateTime(new TimeOnly(0, 0)),
                v => DateOnly.FromDateTime(v));
            });

            modelBuilder.Entity<ItemAlquiler>(entity =>
            {
                entity.HasOne(i => i.Articulo)
                      .WithMany()
                      .HasForeignKey(i => i.ArticuloId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(i => i.Alquiler)
                      .WithMany(a => a.Items)
                      .HasForeignKey(i => i.AlquilerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PagoAlquilerDeArticulos>(entity =>
            {
                entity.Property(p => p.Monto).IsRequired();

                entity.HasOne(p => p.alquiler)
                      .WithMany(a => a.HistorialDePagos)
                      .HasForeignKey(p => p.IdAlquiler)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(p => new { p.IdAlquiler, p.Anio, p.Mes })
                      .IsUnique();
            });


            // ---------------------------
            // Seed data (datos de prueba)
            // ---------------------------

            // 1) Salones
            modelBuilder.Entity<Salon>().HasData(
                new Salon
                {
                    Id = 1,
                    Name = "Salón Central",
                    Direccion = "Calle Falsa 123"
                },
                new Salon
                {
                    Id = 2,
                    Name = "Salón Norte",
                    Direccion = "Av. Siempre Viva 742"
                }
            );

            // 2) Socios
            modelBuilder.Entity<Socio>().HasData(
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Dni = "12345678",
                    Telefono = "341-1234567",
                    Direcccion = "Mitre 100",
                    Localidad = "Rosario",
                    FechaAsociacion = DateOnly.FromDateTime(new DateTime(2020, 5, 10))
                },
                new Socio
                {
                    Id = 2,
                    Nombre = "María",
                    Apellido = "Gómez",
                    Dni = "87654321",
                    Telefono = "341-7654321",
                    Direcccion = "San Martín 200",
                    Localidad = "Córdoba",
                    FechaAsociacion = DateOnly.FromDateTime(new DateTime(2021, 3, 15))
                }
            );

            // 3) MontoCuota (registro de ejemplo)
            modelBuilder.Entity<MontoCuota>().HasData(
                new MontoCuota
                {
                    Id = 1,
                    MontoCuotaFija = 2500.00m,
                    FechaActualizacion = new DateTime(2025, 01, 01)
                }
            );

            // 4) Cuotas (historial de pagos para socio 1)
            modelBuilder.Entity<Cuota>().HasData(
                new
                {
                    Id = 1,
                    FechaPago = DateOnly.FromDateTime(new DateTime(2024, 3, 1)),
                    Monto = 2500.00m,
                    FormaDePago = (FormasDePago)1, // ajustá si necesitás un valor concreto del enum
                    Anio = 2024,
                    Semestre = 1,
                    SocioId = 1
                },
                new
                {
                    Id = 2,
                    FechaPago = DateOnly.FromDateTime(new DateTime(2024, 9, 1)),
                    Monto = 2500.00m,
                    FormaDePago = (FormasDePago)2,
                    Anio = 2024,
                    Semestre = 2,
                    SocioId = 1
                }
            );

            // 5) Reservas de prueba
            modelBuilder.Entity<ReservaSalon>().HasData(
                new
                {
                    Id = 1,
                    Titulo = "fiesta de 15 cele",
                    FechaAlquiler = DateOnly.FromDateTime(new DateTime(2025, 5, 20)),
                    Importe = 5000.00m,
                    TotalPagado = 0.00m,
                    SocioId = 1,
                    SalonId = 1
                },
                new
                {
                    Id = 2,
                    Titulo = "baile abuelos",
                    FechaAlquiler = DateOnly.FromDateTime(new DateTime(2025, 6, 15)),
                    Importe = 7000.00m,
                    TotalPagado = 7000.00m,
                    SocioId = 2,
                    SalonId = 2
                }
            );

            // 6) Artículos de ortopedia
            modelBuilder.Entity<Articulo>().HasData(
                new Articulo
                {
                    Id = 1,
                    Nombre = "Silla de Ruedas",
                    PrecioAlquiler = 10500
                },
                new Articulo
                {
                    Id = 2,
                    Nombre = "Andador",
                    PrecioAlquiler = 8000
                },
                new Articulo
                {
                    Id = 3,
                    Nombre = "Muletas",
                    PrecioAlquiler = 5000
                },
                new Articulo
                {
                    Id = 4,
                    Nombre = "Bastón",
                    PrecioAlquiler = 4500
                }
            );

            modelBuilder.Entity<Alquiler>().HasData(
                new
                {
                    Id = 1,
                    FechaAlquiler = DateOnly.FromDateTime(new DateTime(2025, 11, 10)),
                    Observaciones = "Alquiler por 3 días",
                    IdSocio = 1,
                    Finalizado = false
                },
                new
                {
                    Id = 2,
                    FechaAlquiler = DateOnly.FromDateTime(new DateTime(2025, 10, 05)),
                    Observaciones = "Préstamo semanal",
                    IdSocio = 2,
                    Finalizado = true
                },
                new
                {
                    Id = 3,
                    FechaAlquiler = DateOnly.FromDateTime(new DateTime(2025, 9, 20)),
                    Observaciones = "Rehabilitación post operación",
                    IdSocio = 1,
                    Finalizado = false
                },
                new
                {
                    Id = 4,
                    FechaAlquiler = DateOnly.FromDateTime(new DateTime(2025, 8, 01)),
                    Observaciones = "Alquiler viejo ya cerrado",
                    IdSocio = 2,
                    Finalizado = true
                }
            );

            modelBuilder.Entity<ItemAlquiler>().HasData(
                new ItemAlquiler
                {
                    Id = 1,
                    AlquilerId = 1,
                    ArticuloId = 1,
                    Cantidad = 1
                },
                new ItemAlquiler
                {
                    Id = 2,
                    AlquilerId = 1,
                    ArticuloId = 4, // bastón
                    Cantidad = 2
                },
                new ItemAlquiler
                {
                    Id = 3,
                    AlquilerId = 2,
                    ArticuloId = 2, // andador
                    Cantidad = 1
                },
                new ItemAlquiler
                {
                    Id = 4,
                    AlquilerId = 3,
                    ArticuloId = 3, // muletas
                    Cantidad = 2
                },
                new ItemAlquiler
                {
                    Id = 5,
                    AlquilerId = 3,
                    ArticuloId = 4, // bastón
                    Cantidad = 1
                },
                new ItemAlquiler
                {
                    Id = 6,
                    AlquilerId = 4,
                    ArticuloId = 1,
                    Cantidad = 1
                }
            );

            modelBuilder.Entity<PagoAlquilerDeArticulos>().HasData(
                new
                {
                    Id = 1,
                    IdAlquiler = 1,
                    Anio = 2025,
                    Mes = 1,
                    FechaPago = DateOnly.FromDateTime(new DateTime(2025, 1, 5)),
                    Monto = 10500
                },
                new
                {
                    Id = 2,
                    IdAlquiler = 1,
                    Anio = 2025,
                    Mes = 2,
                    FechaPago = DateOnly.FromDateTime(new DateTime(2025, 2, 5)),
                    Monto = 10500
                },
                new
                {
                    Id = 3,
                    IdAlquiler = 1,
                    Anio = 2025,
                    Mes = 3,
                    FechaPago = DateOnly.FromDateTime(new DateTime(2025, 3, 5)),
                    Monto = 10500
                }
            );

            // 7) PaymentTokens de prueba
            modelBuilder.Entity<PaymentToken>().HasData(
                new PaymentToken
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    nombreSocio = "Juan Pérez",
                    IdSocio = 1,
                    anio = 2025,
                    semestre = 1,
                    monto = 2500.00m,
                    FechaExpiracion = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                    usado = false,
                    Preference_Id = null
                },
                new PaymentToken
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    nombreSocio = "Juan Pérez",
                    IdSocio = 1,
                    anio = 2025,
                    semestre = 2,
                    monto = 2500.00m,
                    FechaExpiracion = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                    usado = false,
                    Preference_Id = null
                },
                new PaymentToken
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    nombreSocio = "María Gómez",
                    IdSocio = 2,
                    anio = 2025,
                    semestre = 1,
                    monto = 2500.00m,
                    FechaExpiracion = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                    usado = false,
                    Preference_Id = null
                },
                new PaymentToken
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    nombreSocio = "María Gómez",
                    IdSocio = 2,
                    anio = 2025,
                    semestre = 2,
                    monto = 2500.00m,
                    FechaExpiracion = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                    usado = false,
                    Preference_Id = null
                }
            );
        }
    }
}
