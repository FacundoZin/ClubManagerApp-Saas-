using APIClub.Application.Common;
using APIClub.Application.Dtos.Viajes.Create;
using APIClub.Application.Dtos.Viajes.Views;
using APIClub.Domain.ModuloGestionViajes.useCases;

namespace APIClub.Application.Services
{
    public class ViajesService : IViajesServices
    {
        public Task<Result<object?>> CreateVarianteViaje(CreateVarianteViajeDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result<object?>> CreateViaje(CreateViajeDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result<object?>> InscriptSocioToViaje(InsertInscriptoViajeDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<PreviewVarianteViajeDto>>> ListarVariantesDeViaje(int IdViajeBase)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<PrevieViajeDto>>> ListarViajesDisponibles()
        {
            throw new NotImplementedException();
        }

        public Task<Result<FullViewViajeDto>> VerViajeCompleto(int IdViajeBase)
        {
            throw new NotImplementedException();
        }
    }
}
