using APIClub.Application.Dtos.Lote;
using APIClub.Application.Dtos.Socios;

namespace APIClub.Domain.ModuloGestionCobradores.UseCases
{
    public interface IPdfPlanillaCobranzaService
    {
        byte[] GenerarPlanillaDeudores(PreviewLote lote, List<PreviewSocioForCobranzaDto> socios, decimal valorCuota);
    }
}
