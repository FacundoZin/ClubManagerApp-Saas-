import { ref } from 'vue';
import PagosService from '../services/PagosService';

export function usePagos() {
    const isProcessing = ref(false);

    const registrarPago = async (inscripcionId, monto, observaciones) => {
        isProcessing.value = true;
        try {
            const dto = {
                inscripcionId,
                monto,
                observaciones,
                fecha: new Date().toISOString()
            };
            const result = await PagosService.registrarPago(dto);
            return result;
        } catch (err) {
            console.error('Error registering payment:', err);
            throw err;
        } finally {
            isProcessing.value = false;
        }
    };

    return {
        isProcessing,
        registrarPago
    };
}
