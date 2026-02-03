namespace APIClub.Application.Dtos.Socios
{
    public class PreviewSocioForCobranzaDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public string? Telefono { get; set; }
        public string? Direcccion { get; set; }
        public List<PeriodoAdeudadoDto> PeriodosAdeudados { get; set; } = new List<PeriodoAdeudadoDto>();
    }
}
