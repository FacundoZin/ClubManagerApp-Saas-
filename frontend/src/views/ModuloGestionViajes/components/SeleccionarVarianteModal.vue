<script setup>
import { ref, computed } from 'vue';

const props = defineProps({
    isOpen: Boolean,
    viaje: {
        type: Object,
        default: null
    },
    socio: {
        type: Object,
        default: null
    }
});

const emit = defineEmits(['close', 'confirm']);

const selectedVarianteId = ref(null);
const montoInicial = ref(0);

const handleClose = () => {
    selectedVarianteId.value = null;
    montoInicial.value = 0;
    emit('close');
};

const selectedVariante = computed(() => {
    if (!props.viaje) return null;
    return props.viaje.variantes?.find(v => v.id === selectedVarianteId.value);
});

const isValid = computed(() => {
    return selectedVarianteId.value && montoInicial.value >= 0 && (selectedVariante.value ? montoInicial.value <= selectedVariante.value.precio : true);
});

const handleConfirm = () => {
    if (!isValid.value) return;
    emit('confirm', {
        varianteId: selectedVarianteId.value,
        montoInicial: montoInicial.value
    });
    handleClose();
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
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                            </svg>
                        </div>
                        <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
                            <h3 class="text-xl font-black text-slate-900 tracking-tight" id="modal-title">
                                Configurar Inscripción
                            </h3>
                            <p class="text-sm text-slate-500 font-medium">Seleccione la variante e ingrese abono inicial.</p>
                        </div>
                    </div>

                    <div class="space-y-6">
                        <!-- Variante Selector -->
                        <div>
                            <label class="block text-xs font-bold text-slate-500 uppercase tracking-widest mb-3 ml-1">Variantes Disponibles</label>
                            <div class="grid grid-cols-1 gap-3">
                                <div 
                                    v-for="variante in viaje?.variantes" 
                                    :key="variante.id"
                                    @click="selectedVarianteId = variante.id"
                                    class="relative flex items-center justify-between p-4 rounded-xl border-2 cursor-pointer transition-all hover:bg-slate-50"
                                    :class="selectedVarianteId === variante.id ? 'border-emerald-500 bg-emerald-50/30' : 'border-slate-100 bg-white'"
                                >
                                    <div>
                                        <span class="block text-sm font-black text-slate-800 leading-none mb-1">{{ variante.nombre }}</span>
                                        <span class="text-[10px] font-bold text-slate-400 tracking-widest uppercase">Valor: {{ formatCurrency(variante.precio) }}</span>
                                    </div>
                                    <div v-if="selectedVarianteId === variante.id" class="w-5 h-5 rounded-full bg-emerald-500 flex items-center justify-center shadow-lg transform scale-110">
                                        <svg class="w-3.5 h-3.5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
                                        </svg>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Initial Payment -->
                        <div>
                            <label class="block text-xs font-bold text-slate-500 uppercase tracking-widest mb-2 ml-1">Pago Inicial (Opcional)</label>
                            <div class="relative">
                                <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                                    <span class="text-slate-400 font-bold">$</span>
                                </div>
                                <input 
                                    v-model="montoInicial" 
                                    type="number" 
                                    class="w-full pl-8 pr-4 py-3 rounded-xl border border-slate-200 focus:ring-2 focus:ring-emerald-500 focus:border-transparent outline-none transition-all font-black text-xl text-slate-800"
                                    placeholder="0.00"
                                >
                            </div>
                            <p v-if="selectedVariante && montoInicial > selectedVariante.precio" class="text-[10px] text-red-500 font-bold mt-1.5 ml-1 italic group-hover:block">El pago inicial no puede superar el precio de la variante.</p>
                        </div>
                    </div>
                </div>

                <div class="bg-slate-50 px-6 py-4 sm:flex sm:flex-row-reverse gap-3 border-t border-slate-100">
                    <button 
                        @click="handleConfirm" 
                        :disabled="!isValid" 
                        type="button" 
                        class="w-full inline-flex justify-center rounded-xl border border-transparent shadow-lg py-2.5 bg-emerald-600 text-sm font-black text-white hover:bg-emerald-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-emerald-500 sm:w-auto px-10 transition-all active:scale-95 disabled:opacity-50"
                    >
                        Confirmar Inscripción
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
