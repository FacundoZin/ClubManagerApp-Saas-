namespace APIClub.Domain.PaymentsOnline.Modelos
{
    public enum PaymentStatus
    {
        Created,                    // Token creado, nadie pagó aún
        Pending,                    // MP dice pending / in_process
        ApprovedByGateway,          // MP dijo approved (aún no procesado internamente)
        Rejected,                   // Falló directo
        Confirmed,                  // Webhook confirmó y se aplicó el pago
        Failed                      // Webhook llegó pero no se pudo procesar
    }
}
