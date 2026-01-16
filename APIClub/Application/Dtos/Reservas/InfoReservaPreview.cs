namespace APIClub.Application.Dtos.Reservas
{
    public class InfoReservaPreview
    {
        public int IdReserva { get; set; }
        public string Titulo { get; set; }
        public DateOnly FechaAlquiler { get; set; }
        public decimal Importe { get; set; }
        public string nombreSalon { get; set; }
        public string nombreSocio { get; set; }
        public string apellidoSocio { get; set; }
    }
}
