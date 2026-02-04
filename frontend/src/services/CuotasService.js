const API_URL = `${import.meta.env.VITE_API_URL}/Cuotas`;

export default {
    async registrarCuota(paymentData) {
        if (!paymentData || !paymentData.socioId) {
            throw new Error("Datos de pago inválidos");
        }

        // Construir el request DTO para pago general (plural)
        const request = {
            IdSocio: paymentData.socioId,
            FormaPago: paymentData.formaPago,
            Periodos: paymentData.periodos || [] // Lista de { anio, semestre }
        };

        try {
            const response = await fetch(`${API_URL}/RegistrarPagos`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
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

    async registrarPagoCobrador(paymentData) {
        if (!paymentData || !paymentData.socioId) {
            throw new Error("Datos de pago inválidos");
        }

        const request = {
            IdSocio: paymentData.socioId,
            FormaPago: paymentData.formaPago,
            Periodos: paymentData.periodos || [] // Lista de { anio, semestre }
        };

        try {
            const response = await fetch(`${API_URL}/RegistrarPagoConCobrador`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(request)
            });

            if (!response.ok) {
                if (response.status >= 500) {
                    throw new Error("Lo sentimos, algo salió mal en el servidor.");
                }

                if (response.status >= 400) {
                    const errorText = await response.text();
                    // El backend devuelve el mensaje directamente como string en este endpoint si falla con StatusCode(result.Errorcode,result.Errormessage)
                    // Intentamos parsear por si es JSON o usamos el texto directamente
                    let errorMessage = errorText;
                    try {
                        const errorData = JSON.parse(errorText);
                        errorMessage = errorData.mensaje || errorData.title || errorText;
                    } catch (e) {
                        // No es JSON, usamos el texto
                    }
                    throw new Error(errorMessage || 'Error en la solicitud');
                }
            }

            return await response.json();
        } catch (error) {
            console.error("Error en registrarPagoCobrador:", error);
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
                credentials: 'include',
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
    },

    async obtenerHistorialCuotas(filtros) {
        try {
            const params = new URLSearchParams({
                tipoFiltro: filtros.tipoFiltro || 'fecha',
                pageNumber: filtros.pageNumber || 1,
                pageSize: filtros.pageSize || 10
            });

            if (filtros.tipoFiltro === 'fecha' && filtros.fechaPago) {
                params.append('fechaPago', filtros.fechaPago);
            } else if (filtros.tipoFiltro === 'periodo') {
                if (filtros.anio) params.append('anio', filtros.anio);
                if (filtros.semestre) params.append('semestre', filtros.semestre);
            }

            const response = await fetch(`${API_URL}/HistorialCuotas?${params.toString()}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
            });

            if (!response.ok) {
                if (response.status >= 500) {
                    throw new Error("Lo sentimos, algo salió mal en el servidor.");
                }

                if (response.status >= 400) {
                    const errorData = await response.json().catch(() => ({}));
                    const errorMessage = errorData.mensaje || errorData.title || 'Error al obtener el historial';
                    throw new Error(errorMessage);
                }
            }

            const result = await response.json();
            return result.data;
        } catch (error) {
            console.error("Error en obtenerHistorialCuotas:", error);
            throw error;
        }
    }
};
