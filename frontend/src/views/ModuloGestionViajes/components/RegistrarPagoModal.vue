<script setup>
import { ref, reactive, computed } from 'vue';

const props = defineProps({
    isOpen: Boolean,
    inscripcion: {
        type: Object,
        default: null
    }
});

const emit = defineEmits(['close', 'confirm']);

const monto = ref(0);
const observaciones = ref('');
const isSubmitting = ref(false);

const resetForm = () => {
    monto.value = 0;
    observaciones.value = '';
};

const handleClose = () => {
    resetForm();
    emit('close');
};

const saldoPendiente = computed(() => {
    if (!props.inscripcion) return 0;
    return props.inscripcion.precioVariante - props.inscripcion.montoAbonado;
});

const isValid = computed(() => {
    return monto.value > 0 && monto.value <= saldoPendiente.value;
});

const handleSubmit = async () => {
    if (!isValid.value) return;

    isSubmitting.value = true;
    try {
        emit('confirm', {
            inscripcionId: props.inscripcion.id,
            monto: monto.value,
            observaciones: observaciones.value
        });
        handleClose();
    } catch (error) {
        console.error('Error in modal submit:', error);
    } finally {
        isSubmitting.value = false;
    }
};

const formatCurrency = (value) => {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value || 0);
};
</script>

<template>
    <div v-if="isOpen" class="fixed inset-0 z-[100] overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
        <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
            <div class="fixed inset-0 bg-slate-500 bg-opacity-75 transition-opacity" aria-hidden="true" @click="handleClose"></div>
            <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>

            <div class="inline-block align-bottom bg-white rounded-2xl text-left overflow-hidden shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full border border-slate-100">
                <div class="bg-white px-4 pt-5 pb-4 sm:p-6">
                    <div class="sm:flex sm:items-start mb-6">
                        <div class="mx-auto flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-full bg-emerald-100 sm:mx-0 sm:h-10 sm:w-10">
                            <svg class="h-6 w-6 text-emerald-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
                            <h3 class="text-xl font-black text-slate-900 tracking-tight" id="modal-title">
                                Registrar Pago
                            </h3>
                            <p class="text-sm text-slate-500 font-medium">Ingrese el monto del nuevo abono del socio.</p>
                        </div>
                    </div>

                    <div v-if="inscripcion" class="space-y-6">
                        <!-- Socio Info Summary -->
                        <div class="bg-slate-50 p-4 rounded-xl border border-slate-100 grid grid-cols-2 gap-4">
                            <div>
                                <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest">Socio</span>
                                <span class="text-sm font-bold text-slate-800">{{ inscripcion.socioNombre }}</span>
                            </div>
                            <div>
                                <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest">Variante</span>
                                <span class="text-sm font-bold text-slate-800">{{ inscripcion.varianteNombre }}</span>
                            </div>
                            <div>
                                <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest">Ya Abonado</span>
                                <span class="text-sm font-black text-teal-600">{{ formatCurrency(inscripcion.montoAbonado) }}</span>
                            </div>
                            <div>
                                <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest text-right">Saldo Pendiente</span>
                                <span class="text-sm font-black text-red-600 block text-right">{{ formatCurrency(saldoPendiente) }}</span>
                            </div>
                        </div>

                        <!-- Form -->
                        <div class="space-y-4">
                            <div>
                                <label class="block text-xs font-bold text-slate-500 uppercase tracking-widest mb-1.5 ml-1">Monto a Pagar</label>
                                <div class="relative">
                                    <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                                        <span class="text-slate-400 font-bold">$</span>
                                    </div>
                                    <input 
                                        v-model="monto" 
                                        type="number" 
                                        class="w-full pl-8 pr-4 py-3 rounded-xl border border-slate-200 focus:ring-2 focus:ring-emerald-500 focus:border-transparent outline-none transition-all font-black text-xl text-slate-800"
                                        placeholder="0.00"
                                        :max="saldoPendiente"
                                    >
                                </div>
                                <p v-if="monto > saldoPendiente" class="text-[10px] text-red-500 font-bold mt-1.5 ml-1 italic group-hover:block">El monto no puede superar el saldo pendiente.</p>
                            </div>

                            <div>
                                <label class="block text-xs font-bold text-slate-500 uppercase tracking-widest mb-1.5 ml-1">Observaciones (Opcional)</label>
                                <textarea 
                                    v-model="observaciones" 
                                    rows="2" 
                                    class="w-full px-4 py-3 rounded-xl border border-slate-200 focus:ring-2 focus:ring-emerald-500 focus:border-transparent outline-none transition-all text-sm font-medium text-slate-700"
                                    placeholder="Ej: Pago en efectivo..."
                                ></textarea>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="bg-slate-50 px-6 py-4 sm:flex sm:flex-row-reverse gap-3 border-t border-slate-100">
                    <button 
                        @click="handleSubmit" 
                        :disabled="!isValid || isSubmitting" 
                        type="button" 
                        class="w-full inline-flex justify-center rounded-xl border border-transparent shadow-lg py-2.5 bg-emerald-600 text-sm font-black text-white hover:bg-emerald-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-emerald-500 sm:w-auto px-10 transition-all active:scale-95 disabled:opacity-50"
                    >
                        {{ isSubmitting ? 'Procesando...' : 'Confirmar Pago' }}
                    </button>
                    <button 
                        @click="handleClose" 
                        type="button" 
                        class="mt-3 w-full inline-flex justify-center rounded-xl border border-slate-200 shadow-sm px-6 py-2.5 bg-white text-sm font-bold text-slate-600 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-emerald-500 sm:mt-0 sm:w-auto transition-all"
                    >
                        Cancelar
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
