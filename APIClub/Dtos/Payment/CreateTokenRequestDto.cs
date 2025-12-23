namespace APIClub.Dtos.Payment
{
    public class CreateTokenRequestDto
    {
        public int IdSocio { get; set; }
        public int anio { get; set; }
        public int semestre { get; set; }
        public decimal monto { get; set; }
    }
}
