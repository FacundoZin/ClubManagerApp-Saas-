namespace APIClub.Application.Helpers
{
    public static class PaymentDescriptionHelper
    {
        public static string GetSemestreText(this int semestre)
        {
            return semestre == 1 ? "primer semestre" : "segundo semestre";
        }

        public static string BuildCuotaDescription(int semestre, int anio)
        {
            return $"cuota socio club de abuelos, correspondiente al {GetSemestreText(semestre)} del año {anio}";
        }

        public static string BuildCuotaDescriptionShort(int semestre, int anio)
        {
            return $"cuota club abuelos correspondiente al {GetSemestreText(semestre)} del año {anio}";
        }
    }
}

