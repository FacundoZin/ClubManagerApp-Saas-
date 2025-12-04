using APIClub.Data;
using APIClub.Interfaces.Repository;
using APIClub.Models;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Repositorio
{
    public class SociosRepository : ISocioRepository
    {
        private readonly AppDbcontext _Dbcontext;

        public SociosRepository(AppDbcontext dbcontext) 
        {
            _Dbcontext = dbcontext;
        }
        
        public async Task cargarSocio(Socio socio)
        {
            _Dbcontext.Socios.Add(socio);
            await _Dbcontext.SaveChangesAsync();
        }

        public async Task<bool> SocioExists(string dni)
        {
            return await _Dbcontext.Socios.AnyAsync(s => s.Dni == dni);
        }
    }
}
