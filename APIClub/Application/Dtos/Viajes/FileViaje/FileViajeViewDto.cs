namespace APIClub.Application.Dtos.Viajes.FileViaje
{
    public class FileViajeViewDto
    {
        public int Id { get; set; }
        public string NumeroFile { get; set; }
        public int ViajeId { get; set; }
        public List<int> InscriptosIds { get; set; } = new();
    }
}