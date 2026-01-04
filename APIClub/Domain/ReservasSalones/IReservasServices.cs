using APIClub.Application.Common;
using APIClub.Application.Dtos.Reservas;

namespace APIClub.Domain.ReservasSalones
{
    public interface IReservasServices
    {
        Task<Result<List<PreviewReservaBySalonDto>>> GetReservasBySalon(int salonId);
        Task<Result<FechaDisponible>> GetDisponibilidadFecha(DateOnly fecha, int Idsalon);
        Task<Result<InfoReservaCompletaDto?>> GetReservaByFechaAndSalon(DateOnly fecha, int salonId);
        Task<Result<InfoReservaCompletaDto?>> GetReservaById(int reservaId);
        Task<Result<object?>> RegistrarReservaSalon(CreteReservaSalonDto dto);
        Task<Result<object?>> CancelarReservas(int idReserva);
    }
}
