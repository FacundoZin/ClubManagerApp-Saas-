import { ref } from 'vue';

const API_URL = `${import.meta.env.VITE_API_URL}/auth`;

// Estado reactivo para el usuario actual
const currentUser = ref(null);
const isAuthenticated = ref(false);

export default {
    currentUser,
    isAuthenticated,

    async login(nombreUsuario, password) {
        try {
            const response = await fetch(`${API_URL}/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify({ nombreUsuario, password })
            });

            if (!response.ok) {
                let errorMessage = 'Algo salió mal. Por favor, intente de nuevo.';
                
                // Si es un error del cliente (400-499), intentamos obtener el mensaje del servidor
                if (response.status >= 400 && response.status < 500) {
                    try {
                        const errorData = await response.json();
                        errorMessage = errorData.message || 'Credenciales inválidas o datos incorrectos.';
                    } catch (e) {
                        // Si no es JSON, mensaje genérico para errores de cliente
                        errorMessage = 'Error en la solicitud. Verifique sus datos.';
                    }
                }
                
                throw new Error(errorMessage);
            }

            // Login exitoso (Cookie establecida).
            // Como el body viene vacío, llamamos a checkAuth para obtener los datos del usuario y actualizar el estado
            const authenticated = await this.checkAuth();
            
            if (authenticated) {
                return { success: true };
            } else {
                return { success: false, message: "Sesión iniciada pero error obteniendo datos" };
            }
        } catch (error) {
            console.error(error);
            return { success: false, message: error.message };
        }
    },

    async logout() {
        try {
            await fetch(`${API_URL}/logout`, {
                method: 'POST',
                credentials: 'include'
            });
        } catch (error) {
            console.error("Error en logout", error);
        } finally {
            this.clearSession();
        }
    },

    async checkAuth() {
        try {
            const response = await fetch(`${API_URL}/me`, {
                method: 'GET',
                credentials: 'include'
            });

            if (response.ok) {
                const userData = await response.json();
                currentUser.value = userData;
                isAuthenticated.value = true;
                return true;
            } else {
                this.clearSession();
                return false;
            }
        } catch (error) {
            this.clearSession();
            return false;
        }
    },

    setSession(data) {
        currentUser.value = {
            nombreUsuario: data.nombreUsuario, 
            rol: data.rol, 
        };
        isAuthenticated.value = true;
    },

    clearSession() {
        currentUser.value = null;
        isAuthenticated.value = false;
    },

    isSuperAdmin() {
        return currentUser.value && currentUser.value.rol === 1; // 1 = SuperAdmin
    }
};
