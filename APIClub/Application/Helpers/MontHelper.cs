namespace APIClub.Application.Helpers
{
    public static class MontHelper
    {
        public static string MapMonthToText(int month)
        {
            string[] meses =
            {
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Agosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Diciembre"
            };

            return meses[month - 1];
        }
    }
}
