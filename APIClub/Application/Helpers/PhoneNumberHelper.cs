using APIClub.Application.Common;

namespace APIClub.Application.Helpers
{
    public static class PhoneNumberHelper
    {
        private static readonly string[] PrefijosCon3Digitos = {
        "220", "221", "223", "230", "236", "237", "249", "260", "261", "263",
        "264", "266", "280", "291", "294", "297", "298", "299", "336", "341",
        "342", "343", "345", "348", "351", "353", "358", "362", "364", "370",
        "376", "379", "380", "381", "383", "385", "387", "388" };

        public static Result<string> FormatearForWhatsapp(this string telefono)
        {
            // 1. Solo dígitos
            var digits = new string(telefono.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digits))
                return Result<string>.Error("El teléfono no contiene dígitos válidos", 400);

            // 2. Eliminar prefijos comunes argentinos
            if (digits.StartsWith("0054"))
                digits = digits.Substring(4);

            if (digits.StartsWith("54"))
                digits = digits.Substring(2);

            if (digits.StartsWith("+54"))
                digits = digits.Substring(3);

            if (digits.StartsWith("0"))
                digits = digits.Substring(1);

            // 3. Eliminar el 9 (WhatsApp humano)
            if (digits.StartsWith("9"))
                digits = digits.Substring(1);

            // 4. Validación mínima
            if (digits.Length != 10)
                return Result<string>.Error($"Número inválido, debe tener 10 caracteres, asegurese de NO incluir el +54 y la caracteristica solo de la provincia sin el 15, por ejemplo 3564 65 1745", 400);

            // 5. Resultado final para la API
            return Result<string>.Exito("54" + digits);
        }

        public static Result<string> FormatearNumero(this string telefono)
        {
            // 1. Solo dígitos
            var digits = new string(telefono.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(digits))
                return Result<string>.Error("El teléfono no contiene dígitos válidos", 400);

            // 2. Eliminar prefijos comunes argentinos
            if (digits.StartsWith("0054"))
                digits = digits.Substring(4);

            if (digits.StartsWith("54"))
                digits = digits.Substring(2);

            if (digits.StartsWith("+54"))
                digits = digits.Substring(3);

            if (digits.StartsWith("0"))
                digits = digits.Substring(1);

            // 3. Eliminar el 9 (WhatsApp humano)
            if (digits.StartsWith("9"))
                digits = digits.Substring(1);

            // 4. Validación mínima
            if (digits.Length != 10)
                return Result<string>.Error($"Número inválido, debe tener 10 caracteres, asegurese de NO incluir el +54 y la caracteristica solo de la provincia sin el 15, por ejemplo 3564651742", 400);

            // 5. Resultado final para la API
            return Result<string>.Exito("54" + digits);
        }

        public static string FormatearForUserVisibility(this string telefono)
        {
            if (telefono.Length == 10) return telefono;

            return telefono.Substring(2);
        }

        public static bool EsTelefonoFijo(this string telefono)
        {

            var telefonoLimpio = telefono.Substring(2);
            string numeroLocal = "";

            if (telefonoLimpio.StartsWith("11"))
            {
                numeroLocal = telefonoLimpio.Substring(2);
            }
            else if (PrefijosCon3Digitos.Any(p => telefonoLimpio.StartsWith(p)))
            {
                numeroLocal = telefonoLimpio.Substring(3);
            }
            else
            {
                numeroLocal = telefonoLimpio.Substring(4);
            }

            bool esFijo = numeroLocal.StartsWith("4");

            return esFijo;
        }

    }
}
