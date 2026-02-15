<script setup>
import { ref, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import AlquilerService from '../../../services/AlquilerService'
import AlquilerCard from '../Alquileres/AlquilerCard.vue'
import Pagination from '../../Common/Pagination.vue'

const router = useRouter()
const emit = defineEmits(['show-toast', 'view-detail'])

// State
const alquileres = ref([])
const loadingAlquileres = ref(false)
const searchDni = ref('')
const searchResultAlquileres = ref([])
const isSearching = ref(false)
const searchError = ref('')

// Pagination
const currentPage = ref(1)
const pageSize = ref(12)
const totalCount = ref(0)
const totalPages = ref(0)

// Methods
const loadAlquileresActivos = async () => {
  loadingAlquileres.value = true
  searchResultAlquileres.value = [] // Reset search
  isSearching.value = false
  searchDni.value = ''
  try {
    const result = await AlquilerService.getAllActive(currentPage.value, pageSize.value)
    alquileres.value = result.items
    totalCount.value = result.totalCount
    totalPages.value = result.totalPages
  } catch (e) {
    emit('show-toast', {
      message: 'Error cargando alquileres activos: ' + e.message,
      type: 'error',
    })
  } finally {
    loadingAlquileres.value = false
  }
}

const handlePageChange = (newPage) => {
  currentPage.value = newPage
  loadAlquileresActivos()
}

const handleSearch = async () => {
  if (!searchDni.value) {
    loadAlquileresActivos()
    return
  }

  loadingAlquileres.value = true
  isSearching.value = true
  searchError.value = ''
  try {
    const result = await AlquilerService.getBySocio(searchDni.value)
    searchResultAlquileres.value = result
  } catch (e) {
    searchError.value = e.message
    searchResultAlquileres.value = []
  } finally {
    loadingAlquileres.value = false
  }
}

const goToAlquilerDetail = (id) => {
  // Can emit to parent to handle navigation, or just navigate here.
  // Parent method was: router.push(`/ortopedia/alquileres/${id}`)
  router.push(`/ortopedia/alquileres/${id}`)
}

// Watchers
watch(searchDni, () => {
  searchError.value = ''
})

onMounted(() => {
  loadAlquileresActivos()
})
</script>

<template>
  <div>
    <!-- Actions / Search Bar -->
    <div
      class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6 bg-white p-4 rounded-lg border border-slate-200 shadow-sm"
    >
      <div class="w-full sm:w-auto flex-1 max-w-lg">
        <label for="search-dni" class="block text-xs font-medium text-slate-500 mb-1"
          >Buscar cualquier alquiler por el socio</label
        >
        <div class="relative rounded-md shadow-sm">
          <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
            <svg
              class="h-5 w-5 text-slate-400"
              xmlns="http://www.w3.org/2000/svg"
              viewBox="0 0 20 20"
              fill="currentColor"
              aria-hidden="true"
            >
              <path
                fill-rule="evenodd"
                d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z"
                clip-rule="evenodd"
              />
            </svg>
          </div>
          <input
            type="text"
            id="search-dni"
            v-model="searchDni"
            @keyup.enter="handleSearch"
            class="block w-full rounded-md border-slate-300 pl-10 focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
            placeholder="Ingrese aquí el DNI del socio"
          />
          <button
            v-if="searchDni"
            @click="loadAlquileresActivos()"
            class="absolute inset-y-0 right-0 pr-3 flex items-center text-slate-400 hover:text-slate-600"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-4 w-4"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>
      </div>
      <button
        @click="handleSearch"
        class="w-full sm:w-auto inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-teal-700 bg-teal-100 hover:bg-teal-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 transition-colors"
      >
        Buscar
      </button>
    </div>

    <!-- Loading -->
    <div v-if="loadingAlquileres" class="flex justify-center py-12">
      <svg
        class="animate-spin h-8 w-8 text-teal-600"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
      >
        <circle
          class="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          stroke-width="4"
        ></circle>
        <path
          class="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
        ></path>
      </svg>
    </div>

    <!-- Error -->
    <div
      v-if="searchError && !loadingAlquileres"
      class="bg-red-50 text-red-700 p-4 rounded-lg mb-6 text-center"
    >
      {{ searchError }}
    </div>

    <!-- Results -->
    <div v-if="!loadingAlquileres">
      <h3 class="text-sm font-medium text-slate-500 uppercase tracking-wider mb-4">
        {{ isSearching ? 'Resultados de búsqueda' : 'Alquileres Activos' }}
      </h3>

      <!-- List -->
      <div
        v-if="(isSearching ? searchResultAlquileres : alquileres).length > 0"
        class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
      >
        <AlquilerCard
          v-for="alq in isSearching ? searchResultAlquileres : alquileres"
          :key="alq.id"
          :alquiler="alq"
          @view-detail="goToAlquilerDetail"
        />
      </div>

      <!-- Pagination -->
      <div v-if="!isSearching && alquileres.length > 0" class="mt-8">
        <Pagination
          :current-page="currentPage"
          :total-pages="totalPages"
          :total-count="totalCount"
          :page-size="pageSize"
          @change-page="handlePageChange"
        />
      </div>

      <!-- Empty State -->
      <div
        v-else
        class="text-center py-12 bg-white rounded-lg border border-slate-200 border-dashed"
      >
        <svg
          class="mx-auto h-12 w-12 text-slate-400"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          aria-hidden="true"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
          />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-slate-900">No se encontraron alquileres</h3>
        <p class="mt-1 text-sm text-slate-500">
          {{ isSearching ? 'Intente con otro DNI.' : 'No hay alquileres activos en este momento.' }}
        </p>
      </div>
    </div>
  </div>
</template>
