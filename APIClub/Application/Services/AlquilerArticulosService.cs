using APIClub.Application.Common;
using APIClub.Application.Dtos.AlquilerDeArticulos;
using APIClub.Application.Dtos.ItemsAlquiler;
using APIClub.Domain.AlquilerArticulos;
using APIClub.Domain.AlquilerArticulos.Models;

namespace APIClub.Application.Services
{
    public class AlquilerArticulosService : IAlquilerArticulosService
    {
        private readonly UnitOfWork _UnitOfWork;


        public AlquilerArticulosService(UnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;   
        }

        public async Task<Result<AlquilerCreated>> RegistrarAlquiler(CreateAlquilerDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return Result<AlquilerCreated>.Error("Debe incluir al menos un artículo", 400);

            var socio = await _UnitOfWork._SocioRepository.GetSocioById(dto.idSocio);
             
            if (socio == null)
                return Result<AlquilerCreated>.Error("No existe un socio con ese id, seleccione correctamente un socio antes de registar el alquiler", 404);

            var alquiler = new Alquiler
            {
                FechaAlquiler = DateOnly.FromDateTime(DateTime.Now),
                Observaciones = dto.Observaciones,
                IdSocio = socio.Id,
                Finalizado = false
            };

            foreach (var item in dto.Items)
            {
                alquiler.Items.Add(new ItemAlquiler
                {
                    ArticuloId = item.ArticuloId,
                    Cantidad = item.Cantidad
                });
            }

            await _UnitOfWork._AlquilerRepository.CrearAlquiler(alquiler);

            return Result<AlquilerCreated>.Exito(new AlquilerCreated { idAlquiler = alquiler.Id, Fecha = alquiler.FechaAlquiler});
        }

        public async Task<Result<object?>> ModificarCantidadItem(int alquilerId, ModifyItemQuantityDto dto)
        {
            var alquiler = await _UnitOfWork._AlquilerRepository.GetAlquilerByIdWithDetails(alquilerId);

            if (alquiler == null)
                return Result<object?>.Error("No se encontró un alquiler con ese ID", 404);

            var itemAlquiler = await _UnitOfWork._ItemAlquilerRepository.GetItemAlquilerById(dto.ItemId);

            if (itemAlquiler == null)
                return Result<object?>.Error("No se encontró el artículo en el alquiler", 404);

            if (itemAlquiler.AlquilerId != alquilerId)
                return Result<object?>.Error("El artículo no pertenece a este alquiler", 400);

            itemAlquiler.Cantidad = dto.NuevaCantidad;

            var exit = await _UnitOfWork._ItemAlquilerRepository.UpdateItemAlquiler(itemAlquiler);


            if (!exit)
                return Result<object?>.Error("Error al moficiar la cantidad", 500);

            return Result<object?>.Exito(null);
        }

        public async Task<Result<object?>> AgregarItemAlquiler(int alquilerId, AddItemToAlquilerDto dto)
        {
            var alquiler = await _UnitOfWork._AlquilerRepository.GetAlquilerByIdWithDetails(alquilerId);

            if (alquiler == null)
                return Result<object?>.Error("No se encontró un alquiler con ese ID", 404);

            if (alquiler.Finalizado)
                return Result<object?>.Error("El alquiler ya está finalizado", 400);

            var articulo = await _UnitOfWork._ArticuloRepository.GetArticuloById(dto.ArticuloId);

            if (articulo == null)
                 return Result<object?>.Error("No se encontró un artículo con ese ID", 404);

            if (alquiler.Items.Any(i => i.ArticuloId == dto.ArticuloId))
                return Result<object?>.Error("El artículo ya existe en el alquiler", 400);

            var item = new ItemAlquiler
            {
                AlquilerId = alquilerId,
                ArticuloId = dto.ArticuloId,
                Cantidad = dto.Cantidad
            };

            await _UnitOfWork._AlquilerRepository.AgregarItem(item);

            return Result<object?>.Exito(null);
        }

        public async Task<Result<object?>> EliminarItemDeAlquiler(int alquilerId, int itemId)
        {
            var alquiler = await _UnitOfWork._AlquilerRepository.GetAlquilerByIdWithDetails(alquilerId);

            if (alquiler == null)
                return Result<object?>.Error("No se encontró un alquiler con ese ID", 404);

            if (alquiler.Items.Count <= 1)
                return Result<object?>.Error("si elimina este item del alquiler, el mismo quedara vacio, directamente deberia marcar como finalizado el alquiler", 400);

            var item = await _UnitOfWork._ItemAlquilerRepository.GetItemAlquilerById(itemId);

            if (item == null)
                return Result<object?>.Error("No se encontró el artículo del alquiler que quiere eliminar", 404);

            if (item.AlquilerId != alquilerId)
                return Result<object?>.Error("El artículo no pertenece a este alquiler", 400);

            var exit = await _UnitOfWork._ItemAlquilerRepository.RemoveItemAlquiler(item);  

            if (!exit)
                return Result<object?>.Error("Error al eliminar el articulo del alquiler", 500); 

            return Result<object?>.Exito(null);
        }

        public async Task<Result<PagoAlquilerDto>> RegistrarPago(int idAlquiler)
        {
            var alquiler = await _UnitOfWork._AlquilerRepository.GetAlquilerByIdWithDetails(idAlquiler);

            if (alquiler == null)
                return Result<PagoAlquilerDto>.Error("No se encontró un alquiler con ese ID", 404);

            int monto = await _UnitOfWork._AlquilerRepository.CalcularMontoAlquiler(idAlquiler);
            var dateHoy = DateOnly.FromDateTime(DateTime.Now);

            var pago = new PagoAlquilerDeArticulos
            {
                Anio = dateHoy.Year,
                Mes = dateHoy.Month,
                Monto = monto,
                IdAlquiler = idAlquiler
            };

            await _UnitOfWork._AlquilerRepository.RegistrarPago(pago);

            var pagoDto = new PagoAlquilerDto
            {
                Id = pago.Id,
                Mes = pago.Mes,
                Anio = pago.Anio,
                Monto = pago.Monto
            };

            return Result<PagoAlquilerDto>.Exito(pagoDto);
        }

        public async Task<Result<object?>> FinalizarAlquiler(int alquilerId)
        {
            var alquiler = await _UnitOfWork._AlquilerRepository.GetAlquilerByIdWithDetails(alquilerId);

            if (alquiler == null)
                return Result<object?>.Error("No se encontró un alquiler con ese ID", 404);

            if (alquiler.Finalizado)
                return Result<object?>.Error("El alquiler ya está finalizado", 400);

            alquiler.Finalizado = true;
            var exit = await _UnitOfWork._AlquilerRepository.UpdateAlquiler(alquiler);

            if (!exit)
                return Result<object?>.Error("Error al modicar el estado del alquiler", 500);

            return Result<object?>.Exito(null);
        }

        public async Task<Result<AlquilerDto>> GetAlquilerById(int id)
        {
            var alquiler = await _UnitOfWork._AlquilerRepository.GetAlquilerByIdWithDetails(id);

            if (alquiler == null)
                return Result<AlquilerDto>.Error("No se encontró un alquiler con ese ID", 404);

            var alquilerDto = MapearAlquilerADto(alquiler);

            return Result<AlquilerDto>.Exito(alquilerDto);
        }

        public async Task<Result<List<AlquilerPreviewDto>>> GetAlquileresActivos()
        {
            var alquileres = await _UnitOfWork._AlquilerRepository.GetAlquileresActivos();

            return Result<List<AlquilerPreviewDto>>.Exito(alquileres);
        }

        public async Task<Result<AlquilerPreviewDto?>> GetAlquilerBySocio(string dniSocio)
        {
            var socio = await _UnitOfWork._SocioRepository.GetSocioByDni(dniSocio);

            if (socio == null)
                return Result<AlquilerPreviewDto?>.Error("No existe un socio en el sistema con ese DNI", 404);

            var alquiler = await _UnitOfWork._AlquilerRepository.GetAlquilerBySocio(socio.Id);

            if (alquiler == null) return Result<AlquilerPreviewDto?>.NotFound("el socio que esta buscando no tiene alquileres registados actualmente.");

            var hoy = DateOnly.FromDateTime(DateTime.Today);

            var dto = new AlquilerPreviewDto
            {
                Id = alquiler.Id,
                FechaAlquiler = alquiler.FechaAlquiler,

                NombreSocio = socio.Nombre,
                ApellidoSocio = socio.Apellido,
                DniSocio = socio.Dni,
                TelefonoSocio = socio.Telefono,
                DireccionSocio = socio.Direcccion,
                LocalidadSocio = socio.Localidad,

                estaAlDia = alquiler.HistorialDePagos.Any(p =>
                    p.Anio == hoy.Year &&
                    p.Mes == hoy.Month
                    )
            };

            return Result<AlquilerPreviewDto?>.Exito(dto);
        }

        private AlquilerDto MapearAlquilerADto(Alquiler alquiler)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);

            return new AlquilerDto
            {
                Id = alquiler.Id,
                FechaAlquiler = alquiler.FechaAlquiler,
                Observaciones = alquiler.Observaciones,

                estaAlDia = alquiler.HistorialDePagos.Any(p =>
                    p.Anio == hoy.Year &&
                    p.Anio == hoy.Year
                    ),

                IdSocio = alquiler.IdSocio,
                NombreSocio = alquiler.Socio.Nombre,
                ApellidoSocio = alquiler.Socio.Apellido,
                DniSocio = alquiler.Socio.Dni,
                TelefonoSocio = alquiler.Socio.Telefono ?? null,
                DireccionSocio = alquiler.Socio.Direcccion ?? null,
                LocalidadSocio = alquiler.Socio.Localidad ?? null,


                Items = alquiler.Items.Select(i => new DetailsItemAlquilerDto
                {
                    Id = i.Id,
                    ArticuloId = i.ArticuloId,
                    NombreArticulo = i.Articulo.Nombre,
                    PrecioAlquiler = i.Articulo.PrecioAlquiler,
                    Cantidad = i.Cantidad,
                    Subtotal = i.Articulo.PrecioAlquiler * i.Cantidad
                }).ToList(),

                HistorialDePagos = alquiler.HistorialDePagos.Select(p => new PagoAlquilerDto
                {
                    Id = p.Id,
                    Mes = p.Mes,
                    Anio = p.Anio,
                    Monto = p.Monto
                }).ToList(),
            };
        }
    }
}
