const API_URL = `${import.meta.env.VITE_API_URL}/Articulos`;

export default {
    async getAll() {
        // GET /api/Articulos
        const response = await fetch(API_URL, { credentials: 'include' });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al obtener artículos');
        }
        const result = await response.json();
        return result;
    },

    async create(dto) {
        // POST /api/Articulos
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify(dto)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al crear artículo');
        }
        return await response.json();
    },

    async updatePrecio(id, nuevoPrecio) {
        // PATCH /api/Articulos/{id}/precio
        const response = await fetch(`${API_URL}/${id}/precio`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify(nuevoPrecio)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al actualizar precio');
        }
        return await response.json();
    }
};
