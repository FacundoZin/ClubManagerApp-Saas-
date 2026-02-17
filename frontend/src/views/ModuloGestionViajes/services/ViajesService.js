const API_URL = `${import.meta.env.VITE_API_URL}/viajes`;

const ViajesService = {
    // Viajes Base
    async getViajesBase() {
        const response = await fetch(`${API_URL}-base`, { credentials: 'include' });
        if (!response.ok) throw new Error('Error al obtener viajes base');
        return await response.json();
    },

    async createViajeBase(viajeBase) {
        const response = await fetch(`${API_URL}-base`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify(viajeBase)
        });
        if (!response.ok) throw new Error('Error al crear viaje base');
        return await response.json();
    },

    async getViajeBaseById(id) {
        const response = await fetch(`${API_URL}-base/${id}`, { credentials: 'include' });
        if (!response.ok) throw new Error('Error al obtener detalle del viaje base');
        return await response.json();
    },

    async getViajeBaseDetailFinanciero(id) {
        const response = await fetch(`${API_URL}-base/${id}/detalle-financiero`, { credentials: 'include' });
        if (!response.ok) throw new Error('Error al obtener detalle financiero');
        return await response.json();
    },

    // Variantes
    async getVariantesByViajeBase(viajeBaseId) {
        const response = await fetch(`${API_URL}-base/${viajeBaseId}/variantes`, { credentials: 'include' });
        if (!response.ok) throw new Error('Error al obtener variantes');
        return await response.json();
    },

    async createVariante(variante) {
        const response = await fetch(`${API_URL}-variantes`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify(variante)
        });
        if (!response.ok) throw new Error('Error al crear variante');
        return await response.json();
    },

    // Inscripciones
    async inscribirSocio(inscripcionDto) {
        const response = await fetch(`${API_URL}-inscripciones`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include',
            body: JSON.stringify(inscripcionDto)
        });
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Error al inscribir al socio');
        }
        return await response.json();
    },

    async getInscriptosPorVariante(varianteId) {
        const response = await fetch(`${API_URL}-variantes/${varianteId}/inscriptos`, { credentials: 'include' });
        if (!response.ok) throw new Error('Error al obtener lista de inscriptos');
        return await response.json();
    }
};

export default ViajesService;
