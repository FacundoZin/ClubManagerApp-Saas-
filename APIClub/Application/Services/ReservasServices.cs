using APIClub.Application.Common;
using APIClub.Application.Dtos.Reservas;
using APIClub.Application.Helpers;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.ModuloReservasSalones.useCases;
using APIClub.Domain.ReservasSalones.Models;
using APIClub.Domain.ReservasSalones.Repositories;

namespace APIClub.Application.Services
{
    public class ReservasServices : IReservasServices
    {
        private readonly IReservasRepository _ReservasRepository;
        private readonly ISocioRepository _SocioRepository;
        private readonly UnitOfWork _UnitOfWork;

        public ReservasServices(IReservasRepository alquilerRepository, ISocioRepository socioRepository, UnitOfWork unitOfWork)
        {
            _ReservasRepository = alquilerRepository;
            _SocioRepository = socioRepository;
            _UnitOfWork = unitOfWork;   
        }

        public async Task<PagedResult<PreviewReservaBySalonDto>> GetReservasBySalon(int salonId, int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _ReservasRepository.GetAlquileresBySalon(salonId, pageNumber, pageSize);

            var dtos = items.Select(a => new PreviewReservaBySalonDto
            {
                Id = a.Id,
                Titulo = a.Titulo,
                FechaAlquiler = a.FechaAlquiler,
                Pagado = a.Importe == a.TotalPagado ? true : false,
                NombreReservante = $"{a.Socio.Nombre} {a.Socio.Apellido}"
            }).ToList();

            return new PagedResult<PreviewReservaBySalonDto>(dtos, totalCount, pageNumber, pageSize);
        }

        public async Task<Result<FechaDisponible>> GetDisponibilidadFecha(DateOnly fecha, int salon)
        {
            var reserva = await _ReservasRepository.SearchReservaByFecha(fecha, salon);

            FechaDisponible dto = new FechaDisponible();

            if(reserva != null)
            {
                dto.Mensaje = $"El salón no se encuentra disponible en esta fecha: {fecha}";
                dto.Disponible = false;

                return Result<FechaDisponible>.Exito(dto);
            }

            dto.Mensaje = $"El salón se encuentra disponible para la fecha: {fecha}";
            dto.Disponible= true;

            return Result<FechaDisponible>.Exito(dto);  
        }

        public async Task<Result<InfoReservaPreview?>> GetReservaByFechaAndSalon(DateOnly fecha, int salonId)
        {
            var reserva = await _ReservasRepository.SearchReservaByFecha(fecha, salonId);

            if(reserva == null)
            {
                return Result<InfoReservaPreview?>.NotFound("No se encontró ninguna reserva para la fecha y el salón especificados.");
            }

            InfoReservaPreview infoReserva = new InfoReservaPreview();

            infoReserva.IdReserva = reserva.Id;
            infoReserva.Titulo = reserva.Titulo;
            infoReserva.FechaAlquiler = reserva.FechaAlquiler;
            infoReserva.Importe = reserva.Importe;

            infoReserva.nombreSalon = reserva.Salon.Name;

            infoReserva.nombreSocio = reserva.Socio.Nombre;
            infoReserva.apellidoSocio = reserva.Socio.Apellido;


            return Result<InfoReservaPreview?>.Exito(infoReserva);
        }

        public async Task<Result<InfoReservaCompletaDto?>> GetReservaById(int reservaId)
        {
            var reserva = await _ReservasRepository.SearchReservaById(reservaId);

            if (reserva == null)
            {
                return Result<InfoReservaCompletaDto?>.NotFound("No se encontró ninguna reserva con el ID especificado.");
            }

            InfoReservaCompletaDto infoReserva = new InfoReservaCompletaDto();

            infoReserva.IdReserva = reserva.Id;
            infoReserva.Titulo = reserva.Titulo;
            infoReserva.FechaAlquiler = reserva.FechaAlquiler;
            infoReserva.Importe = reserva.Importe;
            infoReserva.Importe = reserva.Importe;
            infoReserva.TotalPagado = reserva.TotalPagado;

            infoReserva.nombreSalon = reserva.Salon.Name;
            infoReserva.direccionSalon = reserva.Salon.Direccion;

            infoReserva.nombreSocio = reserva.Socio.Nombre;
            infoReserva.apellidoSocio = reserva.Socio.Apellido;
            infoReserva.telefonoSocio = reserva.Socio.Telefono?.FormatearForUserVisibility();
            infoReserva.direccionSocio = reserva.Socio.Direcccion;
            infoReserva.localidad = reserva.Socio.Localidad;

            infoReserva.HistorialPagos = reserva.historialPagos.Select(p => new PagoDeReservaDto
            {
                Fecha = p.FechaPago,
                monto = p.monto
            }).ToList();

            return Result<InfoReservaCompletaDto?>.Exito(infoReserva);
        }

        public async Task<Result<object?>> RegistrarReservaSalon(CreteReservaSalonDto dto)
        {

            var disponible = await _ReservasRepository.verificarDisponibilidad(dto.Fecha, dto.SalonId);

            if (!disponible)
                return Result<object?>.Error(
                    "El salón no está disponible en la fecha seleccionada.",
                    409
                );


            var socio = await _SocioRepository.GetSocioByDni(dto.DniSocio);

            if (socio == null) 
                return Result<object?>.Error("el socio que esta cargando para hacer la reserva no existe, verifique si ingreso bien  su dni", 404);

            ReservaSalon reserva = new ReservaSalon();

            reserva.FechaAlquiler = dto.Fecha;
            reserva.Titulo = dto.Titulo;
            reserva.Importe = dto.Importe;
            reserva.TotalPagado = dto.TotalPagado;
            reserva.SocioId = socio.Id;
            reserva.SalonId = dto.SalonId;  
            reserva.IsCancelled = false;

            reserva.historialPagos.Add(new PagoReservaSalon { FechaPago= DateOnly.FromDateTime(DateTime.Now), monto = dto.TotalPagado});

            bool exit = await _ReservasRepository.CrearReserva(reserva);

            if(exit == false)
                return Result<object?>.Error("error al registrar la reserva en nuestra base de datos", 500);

            reservaCreatedDto reservaCreada = new reservaCreatedDto
            {
                idReserva = reserva.Id,
                mensaje = $"se creo exitosamente una reserva para el: {reserva.FechaAlquiler}" 
            };
            return Result<object?>.Exito(reservaCreada);
        }

        public async Task<Result<object?>> CancelarReservas(int idReserva)
        {
            var reserva = await _ReservasRepository.SearchReservaByIdWithTracking(idReserva);

            if (reserva == null) 
                return Result<object?>.Error("No se encontró la reserva que intenta cancelar.", 404);

            reserva.IsCancelled = true;

            await _UnitOfWork.SaveChangesAsync();

            return Result<object?>.Exito(null);
        }

        public async Task<Result<object?>> RegistrarPagoDeSalon(int IdReserva ,decimal montoAbonado)
        {
            var reserva = await _ReservasRepository.SearchReservaByIdWithTracking(IdReserva);

            if (reserva == null) return Result<object?>.Error("la reserva no existe", 404);

            decimal RestaPagar = reserva.Importe - reserva.TotalPagado;

            if (montoAbonado > RestaPagar)
                return Result<object?>.Error("el monto de dinero que quiere abonar es mayor al monto de dinero que resta pagar", 400);            

            reserva.TotalPagado += montoAbonado;
            reserva.historialPagos.Add(new PagoReservaSalon { FechaPago = DateOnly.FromDateTime(DateTime.Now), monto = montoAbonado });

            await _UnitOfWork.SaveChangesAsync();

            return Result<object?>.Exito(null);
        }
    }
}
