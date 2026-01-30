const API_URL = `${import.meta.env.VITE_API_URL}/Analiticas`;

const handleResponse = async (response) => {
    if (!response.ok) {
        if (response.status >= 500) {
            throw new Error('Lo sentimos, algo salió mal en el servidor.');
        }

        let errorMessage = 'Error al obtener datos de analíticas';
        try {
            const errorData = await response.json();
            errorMessage = errorData.mensaje || errorData.message || errorMessage;
        } catch (error) {
            // No es JSON
        }
        throw new Error(errorMessage);
    }
    return await response.json();
};

const AnaliticasService = {
    async getResumenSocios(anio, mes = null, semestre = null) {
        let url = `${API_URL}/resumen-socios?anio=${anio}`;
        if (mes) url += `&mes=${mes}`;
        if (semestre) url += `&semestre=${semestre}`;
        
        const response = await fetch(url, { credentials: 'include' });
        return handleResponse(response);
    },

    async getMovimientoSocios(anio, semestre = null) {
        let url = `${API_URL}/movimiento-socios?anio=${anio}`;
        if (semestre) url += `&semestre=${semestre}`;
        
        const response = await fetch(url, { credentials: 'include' });
        return handleResponse(response);
    },

    async getResumenIngresos(anio, mes = null, semestre = null) {
        let url = `${API_URL}/resumen-ingresos?anio=${anio}`;
        if (mes) url += `&mes=${mes}`;
        if (semestre) url += `&semestre=${semestre}`;
        
        const response = await fetch(url, { credentials: 'include' });
        return handleResponse(response);
    },

    async getHistorialIngresos(anio, semestre = null) {
        let url = `${API_URL}/historial-ingresos?anio=${anio}`;
        if (semestre) url += `&semestre=${semestre}`;
        
        const response = await fetch(url, { credentials: 'include' });
        return handleResponse(response);
    },

    async getDistribucionPago(anio, semestre = null) {
        let url = `${API_URL}/distribucion-pago?anio=${anio}`;
        if (semestre) url += `&semestre=${semestre}`;
        
        const response = await fetch(url, { credentials: 'include' });
        return handleResponse(response);
    }
};

export default AnaliticasService;
