using APIClub.Application.Common;
using APIClub.Domain.GestionSocios.Models;

namespace APIClub.Domain.GestionSocios.Validations
{
    public interface IPagoCuotaValidator
    {
        Task<Result<Socio>> ValidarPagoEnEstablecimeinto(int idSocio);
        Task<Result<Socio>> ValidarPagoConCobrador(int idSocio, int idUsuarioSistema);
    }
}
