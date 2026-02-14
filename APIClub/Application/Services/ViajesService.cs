using APIClub.Application.Common;
using APIClub.Application.Dtos.Viajes.Create;
using APIClub.Application.Dtos.Viajes.Views;
using APIClub.Domain.ModuloGestionViajes.Models;
using APIClub.Domain.ModuloGestionViajes.Repositories;
using APIClub.Domain.ModuloGestionViajes.useCases;

namespace APIClub.Application.Services
{
    public class ViajesService : IViajesServices
    {
        private readonly IViajeWriteRepository _viajeWriteRepository;
        private readonly IViajeReadRepository _viajeReadRepository;
        private readonly UnitOfWork _unitOfWork;

        public ViajesService(IViajeWriteRepository viajeWriteRepository, IViajeReadRepository viajeReadRepository)
        {
            _viajeWriteRepository = viajeWriteRepository;
            _viajeReadRepository = viajeReadRepository;
        }

        public async Task<Result<object?>> CreateViaje(CreateViajeDto dto)
        {
            try
            {
                if (dto.FechaSalida < DateOnly.FromDateTime(DateTime.Now))
                    return Result<object?>.Error("La fecha de salida debe ser posterior a la fecha actual", 400);
                

                var viaje = new Viaje
                {
                    Titulo = dto.Titulo,
                    Dias = dto.Dias,
                    Noches = dto.Noches,
                    Fechasalida = dto.FechaSalida,
                    VentasParaLiberado = dto.VentasParaLiberado,
                    ValorBase = dto.ValorBase
                };

                await _viajeWriteRepository.CreateViaje(viaje);

                return Result<object?>.Exito(null);
            }
            catch (Exception ex)
            {
                return Result<object?>.Error($"Lo sentimos hubo un error al cargar el viaje", 500);
            }
        }

        public async Task<Result<object?>> CreateVarianteViaje(CreateVarianteViajeDto dto)
        {
            try
            {
                var viajeExists = await _viajeReadRepository.ViajeExists(dto.IdViaje);
                if (!viajeExists)
                    return Result<object?>.NotFound($"Lo sentimos el viaje sobre el cual quiere añadir una variante no fue encontrado");


                if (dto.ValorSeña > dto.ValorViaje)
                    return Result<object?>.Error("El valor de la seña no puede ser mayor al valor del viaje", 400);

                var variante = new VarianteViaje
                {
                    IdViaje = dto.IdViaje,
                    NombreVariante = dto.NombreVariante,
                    ValorViaje = dto.ValorViaje,
                    ValorSeña = dto.ValorSeña,
                    Regimen = dto.Regimen,
                    TipoDeButaca = dto.TipoDeButaca
                };

                await _viajeWriteRepository.CreateVarianteViaje(variante);

                return Result<object?>.Exito(null);
            }
            catch (Exception ex)
            {
                return Result<object?>.Error($"Lo sentimos hubo un error al cargar la variante del viaje", 500);
            }
        }


        public async Task<Result<object?>> InscriptSocioToViaje(InsertInscriptoViajeDto dto)
        {
            var viaje = await _viajeReadRepository.GetVarianteById(dto.ViajeVarianteId);

            if (viaje == null)
                return Result<object?>.Error("Lo sentimos el viaje al que se quiere inscribir el socio no existe en el sistema", 404);
            if (dto.MontoAbonado < viaje.ValorSeña)
                return Result<object?>.Error("el monto abonado por el socio debe igual o mayor a la seña del viaje", 400);
            if (dto.MontoAbonado > viaje.ValorViaje)
                return Result<object?>.Error("el monto abonado por el socio no puede ser mayor al valor del viaje", 400);

            var montoPendiente = viaje.ValorViaje - dto.MontoAbonado;

            viaje.Inscriptos.Add(new InscriptoViaje
            {
                montoAbonado = dto.MontoAbonado,
                MontoPendiente = montoPendiente,
                SocioId = dto.SocioId,
                VarianteViajeId = dto.ViajeVarianteId
            });

             bool succes = await _unitOfWork.SaveChangesAsync();

            if(!succes)
                return Result<object?>.Error("lo sentimos, algo salio mal al inscribir al socio", 500);

            return Result<object?>.Exito(null);
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

        public Task<Result<bool?>> ActualizarPagoDeViaje(int IdVarianteViaje, decimal montoAbonado)
        {
            throw new NotImplementedException();
        }

        public Task CancelarViajeDeSocio(int idSocio)
        {
            throw new NotImplementedException();
        }
    }
}
