import { ref, reactive } from 'vue';
import ViajesService from '../services/ViajesService';

export function useViajeDetail() {
    const viaje = ref(null);
    const loading = ref(false);
    const error = ref(null);

    const fetchViajeDetail = async (id) => {
        loading.value = true;
        error.value = null;
        try {
            // Obtenemos el detalle base con variantes e información financiera consolidada
            viaje.value = await ViajesService.getViajeBaseDetailFinanciero(id);
        } catch (err) {
            console.error('Error fetching viaje detail:', err);
            error.value = 'No se pudo cargar el detalle del viaje.';
        } finally {
            loading.value = false;
        }
    };

    const inscribirSocio = async (varianteId, socioId, montoInicial) => {
        loading.value = true;
        try {
            const dto = {
                varianteId,
                socioId,
                montoInicial
            };
            const result = await ViajesService.inscribirSocio(dto);
            return result;
        } catch (err) {
            console.error('Error enrolling socio:', err);
            throw err;
        } finally {
            loading.value = false;
        }
    };

    const refreshData = async () => {
        if (viaje.value) {
            await fetchViajeDetail(viaje.value.id);
        }
    };

    return {
        viaje,
        loading,
        error,
        fetchViajeDetail,
        inscribirSocio,
        refreshData
    };
}
