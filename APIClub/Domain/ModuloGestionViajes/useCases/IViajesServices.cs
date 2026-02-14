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

        Task<Result<List<PrevieViajeDto>>> ListarViajesDisponibles();
        Task<Result<List<PreviewVarianteViajeDto>>> ListarVariantesDeViaje(int IdViajeBase);
        Task<Result<FullViewViajeDto>> VerViajeCompleto(int IdViajeBase);

        Task<Result<bool?>> ActualizarPagoDeViaje(int IdVarianteViaje, decimal montoAbonado);
        Task CancelarViajeDeSocio(int idSocio);
    }
}
