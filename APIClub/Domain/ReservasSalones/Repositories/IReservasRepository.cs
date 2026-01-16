using APIClub.Domain.ReservasSalones.Models;

namespace APIClub.Domain.ReservasSalones.Repositories
{
    public interface IReservasRepository
    {
        Task<(List<ReservaSalon> Items, int TotalCount)> GetAlquileresBySalon(int IdSalon, int pageNumber, int pageSize);
        Task<ReservaSalon?> SearchReservaByFecha(DateOnly fehca, int IdSalon);
        Task<bool> verificarDisponibilidad(DateOnly fecha, int IdSalon);
        Task<ReservaSalon?> SearchReservaById(int idReserva);
        Task<bool> CrearReserva(ReservaSalon reserva);
        Task<bool> HasFutureReservationsBySocio(int socioId);

        Task<ReservaSalon?> SearchReservaByIdWithTracking(int idReserva);
    }
}
