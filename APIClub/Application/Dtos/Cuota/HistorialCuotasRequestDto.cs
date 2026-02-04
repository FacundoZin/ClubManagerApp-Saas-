namespace APIClub.Application.Dtos.Cuota
{
    public class HistorialCuotasRequestDto
    {
        public string TipoFiltro { get; set; } = "fecha"; // "fecha" o "periodo"

        // Para filtro por fecha
        public DateOnly? FechaPago { get; set; }

        // Para filtro por periodo
        public int? Anio { get; set; }
        public int? Semestre { get; set; }

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
