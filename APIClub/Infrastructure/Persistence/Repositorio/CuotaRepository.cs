using APIClub.Application.Common;
using APIClub.Application.Dtos.Cuota;
using APIClub.Domain.GestionSocios.Models;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.ModuloGestionCuotas.Models;
using APIClub.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class CuotaRepository : ICuotaRepository
    {
        private readonly AppDbcontext _dbcontext;

        public CuotaRepository(AppDbcontext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<decimal> ActualizarValorCuota(decimal valor, DateTime FechaActualizacion)
        {
            var nuevoMonto = new MontoCuota { MontoCuotaFija = valor, FechaActualizacion = FechaActualizacion };

            _dbcontext.MontoCuota.Add(nuevoMonto);
            await _dbcontext.SaveChangesAsync();

            return nuevoMonto.MontoCuotaFija;
        }

        public async Task<decimal> ObtenerValorCuota()
        {
            var valorCuota = await _dbcontext.MontoCuota
                .OrderByDescending(cuota => cuota.FechaActualizacion)
                .Select(cuota => cuota.MontoCuotaFija)
                .FirstOrDefaultAsync();

            return valorCuota;
        }

        public void RegistrarCuotas(IEnumerable<Cuota> cuotas)
        {
            _dbcontext.Cuotas.AddRange(cuotas);
        }

        public async Task<PagedResult<CuotaConSocioDto>> ObtenerCuotasPorFechaPago(DateOnly fechaPago, int pageNumber, int pageSize)
        {
            var query = _dbcontext.Cuotas
                .Include(c => c.Socio)
                .Where(c => c.FechaPago == fechaPago)
                .OrderByDescending(c => c.Id);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CuotaConSocioDto
                {
                    Id = c.Id,
                    FechaPago = c.FechaPago,
                    Monto = c.Monto,
                    FormaDePago = c.FormaDePago,
                    Anio = c.Anio,
                    Semestre = c.Semestre,
                    NombreSocio = c.Socio!.Nombre,
                    ApellidoSocio = c.Socio!.Apellido
                })
                .ToListAsync();

            return new PagedResult<CuotaConSocioDto>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<CuotaConSocioDto>> ObtenerCuotasPorPeriodo(int anio, int semestre, int pageNumber, int pageSize)
        {
            var query = _dbcontext.Cuotas
                .Include(c => c.Socio)
                .Where(c => c.Anio == anio && c.Semestre == semestre)
                .OrderByDescending(c => c.FechaPago)
                .ThenByDescending(c => c.Id);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CuotaConSocioDto
                {
                    Id = c.Id,
                    FechaPago = c.FechaPago,
                    Monto = c.Monto,
                    FormaDePago = c.FormaDePago,
                    Anio = c.Anio,
                    Semestre = c.Semestre,
                    NombreSocio = c.Socio!.Nombre,
                    ApellidoSocio = c.Socio!.Apellido
                })
                .ToListAsync();

            return new PagedResult<CuotaConSocioDto>(items, totalCount, pageNumber, pageSize);
        }
    }
}
