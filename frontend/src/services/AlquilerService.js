const API_URL = 'http://localhost:5194/api/Alquileres';

export default {
    async create(dto) {
        // POST /api/Alquileres
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al registrar alquiler');
        }
        return await response.json();
    },

    async getAllActive(page = 1, pageSize = 12) {
        // GET /api/Alquileres/activos
        const response = await fetch(`${API_URL}/activos?pageNumber=${page}&pageSize=${pageSize}`);
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al obtener alquileres activos');
        }
        
        const text = await response.text();
        return text ? JSON.parse(text) : { items: [], totalCount: 0, totalPages: 0 };
    },

    async getBySocio(dni) {
        const response = await fetch(`${API_URL}/socio/${dni}`);
        
        if (!response.ok) {
            if (response.status >= 500) {
                throw new Error('Lo sentimos, algo salió mal');
            }
            const error = await response.text();
            throw new Error(error || 'Error al buscar alquileres por socio');
        }
        
        const text = await response.text();
        if (!text) return [];
        
        const data = JSON.parse(text);
        return Array.isArray(data) ? data : [data];
    },

    async getById(id) {
        const response = await fetch(`${API_URL}/${id}`);
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al obtener detalle del alquiler');
        }
        
        const text = await response.text();
        return text ? JSON.parse(text) : null;
    },

    async addItem(id, dto) {
        const response = await fetch(`${API_URL}/agregar-item/${id}`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al agregar item');
        }
        return true;
    },

    async updateItemQuantity(id, dto) {
        const response = await fetch(`${API_URL}/${id}/item/cantidad`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al modificar cantidad');
        }
        return true;
    },

    async removeItem(id, itemId) {
        const response = await fetch(`${API_URL}/${id}/item/${itemId}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al eliminar item');
        }
        return true;
    },

    async registerPayment(idAlquiler, mes, anio) {
        const response = await fetch(`${API_URL}/${idAlquiler}/pagos?mes=${mes}&anio=${anio}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({})
        });
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al registrar pago');
        }
        
        const text = await response.text();
        return text ? JSON.parse(text) : true;
    },

    async finalize(id) {
        const response = await fetch(`${API_URL}/${id}/finalizar`, {
            method: 'PUT'
        });
        if (!response.ok) {
            if (response.status >= 500) throw new Error('Lo sentimos, algo salió mal');
            const error = await response.text();
            throw new Error(error || 'Error al finalizar alquiler');
        }
        return true;
    }
};
