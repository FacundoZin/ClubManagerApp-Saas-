const API_URL = 'http://localhost:5194/api/Cuotas';

export default {
    async registrarCuota(paymentData) {
        if (!paymentData || !paymentData.socioId) {
            throw new Error("Datos de pago inválidos");
        }

        // Construir el request DTO exacto
        const request = {
            IdSocio: paymentData.socioId,
            FormaPago: paymentData.formaPago
        };

        try {
            const response = await fetch(`${API_URL}/pagarCuota`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(request)
            });

            if (!response.ok) {
                if (response.status >= 500) {
                    throw new Error("Lo sentimos, algo salió mal en el servidor.");
                }

                if (response.status >= 400) {
                    const errorData = await response.json().catch(() => ({}));
                    const errorMessage = errorData.mensaje || errorData.title || 'Error en la solicitud';
                    throw new Error(errorMessage);
                }
            }

            return await response.json();
        } catch (error) {
            console.error("Error en registrarCuota:", error);
            throw error; 
        }
    },

    async actualizarValor(nuevoValor) {
        try {
            const response = await fetch(`${API_URL}/actualizarValor`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(nuevoValor),
            });

            if (!response.ok) {
                if (response.status >= 500) {
                    throw new Error("Lo sentimos, algo salió mal en el servidor.");
                }

                if (response.status >= 400) {
                    const errorData = await response.json().catch(() => ({}));
                    const errorMessage = errorData.mensaje || errorData.title || 'Error al actualizar el valor';
                    throw new Error(errorMessage);
                }
            }

            return true;
        } catch (error) {
            console.error("Error en actualizarValor:", error);
            throw error;
        }
    }
};
