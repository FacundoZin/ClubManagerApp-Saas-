using APIClub.Domain.PaymentsOnline.Modelos;
using APIClub.Domain.PaymentsOnline;

namespace APIClub.Helpers
{
    public static class PaymentStatusMapper
    {
        private static readonly Dictionary<string, PaymentStatus> _statusMap = new()
        {
            // Estados finales exitosos
            ["approved"] = PaymentStatus.ApprovedByGateway,
            
            // Estados pendientes/en proceso
            ["pending"] = PaymentStatus.Pending,
            ["in_process"] = PaymentStatus.Pending,
            ["authorized"] = PaymentStatus.Pending, // Autorizado pero no capturado
            
            // Estados de rechazo
            ["rejected"] = PaymentStatus.Rejected,
            ["cancelled"] = PaymentStatus.Rejected,
            
            // Estados especiales
            ["in_mediation"] = PaymentStatus.Pending, // En disputa
            ["refunded"] = PaymentStatus.Rejected, // Reembolsado
            ["charged_back"] = PaymentStatus.Rejected // Contracargo
        };

        public static PaymentStatus MapFromMercadoPago(string mercadoPagoStatus)
        {
            if (string.IsNullOrWhiteSpace(mercadoPagoStatus))
                return PaymentStatus.Pending;

            return _statusMap.TryGetValue(mercadoPagoStatus.ToLower(), out var status)
                ? status
                : PaymentStatus.Pending; // Default seguro para estados desconocidos
        }

        public static bool IsFinalStatus(string mercadoPagoStatus)
        {
            var finalStatuses = new[] { "approved", "rejected", "cancelled", "refunded", "charged_back" };
            return finalStatuses.Contains(mercadoPagoStatus?.ToLower());
        }

        public static bool IsSuccessStatus(string mercadoPagoStatus)
        {
            return mercadoPagoStatus?.ToLower() == "approved";
        }
    }
}
