using APIClub.Application.Common;
using APIClub.Application.Dtos.Viajes.Create;
using APIClub.Application.Dtos.Viajes.Views;

namespace APIClub.Domain.ModuloGestionViajes.useCases
{
    public interface IViajesServices
    {
        Task<Result<object?>> CreateViaje(CreateViajeDto dto);
        Task<Result<object?>> CreateVarianteViaje(CreateVarianteViajeDto dto);
        Task<Result<object?>> InscriptSocioToViaje(InsertInscriptoViajeDto dto);

        Task<Result<List<PreviewViajeDto>>> ListarViajesDisponibles();
        Task<Result<List<PreviewVarianteViajeDto>>> ListarVariantesDeViaje(int IdViajeBase);
        Task<Result<FullViewViajeDto>> VerViajeCompleto(int IdViajeBase);

        Task<Result<object?>> ActualizarPagoDeViaje(int IdInscripto, decimal montoAbonado);
        Task<Result<object?>> CancelarViajeDeSocio(int idInscripto);
    }
}
