const API_URL = `${import.meta.env.VITE_API_URL}/Salon`;

const handleError = async (response, defaultMessage) => {
    if (response.status >= 500) {
        throw new Error('Algo salió mal en el servidor. Por favor intente más tarde.');
    }
    const errorText = await response.text();
    if (!errorText) return defaultMessage;
    
    try {
        const errorObj = JSON.parse(errorText);
        // Si es un error de ModelState (.NET)
        if (errorObj.errors) {
            const firstErrorKey = Object.keys(errorObj.errors)[0];
            return errorObj.errors[firstErrorKey][0];
        }
        return errorObj.errormessage || errorText || defaultMessage;
    } catch (e) {
        return errorText || defaultMessage;
    }
};

export default {
    async getAllSalons() {
        const response = await fetch(`${API_URL}/all`, { credentials: 'include' });
        if (!response.ok) {
            const msg = await handleError(response, 'Error al obtener salones');
            throw new Error(msg);
        }
        return await response.json();
    },

    async getReservasBySalon(idSalon, page = 1, pageSize = 10) {
        const response = await fetch(`${API_URL}/${idSalon}/reservas?pageNumber=${page}&pageSize=${pageSize}`, { credentials: 'include' });
        if (!response.ok) {
            const msg = await handleError(response, 'Error al obtener reservas');
            throw new Error(msg);
        }
        return await response.json();
    },

    async checkAvailability(fecha, salonId) {
        const response = await fetch(`${API_URL}/${salonId}/disponibilidad?fecha=${fecha}`, { credentials: 'include' });
        if (!response.ok) {
            const msg = await handleError(response, 'Error verificando disponibilidad');
            throw new Error(msg);
        }
        return await response.json();
    },

    async getReservaByFecha(fecha, salonId) {
        const response = await fetch(`${API_URL}/${salonId}/reserva?fecha=${fecha}`, { credentials: 'include' });
        if (response.status === 404 || response.status === 204) return null;
        if (!response.ok) {
            const msg = await handleError(response, 'Error buscando reserva');
            throw new Error(msg);
        }
        const text = await response.text();
        return text ? JSON.parse(text) : null;
    },

    async getReservaById(reservaId) {
        const response = await fetch(`${API_URL}/reservas/${reservaId}`, { credentials: 'include' });
        if (response.status === 404 || response.status === 204) return null;
        if (!response.ok) {
            const msg = await handleError(response, 'Error al obtener la reserva');
            throw new Error(msg);
        }
        const text = await response.text();
        return text ? JSON.parse(text) : null;
    },

    async createReserva(dto) {
        const response = await fetch(`${API_URL}/reserva`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            const msg = await handleError(response, 'Error al crear la reserva');
            throw new Error(msg);
        }
        return await response.json();
    },

    async cancelReserva(reservaId) {
        const response = await fetch(`${API_URL}/reserva/${reservaId}`, {
            method: 'DELETE',
            credentials: 'include'
        });
        if (!response.ok) {
            const msg = await handleError(response, 'Error al cancelar la reserva');
            throw new Error(msg);
        }
        return true;
    },

    async actualizarPago(reservaId, montoAbonado) {
        const response = await fetch(`${API_URL}/reservas/${reservaId}/pago?montoAbonado=${montoAbonado}`, {
            method: 'PATCH',
            credentials: 'include'
        });
        if (!response.ok) {
            const msg = await handleError(response, 'Error al registrar el pago');
            throw new Error(msg);
        }
        return true;
    }
};
