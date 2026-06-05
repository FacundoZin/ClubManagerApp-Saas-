namespace APIClub.Domain.ModuloGestionViajes.Models;

public class FileViaje
{
    public int Id { get; set; }

    public string NumeroFile { get; set; } = string.Empty;

    public int ViajeId { get; set; }

    public Viaje Viaje { get; set; } = null!;

    public List<InscriptoViaje> Inscriptos { get; set; } = new();
}
