<script setup>
import { computed, ref } from 'vue'
import CobranzasService from '../../../services/CobranzasService'
import Pagination from '../../Common/Pagination.vue'
import CobradorSocioCard from './CobradorSocioCard.vue'
import LoteInfoCard from './LoteInfoCard.vue'
import LoteSelector from './LoteSelector.vue'

const props = defineProps({
    lotes: {
        type: Array,
        required: true
    },
    loadingLotes: {
        type: Boolean,
        default: false
    }
})

const emit = defineEmits(['pay', 'edit', 'delete'])

// Estado
const selectedLote = ref('')
const socios = ref([])
const loadingSocios = ref(false)
const errorSocios = ref('')

const currentLote = computed(() => {
    return props.lotes.find((l) => l.id == selectedLote.value)
})

// Pagination state
const currentPage = ref(1)
const totalPages = ref(0)
const totalCount = ref(0)
const pageSize = ref(10)

// Buscar socios por lote
const buscarSocios = async (page = 1) => {
    if (!selectedLote.value) {
        socios.value = []
        currentPage.value = 1
        totalPages.value = 0
        totalCount.value = 0
        return
    }

    loadingSocios.value = true
    errorSocios.value = ''
    try {
        const data = await CobranzasService.listarSociosDeudoresPorLote(
            selectedLote.value,
            page,
            pageSize.value,
        )
        socios.value = data.items
        currentPage.value = data.pageNumber
        totalPages.value = data.totalPages
        totalCount.value = data.totalCount
    } catch (e) {
        errorSocios.value = e.message
        socios.value = []
        currentPage.value = 1
        totalPages.value = 0
        totalCount.value = 0
    } finally {
        loadingSocios.value = false
    }
}

const handlePageChange = (page) => {
    buscarSocios(page)
}

// Exponer método para refrescar
defineExpose({
    refresh: () => buscarSocios(currentPage.value)
})
</script>

<template>
    <div>
        <!-- Selector e Información de Lote -->
        <div class="bg-white rounded-xl border border-slate-200 p-6 mb-8 shadow-sm">
            <div class="flex flex-col lg:flex-row lg:items-center gap-8">
                <LoteSelector v-model="selectedLote" :lotes="lotes" :loading="loadingLotes" @change="buscarSocios()" />
                <LoteInfoCard :lote="currentLote" />
            </div>
        </div>

        <!-- Loading State -->
        <div v-if="loadingSocios" class="flex justify-center py-12">
            <svg class="animate-spin h-10 w-10 text-teal-600" xmlns="http://www.w3.org/2000/svg" fill="none"
                viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                <path class="opacity-75" fill="currentColor"
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                </path>
            </svg>
        </div>

        <!-- Error State -->
        <div v-else-if="errorSocios"
            class="bg-red-50 border border-red-200 rounded-xl p-4 mb-6 text-red-700 flex items-center gap-3">
            <svg class="h-6 w-6 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            {{ errorSocios }}
        </div>

        <!-- Lista de Socios -->
        <div v-else-if="socios.length > 0">
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                <CobradorSocioCard v-for="socio in socios" :key="socio.id" :socio="socio"
                    @pay="(data) => $emit('pay', data)" @edit="(data) => $emit('edit', data)"
                    @delete="(data) => $emit('delete', data)" />
            </div>

            <!-- Pagination -->
            <div class="mt-8">
                <Pagination :current-page="currentPage" :total-pages="totalPages" :total-count="totalCount"
                    :page-size="pageSize" @change-page="handlePageChange" />
            </div>
        </div>

        <!-- Empty State -->
        <div v-else-if="selectedLote && !loadingSocios"
            class="text-center py-12 bg-white rounded-xl border border-dashed border-slate-300">
            <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-slate-100 mb-4">
                <svg class="h-8 w-8 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
            </div>
            <h3 class="text-lg font-medium text-slate-900">No hay socios asignados a este lote</h3>
            <p class="text-slate-500 mt-1">Seleccione otro lote o verifique más tarde.</p>
        </div>

        <div v-else class="text-center py-12 text-slate-500">
            Seleccione un lote para comenzar la búsqueda.
        </div>
    </div>
</template>
