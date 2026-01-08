using APIClub.Domain.ReservasSalones.Models;

namespace APIClub.Domain.ReservasSalones.Repositories
{
    public interface IReservasRepository
    {
        Task<List<ReservaSalon>> GetAlquileresBySalon(int IdSalon);
        Task<ReservaSalon?> SearchReservaByFecha(DateOnly fehca, int IdSalon);
        Task<bool> verificarDisponibilidad(DateOnly fecha, int IdSalon);
        Task<ReservaSalon?> SearchReservaById(int idReserva);
        Task<bool> CrearReserva(ReservaSalon reserva);
        Task<bool> BorrarReserva(int idReserva);
        Task<bool> HasFutureReservationsBySocio(int socioId);

        Task<ReservaSalon?> SearchReservaByIdWithTracking(int idReserva);
    }
}
