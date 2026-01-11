using APIClub.Application.Common;
using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.GestionSocios
{
    public interface ICobranzasServices
    {
        Task<Result<List<PreviewSocioForCobranzaDto>>> ListarSociosDedudoresPorLote(int Idlote);
        Task<List<PreviewLote>> GetLotesPreview();
        Task<Result<bool>> CrearLote(CreateLoteDto dto);
    }
}
