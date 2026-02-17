<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useViajes } from '../composables/useViajes';
import { useViajeDetail } from '../composables/useViajeDetail';
import SociosService from '../../../services/SociosService';

// Componentes
import ViajeCard from '../components/ViajeCard.vue';
import CrearViajeModal from '../components/CrearViajeModal.vue';
import BuscarSocioForm from '../components/BuscarSocioForm.vue';
import SeleccionarViajeModal from '../components/SeleccionarViajeModal.vue';
import SeleccionarVarianteModal from '../components/SeleccionarVarianteModal.vue';
import LoadingOverlay from '../../../components/Common/LoadingOverlay.vue';

const router = useRouter();
const { 
    viajesBase, 
    loading, 
    error, 
    fetchViajesBase, 
    createViajeBase, 
    createVariante,
    getVariantes
} = useViajes();

const { inscribirSocio, loading: enrolling } = useViajeDetail();

// Estado Inscripción
const isViajeModalOpen = ref(false);
const isVarianteModalOpen = ref(false);
const selectedSocio = ref(null);
const viajeParaInscripcion = ref(null);

const isModalOpen = ref(false);
const modalTipo = ref('base'); // 'base' o 'variante'
const selectedViajeBase = ref(null);

const toast = ref({ show: false, message: '', type: 'success' });

const showToast = (message, type = 'success') => {
    toast.value = { show: true, message, type };
    setTimeout(() => { toast.value.show = false; }, 3000);
};

onMounted(() => {
    fetchViajesBase();
});

const openNuevoViajeBase = () => {
    modalTipo.value = 'base';
    selectedViajeBase.value = null;
    isModalOpen.value = true;
};

const openNuevaVariante = (viaje) => {
    modalTipo.value = 'variante';
    selectedViajeBase.value = viaje;
    isModalOpen.value = true;
};

const handleSaveViaje = async (data) => {
    try {
        if (modalTipo.value === 'base') {
            await createViajeBase(data);
            showToast('Viaje base creado correctamente');
        } else {
            await createVariante(data);
            showToast('Variante creada correctamente');
            // Recargar el viaje base para mostrar la nueva variante
            await refreshViajeVariantes(selectedViajeBase.value.id);
        }
    } catch (err) {
        showToast(err.message, 'error');
    }
};

const refreshViajeVariantes = async (viajeBaseId) => {
    const data = await getVariantes(viajeBaseId);
    const index = viajesBase.value.findIndex(v => v.id === viajeBaseId);
    if (index !== -1) {
        viajesBase.value[index].variantes = data;
    }
};

const goHome = () => router.push('/');

const handleBuscarSocio = async (dni) => {
    try {
        const socio = await SociosService.getByDni(dni);
        if (socio) {
            selectedSocio.value = socio;
            isViajeModalOpen.value = true;
        } else {
            showToast('Socio no encontrado', 'error');
        }
    } catch (err) {
        showToast(err.message, 'error');
    }
};

const handleConfirmViaje = (viaje) => {
    viajeParaInscripcion.value = viaje;
    isVarianteModalOpen.value = true;
};

const handleConfirmInscripcion = async (data) => {
    try {
        await inscribirSocio(data.varianteId, selectedSocio.value.id, data.montoInicial);
        showToast('Socio inscripto correctamente');
        // Opcional: Refrescar datos si es necesario, pero aquí solo estamos en el dashboard
    } catch (err) {
        showToast(err.message, 'error');
    }
};

const handleViewDetails = (id) => {
    router.push(`/viajes/${id}`);
};
</script>

<template>
    <div class="min-h-screen bg-slate-50 font-sans text-slate-800 pb-12">
        <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 pt-24">
            <!-- Header Section -->
            <div class="mb-8 flex flex-col sm:flex-row justify-between items-start sm:items-end gap-6">
                <div>
                    <nav class="flex mb-4" aria-label="Breadcrumb">
                        <ol class="inline-flex items-center space-x-1 md:space-x-3">
                            <li class="inline-flex items-center">
                                <a @click.prevent="goHome" href="#" class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-teal-600 transition-colors">
                                    <svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
                                        <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a2 2 0 002 2h2a1 1 0 002 2h2a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1h2a2 2 0 002-2v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z" />
                                    </svg>
                                    Inicio
                                </a>
                            </li>
                            <li>
                                <div class="flex items-center">
                                    <svg class="w-4 h-4 text-slate-400 mx-1" fill="currentColor" viewBox="0 0 20 20">
                                        <path fill-rule="evenodd" d="M7.293 14.707a1 1 0 010-1.414L10.586 10 7.293 6.707a1 1 0 011.414-1.414l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                                    </svg>
                                    <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2">Gestión de Viajes</span>
                                </div>
                            </li>
                        </ol>
                    </nav>
                    <h2 class="text-3xl font-black text-slate-900 tracking-tight">Dashboard de Viajes</h2>
                    <p class="text-slate-500 mt-1 font-medium italic">Administración de viajes base y sus variantes disponibles.</p>
                </div>
                <button 
                    @click="openNuevoViajeBase"
                    class="w-full sm:w-auto inline-flex justify-center items-center px-6 py-3 border border-transparent text-sm font-bold rounded-xl shadow-lg text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 transition-all hover:scale-[1.02] active:scale-[0.98]"
                >
                    <svg class="h-5 w-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
                    </svg>
                    Nuevo Viaje Base
                </button>
            </div>

            <!-- Socio Search Section -->
            <div class="mb-12">
                <BuscarSocioForm @buscar="handleBuscarSocio" />
            </div>

            <!-- Content Section -->
            <div v-if="loading" class="flex flex-col items-center justify-center py-24">
                <div class="inline-block animate-spin rounded-full h-12 w-12 border-4 border-teal-600 border-t-transparent shadow-sm"></div>
                <p class="mt-6 text-slate-500 font-bold tracking-tight">Cargando viajes...</p>
            </div>

            <div v-else-if="error" class="bg-red-50 border border-red-100 p-8 rounded-2xl text-center max-w-2xl mx-auto shadow-sm">
                <div class="w-16 h-16 bg-red-100 text-red-600 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                </div>
                <h3 class="text-xl font-bold text-red-900 mb-2">¡Oops! Algo salió mal</h3>
                <p class="text-red-700 font-medium">{{ error }}</p>
                <button @click="fetchViajesBase" class="mt-6 px-6 py-2 bg-red-600 text-white rounded-lg font-bold hover:bg-red-700 transition-all">
                    Reintentar
                </button>
            </div>

            <div v-else-if="viajesBase.length === 0" class="text-center py-24 bg-white rounded-2xl border-2 border-dashed border-slate-200 shadow-inner max-w-4xl mx-auto">
                <div class="w-20 h-20 bg-slate-50 rounded-full flex items-center justify-center mx-auto mb-6">
                    <svg class="w-10 h-10 text-slate-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7" />
                    </svg>
                </div>
                <h3 class="text-2xl font-black text-slate-900 mb-2 tracking-tight">No hay viajes registrados</h3>
                <p class="text-slate-500 font-medium mb-8">Empieza creando tu primer viaje base para ofrecer variantes a los socios.</p>
                <button @click="openNuevoViajeBase" class="px-8 py-3 bg-teal-600 text-white rounded-xl font-bold hover:bg-teal-700 transition-all shadow-md">
                    Crear mi primer Viaje
                </button>
            </div>

            <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 items-start">
                <ViajeCard 
                    v-for="viaje in viajesBase" 
                    :key="viaje.id" 
                    :viaje="viaje" 
                    @add-variante="openNuevaVariante"
                    @fetch-variantes="refreshViajeVariantes"
                    @view-details="handleViewDetails"
                    @edit-variante="(v) => console.log('Editar variante', v)"
                />
            </div>
        </main>

        <!-- Modals -->
        <CrearViajeModal 
            :is-open="isModalOpen" 
            :tipo="modalTipo" 
            :viajeBaseId="selectedViajeBase ? selectedViajeBase.id : null"
            @close="isModalOpen = false" 
            @save="handleSaveViaje"
        />

        <SeleccionarViajeModal 
            :is-open="isViajeModalOpen"
            :viajes="viajesBase"
            :socio="selectedSocio"
            @close="isViajeModalOpen = false"
            @confirm="handleConfirmViaje"
        />

        <SeleccionarVarianteModal 
            :is-open="isVarianteModalOpen"
            :viaje="viajeParaInscripcion"
            :socio="selectedSocio"
            @close="isVarianteModalOpen = false"
            @confirm="handleConfirmInscripcion"
        />

        <LoadingOverlay :active="loading || enrolling" />

        <!-- Toast -->
        <Transition
            enter-active-class="transform ease-out duration-300 transition"
            enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
            enter-to-class="translate-y-0 opacity-100 sm:translate-x-0"
            leave-active-class="transition ease-in duration-100"
            leave-from-class="opacity-100"
            leave-to-class="opacity-0"
        >
            <div v-if="toast.show" class="fixed bottom-10 right-10 z-[100] flex w-full max-w-sm overflow-hidden bg-white rounded-2xl shadow-2xl border border-slate-100 pointer-events-auto">
                <div class="flex items-center justify-center w-14" :class="{'bg-teal-500': toast.type === 'success', 'bg-red-500': toast.type === 'error'}">
                    <svg v-if="toast.type === 'success'" class="w-7 h-7 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                    </svg>
                    <svg v-else class="w-7 h-7 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </div>
                <div class="px-6 py-4">
                    <span class="font-black text-sm uppercase tracking-wider" :class="{'text-teal-500': toast.type === 'success', 'text-red-500': toast.type === 'error'}">
                        {{ toast.type === 'success' ? 'Operación Exitosa' : 'Ocurrió un Error' }}
                    </span>
                    <p class="text-sm text-slate-600 font-medium leading-relaxed">{{ toast.message }}</p>
                </div>
            </div>
        </Transition>
    </div>
</template>
