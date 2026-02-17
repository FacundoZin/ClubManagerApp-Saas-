const API_URL = `${import.meta.env.VITE_API_URL}/viajes-pagos`;

const PagosService = {
    async registrarPago(pagoDto) {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify(pagoDto)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al registrar el pago');
        }
        return await response.json();
    },

    async getPagosSocioVariante(socioId, varianteId) {
        const response = await fetch(`${API_URL}/socio/${socioId}/variante/${varianteId}`, {
            credentials: 'include'
        });
        if (!response.ok) throw new Error('Error al obtener historial de pagos');
        return await response.json();
    }
};

export default PagosService;
