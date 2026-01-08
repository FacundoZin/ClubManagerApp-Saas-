<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import ArticuloService from '../services/ArticuloService'
import AlquilerService from '../services/AlquilerService'

// Componentes
import ArticuloCard from '../components/ModuloAlquilerArticulos/Articulos/ArticuloCard.vue'
import ArticuloFormModal from '../components/ModuloAlquilerArticulos/Articulos/ArticuloFormModal.vue'
import UpdatePrecioModal from '../components/ModuloAlquilerArticulos/Articulos/UpdatePrecioModal.vue'
import AlquilerCard from '../components/ModuloAlquilerArticulos/Alquileres/AlquilerCard.vue'

const router = useRouter()

// Tabs
const activeTab = ref('articulos') // 'articulos' | 'alquileres'

// State - Articulos
const articulos = ref([])
const isArticuloModalOpen = ref(false)
const isUpdatePrecioModalOpen = ref(false)
const selectedArticulo = ref(null)
const loadingArticulos = ref(false)

// State - Alquileres
const alquileres = ref([])
const loadingAlquileres = ref(false)
const searchDni = ref('')
const searchResultAlquileres = ref([])
const isSearching = ref(false)
const searchError = ref('')

// Toast
const toast = ref({ show: false, message: '', type: 'success' })
const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

// Watchers para limpiar errores
watch([searchDni, activeTab], () => {
  searchError.value = ''
})

// Initial Load
onMounted(() => {
  loadArticulos()
  loadAlquileresActivos()
})

// Methods - Articulos
const loadArticulos = async () => {
  loadingArticulos.value = true
  try {
    const result = await ArticuloService.getAll()
    articulos.value = result // Assuming API returns the list directly or unwrapped
  } catch (e) {
    showToast('Error cargando artículos: ' + e.message, 'error')
  } finally {
    loadingArticulos.value = false
  }
}

const handleCreateArticulo = async (newArticulo) => {
  isArticuloModalOpen.value = false
  showToast('Artículo creado correctamente')
  loadArticulos()
}

const openUpdatePrecio = (articulo) => {
  selectedArticulo.value = articulo
  isUpdatePrecioModalOpen.value = true
}

const handleUpdatePrecio = () => {
  isUpdatePrecioModalOpen.value = false
  selectedArticulo.value = null
  showToast('Precio actualizado correctamente')
  loadArticulos()
}

// Methods - Alquileres
const loadAlquileresActivos = async () => {
  loadingAlquileres.value = true
  searchResultAlquileres.value = [] // Reset search
  isSearching.value = false
  searchDni.value = ''
  try {
    const result = await AlquilerService.getAllActive()
    alquileres.value = result // Assuming list
  } catch (e) {
    showToast('Error cargando alquileres activos: ' + e.message, 'error')
  } finally {
    loadingAlquileres.value = false
  }
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
  router.push(`/ortopedia/alquileres/${id}`)
}

const goHome = () => router.push('/')
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <!-- Header -->
    <header class="bg-white border-b border-slate-200 sticky top-0 z-30 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16 items-center">
          <div class="flex items-center gap-3 cursor-pointer" @click="goHome">
            <div
              class="w-9 h-9 bg-teal-600 rounded-lg flex items-center justify-center shadow-md text-white"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z"
                />
              </svg>
            </div>
            <div>
              <h1 class="text-lg font-bold text-slate-900 tracking-tight leading-none">
                Sistema Club Abuelos
              </h1>
              <span class="text-xs text-slate-500 font-medium">Alquiler de Artículos</span>
            </div>
          </div>

          <div class="flex items-center gap-6">
            <div class="hidden md:flex flex-col items-end">
              <span class="text-xs font-semibold text-slate-700">Administrador</span>
              <span class="text-[10px] text-slate-400 uppercase tracking-wider">
                {{
                  new Date().toLocaleDateString('es-AR', {
                    weekday: 'long',
                    day: 'numeric',
                    month: 'short',
                  })
                }}
              </span>
            </div>
            <div
              class="h-9 w-9 rounded-full bg-slate-100 border border-slate-200 flex items-center justify-center text-slate-600 text-xs font-bold shadow-sm ring-2 ring-white"
            >
              AD
            </div>
          </div>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb -->
      <div class="mb-6">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a
                href="#"
                @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-teal-600"
              >
                <svg
                  class="w-3 h-3 mr-2.5"
                  aria-hidden="true"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path
                    d="m19.707 9.293-2-2-7-7a1 1 0 0 0-1.414 0l-7 7-2 2a1 1 0 0 0 1.414 1.414L2 10.414V18a2 2 0 0 0 2 2h3a1 1 0 0 0 1-1v-4a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v4a1 1 0 0 0 1 1h3a2 2 0 0 0 2-2v-7.586l.293.293a1 1 0 0 0 1.414-1.414Z"
                  />
                </svg>
                Inicio
              </a>
            </li>
            <li>
              <div class="flex items-center">
                <svg
                  class="w-3 h-3 text-slate-400 mx-1"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 6 10"
                >
                  <path
                    stroke="currentColor"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="m1 9 4-4-4-4"
                  />
                </svg>
                <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2"
                  >Alquiler Artículos</span
                >
              </div>
            </li>
          </ol>
        </nav>
        <div class="flex justify-between items-end">
          <div>
            <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Ortopedia y Alquileres</h2>
            <p class="text-slate-500 mt-1">
              Gestión de artículos ortopédicos y seguimiento de alquileres.
            </p>
          </div>
          <div>
            <button
              v-if="activeTab === 'articulos'"
              @click="isArticuloModalOpen = true"
              class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 transition-colors"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-2"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 4v16m8-8H4"
                />
              </svg>
              Nuevo Artículo
            </button>
            <!-- Create Rental could go here or as a floating button, or within Alquileres tab -->
          </div>
        </div>
      </div>

      <!-- Tabs -->
      <div class="border-b border-slate-200 mb-6">
        <nav class="-mb-px flex space-x-8" aria-label="Tabs">
          <button
            @click="activeTab = 'articulos'"
            :class="[
              activeTab === 'articulos'
                ? 'border-teal-500 text-teal-600'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors',
            ]"
          >
            Gestión de Artículos
          </button>
          <button
            @click="activeTab = 'alquileres'"
            :class="[
              activeTab === 'alquileres'
                ? 'border-teal-500 text-teal-600'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors',
            ]"
          >
            Registro y Gestión de Alquileres
          </button>
        </nav>
      </div>

      <!-- Tab Content: ARTICULOS -->
      <div v-if="activeTab === 'articulos'">
        <div v-if="loadingArticulos" class="flex justify-center py-12">
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
        <div
          v-else-if="articulos.length > 0"
          class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6"
        >
          <ArticuloCard
            v-for="art in articulos"
            :key="art.id"
            :articulo="art"
            @update-price="openUpdatePrecio"
          />
        </div>
        <div v-else class="text-center py-12 text-slate-500">
          No hay artículos registrados. Comience creando uno.
        </div>
      </div>

      <!-- Tab Content: ALQUILERES -->
      <div v-if="activeTab === 'alquileres'">
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
            class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-teal-700 bg-teal-100 hover:bg-teal-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 transition-colors"
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
              {{
                isSearching ? 'Intente con otro DNI.' : 'No hay alquileres activos en este momento.'
              }}
            </p>
          </div>
        </div>
      </div>
    </main>

    <!-- Modals -->
    <ArticuloFormModal
      :is-open="isArticuloModalOpen"
      @close="isArticuloModalOpen = false"
      @save="handleCreateArticulo"
    />
    <UpdatePrecioModal
      :is-open="isUpdatePrecioModalOpen"
      :articulo="selectedArticulo"
      @close="isUpdatePrecioModalOpen = false"
      @save="handleUpdatePrecio"
    />

    <!-- Toast -->
    <Transition
      enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
      enter-to-class="translate-y-0 opacity-100 sm:translate-x-0"
      leave-active-class="transition ease-in duration-100"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="toast.show"
        class="fixed bottom-5 right-5 z-50 flex w-full max-w-sm overflow-hidden bg-white rounded-lg shadow-2xl border border-slate-200 pointer-events-auto ring-1 ring-black ring-opacity-5"
      >
        <div
          class="flex items-center justify-center w-12"
          :class="{ 'bg-teal-500': toast.type === 'success', 'bg-red-500': toast.type === 'error' }"
        >
          <svg
            v-if="toast.type === 'success'"
            class="w-6 h-6 text-white"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M5 13l4 4L19 7"
            />
          </svg>
          <svg
            v-else
            class="w-6 h-6 text-white"
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
        </div>
        <div class="px-4 py-3">
          <span
            class="font-semibold"
            :class="{
              'text-teal-500': toast.type === 'success',
              'text-red-500': toast.type === 'error',
            }"
          >
            {{ toast.type === 'success' ? 'Éxito' : 'Error' }}
          </span>
          <p class="text-sm text-slate-600">{{ toast.message }}</p>
        </div>
      </div>
    </Transition>
  </div>
</template>
