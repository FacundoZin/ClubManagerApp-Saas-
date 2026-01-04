namespace APIClub.Application.Dtos.Payment
{
    public class CreateTokenRequestDto
    {
        public string nombreSocio { get; set; }
        public int IdSocio { get; set; }
        public int anio { get; set; }
        public int semestre { get; set; }
        public decimal monto { get; set; }
    }
}
