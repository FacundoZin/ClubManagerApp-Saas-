using APIClub.Application.Dtos.AlquilerDeArticulos;
using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class AlquilerRepository : IAlquilerRepository
    {
        private readonly AppDbcontext _dbContext;

        public AlquilerRepository(AppDbcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Alquiler> CrearAlquiler(Alquiler alquiler)
        {
            _dbContext.alquileresArticulos.Add(alquiler);
            await _dbContext.SaveChangesAsync();
            return alquiler;
        }

        public async Task<Alquiler?> GetAlquilerById(int id)
        {
            return await _dbContext.alquileresArticulos
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Alquiler?> GetAlquilerByIdWithDetails(int id)
        {
            return await _dbContext.alquileresArticulos
                .Include(a => a.Socio)
                .Include(a => a.Items)
                    .ThenInclude(aa => aa.Articulo)
                .Include(a => a.HistorialDePagos)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Alquiler?> GetAlquilerBySocio(int idSocio)
        {
            return await _dbContext.alquileresArticulos
                .Include(a => a.Items)
                    .ThenInclude(aa => aa.Articulo)
                .Include(a => a.HistorialDePagos)
                .Where(a => a.IdSocio == idSocio)
                .OrderByDescending(a => a.FechaAlquiler)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<(List<AlquilerPreviewDto> Items, int TotalCount)> GetAlquileresActivos(int pageNumber, int pageSize)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            var totalCount = await _dbContext.alquileresArticulos.CountAsync();

            var items = await _dbContext.alquileresArticulos
                .AsNoTracking()
                .OrderByDescending(a => a.FechaAlquiler)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AlquilerPreviewDto
                {
                    Id = a.Id,
                    FechaAlquiler = a.FechaAlquiler,

                    NombreSocio = a.Socio.Nombre,
                    ApellidoSocio = a.Socio.Apellido,
                    DniSocio = a.Socio.Dni,
                    TelefonoSocio = a.Socio.Telefono,
                    DireccionSocio = a.Socio.Direcccion,
                    LocalidadSocio = a.Socio.Localidad,

                    estaAlDia = a.HistorialDePagos.Any(p =>
                        p.Anio == hoy.Year &&
                        p.Mes == hoy.Month
                    )
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> UpdateAlquiler(Alquiler alquiler)
        {
            _dbContext.alquileresArticulos.Update(alquiler);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<PagoAlquilerDeArticulos> RegistrarPago(PagoAlquilerDeArticulos pago)
        {
            _dbContext.PagosAlquilerDeArticulos.Add(pago);
            await _dbContext.SaveChangesAsync();
            return pago;
        }

        public async Task<ItemAlquiler> AgregarItem(ItemAlquiler item)
        {
            _dbContext.ItemALquiler.Add(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<int> CalcularMontoAlquiler(int alquilerId)
        {
            return await _dbContext.ItemALquiler
                .AsNoTracking()
                .Where(i => i.AlquilerId == alquilerId)
                .SumAsync(i => i.Cantidad * i.Articulo.PrecioAlquiler);
        }

        public async Task<bool> HasActiveAlquilerBySocio(int socioId)
        {
            return await _dbContext.alquileresArticulos
                .AnyAsync(a => a.IdSocio == socioId);
        }

        public async Task<(bool HasActiveAlquiler, int? AlquilerId)> TrySearchActiveAlquilerBySocio(int socioId)
        {
            var alquilerId = await _dbContext.alquileresArticulos
                .AsNoTracking()
                .Where(a => a.IdSocio == socioId)
                .Select(a => (int?)a.Id)
                .FirstOrDefaultAsync();

            if(alquilerId == null) return (false, null);
            return (true, alquilerId);
        }
    }
}
