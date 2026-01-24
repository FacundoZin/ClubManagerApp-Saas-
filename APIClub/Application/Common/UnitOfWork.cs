using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Domain.Auth.Repositories;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.PaymentsOnline.Repository;
using APIClub.Domain.ReservasSalones.Repositories;
using APIClub.Infrastructure.Persistence.Data;
namespace APIClub.Application.Common
{
    public class UnitOfWork
    {
        private readonly AppDbcontext _context;

        public IAlquilerRepository _AlquilerRepository { get; }
        public IArticuloRepository _ArticuloRepository { get; }
        public ICuotaRepository _CuotaRepository { get; }
        public IitemAlquilerRepository _ItemAlquilerRepository { get; }
        public IReservasRepository _ReservasRepository { get; }
        public ISocioRepository _SocioRepository { get; }
        public IPaymentTokenRepository _PaymentTokenRepository { get; }
        public IUsuariosRepository _UsuariosRepository { get; } 

        public UnitOfWork(IAlquilerRepository alquilerRepository, IArticuloRepository articuloRepository, ICuotaRepository cuotaRepository,
            IitemAlquilerRepository iitemAlquilerRepository, IReservasRepository reservasRepository, ISocioRepository socioRepository,
            IPaymentTokenRepository paymentTokenRepository, AppDbcontext context,
            IUsuariosRepository usuariosRepository)
        {
            _AlquilerRepository = alquilerRepository;
            _ArticuloRepository = articuloRepository;
            _CuotaRepository = cuotaRepository;
            _ItemAlquilerRepository = iitemAlquilerRepository;
            _ReservasRepository = reservasRepository;
            _SocioRepository = socioRepository;
            _PaymentTokenRepository = paymentTokenRepository;
            _UsuariosRepository = usuariosRepository;   
            _context = context;
        }

        public async Task<bool> SaveChangesAsync()
        {
            var changes = await _context.SaveChangesAsync();
            return changes > 0;
        }

    }
}
