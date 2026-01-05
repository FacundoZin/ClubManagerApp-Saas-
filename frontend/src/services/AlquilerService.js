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
            const error = await response.text();
            throw new Error(error || 'Error al registrar alquiler');
        }
        return await response.json();
    },

    async getAllActive() {
        // GET /api/Alquileres/activos
        const response = await fetch(`${API_URL}/activos`);
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al obtener alquileres activos');
        }
        return await response.json();
    },

    async getBySocio(dni) {
        // GET /api/Alquileres/socio/{dni}
        const response = await fetch(`${API_URL}/socio/${dni}`);
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al buscar alquileres por socio');
        }
        return await response.json();
    },

    async getById(id) {
        // GET /api/Alquileres/{id}
        const response = await fetch(`${API_URL}/${id}`);
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al obtener detalle del alquiler');
        }
        return await response.json();
    },

    async addItem(id, dto) {
        // PATCH /api/Alquileres/agregar-item/{id}
        const response = await fetch(`${API_URL}/agregar-item/${id}`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al agregar item');
        }
        // Returns NoContent on success usually
        return true;
    },

    async updateItemQuantity(id, dto) {
        // PATCH /api/Alquileres/{id}/item/cantidad
        const response = await fetch(`${API_URL}/${id}/item/cantidad`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al modificar cantidad');
        }
        return true;
    },

    async removeItem(id, itemId) {
        // DELETE /api/Alquileres/{id}/item/{itemId}
        const response = await fetch(`${API_URL}/${id}/item/${itemId}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al eliminar item');
        }
        return true;
    },

    async registerPayment(idAlquiler) {
        // POST /api/Alquileres/{idAlquiler}/pagos
        // Assuming no body needed or maybe empty object based on controller "RegistrarPago(int idAlquiler)" 
        // Controller params: [HttpPost("{idAlquiler}/pagos")] public async Task... RegistrarPago(int idAlquiler)
        // Usually POST needs body or it might fail if content-length is missing in some server configs, but let's try empty.
        const response = await fetch(`${API_URL}/${idAlquiler}/pagos`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({})
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al registrar pago');
        }
        return await response.json();
    },

    async finalize(id) {
        // PUT /api/Alquileres/{id}/finalizar
        const response = await fetch(`${API_URL}/${id}/finalizar`, {
            method: 'PUT'
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al finalizar alquiler');
        }
        return true;
    }
};
