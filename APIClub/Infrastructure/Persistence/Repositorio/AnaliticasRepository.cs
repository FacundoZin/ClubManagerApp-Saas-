using APIClub.Application.Dtos.Analiticas;
using APIClub.Domain.Analiticas;
using APIClub.Domain.Enums;
using APIClub.Infrastructure.Persistence.Data;
using APIClub.Domain.ReservasSalones.Models; // Solves PagoReservaSalon
using Microsoft.EntityFrameworkCore;
using APIClub.Domain.AlquilerArticulos.Models;
using APIClub.Domain.ModuloGestionCuotas.Models;

namespace APIClub.Infrastructure.Persistence.Repositorio
{
    public class AnaliticasRepository : IAnaliticasRepository
    {
        private readonly AppDbcontext _context;

        public AnaliticasRepository(AppDbcontext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------------------
        // SOCIOS
        // ---------------------------------------------------------------------

        public async Task<int> GetSociosActivosCount()
        {
            // Usa el filtro global IsActivo = true
            return await _context.Socios.CountAsync();
        }

        public async Task<int> GetAltasByPeriodo(int anio, int? mes, int? semestre)
        {
            var query = _context.Socios.AsQueryable();

            query = query.Where(s => s.FechaAsociacion.Year == anio);

            if (mes.HasValue)
            {
                query = query.Where(s => s.FechaAsociacion.Month == mes.Value);
            }
            else if (semestre.HasValue)
            {
                if (semestre.Value == 1)
                    query = query.Where(s => s.FechaAsociacion.Month <= 6);
                else
                    query = query.Where(s => s.FechaAsociacion.Month > 6);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetBajasByPeriodo(int anio, int? mes, int? semestre)
        {
            // Ignoramos filtro global para ver los inactivos
            var query = _context.Socios.IgnoreQueryFilters()
                .Where(s => !s.IsActivo && s.FechaDeBaja != null);

            // Filtramos por fecha de baja
            if (mes.HasValue)
            {
                query = query.Where(s => s.FechaDeBaja!.Value.Year == anio && s.FechaDeBaja!.Value.Month == mes.Value);
            }
            else if (semestre.HasValue)
            {
                if (semestre.Value == 1)
                    query = query.Where(s => s.FechaDeBaja!.Value.Year == anio && s.FechaDeBaja!.Value.Month <= 6);
                else
                    query = query.Where(s => s.FechaDeBaja!.Value.Year == anio && s.FechaDeBaja!.Value.Month > 6);
            }
            else
            {
                query = query.Where(s => s.FechaDeBaja!.Value.Year == anio);
            }

            return await query.CountAsync();
        }

        public async Task<int[]> GetAltasByMes(int anio, int? semestre)
        {
            var query = _context.Socios.AsQueryable();

            // Aplicar filtros de año/semestre (sin mes específico para poder agrupar)
            query = query.Where(s => s.FechaAsociacion.Year == anio);

            if (semestre.HasValue)
            {
                if (semestre.Value == 1)
                    query = query.Where(s => s.FechaAsociacion.Month <= 6);
                else
                    query = query.Where(s => s.FechaAsociacion.Month > 6);
            }

            var AltasPorMes = await query
                .GroupBy(s => s.FechaAsociacion.Month)
                .Select(g => new { Mes = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            // Crear array de 12 meses
            int[] resultado = new int[12];
            foreach (var alta in AltasPorMes)
            {
                resultado[alta.Mes - 1] = alta.Cantidad; // Mes 1 -> índice 0
            }

            return resultado;
        }

        public async Task<int[]> GetBajasByMes(int anio, int? semestre)
        {
            var query = _context.Socios.IgnoreQueryFilters()
                .Where(s => !s.IsActivo && s.FechaDeBaja != null);

            query = query.Where(s => s.FechaDeBaja!.Value.Year == anio);

            if (semestre.HasValue)
            {
                if (semestre.Value == 1)
                    query = query.Where(s => s.FechaDeBaja!.Value.Month <= 6);
                else
                    query = query.Where(s => s.FechaDeBaja!.Value.Month > 6);
            }

            var BajasPorMes = await query
                .GroupBy(s => s.FechaDeBaja!.Value.Month)
                .Select(g => new { Mes = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            // Crear array de 12 meses
            int[] resultado = new int[12];
            foreach (var baja in BajasPorMes)
            {
                resultado[baja.Mes - 1] = baja.Cantidad; // Mes 1 -> índice 0
            }

            return resultado;
        }

        public async Task<int[]> GetCantidadSociosByMes(int anio, int? semestre)
        {
            // Determinar el rango de meses a calcular
            int mesInicio = semestre.HasValue ? (semestre.Value == 1 ? 1 : 7) : 1;
            int mesFin = semestre.HasValue ? (semestre.Value == 1 ? 6 : 12) : 12;

            // Obtener la cantidad de socios activos al inicio del periodo
            // (socios que se dieron de alta antes del periodo y no se dieron de baja antes del periodo)
            DateOnly fechaInicioPeriodo = new DateOnly(anio, mesInicio, 1);

            int sociosIniciales = await _context.Socios.IgnoreQueryFilters()
                .Where(s => s.FechaAsociacion < fechaInicioPeriodo &&
                           (s.IsActivo || (s.FechaDeBaja.HasValue && s.FechaDeBaja.Value >= fechaInicioPeriodo)))
                .CountAsync();

            // Obtener altas y bajas por mes
            var altasPorMes = await GetAltasByMes(anio, semestre);
            var bajasPorMes = await GetBajasByMes(anio, semestre);

            // Calcular cantidad de socios mes a mes
            int[] resultado = new int[12];
            int cantidadActual = sociosIniciales;

            for (int mes = mesInicio; mes <= mesFin; mes++)
            {
                // Sumar altas del mes
                cantidadActual += altasPorMes[mes - 1];
                // Restar bajas del mes
                cantidadActual -= bajasPorMes[mes - 1];
                // Guardar el resultado
                resultado[mes - 1] = cantidadActual;
            }

            return resultado;
        }

        public async Task<decimal> GetTasaMorosidad(int anio, int semestre)
        {
            // Total de socios activos que deberían pagar
            int totalSocios = await _context.Socios.CountAsync();

            if (totalSocios == 0) return 0;

            // Socios DEUDORES = Socios Activos que NO tienen cuota paga para el periodo dado
            int sociosAlDia = await _context.Socios
                .Where(s => s.HistorialCuotas.Any(c => c.Anio == anio && c.Semestre == semestre))
                .CountAsync();

            int sociosDeudores = totalSocios - sociosAlDia;

            return Math.Round(((decimal)sociosDeudores / totalSocios) * 100, 2);
        }


        // ---------------------------------------------------------------------
        // INGRESOS
        // ---------------------------------------------------------------------

        public async Task<ResumenIngresosDto> GetResumenIngresos(int anio, int? mes, int? semestre)
        {
            var dto = new ResumenIngresosDto();

            // 1. Cuotas
            var queryPagosCuotas = _context.Cuotas.AsQueryable();
            queryPagosCuotas = ApplyFilter_Cuotas(queryPagosCuotas, anio, mes, semestre);
            dto.TotalCuotas = Math.Round((decimal)await queryPagosCuotas.SumAsync(c => (double)c.Monto), 2);

            // 2. Alquileres
            var queryPagosAlquileres = _context.PagosAlquilerDeArticulos.AsQueryable();
            queryPagosAlquileres = ApplyFilter_PagoAlquiler(queryPagosAlquileres, anio, mes, semestre);
            dto.TotalAlquilerArticulos = Math.Round((decimal)await queryPagosAlquileres.SumAsync(p => (double)p.Monto), 2);

            // 3. Reservas
            var queryPagosReservas = _context.pagoReservaSalon.AsQueryable();
            queryPagosReservas = ApplyFilter_PagoReserva(queryPagosReservas, anio, mes, semestre);
            dto.TotalReservasSalones = Math.Round((decimal)await queryPagosReservas.SumAsync(p => (double)p.monto), 2);

            dto.TotalGeneral = Math.Round(dto.TotalCuotas + dto.TotalAlquilerArticulos + dto.TotalReservasSalones, 2);

            return dto;
        }

        public async Task<List<IngresoMensualDto>> GetIngresosMesAMes(int anio, int? semestre)
        {
            // Necesitamos agrupar mes a mes. Haremos 3 consultas agrupadas y luego uniremos en memoria.

            // Cuotas Por Mes
            var qCuotas = _context.Cuotas.Where(c => c.FechaPago.Year == anio);
            if (semestre.HasValue) qCuotas = semestre == 1 ? qCuotas.Where(c => c.FechaPago.Month <= 6) : qCuotas.Where(c => c.FechaPago.Month > 6);

            var cuotasGroup = await qCuotas
                .GroupBy(c => c.FechaPago.Month)
                .Select(g => new { Mes = g.Key, Total = Math.Round((decimal)g.Sum(c => (double)c.Monto), 2) })
                .ToListAsync();

            // Alquileres Por Mes (PagoAlquilerDeArticulos tiene Anio/Mes columnas explícitas)
            var qAlq = _context.PagosAlquilerDeArticulos.Where(p => p.Anio == anio);
            if (semestre.HasValue) qAlq = semestre == 1 ? qAlq.Where(p => p.Mes <= 6) : qAlq.Where(p => p.Mes > 6);

            var alqGroup = await qAlq
                 .GroupBy(p => p.Mes)
                 .Select(g => new { Mes = g.Key, Total = Math.Round((decimal)g.Sum(p => (double)p.Monto), 2) })
                 .ToListAsync();

            // Reservas Por Mes
            var qRes = _context.pagoReservaSalon.Where(p => p.FechaPago.Year == anio);
            if (semestre.HasValue) qRes = semestre == 1 ? qRes.Where(p => p.FechaPago.Month <= 6) : qRes.Where(p => p.FechaPago.Month > 6);

            var resGroup = await qRes
                .GroupBy(p => p.FechaPago.Month)
                .Select(g => new { Mes = g.Key, Total = Math.Round((decimal)g.Sum(p => (double)p.monto), 2) })
                .ToListAsync();


            // Unificar
            var result = new List<IngresoMensualDto>();

            // Determinar el rango de meses a procesar para asegurar que el gráfico tenga todos los puntos
            int mesInicio = semestre.HasValue ? (semestre.Value == 1 ? 1 : 7) : 1;
            int mesFin = semestre.HasValue ? (semestre.Value == 1 ? 6 : 12) : 12;

            for (int mes = mesInicio; mes <= mesFin; mes++)
            {
                var row = new IngresoMensualDto
                {
                    Anio = anio,
                    Mes = mes,
                    Cuotas = cuotasGroup.FirstOrDefault(x => x.Mes == mes)?.Total ?? 0,
                    AlquilerArticulos = alqGroup.FirstOrDefault(x => x.Mes == mes)?.Total ?? 0,
                    ReservasSalones = resGroup.FirstOrDefault(x => x.Mes == mes)?.Total ?? 0
                };
                row.Total = Math.Round(row.Cuotas + row.AlquilerArticulos + row.ReservasSalones, 2);
                result.Add(row);
            }

            return result;
        }


        // ---------------------------------------------------------------------
        // DISTRIBUCIÓN PAGO
        // ---------------------------------------------------------------------

        public async Task<DistribucionFormaPagoDto> GetDistribucionFormaPago(int anio, int? semestre)
        {
            // Obtenemos los socios que tienen cuotas pagadas en el periodo especificado
            var queryCuotas = _context.Cuotas.AsQueryable();

            // Filtrar por año
            queryCuotas = queryCuotas.Where(c => c.FechaPago.Year == anio);

            // Filtrar por semestre si se especifica
            if (semestre.HasValue)
            {
                if (semestre.Value == 1)
                    queryCuotas = queryCuotas.Where(c => c.FechaPago.Month <= 6);
                else
                    queryCuotas = queryCuotas.Where(c => c.FechaPago.Month > 6);
            }

            // Obtener los socios únicos que pagaron en el periodo y agrupar por forma de pago
            var dist = await queryCuotas
                .Select(c => c.Socio!)
                .Distinct()
                .GroupBy(s => s.PreferenciaDePago)
                .Select(g => new { Metodo = g.Key, Count = g.Count() })
                .ToListAsync();

            // Calcular el total de socios que pagaron en el periodo
            int totalSocios = dist.Sum(x => x.Count);

            // Si no hay pagos en el periodo, devolver todos en 0
            if (totalSocios == 0)
            {
                return new DistribucionFormaPagoDto
                {
                    Cobrador = 0,
                    LinkDePago = 0,
                    Sede = 0
                };
            }

            // Calcular porcentajes
            var dto = new DistribucionFormaPagoDto
            {
                Cobrador = Math.Round((decimal)(dist.FirstOrDefault(x => x.Metodo == MetodosDePago.Cobrador)?.Count ?? 0) / totalSocios * 100, 2),
                LinkDePago = Math.Round((decimal)(dist.FirstOrDefault(x => x.Metodo == MetodosDePago.LinkDePago)?.Count ?? 0) / totalSocios * 100, 2),
                Sede = Math.Round((decimal)(dist.FirstOrDefault(x => x.Metodo == MetodosDePago.Sede)?.Count ?? 0) / totalSocios * 100, 2)
            };

            return dto;
        }

        // ---------------------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------------------

        private IQueryable<Cuota> ApplyFilter_Cuotas(IQueryable<Cuota> query, int anio, int? mes, int? semestre)
        {
            query = query.Where(c => c.FechaPago.Year == anio);
            if (mes.HasValue) query = query.Where(c => c.FechaPago.Month == mes.Value);
            else if (semestre.HasValue)
            {
                if (semestre.Value == 1) query = query.Where(c => c.FechaPago.Month <= 6);
                else query = query.Where(c => c.FechaPago.Month > 6);
            }
            return query;
        }

        private IQueryable<PagoReservaSalon> ApplyFilter_PagoReserva(IQueryable<PagoReservaSalon> query, int anio, int? mes, int? semestre)
        {
            query = query.Where(p => p.FechaPago.Year == anio);
            if (mes.HasValue) query = query.Where(p => p.FechaPago.Month == mes.Value);
            else if (semestre.HasValue)
            {
                if (semestre.Value == 1) query = query.Where(p => p.FechaPago.Month <= 6);
                else query = query.Where(p => p.FechaPago.Month > 6);
            }
            return query;
        }

        private IQueryable<PagoAlquilerDeArticulos> ApplyFilter_PagoAlquiler(IQueryable<PagoAlquilerDeArticulos> query, int anio, int? mes, int? semestre)
        {
            if (mes.HasValue)
                query = query.Where(p => p.Anio == anio && p.Mes == mes.Value);
            else if (semestre.HasValue)
            {
                if (semestre.Value == 1) query = query.Where(p => p.Anio == anio && p.Mes <= 6);
                else query = query.Where(p => p.Anio == anio && p.Mes > 6);
            }
            else query = query.Where(p => p.Anio == anio);
            return query;
        }
    }
}
