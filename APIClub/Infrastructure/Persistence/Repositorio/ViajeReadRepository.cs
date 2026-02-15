using APIClub.Application.Dtos.Viajes.ComboBox;
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

        public async Task<Viaje?> GetViajeCompleto(int id)
        {
            return await _dbContext.Viajes
                .Include(v => v.Variantes)
                    .ThenInclude(vv => vv.Inscriptos)
                        .ThenInclude(i => i.Socio)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);
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

        public async Task<InscriptoViaje?> GetInscriptoById(int id)
        {
            return await _dbContext.Inscriptos
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> ViajeExists(int id)
        {
            return await _dbContext.Viajes.AnyAsync(v => v.Id == id);
        }

        public async Task<bool> VarianteExists(int id)
        {
            return await _dbContext.VariantesViaje.AnyAsync(v => v.Id == id);
        }

        public async Task<List<ComboBoxViajes>> GetComboBoxViajes()
        {
            return await _dbContext.Viajes
                .AsNoTracking()
                .Select(v => new ComboBoxViajes
                {
                    idViaje = v.Id,
                    NombreViaje = v.Titulo
                }).ToListAsync();
        }

        public async Task<List<ComboBoxVariantesViaje>> GetComboBoxVariantesDeViaje(int idViajeBase)
        {
            return await _dbContext.VariantesViaje
                .AsNoTracking()
                .Where(vv => vv.IdViaje == idViajeBase)
                .Select(vv => new ComboBoxVariantesViaje
                {
                    IdVariante = vv.Id,
                    NombreVariante = vv.NombreVariante
                }).ToListAsync();
        }

        public async Task<bool> EstaInscripto(int socioId, int varianteViajeId)
        {
            // buscamos el viaje al que pertenece la variante 
            var variante = await _dbContext.VariantesViaje
                .AsNoTracking()
                .FirstOrDefaultAsync(vv => vv.Id == varianteViajeId);

            if (variante == null) return false;

            // verificamos si el socio ya esta en cualquier variante de ese mismo viaje
            return await _dbContext.Inscriptos
                .AnyAsync(i => i.SocioId == socioId && i.Variante.IdViaje == variante.IdViaje && !i.cancelado);
        }
    }
}
