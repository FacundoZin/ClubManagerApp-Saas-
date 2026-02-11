using APIClub.Application.Common;
using APIClub.Application.Dtos.AlquilerDeArticulos;
using APIClub.Application.Dtos.ItemsAlquiler;

namespace APIClub.Domain.AlquilerArticulos
{
    public interface IAlquilerArticulosManagmentService
    {
        Task<Result<AlquilerCreated>> RegistrarAlquiler(CreateAlquilerDto dto);
        Task<Result<object?>> ModificarCantidadItem(int alquilerId, ModifyItemQuantityDto dto);
        Task<Result<object?>> AgregarItemAlquiler(int alquilerId, AddItemToAlquilerDto dto);
        Task<Result<object?>> EliminarItemDeAlquiler(int alquilerId, int itemId);
        Task<Result<PagoAlquilerDto>> RegistrarPago(int idAlquiler, int mes, int anio);
        Task<Result<object?>> FinalizarAlquiler(int alquilerId);
    }
}
