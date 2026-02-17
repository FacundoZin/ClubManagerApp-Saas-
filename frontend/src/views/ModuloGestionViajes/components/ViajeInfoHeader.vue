<script setup>
import { computed } from 'vue';

const props = defineProps({
    viaje: {
        type: Object,
        required: true
    }
});

const formatCurrency = (value) => {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value || 0);
};

// Se asume que el backend envía estos campos calculados
const stats = computed(() => [
    { label: 'Recaudación Total', value: formatCurrency(props.viaje.totalRecaudado), icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z', color: 'text-emerald-600', bg: 'bg-emerald-50' },
    { label: 'Porcentaje Agencia', value: `${props.viaje.porcentajeAgencia || 0}%`, icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4', color: 'text-indigo-600', bg: 'bg-indigo-50' },
    { label: 'Porcentaje Asociación', value: `${props.viaje.porcentajeAsociacion || 0}%`, icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0z', color: 'text-teal-600', bg: 'bg-teal-50' },
    { label: 'Monto Neto Club', value: formatCurrency(props.viaje.montoNetoClub), icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z', color: 'text-blue-600', bg: 'bg-blue-50' }
]);
</script>

<template>
    <div class="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
        <div class="p-6 sm:p-8 bg-gradient-to-r from-slate-50 to-white border-b border-slate-100">
            <div class="flex flex-col md:flex-row justify-between gap-6">
                <div>
                    <div class="flex items-center gap-3 mb-2">
                        <span class="px-2.5 py-1 rounded-full bg-teal-100 text-teal-700 text-xs font-black uppercase tracking-widest">Viaje Base</span>
                        <span class="text-slate-400 font-bold">#{{ viaje.id }}</span>
                    </div>
                    <h1 class="text-3xl font-black text-slate-900 tracking-tight">{{ viaje.nombre }}</h1>
                    <p class="text-slate-500 font-medium mt-1 flex items-center gap-1.5">
                        <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                        </svg>
                        {{ viaje.destino }}
                    </p>
                </div>
                <div class="flex flex-wrap gap-4 items-center">
                    <div class="px-4 py-2 bg-white rounded-xl border border-slate-100 shadow-sm">
                        <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest">Fecha Estimada</span>
                        <span class="text-sm font-bold text-slate-700">{{ new Date(viaje.fecha).toLocaleDateString('es-AR') }}</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- Financial Stats Grid -->
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 divide-y sm:divide-y-0 sm:divide-x divide-slate-100 bg-white">
            <div v-for="stat in stats" :key="stat.label" class="p-6 flex items-center gap-4 hover:bg-slate-50 transition-colors">
                <div :class="[stat.bg, stat.color]" class="w-12 h-12 rounded-xl flex items-center justify-center shadow-inner">
                    <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" :d="stat.icon" />
                    </svg>
                </div>
                <div>
                    <span class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest leading-none mb-1">{{ stat.label }}</span>
                    <span class="text-lg font-black text-slate-800 tracking-tight">{{ stat.value }}</span>
                </div>
            </div>
        </div>
    </div>
</template>
