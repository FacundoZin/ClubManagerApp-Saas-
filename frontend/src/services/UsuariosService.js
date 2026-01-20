const API_URL = `${import.meta.env.VITE_API_URL}/usuarios`;

const handleError = async (response, defaultMessage) => {
    let errorMessage = defaultMessage;
    try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorData.Errormessage || defaultMessage;
    } catch (e) {
        // Fallback si no es JSON
    }
    return errorMessage;
};

export default {
    async create(usuarioDto) {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include', // Enviar cookies de autenticación
            body: JSON.stringify(usuarioDto)
        });

        if (!response.ok) {
            const msg = await handleError(response, 'Error al crear usuario');
            throw new Error(msg);
        }

        return await response.json();
    },

    async getAll() {
        const response = await fetch(API_URL, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include' // Enviar cookies de autenticación
        });

        if (!response.ok) {
            const msg = await handleError(response, 'Error al obtener usuarios');
            throw new Error(msg);
        }

        return await response.json();
    }
};
