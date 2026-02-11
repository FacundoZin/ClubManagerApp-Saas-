using APIClub.Application.Common;
using APIClub.Application.Dtos.Cobrador;
using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;

namespace APIClub.Domain.ModuloGestionCobradores.UseCases
{
    public interface ICobranzasServices
    {
        Task<Result<PagedResult<PreviewSocioForCobranzaDto>>> ListarSociosDedudoresPorLote(int Idlote, int pageNumber = 1, int pageSize = 10);
        Task<List<PreviewLote>> GetLotesPreview();
        Task<Result<bool>> CrearLote(CreateLoteDto dto);
        Task<List<CobradorDto>> GetListaCobradores();
        Task<HistorialCobradorDto> GetHistorialCobradorByMes(int idCobrador, int mes, int anio);
    }
}
