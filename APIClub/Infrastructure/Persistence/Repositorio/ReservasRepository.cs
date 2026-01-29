using APIClub.Domain.ReservasSalones.Models;
using APIClub.Domain.ReservasSalones.Repositories;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class ReservasRepository : IReservasRepository
    {
        private readonly AppDbcontext _Context;

        public ReservasRepository(AppDbcontext dbcontext)
        {
            _Context = dbcontext;
        }

        public async Task<bool> CrearReserva(ReservaSalon reserva)
        {
            _Context.ReservasSalones.Add(reserva);

            if (await _Context.SaveChangesAsync() == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<(List<ReservaSalon> Items, int TotalCount)> GetAlquileresBySalon(int IdSalon, int pageNumber, int pageSize)
        {
            var query = _Context.ReservasSalones
                .Where(a => a.SalonId == IdSalon);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(s => s.Socio)
                .OrderBy(a => a.FechaAlquiler)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ReservaSalon?> SearchReservaByFecha(DateOnly fehca, int IdSalon)
        {
            return await _Context.ReservasSalones
                .Include(r => r.Salon)
                .Include(s => s.Socio)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.SalonId == IdSalon && r.FechaAlquiler == fehca);
        }

        public async Task<ReservaSalon?> SearchReservaById(int idReserva)
        {
            return await _Context.ReservasSalones
                .Include(r => r.Salon)
                .Include(r => r.Socio)
                .Include(r => r.historialPagos)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == idReserva);
        }

        public async Task<bool> verificarDisponibilidad(DateOnly fecha, int IdSalon)
        {
            bool existeReserva = await _Context.ReservasSalones
                .AnyAsync(r => r.SalonId == IdSalon && r.FechaAlquiler == fecha);


            return !existeReserva;
        }

        public async Task<bool> HasFutureReservationsBySocio(int socioId)
        {
            var fechaHoy = DateOnly.FromDateTime(DateTime.Now);
            return await _Context.ReservasSalones
                .AnyAsync(r => r.SocioId == socioId);
        }

        public async Task<ReservaSalon?> SearchReservaByIdWithTracking(int idReserva)
        {
            return await _Context.ReservasSalones.FirstOrDefaultAsync(r => r.Id == idReserva);
        }
    }
}
