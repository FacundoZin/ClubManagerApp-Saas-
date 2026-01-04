using APIClub.Application.Common;
using APIClub.Application.Dtos.Socios;

namespace APIClub.Domain.GestionSocios
{
    public interface ICobranzasServices
    {
        Task<Result<List<PreviewSocioForCobranzaDto>>> ListarSociosDedudoresPorLote(string lote); 
    }
}
