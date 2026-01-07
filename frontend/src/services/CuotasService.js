const API_URL = 'http://localhost:5194/api/Cuotas';

export default {
    async registrarCuota(paymentData) {
        // POST /api/Cuotas/RegistrarCuota
        const response = await fetch(`${API_URL}/RegistrarCuota`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(paymentData)
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al registrar la cuota');
        }

        return await response.json();
    }
};
