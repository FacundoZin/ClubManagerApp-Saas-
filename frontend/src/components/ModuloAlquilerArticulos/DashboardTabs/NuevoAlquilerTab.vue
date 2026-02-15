<script setup>
import { ref, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import AlquilerService from '../../../services/AlquilerService'
import SociosService from '../../../services/SociosService'
import ArticuloService from '../../../services/ArticuloService'
import SocioRentStatusCard from '../Alquileres/SocioRentStatusCard.vue'

const router = useRouter()
const emit = defineEmits(['show-toast', 'alquiler-created'])

// State - Nuevo Alquiler
const searchSocioDni = ref('')
const searchingSocio = ref(false)
const foundSocio = ref(null)
const rentCheckStatus = ref(null)
const newAlquilerItems = ref([]) // { articuloId, cantidad, nombre, precio }
const observaciones = ref('')
const isRegisteringAlquiler = ref(false)
const searchError = ref('')

// State - Articulos available for selection
const articulos = ref([])
const loadingArticulos = ref(false)

// Methods - Articulos
const loadArticulos = async () => {
  loadingArticulos.value = true
  try {
    const result = await ArticuloService.getAll()
    articulos.value = result
  } catch (e) {
    emit('show-toast', { message: 'Error cargando artículos: ' + e.message, type: 'error' })
  } finally {
    loadingArticulos.value = false
  }
}

// Methods - Search Socio
const handleSearchSocio = async () => {
  if (!searchSocioDni.value) return
  searchingSocio.value = true
  foundSocio.value = null
  rentCheckStatus.value = null
  searchError.value = ''
  try {
    rentCheckStatus.value = await AlquilerService.getAlquilerStatusBySocio(searchSocioDni.value)
  } catch (e) {
    searchError.value = e.message
  } finally {
    searchingSocio.value = false
  }
}

const startNewAlquiler = async () => {
  searchingSocio.value = true
  try {
    const socio = await SociosService.getByDni(searchSocioDni.value)
    foundSocio.value = socio
    rentCheckStatus.value = null
  } catch (e) {
    searchError.value = 'Error al cargar datos del socio: ' + e.message
  } finally {
    searchingSocio.value = false
  }
}

const navigateToAlquiler = (idAlquiler) => {
  router.push(`/ortopedia/alquileres/${idAlquiler}`)
}

const cancelSearch = () => {
  rentCheckStatus.value = null
  searchSocioDni.value = ''
}

const resetNuevoAlquiler = () => {
  foundSocio.value = null
  rentCheckStatus.value = null
  searchSocioDni.value = ''
  newAlquilerItems.value = []
  observaciones.value = ''
  searchError.value = ''
  loadArticulos() // Refresh articles availability/prices if needed
}

// Methods - Cart Management
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

const handleRegisterAlquiler = async () => {
  if (newAlquilerItems.value.length === 0) {
    emit('show-toast', { message: 'Debe agregar al menos un artículo', type: 'error' })
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
    emit('show-toast', { message: 'Alquiler registrado correctamente', type: 'success' })
    emit('alquiler-created', result) // Let parent handle redirection to list or detail
    resetNuevoAlquiler()
  } catch (e) {
    emit('show-toast', { message: e.message, type: 'error' })
  } finally {
    isRegisteringAlquiler.value = false
  }
}

// Watchers
watch(searchSocioDni, () => {
  searchError.value = ''
})

onMounted(() => {
  loadArticulos()
})
</script>

<template>
  <div class="h-full">
    <!-- Paso 1: Buscar Socio -->
    <div v-if="!foundSocio && !rentCheckStatus" class="max-w-2xl mx-auto mt-10">
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

        <div class="flex flex-col sm:flex-row gap-2 max-w-md mx-auto">
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

    <!-- Paso 1.5: Resultado de Verificación (Estado del Socio) -->
    <div v-else-if="rentCheckStatus" class="max-w-2xl mx-auto py-8">
      <SocioRentStatusCard
        :status-data="rentCheckStatus"
        @create-new="startNewAlquiler"
        @view-detail="navigateToAlquiler"
        @cancel="cancelSearch"
      />
    </div>

    <!-- Paso 2: Formulario de Alquiler -->
    <div v-else-if="foundSocio" class="grid grid-cols-1 lg:grid-cols-3 gap-8">
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
            class="flex flex-col sm:items-center sm:flex-row gap-4 sm:gap-6 bg-slate-50 p-4 sm:p-6 rounded-xl border border-slate-100"
          >
            <div
              class="w-12 h-12 sm:w-16 sm:h-16 bg-teal-100 text-teal-700 rounded-full flex items-center justify-center text-lg sm:text-xl font-black shadow-inner self-center sm:self-auto"
            >
              {{ foundSocio.nombre[0] }}{{ foundSocio.apellido[0] }}
            </div>
            <div
              class="flex-1 grid grid-cols-1 sm:grid-cols-3 gap-4 sm:gap-6 text-center sm:text-left"
            >
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

          <div v-if="loadingArticulos" class="flex justify-center py-8">
            <svg
              class="animate-spin h-6 w-6 text-teal-600"
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

          <div v-else-if="articulos.length > 0" class="grid grid-cols-1 sm:grid-cols-2 gap-4">
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
                class="p-2 bg-teal-50 text-teal-600 rounded-lg sm:opacity-0 sm:group-hover:opacity-100 hover:bg-teal-600 hover:text-white transition-all"
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
        <div class="bg-white p-6 rounded-xl border border-slate-200 shadow-sm sticky top-6">
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
              <p class="text-xs italic text-center">No se han agregado artículos al alquiler.</p>
            </div>
            <ul v-else class="divide-y divide-slate-100">
              <li
                v-for="(item, index) in newAlquilerItems"
                :key="index"
                class="py-3 flex justify-between items-start"
              >
                <div>
                  <p class="text-sm font-medium text-slate-900">{{ item.nombre }}</p>
                  <p class="text-xs text-slate-500">Cantidad: {{ item.cantidad }}</p>
                </div>
                <div class="flex items-center gap-3">
                  <p class="text-sm font-bold text-teal-600">
                    ${{ (item.precio * item.cantidad).toLocaleString() }}
                  </p>
                  <button
                    @click="removeArticuloFromAlquiler(index)"
                    class="text-slate-400 hover:text-red-500 transition-colors"
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
                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                      />
                    </svg>
                  </button>
                </div>
              </li>
            </ul>
          </div>

          <!-- Total -->
          <div
            v-if="newAlquilerItems.length > 0"
            class="flex justify-between items-center py-4 border-t border-slate-100"
          >
            <span class="text-sm font-medium text-slate-600">Total Mensual</span>
            <span class="text-xl font-bold text-slate-900">
              ${{
                newAlquilerItems
                  .reduce((sum, item) => sum + item.precio * item.cantidad, 0)
                  .toLocaleString()
              }}
            </span>
          </div>

          <!-- Observaciones -->
          <div class="mb-6">
            <label
              for="observaciones"
              class="block text-xs font-bold text-slate-500 uppercase tracking-wider mb-2"
            >
              Observaciones (Opcional)
            </label>
            <textarea
              id="observaciones"
              v-model="observaciones"
              rows="3"
              class="shadow-sm focus:ring-teal-500 focus:border-teal-500 block w-full sm:text-sm border-slate-300 rounded-lg"
              placeholder="Detalles adicionales..."
            ></textarea>
          </div>

          <!-- Button -->
          <button
            @click="handleRegisterAlquiler"
            :disabled="isRegisteringAlquiler || newAlquilerItems.length === 0"
            class="w-full flex justify-center py-3 px-4 border border-transparent rounded-lg shadow-sm text-sm font-bold text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
          >
            <svg
              v-if="isRegisteringAlquiler"
              class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
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
            {{ isRegisteringAlquiler ? 'Registrando...' : 'Confirmar Alquiler' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
