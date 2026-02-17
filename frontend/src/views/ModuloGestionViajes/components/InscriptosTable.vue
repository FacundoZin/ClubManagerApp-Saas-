<script setup>
const props = defineProps({
    inscriptos: {
        type: Array,
        default: () => []
    },
    variante: {
        type: Object,
        required: true
    }
});

const emit = defineEmits(['registrar-pago']);

const formatCurrency = (value) => {
    return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value || 0);
};

const handleSocioClick = (socio) => {
    emit('registrar-pago', {
        ...socio,
        varianteId: props.variante.id,
        varianteNombre: props.variante.nombre,
        precioVariante: props.variante.precio
    });
};
</script>

<template>
    <div class="overflow-x-auto">
        <table class="w-full text-left">
            <thead>
                <tr class="border-b border-slate-100 bg-slate-50/50">
                    <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Socio / DNI</th>
                    <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Monto Abonado</th>
                    <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Saldo Pendiente</th>
                    <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest">Estado</th>
                    <th class="px-6 py-4 text-[10px] font-bold text-slate-400 uppercase tracking-widest text-right">Acciones</th>
                </tr>
            </thead>
            <tbody class="divide-y divide-slate-100">
                <tr v-for="socio in inscriptos" :key="socio.id" class="group hover:bg-slate-50/80 transition-all">
                    <td class="px-6 py-4">
                        <div class="flex items-center gap-3">
                            <div class="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center text-[10px] font-black text-slate-500 uppercase">
                                {{ socio.nombre.charAt(0) }}
                            </div>
                            <div>
                                <span class="block text-sm font-bold text-slate-700 leading-none mb-1">{{ socio.nombre }}</span>
                                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-widest">{{ socio.dni }}</span>
                            </div>
                        </div>
                    </td>
                    <td class="px-6 py-4">
                        <span class="text-sm font-black text-teal-600">{{ formatCurrency(socio.montoAbonado) }}</span>
                    </td>
                    <td class="px-6 py-4">
                        <span class="text-sm font-black" :class="socio.montoAbonado >= variante.precio ? 'text-slate-400' : 'text-red-600'">
                            {{ formatCurrency(variante.precio - socio.montoAbonado) }}
                        </span>
                    </td>
                    <td class="px-6 py-4">
                        <span v-if="socio.montoAbonado >= variante.precio" class="inline-flex items-center px-2 py-0.5 rounded-full bg-emerald-100 text-emerald-700 text-[10px] font-black uppercase tracking-tighter">Liquidado</span>
                        <span v-else-if="socio.montoAbonado > 0" class="inline-flex items-center px-2 py-0.5 rounded-full bg-amber-100 text-amber-700 text-[10px] font-black uppercase tracking-tighter">Parcial</span>
                        <span v-else class="inline-flex items-center px-2 py-0.5 rounded-full bg-slate-100 text-slate-700 text-[10px] font-black uppercase tracking-tighter">Pendiente</span>
                    </td>
                    <td class="px-6 py-4 text-right">
                        <button 
                            @click="handleSocioClick(socio)"
                            class="p-2 rounded-lg bg-teal-50 text-teal-600 hover:bg-teal-100 transition-all border border-teal-100 shadow-sm opacity-0 group-hover:opacity-100"
                            title="Registrar Abono"
                        >
                            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </button>
                    </td>
                </tr>
                <tr v-if="inscriptos.length === 0">
                    <td colspan="5" class="px-6 py-12 text-center">
                        <div class="flex flex-col items-center gap-2">
                            <div class="w-12 h-12 rounded-full bg-slate-50 flex items-center justify-center">
                                <svg class="w-6 h-6 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
                                </svg>
                            </div>
                            <span class="text-xs font-bold text-slate-400 uppercase tracking-widest mt-2">No hay socios inscriptos aún</span>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>
