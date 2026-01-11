const API_URL = 'http://localhost:5194/api/Cobranzas';

export default {
    async listarSociosDeudoresPorLote(lote) {
        const response = await fetch(`${API_URL}/lotes/${lote}/deudores`);

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al obtener socios deudores');
        }

        return await response.json();
    }
};
