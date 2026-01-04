using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class ItemsAlquilerRepository : IitemAlquilerRepository
    {
        private readonly AppDbcontext _context;
        public ItemsAlquilerRepository(AppDbcontext context) 
        {
            _context = context;
        }

        public async Task<ItemAlquiler?> GetItemAlquilerById(int id)
        {
            return await _context.ItemALquiler.FirstOrDefaultAsync(i => i.Id == id); 
        }

        public async Task<bool> RemoveItemAlquiler(ItemAlquiler alquilerArticulo)
        {
            _context.ItemALquiler.Remove(alquilerArticulo);
            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateItemAlquiler(ItemAlquiler alquilerArticulo)
        {
            _context.ItemALquiler.Update(alquilerArticulo);
            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }
    }
}
