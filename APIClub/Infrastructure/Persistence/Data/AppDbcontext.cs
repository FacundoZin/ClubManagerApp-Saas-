using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.Auth.Models;
using APIClub.Domain.Enums;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.ModuloGestionCobradores.Models;
using APIClub.Domain.ModuloGestionCuotas.Models;
using APIClub.Domain.ModuloGestionViajes.Models;
using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Domain.ReservasSalones.Models;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Data
{
    public class AppDbcontext : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options)
            : base(options)
        {
        }

        public DbSet<ReservaSalon> ReservasSalones { get; set; }
        public DbSet<PagoReservaSalon> pagoReservaSalon { get; set; }
        public DbSet<Cuota> Cuotas { get; set; }
        public DbSet<Salon> Salones { get; set; }
        public DbSet<Socio> Socios { get; set; }
        public DbSet<MontoCuota> MontoCuota { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Alquiler> alquileresArticulos { get; set; }
        public DbSet<ItemAlquiler> ItemALquiler { get; set; }
        public DbSet<PagoAlquilerDeArticulos> PagosAlquilerDeArticulos { get; set; }
        public DbSet<PaymentToken> PaymentTokens { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RegistroCobrador> RegistroCobradores { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<InscriptoViaje> Inscriptos { get; set; }
        public DbSet<VarianteViaje> VariantesViaje { get; set; }



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

                entity.HasQueryFilter(s => s.IsActivo);

                entity.HasOne(s => s.Lote)
                      .WithMany()
                      .HasForeignKey(s => s.LoteId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Lote>(entity =>
            {
                entity.Property(l => l.CalleNorte).IsRequired().HasMaxLength(100);
                entity.Property(l => l.CalleSur).IsRequired().HasMaxLength(100);
                entity.Property(l => l.CalleEste).IsRequired().HasMaxLength(100);
                entity.Property(l => l.CalleOeste).IsRequired().HasMaxLength(100);
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

                entity.HasMany(r => r.historialPagos)
                      .WithOne()
                      .HasForeignKey("ReservaSalonId")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(r => !r.IsCancelled && r.FechaAlquiler >= DateOnly.FromDateTime(DateTime.Today));
            });

            modelBuilder.Entity<ReservaSalon>()
                .Property(a => a.FechaAlquiler)
                .HasConversion(
                    v => v.ToDateTime(new TimeOnly(0, 0)),
                    v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<PagoReservaSalon>(entity =>
            {
                entity.Property(p => p.monto).HasColumnType("decimal(18,2)");

                entity.Property(p => p.FechaPago)
                      .HasConversion(
                          v => v.ToDateTime(new TimeOnly(0, 0)),
                          v => DateOnly.FromDateTime(v));
            });

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

                entity.HasQueryFilter(a => !a.Finalizado);
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

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Rol).IsRequired();

                entity.HasIndex(u => u.NombreUsuario).IsUnique();
            });

            // ---------------------------
            // Modulo Gestión de Viajes
            // ---------------------------
            modelBuilder.Entity<Viaje>(entity =>
            {
                entity.Property(v => v.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(v => v.ValorBase).HasColumnType("decimal(18,2)");
                entity.Property(v => v.Fechasalida)
                      .HasConversion(
                          v => v.ToDateTime(new TimeOnly(0, 0)),
                          v => DateOnly.FromDateTime(v));

                entity.HasQueryFilter(v => v.Fechasalida >= DateOnly.FromDateTime(DateTime.Today));
            });

            modelBuilder.Entity<VarianteViaje>(entity =>
            {
                entity.Property(vv => vv.NombreVariante).IsRequired().HasMaxLength(150);
                entity.Property(vv => vv.ValorViaje).HasColumnType("decimal(18,2)");
                entity.Property(vv => vv.ValorSeña).HasColumnType("decimal(18,2)");
                entity.Property(vv => vv.TipoDeButaca).HasMaxLength(100);

                entity.HasOne(vv => vv.Viaje)
                      .WithMany(v => v.Variantes)
                      .HasForeignKey(vv => vv.IdViaje)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InscriptoViaje>(entity =>
            {
                entity.Property(iv => iv.montoAbonado).HasColumnType("decimal(18,2)");
                entity.Property(iv => iv.MontoPendiente).HasColumnType("decimal(18,2)");

                entity.HasOne(iv => iv.Variante)
                      .WithMany(vv => vv.Inscriptos)
                      .HasForeignKey(iv => iv.VarianteViajeId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(iv => iv.Socio)
                      .WithMany()
                      .HasForeignKey(iv => iv.SocioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 0) Usuarios (SuperAdmin)
            // Password: "Admin123!" hasheado con BCrypt (Placeholder, se debe generar uno real al ejecutar)
            // Usaremos un hash real generado previamente para "Admin123!"
            // $2a$11$u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u.u (Ejemplo)
            // Pondremos este string que simula ser un hash válido para que compile y funcione el seed inicial si se usa la lógica correcta.
            // Nota: Para producción o test real, se generará desde la app o un script.
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    NombreUsuario = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), // Hash generado para "Admin123!"
                    Rol = RolUsuario.SuperAdmin,
                    FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UltimoAcceso = null
                }
            );


            // 1) Salones
            modelBuilder.Entity<Salon>().HasData(
                new Salon
                {
                    Id = 1,
                    Name = "Salón chico",
                    Direccion = "Calle Falsa 123"
                },
                new Salon
                {
                    Id = 2,
                    Name = "Salón grande",
                    Direccion = "Av. Siempre Viva 742"
                }
            );
        }
    }
}
