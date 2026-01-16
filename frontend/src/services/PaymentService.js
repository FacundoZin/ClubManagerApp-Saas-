const API_URL = `${import.meta.env.VITE_API_URL}/Payment`;

const handleError = async (response, defaultMessage) => {
    if (response.status >= 500) {
        throw new Error('Algo salió mal en el servidor. Por favor intente más tarde.');
    }
    const errorText = await response.text();
    if (!errorText) return defaultMessage;
    
    try {
        const errorObj = JSON.parse(errorText);
        return errorObj.errormessage || errorObj.message || errorObj.title || errorText || defaultMessage;
    } catch (e) {
        return errorText || defaultMessage;
    }
};

export default {
    async initPayment(token) {
        const response = await fetch(`${API_URL}/initPayment`, {
            method: 'POST',
            headers: {
                PaymentToken: token,
            },
        });

        if (!response.ok) {
            const error = new Error('Error initPayment');
            error.status = response.status;
            try {
                const errorData = await response.json();
                error.message = errorData.errormessage || errorData.message || 'Error al iniciar el pago';
                error.data = errorData;
            } catch (e) {
                error.message = `Error del servidor (${response.status})`;
            }
            throw error;
        }

        return await response.json();
    },

    async processPayment(formData) {
        const response = await fetch(`${API_URL}/processPayment`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(formData),
        });

        if (!response.ok) {
            const msg = await handleError(response, 'Error procesando el pago');
            throw new Error(msg);
        }

        return await response.json();
    },

    async getComprobante(token) {
        const response = await fetch(`${API_URL}/comprobante`, {
            method: 'GET',
            headers: {
                PaymentToken: token,
            },
        });

        if (!response.ok) {
            const error = new Error('Error getComprobante');
            error.status = response.status;
            try {
                const errorData = await response.json();
                error.message = errorData.errormessage || errorData.message || 'Error al obtener el comprobante';
            } catch (e) {
                error.message = 'Error al verificar el estado del pago';
            }
            throw error;
        }

        return await response.json();
    }
};
