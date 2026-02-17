<script setup>
import { ref, onMounted } from 'vue';

const props = defineProps({
    viajeBaseId: {
        type: Number,
        required: true
    },
    variantes: {
        type: Array,
        default: () => []
    },
    loading: Boolean
});

const emit = defineEmits(['edit-variante']);

const formatCurrency = (value) => {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
};

const formatDate = (dateString) => {
    if (!dateString) return 'Sin fecha';
    const date = new Date(dateString);
    return date.toLocaleDateString('es-AR');
};
</script>

<template>
    <div class="bg-slate-50 rounded-b-xl border-t border-slate-100 overflow-hidden transition-all duration-300">
        <div v-if="loading" class="p-6 text-center">
            <div class="inline-block animate-spin rounded-full h-6 w-6 border-b-2 border-teal-600"></div>
            <p class="mt-2 text-sm text-slate-500 font-medium">Cargando variantes...</p>
        </div>
        
        <div v-else-if="variantes.length === 0" class="p-8 text-center">
            <div class="mx-auto w-12 h-12 rounded-full bg-slate-100 flex items-center justify-center mb-3">
                <svg class="w-6 h-6 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                </svg>
            </div>
            <p class="text-sm text-slate-500 font-medium">No hay variantes registradas para este viaje.</p>
        </div>

        <div v-else class="divide-y divide-slate-100">
            <div v-for="variante in variantes" :key="variante.id" class="p-4 hover:bg-white transition-colors flex items-center justify-between group">
                <div class="flex-1">
                    <div class="flex items-center gap-2">
                        <h4 class="font-bold text-slate-800 text-sm tracking-tight">{{ variante.nombre }}</h4>
                        <span class="px-2 py-0.5 rounded-full bg-teal-100 text-teal-700 text-[10px] font-bold uppercase tracking-wider">Variante</span>
                    </div>
                    <div class="flex items-center gap-4 mt-1 text-xs text-slate-500 font-medium">
                        <span class="flex items-center gap-1">
                            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                            </svg>
                            {{ formatDate(variante.fecha) }}
                        </span>
                        <span class="flex items-center gap-1">
                            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            {{ formatCurrency(variante.precio) }}
                        </span>
                    </div>
                </div>
                <button 
                    @click="emit('edit-variante', variante)"
                    class="p-2 rounded-lg bg-teal-50 text-teal-600 opacity-0 group-hover:opacity-100 transition-all hover:bg-teal-100"
                    title="Editar variante"
                >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                </button>
            </div>
        </div>
    </div>
</template>
