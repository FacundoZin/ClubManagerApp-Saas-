using APIClub.Domain.Enums;

namespace APIClub.Application.Dtos.Cuota
{
    public class CuotaConSocioDto
    {
        public int Id { get; set; }
        public DateOnly FechaPago { get; set; }
        public decimal Monto { get; set; }
        public MetodosDePago FormaDePago { get; set; }
        public int Anio { get; set; }
        public int Semestre { get; set; }

        // Datos del socio
        public string NombreSocio { get; set; } = string.Empty;
        public string ApellidoSocio { get; set; } = string.Empty;
    }
}
