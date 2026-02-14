using APIClub.Domain.ModuloGestionViajes.Models;
using APIClub.Domain.ModuloGestionViajes.Repositories;
using APIClub.Infrastructure.Persistence.Data;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class ViajeWriteRepository : IViajeWriteRepository
    {
        private readonly AppDbcontext _dbContext;

        public ViajeWriteRepository(AppDbcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateViaje(Viaje viaje)
        {
            _dbContext.Viajes.Add(viaje);
            await _dbContext.SaveChangesAsync();
            return;
        }

        public async Task CreateVarianteViaje(VarianteViaje variante)
        {
            _dbContext.VariantesViaje.Add(variante);
            await _dbContext.SaveChangesAsync();
            return;
        }
    }
}
