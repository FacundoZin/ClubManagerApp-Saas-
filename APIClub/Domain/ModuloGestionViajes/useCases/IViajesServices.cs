using APIClub.Application.Common;
using APIClub.Application.Dtos.Viajes.Create;
using APIClub.Application.Dtos.Viajes.Views;
using APIClub.Application.Dtos.Viajes.Update;


namespace APIClub.Domain.ModuloGestionViajes.useCases
{
    public interface IViajesServices
    {
        Task<Result<object?>> CreateViaje(CreateViajeDto dto);
        Task<Result<object?>> CreateVarianteViaje(CreateVarianteViajeDto dto);
        Task<Result<object?>> InscribirPersonasAlViaje(InsertInscriptoViajeDto dto);

        Task<Result<List<PreviewViajeDto>>> ListarViajesDisponibles();
        Task<Result<List<PreviewVarianteViajeDto>>> ListarVariantesDeViaje(int IdViajeBase);
        Task<Result<FullViewViajeDto>> VerViajeCompleto(int IdViajeBase);

        Task<Result<object?>> ActualizarPagoDeViaje(int IdInscripto, decimal montoAbonado, string numeroRecibo);
        Task<Result<object?>> EditarPagoDeViaje(int IdInscripto, decimal nuevoMontoAbonado, string motivoModificacion, int usuarioId, string usuarioNombre);
        Task<Result<object?>> CancelarInscripcionDeViaje(int idInscripto);

        Task<Result<object?>> UpdateViaje(UpdateViajeDto dto);
        Task<Result<object?>> UpdateVarianteViaje(UpdateVarianteViajeDto dto);
    }
}

