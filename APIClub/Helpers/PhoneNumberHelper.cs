namespace APIClub.Helpers
{
    public static class PhoneNumberHelper
    {

        public static string FormatearForWhatsapp(this string telefono)
        {

            var numeroFormateado = new string(telefono.Where(char.IsDigit).ToArray());

            if (!numeroFormateado.StartsWith("54"))
            {
                if (numeroFormateado.StartsWith("0"))
                    numeroFormateado = numeroFormateado.Substring(1);

                if (numeroFormateado.Length == 10 && !numeroFormateado.StartsWith("9"))
                    numeroFormateado = "9" + numeroFormateado;

                numeroFormateado = "54" + numeroFormateado;
            }

            return numeroFormateado;
        }
    }
}
