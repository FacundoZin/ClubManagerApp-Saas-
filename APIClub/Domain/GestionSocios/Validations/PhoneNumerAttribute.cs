using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace APIClub.Domain.GestionSocios.Validations
{
    public class PhoneNumerAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var telefono = value.ToString();

            if (!Regex.IsMatch(telefono!, @"^\d{10}$"))
                return new ValidationResult(
                    "El teléfono debe tener exactamente 10 dígitos numéricos."
                );

            return ValidationResult.Success;
        }
    }
}
