using APIClub.Application.Common;
using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.GestionSocios
{
    public interface ICobranzasServices
    {
        Task<Result<PagedResult<PreviewSocioForCobranzaDto>>> ListarSociosDedudoresPorLote(int Idlote, int pageNumber = 1, int pageSize = 10);
        Task<List<PreviewLote>> GetLotesPreview();
        Task<Result<bool>> CrearLote(CreateLoteDto dto);
    }
}
