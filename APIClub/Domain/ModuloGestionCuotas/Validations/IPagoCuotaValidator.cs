using APIClub.Application.Common;
using APIClub.Application.Dtos.Socios;
using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.ModuloGestionCuotas.Validations
{
    public interface IPagoCuotaValidator
    {
        Task<Result<Socio>> ValidarPagoEnEstablecimeinto(int idSocio, List<PeriodoAdeudadoDto> periodos);
        Task<Result<Socio>> ValidarPagoConCobrador(int idSocio, int idUsuarioSistema, List<PeriodoAdeudadoDto> periodos);
    }
}
