<script setup>
import { ref } from 'vue';

const props = defineProps({
    isOpen: Boolean,
    viajes: {
        type: Array,
        default: () => []
    },
    socio: {
        type: Object,
        default: null
    }
});

const emit = defineEmits(['close', 'confirm']);

const selectedViajeId = ref(null);

const handleClose = () => {
    selectedViajeId.value = null;
    emit('close');
};

const handleConfirm = () => {
    if (!selectedViajeId.value) return;
    const viaggio = props.viajes.find(v => v.id === selectedViajeId.value);
    emit('confirm', viaggio);
    handleClose();
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
                        <div class="mx-auto flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-full bg-teal-100 sm:mx-0 sm:h-10 sm:w-10">
                            <svg class="h-6 w-6 text-teal-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left">
                            <h3 class="text-xl font-black text-slate-900 tracking-tight" id="modal-title">
                                Seleccionar Viaje
                            </h3>
                            <p class="text-sm text-slate-500 font-medium">Elija el viaje base para <span class="text-teal-600 font-bold">{{ socio?.nombre }}</span>.</p>
                        </div>
                    </div>

                    <div class="space-y-4">
                        <div 
                            v-for="viaje in viajes" 
                            :key="viaje.id"
                            @click="selectedViajeId = viaje.id"
                            class="relative flex items-center p-4 rounded-xl border-2 cursor-pointer transition-all hover:bg-slate-50"
                            :class="selectedViajeId === viaje.id ? 'border-teal-500 bg-teal-50/30' : 'border-slate-100 bg-white'"
                        >
                            <div class="flex-grow">
                                <span class="block text-sm font-black text-slate-800">{{ viaje.nombre }}</span>
                                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-widest">{{ viaje.destino }} - {{ new Date(viaje.fecha).toLocaleDateString('es-AR') }}</span>
                            </div>
                            <div v-if="selectedViajeId === viaje.id" class="w-6 h-6 rounded-full bg-teal-500 flex items-center justify-center">
                                <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
                                </svg>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="bg-slate-50 px-6 py-4 sm:flex sm:flex-row-reverse gap-3 border-t border-slate-100">
                    <button 
                        @click="handleConfirm" 
                        :disabled="!selectedViajeId" 
                        type="button" 
                        class="w-full inline-flex justify-center rounded-xl border border-transparent shadow-lg py-2.5 bg-slate-900 text-sm font-black text-white hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 sm:w-auto px-10 transition-all active:scale-95 disabled:opacity-50"
                    >
                        Siguiente
                    </button>
                    <button 
                        @click="handleClose" 
                        type="button" 
                        class="mt-3 w-full inline-flex justify-center rounded-xl border border-slate-200 shadow-sm px-6 py-2.5 bg-white text-sm font-bold text-slate-600 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 sm:mt-0 sm:w-auto transition-all"
                    >
                        Cancelar
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
