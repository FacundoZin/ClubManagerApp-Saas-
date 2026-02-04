<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import CuotasService from '../../services/CuotasService'
import Pagination from '../../components/Common/Pagination.vue'
import LoadingOverlay from '../../components/Common/LoadingOverlay.vue'

const router = useRouter()

// State
const isLoading = ref(false)
const cuotas = ref([])
const paginationData = ref({
  totalCount: 0,
  pageNumber: 1,
  pageSize: 10,
  totalPages: 0,
})

// Filtros
const tipoFiltro = ref('fecha')
const fechaPago = ref('')
const anio = ref(new Date().getFullYear())
const semestre = ref(1)

// Toast
const toast = ref({
  show: false,
  message: '',
  type: 'success',
})

const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(
    () => {
      toast.value.show = false
    },
    type === 'error' ? 6000 : 3000,
  )
}

const goHome = () => {
  router.push('/')
}

const goToCuotasDashboard = () => {
  router.push('/cuotas')
}

const cargarHistorial = async (pageNumber = 1) => {
  isLoading.value = true
  try {
    const filtros = {
      tipoFiltro: tipoFiltro.value,
      pageNumber,
      pageSize: paginationData.value.pageSize,
    }

    if (tipoFiltro.value === 'fecha') {
      if (!fechaPago.value) {
        showToast('Debe seleccionar una fecha', 'error')
        return
      }
      filtros.fechaPago = fechaPago.value
    } else {
      filtros.anio = anio.value
      filtros.semestre = semestre.value
    }

    const resultado = await CuotasService.obtenerHistorialCuotas(filtros)
    cuotas.value = resultado.items || []
    paginationData.value = {
      totalCount: resultado.totalCount,
      pageNumber: resultado.pageNumber,
      pageSize: resultado.pageSize,
      totalPages: resultado.totalPages,
    }
  } catch (error) {
    showToast(error.message || 'Error al cargar el historial', 'error')
  } finally {
    isLoading.value = false
  }
}

const handlePageChange = (newPage) => {
  cargarHistorial(newPage)
}

const aplicarFiltros = () => {
  cargarHistorial(1)
}

const getFormaPagoLabel = (formaPago) => {
  const formas = {
    0: 'Cobrador',
    1: 'Link de Pago',
    2: 'Sede',
  }
  return formas[formaPago] || 'Desconocido'
}

const formatearFecha = (fecha) => {
  const date = new Date(fecha)
  return date.toLocaleDateString('es-AR')
}

onMounted(() => {
  // Set default date to today
  const today = new Date()
  fechaPago.value = today.toISOString().split('T')[0]
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <LoadingOverlay :show="isLoading" />

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb & Page Title -->
      <div class="mb-8">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a
                href="#"
                @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-blue-600"
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
                  aria-hidden="true"
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
                <a
                  href="#"
                  @click.prevent="goToCuotasDashboard"
                  class="ml-1 text-sm font-medium text-slate-500 hover:text-blue-600 md:ml-2"
                  >Gestión de Cuotas</a
                >
              </div>
            </li>
            <li>
              <div class="flex items-center">
                <svg
                  class="w-3 h-3 text-slate-400 mx-1"
                  aria-hidden="true"
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
                  >Historial de Cuotas</span
                >
              </div>
            </li>
          </ol>
        </nav>
        <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Historial de Cuotas</h2>
        <p class="text-slate-500 mt-1 text-lg">
          Consulte el historial de pagos registrados filtrando por fecha o periodo.
        </p>
      </div>

      <!-- Filtros -->
      <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-6 mb-6">
        <h3 class="text-lg font-bold text-slate-900 mb-4">Filtros de Búsqueda</h3>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <!-- Tipo de Filtro -->
          <div>
            <label class="block text-sm font-medium text-slate-700 mb-2">Tipo de Filtro</label>
            <select
              v-model="tipoFiltro"
              class="block w-full py-2 px-3 border border-slate-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            >
              <option value="fecha">Por Fecha de Pago</option>
              <option value="periodo">Por Periodo</option>
            </select>
          </div>

          <!-- Filtro por Fecha -->
          <div v-if="tipoFiltro === 'fecha'">
            <label class="block text-sm font-medium text-slate-700 mb-2">Fecha de Pago</label>
            <input
              type="date"
              v-model="fechaPago"
              class="block w-full py-2 px-3 border border-slate-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
            />
          </div>

          <!-- Filtro por Periodo -->
          <template v-if="tipoFiltro === 'periodo'">
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Año</label>
              <input
                type="number"
                v-model.number="anio"
                min="2000"
                :max="new Date().getFullYear() + 1"
                class="block w-full py-2 px-3 border border-slate-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 mb-2">Semestre</label>
              <select
                v-model.number="semestre"
                class="block w-full py-2 px-3 border border-slate-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              >
                <option :value="1">Primer Semestre</option>
                <option :value="2">Segundo Semestre</option>
              </select>
            </div>
          </template>

          <!-- Botón Buscar -->
          <div class="flex items-end">
            <button
              @click="aplicarFiltros"
              :disabled="isLoading"
              class="w-full inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none disabled:opacity-50"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
                />
              </svg>
              Buscar
            </button>
          </div>
        </div>
      </div>

      <!-- Tabla de Resultados -->
      <div class="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200">
            <thead class="bg-slate-50">
              <tr>
                <th
                  class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider border-r border-slate-200"
                >
                  Socio
                </th>
                <th
                  class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider border-r border-slate-200"
                >
                  Fecha de Pago
                </th>
                <th
                  class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider border-r border-slate-200"
                >
                  Periodo
                </th>
                <th
                  class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider border-r border-slate-200"
                >
                  Monto
                </th>
                <th
                  class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider"
                >
                  Forma de Pago
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-slate-200">
              <tr v-if="cuotas.length === 0 && !isLoading">
                <td colspan="5" class="px-6 py-12 text-center text-slate-500">
                  <svg
                    class="mx-auto h-12 w-12 text-slate-400 mb-3"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                    />
                  </svg>
                  <p class="text-sm font-medium">
                    No se encontraron cuotas con los filtros seleccionados
                  </p>
                </td>
              </tr>
              <tr v-for="cuota in cuotas" :key="cuota.id" class="hover:bg-slate-50">
                <td
                  class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 border-r border-slate-200"
                >
                  {{ cuota.apellidoSocio }}, {{ cuota.nombreSocio }}
                </td>
                <td
                  class="px-6 py-4 whitespace-nowrap text-sm text-slate-500 border-r border-slate-200"
                >
                  {{ formatearFecha(cuota.fechaPago) }}
                </td>
                <td
                  class="px-6 py-4 whitespace-nowrap text-sm text-slate-500 border-r border-slate-200"
                >
                  {{ cuota.anio }} - {{ cuota.semestre }}° Semestre
                </td>
                <td
                  class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-emerald-600 border-r border-slate-200"
                >
                  ${{ cuota.monto.toFixed(2) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                  <span
                    class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                    :class="{
                      'bg-blue-100 text-blue-800': cuota.formaDePago === 2,
                      'bg-purple-100 text-purple-800': cuota.formaDePago === 1,
                      'bg-orange-100 text-orange-800': cuota.formaDePago === 0,
                    }"
                  >
                    {{ getFormaPagoLabel(cuota.formaDePago) }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Paginación -->
        <div v-if="cuotas.length > 0" class="px-6">
          <Pagination
            :current-page="paginationData.pageNumber"
            :total-pages="paginationData.totalPages"
            :total-count="paginationData.totalCount"
            :page-size="paginationData.pageSize"
            @change-page="handlePageChange"
          />
        </div>
      </div>
    </main>

    <!-- Toast Notification -->
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
          :class="{
            'bg-emerald-500': toast.type === 'success',
            'bg-blue-500': toast.type === 'info',
            'bg-red-500': toast.type === 'error',
          }"
        >
          <svg
            v-if="toast.type === 'success'"
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 40 40"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM16.6667 28.3333L8.33337 20L10.6834 17.65L16.6667 23.6166L29.3167 10.9666L31.6667 13.3333L16.6667 28.3333Z"
            />
          </svg>
          <svg
            v-else
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 40 40"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM21.6667 28.3333H18.3334V25H21.6667V28.3333ZM21.6667 21.6666H18.3334V11.6666H21.6667V21.6666Z"
            />
          </svg>
        </div>

        <div class="px-4 py-3 -mx-3">
          <div class="mx-3">
            <span
              class="font-semibold"
              :class="{
                'text-emerald-500': toast.type === 'success',
                'text-blue-500': toast.type === 'info',
                'text-red-500': toast.type === 'error',
              }"
            >
              {{ toast.type === 'success' ? 'Éxito' : toast.type === 'info' ? 'Info' : 'Error' }}
            </span>
            <p class="text-sm text-slate-600">
              {{ toast.message }}
            </p>
          </div>
        </div>

        <button
          @click="toast.show = false"
          class="ml-auto p-2 text-slate-400 hover:text-slate-600 focus:outline-none"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>
    </Transition>
  </div>
</template>
