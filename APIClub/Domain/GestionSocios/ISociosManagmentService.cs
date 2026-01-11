using APIClub.Application.Common;
using APIClub.Application.Dtos.Cuota;
using APIClub.Application.Dtos.Socios;

namespace APIClub.Domain.GestionSocios
{
    public interface ISociosManagmentService
    {
        Task<Result<PagedResult<SocioCardDto>>> GetSociosDeudores(int pageNumber, int pageSize);
        Task<Result<ExistingSocio>> cargarSocio(CreateSocioDto _dto);
        Task<Result<PreviewSocioDto>> GetSocioByDni(string dni);
        Task<Result<object>> UpdateSocio(int id, UpdateSocio dto);
        Task<Result<object>> RemoveSocio(int id);
        Task<Result<object>> ReactivarSocio(int id);
        Task<Result<PreviewSocioDto>> GetSocioById(int id);
        Task<Result<FullSocioDto>> GetFullSocioById(int id);
        Task<Result<List<PreviewCuotaDto>>> GetHistorialCuotas(int socioId);

    }
}
