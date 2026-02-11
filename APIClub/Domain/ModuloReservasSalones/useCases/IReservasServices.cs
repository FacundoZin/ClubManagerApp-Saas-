using APIClub.Application.Common;
using APIClub.Application.Dtos.Reservas;

namespace APIClub.Domain.ModuloReservasSalones.useCases
{
    public interface IReservasServices
    {
        Task<PagedResult<PreviewReservaBySalonDto>> GetReservasBySalon(int salonId, int pageNumber, int pageSize);
        Task<Result<FechaDisponible>> GetDisponibilidadFecha(DateOnly fecha, int Idsalon);
        Task<Result<InfoReservaPreview?>> GetReservaByFechaAndSalon(DateOnly fecha, int salonId);
        Task<Result<InfoReservaCompletaDto?>> GetReservaById(int reservaId);
        Task<Result<object?>> RegistrarReservaSalon(CreteReservaSalonDto dto);
        Task<Result<object?>> CancelarReservas(int idReserva);
        Task<Result<object?>> RegistrarPagoDeSalon(int IdReserva,decimal montoAbonado);
    }
}
