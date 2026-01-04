namespace APIClub.Application.Dtos.Reservas
{
    public class PreviewReservaBySalonDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateOnly FechaAlquiler { get; set; }
        public bool Pagado { get; set; }
        public string NombreReservante { get; set; }
    }
}
