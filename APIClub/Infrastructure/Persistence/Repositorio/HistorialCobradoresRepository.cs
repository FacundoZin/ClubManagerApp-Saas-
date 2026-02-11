using APIClub.Domain.ModuloGestionCobradores.Models;
using APIClub.Domain.ModuloGestionCobradores.Repositorios;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class HistorialCobradoresRepository : IHistorialCobradoresRepository
    {
        private readonly AppDbcontext _context;
        public HistorialCobradoresRepository (AppDbcontext context)
        {
            _context = context; 
        }

        public void AddCobroToHistorial(RegistroCobrador cobro)
        {
            _context.RegistroCobradores.Add(cobro);
        }

        public async Task<List<RegistroCobrador>> GetHistorialCobradorByMes(int idCobrador, int mes, int anio)
        {
            return await _context.RegistroCobradores
                .AsNoTracking()
                .Where(c => c.IdCobrador == idCobrador && c.FechaCobro.Year == anio && c.FechaCobro.Month == mes)
                .ToListAsync();
        }
    }
}
