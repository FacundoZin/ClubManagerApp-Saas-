const API_URL = 'http://localhost:5194/api/Salon';

export default {
    async getAllSalons() {
        const response = await fetch(`${API_URL}/all`);
        if (!response.ok) throw new Error('Error al obtener salones');
        return await response.json();
    },

    async getReservasBySalon(idSalon) {
        const response = await fetch(`${API_URL}/${idSalon}/reservas`);
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al obtener reservas');
        }
        return await response.json();
    },

    async checkAvailability(fecha, salonId) {
        // GET {salonId}/disponibilidad?fecha=YYYY-MM-DD
        const response = await fetch(`${API_URL}/${salonId}/disponibilidad?fecha=${fecha}`);
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error verificando disponibilidad'); // Backend might establish specific error codes
        }
        return await response.json();
    },

    async getReservaByFecha(fecha, salonId) {
        // GET {salonId}/reserva?fecha=YYYY-MM-DD
        const response = await fetch(`${API_URL}/${salonId}/reserva?fecha=${fecha}`);
        if (response.status === 404) return null; // Handle not found gracefully
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error buscando reserva');
        }
        return await response.json();
    },

    async getReservaById(reservaId) {
        const response = await fetch(`${API_URL}/reservas/${reservaId}`);
        if (!response.ok) throw new Error('Error al obtener la reserva');
        return await response.json();
    },

    async createReserva(dto) {
        const response = await fetch(`${API_URL}/reserva`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al crear la reserva');
        }
        return await response.json();
    },

    async cancelReserva(reservaId) {
        const response = await fetch(`${API_URL}/reserva/${reservaId}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al cancelar la reserva');
        }
        // Returns NoContentusually
        return true;
    }
};
