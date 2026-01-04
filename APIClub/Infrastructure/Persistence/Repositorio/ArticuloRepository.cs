using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class ArticuloRepository : IArticuloRepository
    {
        private readonly AppDbcontext _dbContext;

        public ArticuloRepository(AppDbcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CargarArticulo(Articulo articulo)
        {
            _dbContext.Articulos.Add(articulo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Articulo>> GetAllArticulos()
        {
            return await _dbContext.Articulos
                .AsNoTracking()
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }

        public async Task<Articulo?> GetArticuloById(int id)
        {
            return await _dbContext.Articulos
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateArticulo(Articulo articulo)
        {
            _dbContext.Articulos.Update(articulo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ArticuloExistsByNombre(string nombre)
        {
            return await _dbContext.Articulos
                .AnyAsync(a => a.Nombre.ToLower() == nombre.ToLower());
        }
    }
}
