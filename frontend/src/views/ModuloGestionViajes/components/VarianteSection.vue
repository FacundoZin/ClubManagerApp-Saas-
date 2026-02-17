<script setup>
import { computed } from 'vue';
import InscriptosTable from './InscriptosTable.vue';

const props = defineProps({
    variante: {
        type: Object,
        required: true
    }
});

const emit = defineEmits(['registrar-pago']);

const formatCurrency = (value) => {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value || 0);
};

const totalRecaudadoVariante = computed(() => {
    if (!props.variante.inscriptos) return 0;
    return props.variante.inscriptos.reduce((total, s) => total + (s.montoAbonado || 0), 0);
});

// Nota: Estos porcentajes vienen del backend en un escenario real, pero para UI los mostramos de la variante
</script>

<template>
    <div class="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden mb-8">
        <div class="p-6 bg-slate-50/50 border-b border-slate-100 flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
            <div>
                <div class="flex items-center gap-2 mb-1">
                    <span class="px-2 py-0.5 rounded-full bg-slate-200 text-slate-600 text-[10px] font-black uppercase tracking-widest">Variante</span>
                    <h2 class="text-xl font-black text-slate-900 tracking-tight">{{ variante.nombre }}</h2>
                </div>
                <p class="text-[10px] font-bold text-slate-400 uppercase tracking-widest leading-none">Precio Unitario: <span class="text-slate-600 font-black">{{ formatCurrency(variante.precio) }}</span></p>
            </div>
            <div class="flex gap-4">
                <div class="text-right">
                    <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1">Total Recaudado</span>
                    <span class="text-lg font-black text-emerald-600 tabular-nums">{{ formatCurrency(totalRecaudadoVariante) }}</span>
                </div>
                <div class="w-px h-10 bg-slate-200"></div>
                <div class="text-right">
                    <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1">Cupos</span>
                    <span class="text-lg font-black text-slate-800 tabular-nums">{{ variante.inscriptos?.length || 0 }} / {{ variante.cupo || '∞' }}</span>
                </div>
            </div>
        </div>
        
        <InscriptosTable 
            :inscriptos="variante.inscriptos" 
            :variante="variante"
            @registrar-pago="emit('registrar-pago', $event)"
        />
    </div>
</template>
