using APIClub.Application.Common;
using APIClub.Application.Dtos.Cuota;
using APIClub.Application.Dtos.Socios;
using APIClub.Application.Helpers;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.GestionSocios.Validations;


namespace APIClub.Application.Services
{
    public class SociosManagmentService : ISociosManagmentService
    {
        private readonly ISocioRepository _SocioRepository;
        private readonly ISocioIntegrityValidator _validator;

        public SociosManagmentService(ISocioRepository socioRepository, ISocioIntegrityValidator validator)
        {
            _SocioRepository = socioRepository;
            _validator = validator;
        }


        public async Task<Result<ExistingSocio>> cargarSocio(CreateSocioDto _dto)
        {
            var existingSocio = await _SocioRepository.GetSocioByDniIgnoreFilter(_dto.Dni);

            if (existingSocio != null)
            {
                if (existingSocio.IsActivo)
                {
                    return Result<ExistingSocio>.Error("El socio que quiere cargar ya está activo.", 400);
                }
                else
                {
                    var data = new ExistingSocio 
                    { 
                        Id = existingSocio.Id, 
                        Nombre = existingSocio.Nombre + " " + existingSocio.Apellido 
                    };
                    return Result<ExistingSocio>.Conflict("El socio ya existe pero está dado de baja.", 409, data);
                }
            }

            string telefonoFormateado = null;

            if (_dto.Telefono != null)
            {
                var FormatResult = _dto.Telefono.FormatearForWhatsapp();

                if (!FormatResult.Exit)
                    return Result<ExistingSocio>.Error(FormatResult.Errormessage, FormatResult.Errorcode);

                telefonoFormateado = FormatResult.Data;
            };

            var socio = new Socio
            {
                Nombre = _dto.Nombre,
                Apellido = _dto.Apellido,
                Dni = _dto.Dni,
                Telefono = telefonoFormateado,
                Direcccion = _dto.Direcccion,
                Lote = _dto.Lote,
                Localidad = _dto.Localidad,
                FechaAsociacion = DateOnly.FromDateTime(DateTime.Now),
                IsActivo = true
            };

            await _SocioRepository.cargarSocio(socio);

            return Result<ExistingSocio>.Exito(new ExistingSocio
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
            });
        }

        public async Task<Result<object>> ReactivarSocio(int id)
        {
            var socio = await _SocioRepository.GetSocioByIdIgnoreFilter(id);

            if (socio is null)
            {
                return Result<object>.Error("No se encontró el socio.", 404);
            }

            if (socio.IsActivo)
            {
                return Result<object>.Error("El socio ya se encuentra activo.", 400);
            }

            socio.IsActivo = true;
            socio.FechaDeBaja = null;

            await _SocioRepository.UpdateSocio(socio);

            return Result<object>.Exito(new { Message = "Socio reactivado correctamente.", SocioId = socio.Id });
        }

        public async Task<Result<PreviewSocioDto>> GetSocioByDni(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return Result<PreviewSocioDto>.Error("Debe indicar un DNI válido.", 400);
            }

            var socio = await _SocioRepository.GetSocioByDni(dni);

            if (socio is null)
            {
                return Result<PreviewSocioDto>.Error("No se encontró un socio con ese DNI.", 404);
            }

            var fechaPagoActual = DateOnly.FromDateTime(DateTime.Now);
            int anioActual = fechaPagoActual.Year;
            int semestreActual = fechaPagoActual.Month <= 6 ? 1 : 2;

            bool debeCuota = false;

            if (!socio.HistorialCuotas.Any(c => c.Anio == anioActual && c.Semestre == semestreActual))
                debeCuota = true;

            var dto = new PreviewSocioDto
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Dni = socio.Dni,
                Telefono = socio.Telefono?.FormatearForUserVisibility(),
                Direcccion = socio.Direcccion,
                Lote = socio.Lote,
                Localidad = socio.Localidad,
                AdeudaCuotas = debeCuota,
            };

            return Result<PreviewSocioDto>.Exito(dto);
        }

        public async Task<Result<PagedResult<PreviewSocioDto>>> GetSociosDeudores(int pageNumber, int pageSize)
        {
            var fechaPagoActual = DateOnly.FromDateTime(DateTime.Now);
            int anioActual = fechaPagoActual.Year;
            int semestreActual = fechaPagoActual.Month <= 6 ? 1 : 2;

            var (socios, totalCount) = await _SocioRepository.GetSociosDeudoresPaginado(anioActual, semestreActual, pageNumber, pageSize);
            
            var items = socios.Select(s => new PreviewSocioDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Apellido = s.Apellido,
                Dni = s.Dni,
                Telefono = s.Telefono?.FormatearForUserVisibility(),
                Direcccion = s.Direcccion,
                Lote = s.Lote,
                Localidad = s.Localidad,
                AdeudaCuotas = true,
            }).ToList();

            var result = new PagedResult<PreviewSocioDto>(items, totalCount, pageNumber, pageSize);

            return Result<PagedResult<PreviewSocioDto>>.Exito(result);
        }

        public async Task<Result<object>> UpdateSocio(int id, CreateSocioDto dto)
        {
            if (id <= 0)
            {
                return Result<object>.Error("El ID proporcionado no es válido.", 400);
            }

            var socio = await _SocioRepository.GetSocioById(id);

            if (socio is null)
            {
                return Result<object>.Error("No se encontró un socio con ese ID.", 404);
            }

            // Validar que el nuevo DNI no esté asignado a otro socio
            if (socio.Dni != dto.Dni)
            {
                var dniExists = await _SocioRepository.SocioExists(dto.Dni);
                if (dniExists)
                {
                    return Result<object>.Error("Ya existe un socio con ese DNI.", 400);
                }
            }

            string telefonoFormateado = null;

            if (dto.Telefono != null)
            {
                var FormatResult = dto.Telefono.FormatearForWhatsapp();

                if (!FormatResult.Exit)
                    return Result<object>.Error(FormatResult.Errormessage, FormatResult.Errorcode);

                telefonoFormateado = FormatResult.Data;
            };

            // Actualizar propiedades
            socio.Nombre = dto.Nombre;
            socio.Apellido = dto.Apellido;
            socio.Dni = dto.Dni;
            socio.Telefono = telefonoFormateado;
            socio.Direcccion = dto.Direcccion;
            socio.Lote = dto.Lote;
            socio.Localidad = dto.Localidad;

            await _SocioRepository.UpdateSocio(socio);

            return Result<object>.Exito(new
            {
                Message = "Socio actualizado correctamente.",
                SocioId = socio.Id
            });
        

        }
        public async Task<Result<PreviewSocioDto>> GetSocioById(int id)
        {
            if (id <= 0)
            {
                return Result<PreviewSocioDto>.Error("El ID proporcionado no es válido.", 400);
            }

            var socio = await _SocioRepository.GetSocioById(id);

            if (socio is null)
            {
                return Result<PreviewSocioDto>.Error("No se encontró un socio con ese ID.", 404);
            }

            var dto = new PreviewSocioDto
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Dni = socio.Dni,
                Telefono = socio.Telefono?.FormatearForUserVisibility(),
                Direcccion = socio.Direcccion,
                Lote = socio.Lote,
                Localidad = socio.Localidad,
                AdeudaCuotas = false // Tendriamos que calcularlo si fuera necesario, pero para el form no hace falta
            };

            return Result<PreviewSocioDto>.Exito(dto);
        }

        public async Task<Result<FullSocioDto>> GetFullSocioById(int id)
        {
            if (id <= 0)
            {
                return Result<FullSocioDto>.Error("El ID proporcionado no es válido.", 400);
            }

            var socio = await _SocioRepository.GetSocioByIdWithCuotas(id);

            if (socio is null)
            {
                return Result<FullSocioDto>.Error("No se encontró un socio con ese ID.", 404);
            }

            var fechaPagoActual = DateOnly.FromDateTime(DateTime.Now);
            int anioActual = fechaPagoActual.Year;
            int semestreActual = fechaPagoActual.Month <= 6 ? 1 : 2;

            bool debeCuota = !socio.HistorialCuotas.Any(c => c.Anio == anioActual && c.Semestre == semestreActual);

            var dto = new FullSocioDto
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Dni = socio.Dni,
                Telefono = socio.Telefono?.FormatearForUserVisibility(),
                Direcccion = socio.Direcccion,
                Lote = socio.Lote,
                Localidad = socio.Localidad,
                FechaAsociacion = socio.FechaAsociacion,
                AdeudaCuotas = debeCuota,
                HistorialCuotas = socio.HistorialCuotas.Select(c => new PreviewCuotaDto
                {
                    Id = c.Id,
                    FechaPago = c.FechaPago,
                    Importe = c.Monto,
                    MetodoPago = c.FormaDePago.ToString()
                }).OrderByDescending(c => c.FechaPago).ToList()
            };

            return Result<FullSocioDto>.Exito(dto);
        }

        public async Task<Result<object>> RemoveSocio(int id)
        {
            if (id <= 0)
            {
                return Result<object>.Error("El ID proporcionado no es válido.", 400);
            }

            // 1. Validar Integridad antes de proceder
            var validationResult = await _validator.ValidateSocioDeletion(id);
            if (!validationResult.Exit)
            {
                return Result<object>.Error(validationResult.Errormessage, validationResult.Errorcode);
            }

            var socio = await _SocioRepository.GetSocioById(id);

            if (socio is null)
            {
                return Result<object>.Error("No se encontró un socio con ese ID.", 404);
            }

            await _SocioRepository.RemoveSocios(socio);

            return Result<object>.Exito(new
            {
                Message = "Socio eliminado correctamente.",
                SocioId = id
            });
        }
        
        public async Task<Result<List<PreviewCuotaDto>>> GetHistorialCuotas(int socioId)
        {
            // Validar ID
            if (socioId <= 0)
                return Result<List<PreviewCuotaDto>>.Error("El ID del socio no es válido.", 400);

            // Buscar socio
            var socio = await _SocioRepository.GetSocioById(socioId);

            if (socio is null)
                return Result<List<PreviewCuotaDto>>.Error("No existe un socio con ese ID.", 404);

            // Obtener cuotas del socio
            var cuotas = await _SocioRepository.GetCuotasSocioById(socioId);

            if (cuotas is null || !cuotas.Any())
                return Result<List<PreviewCuotaDto>>.Exito(new List<PreviewCuotaDto>());

            // Mapear a DTO
            var dto = cuotas.Select(c => new PreviewCuotaDto
            {
                Id = c.Id,
                FechaPago = c.FechaPago,
                Importe = c.Monto,
                MetodoPago = c.FormaDePago.ToString()
            }).ToList();


            return Result<List<PreviewCuotaDto>>.Exito(dto);
        }
    }
}

