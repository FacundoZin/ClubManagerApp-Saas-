namespace APIClub.Application.Helpers
{
    public static class PaymentRejectedMessageHelper
    {
        public static string GetMessage(string? statusDetail)
        {
            if (string.IsNullOrEmpty(statusDetail))
                return "El pago fue rechazado. Por favor, intente nuevamente o use otro medio de pago.";

            switch (statusDetail)
            {
                case "cc_rejected_insufficient_amount":
                    return "Fondos insuficientes en la tarjeta.";
                case "cc_rejected_bad_filled_card_number":
                    return "Número de tarjeta incorrecto.";
                case "cc_rejected_bad_filled_date":
                    return "Fecha de vencimiento de la tarjeta incorrecta.";
                case "cc_rejected_bad_filled_security_code":
                    return "Código de seguridad (CVV) incorrecto.";
                case "cc_rejected_max_attempts":
                    return "Se superó el número de intentos permitidos con esta tarjeta.";
                case "cc_rejected_other_reason":
                    return "Pago rechazado, consulte con su banco.";
                case "rejected_by_bank":
                    return "Pago rechazado por el banco.";
                case "rejected_by_regulations":
                    return "Pago rechazado por regulaciones del sistema.";
                case "insufficient_amount":
                    return "Monto insuficiente para procesar el pago.";
                case "rejected_insufficient_data":
                    return "Faltan datos obligatorios para procesar el pago.";
                default:
                    return "El pago fue rechazado. Por favor, intente nuevamente o use otro medio de pago.";
            }
        }
    }
}
