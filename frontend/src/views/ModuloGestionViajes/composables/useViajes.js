import { ref, reactive } from 'vue';
import ViajesService from '../services/ViajesService';

export function useViajes() {
    const viajesBase = ref([]);
    const loading = ref(false);
    const error = ref(null);

    const fetchViajesBase = async () => {
        loading.value = true;
        error.value = null;
        try {
            viajesBase.value = await ViajesService.getViajesBase();
        } catch (err) {
            console.error('Error fetching viajes base:', err);
            error.value = 'No se pudieron cargar los viajes base.';
        } finally {
            loading.value = false;
        }
    };

    const createViajeBase = async (nuevoViaje) => {
        loading.value = true;
        try {
            const result = await ViajesService.createViajeBase(nuevoViaje);
            viajesBase.value.push(result);
            return result;
        } catch (err) {
            console.error('Error creating viaje base:', err);
            throw new Error('Error al crear el viaje base.');
        } finally {
            loading.value = false;
        }
    };

    const createVariante = async (nuevaVariante) => {
        loading.value = true;
        try {
            const result = await ViajesService.createVariante(nuevaVariante);
            // Actualizar el estado local si es necesario
            // Por ejemplo, recargar las variantes del viaje base afectado
            return result;
        } catch (err) {
            console.error('Error creating variante:', err);
            throw new Error('Error al crear la variante.');
        } finally {
            loading.value = false;
        }
    };

    const getVariantes = async (viajeBaseId) => {
        try {
            return await ViajesService.getVariantesByViajeBase(viajeBaseId);
        } catch (err) {
            console.error('Error fetching variantes:', err);
            return [];
        }
    };

    return {
        viajesBase,
        loading,
        error,
        fetchViajesBase,
        createViajeBase,
        createVariante,
        getVariantes
    };
}
