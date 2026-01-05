const API_URL = 'http://localhost:5194/api/Articulos';

export default {
    async getAll() {
        // GET /api/Articulos
        const response = await fetch(API_URL);
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al obtener artículos');
        }
        // Result wrapper { exit: boolean, data: [], ... }
        const result = await response.json();
        // Controller returns Ok(result.Data), so it's already the array or object?
        // Checking controller: 
        // return Ok(result.Data); 
        // So it returns the list directly or the object? 
        // _articulosService.GetAllArticulos() returns Result<List<ArticuloDto>>
        // Controller: return Ok(result.Data); -> The body IS the list/dta.
        return result;
    },

    async create(dto) {
        // POST /api/Articulos
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
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
            body: JSON.stringify(nuevoPrecio)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al actualizar precio');
        }
        return await response.json();
    }
};
