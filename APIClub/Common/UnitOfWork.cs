using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.ReservasSalones.Repositories;
using APIClub.Repositorio;

namespace APIClub.Common
{
    public class UnitOfWork
    {
        public IAlquilerRepository _AlquilerRepository { get; }
        public IArticuloRepository _ArticuloRepository { get; }
        public ICuotaRepository _CuotaRepository { get; }
        public IitemAlquilerRepository _ItemAlquilerRepository { get; }
        public IReservasRepository _ReservasRepository { get; }
        public ISocioRepository _SocioRepository { get; }

        public UnitOfWork(IAlquilerRepository alquilerRepository, IArticuloRepository articuloRepository, ICuotaRepository cuotaRepository,
            IitemAlquilerRepository iitemAlquilerRepository, IReservasRepository reservasRepository, ISocioRepository socioRepository)
        {
            _AlquilerRepository = alquilerRepository;
            _ArticuloRepository = articuloRepository;
            _CuotaRepository = cuotaRepository;
            _ItemAlquilerRepository = iitemAlquilerRepository;
            _ReservasRepository = reservasRepository;
            _SocioRepository = socioRepository;
        }
    }
}
