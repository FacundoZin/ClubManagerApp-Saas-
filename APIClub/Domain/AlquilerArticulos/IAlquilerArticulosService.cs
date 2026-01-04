using APIClub.Application.Common;
using APIClub.Application.Dtos.AlquilerDeArticulos;
using APIClub.Application.Dtos.ItemsAlquiler;

namespace APIClub.Domain.AlquilerArticulos
{
    public interface IAlquilerArticulosService
    {
        Task<Result<AlquilerCreated>> RegistrarAlquiler(CreateAlquilerDto dto);
        Task<Result<object?>> ModificarCantidadItem(int alquilerId, ModifyItemQuantityDto dto);
        Task<Result<object?>> AgregarItemAlquiler(int alquilerId, AddItemToAlquilerDto dto);
        Task<Result<object?>> EliminarItemDeAlquiler(int alquilerId, int itemId);
        Task<Result<PagoAlquilerDto>> RegistrarPago(int idAlquiler);
        Task<Result<object?>> FinalizarAlquiler(int alquilerId);
        Task<Result<AlquilerDto>> GetAlquilerById(int id);
        Task<Result<List<AlquilerPreviewDto>>> GetAlquileresActivos();
        Task<Result<AlquilerPreviewDto?>> GetAlquilerBySocio(string dniSocio);
    }
}
