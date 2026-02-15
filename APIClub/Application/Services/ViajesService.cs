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

        public ViajesService(IViajeWriteRepository viajeWriteRepository, IViajeReadRepository viajeReadRepository, UnitOfWork unitOfWork)
        {
            _viajeWriteRepository = viajeWriteRepository;
            _viajeReadRepository = viajeReadRepository;
            _unitOfWork = unitOfWork;
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
                    ValorBase = dto.ValorBase,
                    PorcentajeComision = dto.PorcentajeComision
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

            var Inscripto = await _viajeReadRepository.EstaInscripto(dto.SocioId, dto.ViajeVarianteId);

            if (Inscripto)
                return Result<object?>.Error("El socio ya se encuentra inscripto en este viaje.", 400);

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

            if (!succes)
                return Result<object?>.Error("lo sentimos, algo salio mal al inscribir al socio", 500);

            return Result<object?>.Exito(null);
        }

        public async Task<Result<List<PreviewVarianteViajeDto>>> ListarVariantesDeViaje(int IdViajeBase)
        {
            try
            {
                var variantes = await _viajeReadRepository.ListarVariantesDeViaje(IdViajeBase);

                var result = variantes.Select(v => new PreviewVarianteViajeDto
                {
                    Id = v.Id,
                    NombreVariante = v.NombreVariante,
                    ValorViaje = v.ValorViaje,
                    ValorSeña = v.ValorSeña,
                    Regimen = v.Regimen,
                    TipoDeButaca = v.TipoDeButaca
                }).ToList();

                return Result<List<PreviewVarianteViajeDto>>.Exito(result);
            }
            catch (Exception)
            {
                return Result<List<PreviewVarianteViajeDto>>.Error("Lo sentimos hubo un error al listar las variantes del viaje", 500);
            }
        }

        public async Task<Result<List<PreviewViajeDto>>> ListarViajesDisponibles()
        {
            try
            {
                var viajes = await _viajeReadRepository.ListarViajesDisponibles();

                var result = viajes.Select(v => new PreviewViajeDto
                {
                    Id = v.Id,
                    Titulo = v.Titulo,
                    Dias = v.Dias,
                    Noches = v.Noches,
                    Fechasalida = v.Fechasalida,
                    VentasParaLiberado = v.VentasParaLiberado,
                    ValorBase = v.ValorBase,
                    PorcentajeComision = v.PorcentajeComision
                }).ToList();

                return Result<List<PreviewViajeDto>>.Exito(result);
            }
            catch (Exception)
            {
                return Result<List<PreviewViajeDto>>.Error("Lo sentimos hubo un error al listar los viajes", 500);
            }
        }

        public async Task<Result<FullViewViajeDto>> VerViajeCompleto(int IdViajeBase)
        {
            try
            {
                var viaje = await _viajeReadRepository.GetViajeCompleto(IdViajeBase);

                if (viaje == null)
                    return Result<FullViewViajeDto>.NotFound("Lo sentimos, el viaje solicitado no fue encontrado.");

                var todosLosInscriptos = viaje.Variantes.SelectMany(v => v.Inscriptos).ToList();

                var inscriptosActivos = todosLosInscriptos.Where(i => !i.cancelado).ToList();
                var totalInscriptos = inscriptosActivos.Count;
                var totalCancelados = todosLosInscriptos.Count(i => i.cancelado);

                var totalRecaudado = inscriptosActivos.Sum(i => i.montoAbonado);
                var totalPendiente = inscriptosActivos.Sum(i => i.MontoPendiente);

                var montoComision = totalRecaudado * (viaje.PorcentajeComision / 100);
                var montoParaAgencia = totalRecaudado - montoComision;

                //al hacer el calculo entre dos numeros enteros el lenguajes siempre va a quitar los decimales
                var totalLiberados = 0;
                if (viaje.VentasParaLiberado.HasValue && viaje.VentasParaLiberado.Value > 0)
                {
                    totalLiberados = totalInscriptos / viaje.VentasParaLiberado.Value;
                }

                var dto = new FullViewViajeDto
                {
                    Id = viaje.Id,
                    Titulo = viaje.Titulo,
                    Dias = viaje.Dias,
                    Noches = viaje.Noches,
                    Fechasalida = viaje.Fechasalida,
                    VentasParaLiberado = viaje.VentasParaLiberado,
                    ValorBase = viaje.ValorBase,
                    TotalInscriptos = totalInscriptos,
                    TotalCancelados = totalCancelados,
                    TotalRecaudado = totalRecaudado,
                    TotalPendiente = totalPendiente,
                    MontoComision = montoComision,
                    MontoParaAgencia = montoParaAgencia,
                    TotalLiberados = totalLiberados,
                    Variantes = viaje.Variantes.Select(vv => new VarianteViajeWithInscriptosDto
                    {
                        Id = vv.Id,
                        NombreVariante = vv.NombreVariante,
                        ValorViaje = vv.ValorViaje,
                        ValorSeña = vv.ValorSeña,
                        Regimen = vv.Regimen,
                        TipoDeButaca = vv.TipoDeButaca,
                        Inscriptos = vv.Inscriptos
                            .OrderBy(i => i.Socio.Apellido)
                            .ThenBy(i => i.Socio.Nombre)
                            .Select(i => new InscriptosDto
                            {
                                Id = i.Id,
                                NombreSocio = $"{i.Socio.Apellido} {i.Socio.Nombre}",
                                DniSocio = i.Socio.Dni,
                                TelefonoSocio = i.Socio.Telefono ?? string.Empty,
                                MontoAbonado = i.montoAbonado,
                                MontoPendiente = i.MontoPendiente,
                                Cancelado = i.cancelado
                            }).ToList()
                    }).ToList()
                };

                return Result<FullViewViajeDto>.Exito(dto);
            }
            catch (Exception ex)
            {
                return Result<FullViewViajeDto>.Error("Lo sentimos hubo un error al obtener la vista completa del viaje", 500);
            }
        }

        public async Task<Result<object?>> ActualizarPagoDeViaje(int IdInscripto, decimal montoAbonado)
        {
            try
            {
                var inscripto = await _viajeReadRepository.GetInscriptoById(IdInscripto);

                if (inscripto == null)
                    return Result<object?>.NotFound("Lo sentimos, no se encontró el socio inscripto al viaje.");

                if (montoAbonado > inscripto.MontoPendiente)
                    return Result<object?>.Error($"El monto a abonar (${montoAbonado}) no puede ser mayor al saldo pendiente (${inscripto.MontoPendiente}).", 400);

                inscripto.montoAbonado += montoAbonado;
                inscripto.MontoPendiente -= montoAbonado;

                bool success = await _unitOfWork.SaveChangesAsync();

                if (!success)
                    return Result<object?>.Error("Hubo un error al procesar el pago.", 500);

                return Result<object?>.Exito(null);
            }
            catch (Exception)
            {
                return Result<object?>.Error("Lo sentimos, hubo un error inesperado al actualizar el pago.", 500);
            }
        }

        public async Task<Result<object?>> CancelarViajeDeSocio(int idInscripto)
        {
            try
            {
                var inscripto = await _viajeReadRepository.GetInscriptoById(idInscripto);

                if (inscripto == null)
                    return Result<object?>.NotFound("Lo sentimos no se pudo encontrar el inscripto");

                inscripto.cancelado = true;
                bool success = await _unitOfWork.SaveChangesAsync();

                if (!success)
                    return Result<object?>.Error("Lo sentimos hubo un error al cancelar la inscripción", 500);

                return Result<object?>.Exito(null);
            }
            catch (Exception)
            {
                return Result<object?>.Error("Lo sentimos hubo un error inesperado al cancelar la inscripción", 500);
            }
        }
    }
}
