using APIClub.Application.Common;

namespace APIClub.Application.Helpers
{
    public static class PhoneNumberHelper
    {

        public static Result<string> FormatearForWhatsapp(this string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return Result<string>.Error("El teléfono está vacío", 400);

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
            if (digits.Length < 10 || digits.Length > 11)
                return Result<string>.Error($"Número inválido, debe tener 10 caracteres, asegurese de NO incluir el +54 y la caracteristica solo de la provincia sin el 15, por ejemplo 3564 65 1745", 400);

            // 5. Resultado final para la API
            return Result<string>.Exito("54" + digits);
        }

        public static string FormatearForUserVisibility(this string telefono)
        {
            if(telefono.Length == 10) return telefono;

            return telefono.Substring(2);
        }
    }
}
