<script setup>
import { onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AlquilerService from '../../services/AlquilerService'
import ArticuloService from '../../services/ArticuloService'
import SociosService from '../../services/SociosService'

// Componentes
import LoadingOverlay from '../../components/Common/LoadingOverlay.vue'
import AlquilerCard from '../../components/ModuloAlquilerArticulos/Alquileres/AlquilerCard.vue'
import ArticuloCard from '../../components/ModuloAlquilerArticulos/Articulos/ArticuloCard.vue'
import ArticuloFormModal from '../../components/ModuloAlquilerArticulos/Articulos/ArticuloFormModal.vue'
import UpdatePrecioModal from '../../components/ModuloAlquilerArticulos/Articulos/UpdatePrecioModal.vue'

const router = useRouter()
const route = useRoute()

// Tabs
const activeTab = ref('articulos')

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

// State - Nuevo Alquiler
const searchSocioDni = ref('')
const searchingSocio = ref(false)
const foundSocio = ref(null)
const newAlquilerItems = ref([]) // { articuloId, cantidad, nombre, precio }
const observaciones = ref('')
const isRegisteringAlquiler = ref(false)

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

watch(searchSocioDni, () => {
  searchError.value = ''
})

// Initial Load
onMounted(() => {
  if (route.query.success === 'finalizado') {
    showToast('Alquiler finalizado correctamente')
    activeTab.value = 'gestionar-alquileres'
    router.replace({ query: { ...route.query, success: undefined } })
  }
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

// Methods - Nuevo Alquiler
const handleSearchSocio = async () => {
  if (!searchSocioDni.value) return
  searchingSocio.value = true
  foundSocio.value = null
  searchError.value = ''
  try {
    foundSocio.value = await SociosService.getByDni(searchSocioDni.value)
  } catch (e) {
    searchError.value = e.message
  } finally {
    searchingSocio.value = false
  }
}

const addArticuloToAlquiler = (articulo) => {
  const existing = newAlquilerItems.value.find((i) => i.articuloId === articulo.id)
  if (existing) {
    existing.cantidad++
  } else {
    newAlquilerItems.value.push({
      articuloId: articulo.id,
      cantidad: 1,
      nombre: articulo.nombre,
      precio: articulo.precioAlquiler,
    })
  }
}

const removeArticuloFromAlquiler = (index) => {
  newAlquilerItems.value.splice(index, 1)
}

const resetNuevoAlquiler = () => {
  foundSocio.value = null
  searchSocioDni.value = ''
  newAlquilerItems.value = []
  observaciones.value = ''
  searchError.value = ''
}

const handleRegisterAlquiler = async () => {
  if (newAlquilerItems.value.length === 0) {
    showToast('Debe agregar al menos un artículo', 'error')
    return
  }

  isRegisteringAlquiler.value = true
  try {
    const dto = {
      idSocio: foundSocio.value.id,
      observaciones: observaciones.value,
      items: newAlquilerItems.value.map((i) => ({
        articuloId: i.articuloId,
        cantidad: i.cantidad,
      })),
    }
    const result = await AlquilerService.create(dto)
    showToast('Alquiler registrado correctamente')
    resetNuevoAlquiler()
    activeTab.value = 'gestionar-alquileres'
    loadAlquileresActivos()
    goToAlquilerDetail(result.idAlquiler)
  } catch (e) {
    showToast(e.message, 'error')
  } finally {
    isRegisteringAlquiler.value = false
  }
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
            @click="activeTab = 'gestionar-alquileres'"
            :class="[
              activeTab === 'gestionar-alquileres'
                ? 'border-teal-500 text-teal-600'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors',
            ]"
          >
            Gestion de Alquileres
          </button>
          <button
            @click="activeTab = 'nuevo-alquiler'"
            :class="[
              activeTab === 'nuevo-alquiler'
                ? 'border-teal-500 text-teal-600'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors',
            ]"
          >
            Registrar nuevo alquiler
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
      <div v-if="activeTab === 'gestionar-alquileres'">
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

      <!-- Tab Content: NUEVO ALQUILER -->
      <div v-if="activeTab === 'nuevo-alquiler'">
        <!-- Paso 1: Buscar Socio -->
        <div v-if="!foundSocio" class="max-w-2xl mx-auto">
          <div class="bg-white p-8 rounded-xl border border-slate-200 shadow-sm text-center">
            <div
              class="w-16 h-16 bg-teal-50 text-teal-600 rounded-full flex items-center justify-center mx-auto mb-6"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-8 w-8"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                />
              </svg>
            </div>
            <h3 class="text-xl font-bold text-slate-900 mb-2">Buscar Socio</h3>
            <p class="text-slate-500 mb-8">
              Ingrese el DNI del socio para iniciar el registro de un nuevo alquiler.
            </p>

            <div class="flex gap-2 max-w-md mx-auto">
              <div class="relative flex-1">
                <input
                  type="text"
                  v-model="searchSocioDni"
                  @keyup.enter="handleSearchSocio"
                  class="block w-full rounded-lg border-slate-300 pl-4 pr-10 focus:border-teal-500 focus:ring-teal-500 sm:text-sm py-3 border transition-shadow shadow-sm"
                  placeholder="DNI del socio..."
                />
                <div
                  v-if="searchingSocio"
                  class="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none"
                >
                  <svg
                    class="animate-spin h-5 w-5 text-teal-500"
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
              </div>
              <button
                @click="handleSearchSocio"
                :disabled="searchingSocio || !searchSocioDni"
                class="px-6 py-3 bg-teal-600 text-white rounded-lg font-semibold hover:bg-teal-700 disabled:opacity-50 transition-colors shadow-md"
              >
                Buscar
              </button>
            </div>

            <div
              v-if="searchError"
              class="mt-4 text-sm text-red-600 bg-red-50 p-3 rounded-lg border border-red-100"
            >
              {{ searchError }}
            </div>
          </div>
        </div>

        <!-- Paso 2: Formulario de Alquiler -->
        <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Columna Izquierda: Información del Socio y Selección de Artículos -->
          <div class="lg:col-span-2 space-y-6">
            <!-- Socio Info -->
            <div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm">
              <div class="flex justify-between items-start mb-4">
                <h3 class="text-lg font-bold text-slate-900">Socio Seleccionado</h3>
                <button
                  @click="resetNuevoAlquiler"
                  class="text-xs text-slate-500 hover:text-red-500 underline"
                >
                  Cambiar Socio
                </button>
              </div>
              <div
                class="flex items-center gap-6 bg-slate-50 p-6 rounded-xl border border-slate-100"
              >
                <div
                  class="w-16 h-16 bg-teal-100 text-teal-700 rounded-full flex items-center justify-center text-xl font-black shadow-inner"
                >
                  {{ foundSocio.nombre[0] }}{{ foundSocio.apellido[0] }}
                </div>
                <div class="flex-1 grid grid-cols-1 sm:grid-cols-3 gap-6">
                  <div class="sm:col-span-1">
                    <p class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1">
                      Socio
                    </p>
                    <p class="text-base font-bold text-slate-900 leading-tight">
                      {{ foundSocio.apellido }}, {{ foundSocio.nombre }}
                    </p>
                  </div>
                  <div>
                    <p class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1">
                      DNI
                    </p>
                    <p class="text-base font-medium text-slate-700">{{ foundSocio.dni }}</p>
                  </div>
                  <div>
                    <p class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1">
                      Contacto
                    </p>
                    <p class="text-base font-medium text-slate-700">
                      {{ foundSocio.telefono || 'N/A' }}
                    </p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Artículos Disponibles -->
            <div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm">
              <h3 class="text-lg font-bold text-slate-900 mb-4">Artículos Disponibles</h3>
              <div v-if="articulos.length > 0" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div
                  v-for="art in articulos"
                  :key="art.id"
                  class="border border-slate-100 rounded-lg p-3 flex justify-between items-center hover:bg-slate-50 transition-colors group"
                >
                  <div>
                    <p class="font-medium text-slate-800">{{ art.nombre }}</p>
                    <p class="text-xs text-teal-600 font-bold">
                      ${{ art.precioAlquiler.toLocaleString() }} / mes
                    </p>
                  </div>
                  <button
                    @click="addArticuloToAlquiler(art)"
                    class="p-2 bg-teal-50 text-teal-600 rounded-lg opacity-0 group-hover:opacity-100 hover:bg-teal-600 hover:text-white transition-all"
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
                        d="M12 4v16m8-8H4"
                      />
                    </svg>
                  </button>
                </div>
              </div>
              <div v-else class="text-center py-8 text-slate-400">
                No hay artículos cargados en el sistema.
              </div>
            </div>
          </div>

          <!-- Columna Derecha: Resumen y Confirmación -->
          <div class="space-y-6">
            <div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm sticky top-24">
              <h3 class="text-lg font-bold text-slate-900 mb-6">Resumen del Alquiler</h3>

              <!-- Items List -->
              <div class="space-y-4 mb-6">
                <div
                  v-if="newAlquilerItems.length === 0"
                  class="flex flex-col items-center justify-center py-10 px-4 bg-slate-50 border border-dashed border-slate-200 rounded-2xl text-slate-400"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-8 w-8 mb-2 opacity-50"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M12 9v3m0 0v3m0-3h3m-3 0H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                  <p class="text-xs italic text-center">
                    No se han agregado artículos al alquiler.
                  </p>
                </div>
                <div
                  v-else
                  v-for="(item, index) in newAlquilerItems"
                  :key="item.articuloId"
                  class="flex justify-between items-start gap-4 pb-4 border-b border-slate-50 last:border-0 last:pb-0"
                >
                  <div class="flex-1">
                    <p class="text-sm font-bold text-slate-800">{{ item.nombre }}</p>
                    <div class="flex items-center gap-2 mt-1">
                      <label class="text-[10px] text-slate-400 uppercase tracking-wider"
                        >Cant:</label
                      >
                      <div
                        class="flex items-center border border-slate-200 rounded-md overflow-hidden bg-slate-50"
                      >
                        <button
                          @click="item.cantidad > 1 ? item.cantidad-- : null"
                          class="px-2 py-1 hover:bg-slate-200 text-slate-600 transition-colors"
                        >
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            class="h-3 w-3"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                          >
                            <path
                              stroke-linecap="round"
                              stroke-linejoin="round"
                              stroke-width="3"
                              d="M20 12H4"
                            />
                          </svg>
                        </button>
                        <span
                          class="px-2 text-xs font-bold text-slate-700 min-w-[24px] text-center"
                        >
                          {{ item.cantidad }}
                        </span>
                        <button
                          @click="item.cantidad++"
                          class="px-2 py-1 hover:bg-slate-200 text-slate-600 transition-colors"
                        >
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            class="h-3 w-3"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                          >
                            <path
                              stroke-linecap="round"
                              stroke-linejoin="round"
                              stroke-width="3"
                              d="M12 4v16m8-8H4"
                            />
                          </svg>
                        </button>
                      </div>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-bold text-teal-600">
                      ${{ (item.precio * item.cantidad).toLocaleString() }}
                    </p>
                    <button
                      @click="removeArticuloFromAlquiler(index)"
                      class="text-[10px] text-red-400 hover:text-red-600 mt-1 uppercase font-bold tracking-widest"
                    >
                      Quitar
                    </button>
                  </div>
                </div>
              </div>

              <!-- Observaciones -->
              <div class="mb-6">
                <label
                  class="block text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2"
                >
                  Observaciones
                </label>
                <textarea
                  v-model="observaciones"
                  rows="3"
                  class="block w-full rounded-lg border-slate-300 text-sm focus:border-teal-500 focus:ring-teal-500 p-3 border"
                  placeholder="Ej: El artículo se entrega con detalles de uso normal..."
                ></textarea>
              </div>

              <!-- Total -->
              <div class="flex justify-between items-center py-4 border-t border-slate-100 mb-6">
                <span class="text-slate-500 font-medium">Total / Cuota Mensual:</span>
                <span class="text-2xl font-black text-slate-900">
                  ${{
                    newAlquilerItems
                      .reduce((acc, i) => acc + i.precio * i.cantidad, 0)
                      .toLocaleString()
                  }}
                </span>
              </div>

              <!-- Action -->
              <button
                @click="handleRegisterAlquiler"
                :disabled="isRegisteringAlquiler || newAlquilerItems.length === 0"
                class="w-full py-4 bg-teal-600 text-white rounded-xl font-bold flex items-center justify-center gap-2 hover:bg-teal-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-lg hover:shadow-teal-100"
              >
                <span v-if="isRegisteringAlquiler">
                  <svg
                    class="animate-spin h-5 w-5 text-white"
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
                </span>
                <span v-else>Confirmar Alquiler</span>
              </button>
            </div>
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

    <LoadingOverlay :show="isRegisteringAlquiler" message="Registrando alquiler..." />

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
