using APIClub.Domain.ModuloGestionViajes.Models;

namespace APIClub.Domain.ModuloGestionViajes.Repositories
{
    public interface IViajeWriteRepository
    {

        Task CreateViaje(Viaje viaje);
        Task CreateVarianteViaje(VarianteViaje variante);
    }
}
