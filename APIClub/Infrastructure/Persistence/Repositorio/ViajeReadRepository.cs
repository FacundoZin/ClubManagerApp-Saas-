using APIClub.Application.Dtos.Viajes.Views;
using APIClub.Domain.ModuloGestionViajes.Models;
using APIClub.Domain.ModuloGestionViajes.Repositories;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{

    public class ViajeReadRepository : IViajeReadRepository
    {
        private readonly AppDbcontext _dbContext;

        public ViajeReadRepository(AppDbcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Viaje?> GetViajeById(int id)
        {
            return await _dbContext.Viajes
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Viaje?> GetViajeByIdWithVariantes(int id)
        {
            return await _dbContext.Viajes
                .Include(v => v.Variantes)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<FullViewViajeDto?> GetViajeCompleto(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<VarianteViaje?> GetVarianteById(int id)
        {
            return await _dbContext.VariantesViaje
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Viaje>> ListarViajesDisponibles()
        {
            var fechaActual = DateOnly.FromDateTime(DateTime.Now);

            return await _dbContext.Viajes
                .Where(v => v.Fechasalida >= fechaActual)
                .Include(v => v.Variantes)
                .AsNoTracking()
                .OrderBy(v => v.Fechasalida)
                .ToListAsync();
        }

        public async Task<List<VarianteViaje>> ListarVariantesDeViaje(int idViajeBase)
        {
            return await _dbContext.VariantesViaje
                .Where(v => v.IdViaje == idViajeBase)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ViajeExists(int id)
        {
            return await _dbContext.Viajes.AnyAsync(v => v.Id == id);
        }

        public async Task<bool> VarianteExists(int id)
        {
            return await _dbContext.VariantesViaje.AnyAsync(v => v.Id == id);
        }
    }
}
