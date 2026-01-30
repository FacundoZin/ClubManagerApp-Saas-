namespace APIClub.Application.Dtos.Analiticas
{
    public class MovimientoMensualSociosDto
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int CantidadAltas { get; set; }
        public int CantidadBajas { get; set; }
        public int CantidadDeSocios { get; set; }   
    }
}
