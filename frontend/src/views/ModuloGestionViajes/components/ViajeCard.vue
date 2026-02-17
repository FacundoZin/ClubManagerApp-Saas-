<script setup>
import { ref } from 'vue';
import VariantesDropdown from './VariantesDropdown.vue';

const props = defineProps({
    viaje: {
        type: Object,
        required: true
    }
});

const emit = defineEmits(['add-variante', 'edit-variante', 'view-details']);

const isExpanded = ref(false);
const variantes = ref([]);
const loadingVariantes = ref(false);

const toggleExpand = async () => {
    isExpanded.value = !isExpanded.value;
    if (isExpanded.value && variantes.value.length === 0) {
        // En un escenario real, llamaríamos al composable aquí para cargar variantes
        // Pero para mantener el componente limpio, el dashboard pasará la lógica
        emit('fetch-variantes', props.viaje.id);
    }
};

// Exponemos una forma de que el padre actualice las variantes de esta card
const setVariantes = (data) => {
    variantes.value = data;
};

defineExpose({ setVariantes, isExpanded });

const formatCurrency = (value) => {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value);
};
</script>

<template>
    <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden transition-all duration-300 hover:shadow-md h-fit">
        <!-- Card Header/Content -->
        <div class="p-5">
            <div class="flex justify-between items-start mb-4">
                <div class="p-2.5 bg-teal-50 rounded-lg text-teal-600">
                    <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
                    </svg>
                </div>
                <div class="flex gap-1">
                    <button @click="emit('view-details', viaje)" class="p-2 text-slate-400 hover:text-teal-600 hover:bg-teal-50 rounded-lg transition-all" title="Ver detalles">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                        </svg>
                    </button>
                    <button @click="emit('add-variante', viaje)" class="p-2 text-slate-400 hover:text-teal-600 hover:bg-teal-50 rounded-lg transition-all" title="Añadir variante">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
                        </svg>
                    </button>
                </div>
            </div>

            <h3 class="text-lg font-bold text-slate-800 leading-tight mb-1">{{ viaje.nombre }}</h3>
            <p class="text-sm text-slate-500 font-medium mb-4 flex items-center gap-1">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                {{ viaje.destino }}
            </p>

            <div class="flex items-center justify-between pt-4 border-t border-slate-50">
                <div>
                    <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Desde</span>
                    <span class="text-lg font-black text-teal-600">{{ formatCurrency(viaje.precioBase || 0) }}</span>
                </div>
                <button 
                    @click="emit('view-details', viaje.id)"
                    class="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-teal-50 text-teal-600 text-xs font-bold hover:bg-teal-100 transition-all border border-teal-100"
                >
                    Ver Detalle
                </button>
                <button 
                    @click="toggleExpand"
                    class="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-slate-50 text-slate-600 text-xs font-bold hover:bg-slate-100 transition-all border border-slate-100"
                >
                    {{ isExpanded ? 'Contraer' : 'Ver Variantes' }}
                    <svg :class="{'rotate-180': isExpanded}" class="w-3 h-3 transition-transform duration-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                    </svg>
                </button>
            </div>
        </div>

        <!-- Expanded Section -->
        <Transition
            enter-active-class="transition duration-300 ease-out"
            enter-from-class="transform scale-95 opacity-0"
            enter-to-class="transform scale-100 opacity-100"
            leave-active-class="transition duration-200 ease-in"
            leave-from-class="transform scale-100 opacity-100"
            leave-to-class="transform scale-95 opacity-0"
        >
            <div v-if="isExpanded">
                <VariantesDropdown 
                    :viajeBaseId="viaje.id" 
                    :variantes="viaje.variantes || []" 
                    @edit-variante="emit('edit-variante', $event)"
                />
            </div>
        </Transition>
    </div>
</template>
