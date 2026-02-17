<script setup>
import { onMounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useViajeDetail } from '../composables/useViajeDetail';
import { usePagos } from '../composables/usePagos';

import ViajeInfoHeader from '../components/ViajeInfoHeader.vue';
import VarianteSection from '../components/VarianteSection.vue';
import RegistrarPagoModal from '../components/RegistrarPagoModal.vue';
import LoadingOverlay from '../../../components/Common/LoadingOverlay.vue';

const route = useRoute();
const router = useRouter();
const { viaje, loading, error, fetchViajeDetail, refreshData } = useViajeDetail();
const { registrarPago, isProcessing } = usePagos();

const isPagoModalOpen = ref(false);
const selectedInscripcion = ref(null);

onMounted(() => {
    const id = route.params.id;
    if (id) {
        fetchViajeDetail(id);
    }
});

const openRegistrarPago = (inscripcion) => {
    selectedInscripcion.value = inscripcion;
    isPagoModalOpen.value = true;
};

const handleConfirmPago = async (pagoDto) => {
    try {
        await registrarPago(pagoDto.inscripcionId, pagoDto.monto, pagoDto.observaciones);
        // Refresh para ver el saldo actualizado y recaudación
        await refreshData();
    } catch (err) {
        alert(err.message || 'Error al registrar el pago');
    }
};

const goBack = () => {
    router.push('/viajes');
};
</script>

<template>
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <!-- Breadcrumb & Back -->
        <div class="mb-8 flex items-center justify-between">
            <button 
                @click="goBack"
                class="group flex items-center gap-2 text-slate-500 hover:text-teal-600 transition-colors"
            >
                <div class="w-8 h-8 rounded-lg bg-white border border-slate-200 flex items-center justify-center group-hover:border-teal-200 group-hover:bg-teal-50 transition-all shadow-sm">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M15 19l-7-7 7-7" />
                    </svg>
                </div>
                <span class="text-xs font-black uppercase tracking-widest">Volver al Dashboard</span>
            </button>
            <div class="flex items-center gap-2">
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Modulo</span>
                <span class="px-2 py-0.5 rounded-lg bg-slate-100 text-slate-600 text-[10px] font-black uppercase tracking-widest">Gestión de Viajes</span>
            </div>
        </div>

        <div v-if="loading && !viaje" class="flex justify-center py-20">
            <LoadingOverlay :active="true" />
        </div>

        <div v-else-if="error" class="bg-red-50 border border-red-100 rounded-2xl p-8 text-center">
            <div class="w-16 h-16 rounded-full bg-red-100 flex items-center justify-center mx-auto mb-4">
                <svg class="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
            </div>
            <h3 class="text-xl font-black text-slate-900 mb-2">{{ error }}</h3>
            <button @click="fetchViajeDetail(route.params.id)" class="text-teal-600 font-bold hover:underline">Reintentar cargar</button>
        </div>

        <div v-else-if="viaje" class="space-y-8 animate-in fade-in duration-500">
            <ViajeInfoHeader :viaje="viaje" />

            <div v-if="viaje.variantes?.length > 0">
                <div class="flex items-center gap-3 mb-6 ml-1">
                    <div class="w-1.5 h-6 bg-teal-500 rounded-full"></div>
                    <h2 class="text-xl font-black text-slate-800 tracking-tight">Variantes del Viaje</h2>
                </div>
                
                <VarianteSection 
                    v-for="variante in viaje.variantes" 
                    :key="variante.id" 
                    :variante="variante"
                    @registrar-pago="openRegistrarPago"
                />
            </div>

            <div v-else class="bg-slate-50 border border-slate-100 rounded-2xl p-12 text-center text-slate-400">
                <p class="text-sm font-bold uppercase tracking-widest">No se encontraron variantes configuradas para este viaje</p>
            </div>
        </div>

        <!-- Modals -->
        <RegistrarPagoModal 
            :is-open="isPagoModalOpen"
            :inscripcion="selectedInscripcion"
            @close="isPagoModalOpen = false"
            @confirm="handleConfirmPago"
        />

        <LoadingOverlay :active="isProcessing || (loading && viaje)" />
    </div>
</template>
