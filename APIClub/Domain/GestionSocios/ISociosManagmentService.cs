using APIClub.Common;
using APIClub.Dtos.Cuota;
using APIClub.Dtos.Socios;

namespace APIClub.Domain.GestionSocios
{
    public interface ISociosManagmentService
    {
        Task<Result<PagedResult<PreviewSocioDto>>> GetSociosDeudores(int pageNumber, int pageSize);
        Task<Result<CreatedSocio>> cargarSocio(CreateSocioDto _dto);
        Task<Result<PreviewSocioDto>> GetSocioByDni(string dni);
        Task<Result<object>> UpdateSocio(int id, CreateSocioDto dto);
        Task<Result<object>> RemoveSocio(int id);
        Task<Result<object>> ReactivarSocio(int id);
        Task<Result<PreviewSocioDto>> GetSocioById(int id);
        Task<Result<FullSocioDto>> GetFullSocioById(int id);
        Task<Result<List<PreviewCuotaDto>>> GetHistorialCuotas(int socioId);

    }
}
