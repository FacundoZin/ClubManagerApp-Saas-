const API_URL = 'http://localhost:5194/api/Cobranzas';

export default {
    async listarSociosDeudoresPorLote(lote) {
        // GET /api/Cobranzas/ListarSociosDeudoresPorLote?lote=X
        const url = new URL(`${API_URL}/ListarSociosDeudoresPorLote`);
        url.searchParams.append('lote', lote);

        const response = await fetch(url.toString());
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al obtener socios deudores');
        }

        // Assuming controller returns Ok(List<SocioDto>)
        return await response.json();
    }
};
