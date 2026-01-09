const API_URL = 'http://localhost:5194/api/Socios';

export default {
    async removeSocio(id) {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al dar de baja al socio');
        }

        return await response.json();
    },

    async getByDni(dni) {
        const response = await fetch(`${API_URL}/${dni}`);

        if (!response.ok) {
            if (response.status >= 500) {
                throw new Error('Lo sentimos, algo salió mal');
            }
            const data = await response.json();
            throw new Error(data.mensaje || 'Error al buscar socio');
        }

        return await response.json();
    }
};
