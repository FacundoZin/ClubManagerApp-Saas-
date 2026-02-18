<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import DataTable from '../../components/Common/DataTable.vue'
import LoadingOverlay from '../../components/Common/LoadingOverlay.vue'
import CuotasService from '../../services/CuotasService'

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
              <a href="#" @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-blue-600">
                <svg class="w-3 h-3 mr-2.5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor"
                  viewBox="0 0 20 20">
                  <path
                    d="m19.707 9.293-2-2-7-7a1 1 0 0 0-1.414 0l-7 7-2 2a1 1 0 0 0 1.414 1.414L2 10.414V18a2 2 0 0 0 2 2h3a1 1 0 0 0 1-1v-4a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v4a1 1 0 0 0 1 1h3a2 2 0 0 0 2-2v-7.586l.293.293a1 1 0 0 0 1.414-1.414Z" />
                </svg>
                Inicio
              </a>
            </li>
            <li>
              <div class="flex items-center">
                <svg class="w-3 h-3 text-slate-400 mx-1" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                  fill="none" viewBox="0 0 6 10">
                  <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="m1 9 4-4-4-4" />
                </svg>
                <a href="#" @click.prevent="goToCuotasDashboard"
                  class="ml-1 text-sm font-medium text-slate-500 hover:text-blue-600 md:ml-2">Gestión de Cuotas</a>
              </div>
            </li>
            <li>
              <div class="flex items-center">
                <svg class="w-3 h-3 text-slate-400 mx-1" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                  fill="none" viewBox="0 0 6 10">
                  <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="m1 9 4-4-4-4" />
                </svg>
                <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2">Historial de Cuotas</span>
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
      <div class="bg-white rounded-3xl border border-slate-200 shadow-sm p-6 mb-6">
        <h3 class="text-lg font-bold text-slate-900 mb-4">Filtros de Búsqueda</h3>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <!-- Tipo de Filtro -->
          <div>
            <label class="block text-sm font-bold text-slate-700 mb-2">Tipo de Filtro</label>
            <select v-model="tipoFiltro"
              class="block w-full py-3 px-4 border border-slate-300 bg-white rounded-xl shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm transition-all">
              <option value="fecha">Por Fecha de Pago</option>
              <option value="periodo">Por Periodo</option>
            </select>
          </div>

          <!-- Filtro por Fecha -->
          <div v-if="tipoFiltro === 'fecha'">
            <label class="block text-sm font-bold text-slate-700 mb-2">Fecha de Pago</label>
            <input type="date" v-model="fechaPago"
              class="block w-full py-3 px-4 border border-slate-300 rounded-xl shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm transition-all" />
          </div>

          <!-- Filtro por Periodo -->
          <template v-if="tipoFiltro === 'periodo'">
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Año</label>
              <input type="number" v-model.number="anio" min="2000" :max="new Date().getFullYear() + 1"
                class="block w-full py-3 px-4 border border-slate-300 rounded-xl shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm transition-all" />
            </div>
            <div>
              <label class="block text-sm font-bold text-slate-700 mb-2">Semestre</label>
              <select v-model.number="semestre"
                class="block w-full py-3 px-4 border border-slate-300 bg-white rounded-xl shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm transition-all">
                <option :value="1">Primer Semestre</option>
                <option :value="2">Segundo Semestre</option>
              </select>
            </div>
          </template>

          <!-- Botón Buscar -->
          <div class="flex items-end">
            <button @click="aplicarFiltros" :disabled="isLoading"
              class="w-full inline-flex justify-center items-center px-6 py-3 border border-transparent text-sm font-bold rounded-xl shadow-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none disabled:opacity-50 transition-all active:scale-95">
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              Buscar
            </button>
          </div>
        </div>
      </div>

      <!-- Tabla de Resultados -->
      <DataTable :headers="[
        { label: 'Socio', key: 'socio' },
        { label: 'Fecha de Pago', key: 'fechaPago' },
        { label: 'Periodo', key: 'periodo' },
        { label: 'Monto', key: 'monto', class: 'font-bold text-emerald-600' },
        { label: 'Forma de Pago', key: 'formaDePago' },
      ]" :items="cuotas" :isLoading="isLoading" :showPagination="true" :paginationData="paginationData"
        @change-page="handlePageChange">
        <template #cell(socio)="{ item }">
          <span class="font-medium text-slate-900">{{ item.apellidoSocio }}, {{ item.nombreSocio }}</span>
        </template>
        <template #cell(fechaPago)="{ item }">
          {{ formatearFecha(item.fechaPago) }}
        </template>
        <template #cell(periodo)="{ item }">
          <span class="font-medium">{{ item.anio }} - {{ item.semestre }}° Semestre</span>
        </template>
        <template #cell(monto)="{ item }"> <span class="font-bold text-emerald-600">${{ item.monto.toFixed(2) }}</span>
        </template>
        <template #cell(formaDePago)="{ item }">
          <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold border" :class="{
            'bg-blue-50 text-blue-700 border-blue-200': item.formaDePago === 2,
            'bg-purple-50 text-purple-700 border-purple-200': item.formaDePago === 1,
            'bg-orange-50 text-orange-700 border-orange-200': item.formaDePago === 0,
          }">
            {{ getFormaPagoLabel(item.formaDePago) }}
          </span>
        </template>
      </DataTable>

    </main>

    <!-- Toast Notification -->
    <Transition enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
      enter-to-class="translate-y-0 opacity-100 sm:translate-x-0" leave-active-class="transition ease-in duration-100"
      leave-from-class="opacity-100" leave-to-class="opacity-0">
      <div v-if="toast.show"
        class="fixed bottom-5 right-5 z-50 flex w-full max-w-sm overflow-hidden bg-white rounded-lg shadow-2xl border border-slate-200 pointer-events-auto ring-1 ring-black ring-opacity-5">
        <div class="flex items-center justify-center w-12" :class="{
          'bg-emerald-500': toast.type === 'success',
          'bg-blue-500': toast.type === 'info',
          'bg-red-500': toast.type === 'error',
        }">
          <svg v-if="toast.type === 'success'" class="w-6 h-6 text-white fill-current" viewBox="0 0 40 40"
            xmlns="http://www.w3.org/2000/svg">
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM16.6667 28.3333L8.33337 20L10.6834 17.65L16.6667 23.6166L29.3167 10.9666L31.6667 13.3333L16.6667 28.3333Z" />
          </svg>
          <svg v-else class="w-6 h-6 text-white fill-current" viewBox="0 0 40 40" xmlns="http://www.w3.org/2000/svg">
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM21.6667 28.3333H18.3334V25H21.6667V28.3333ZM21.6667 21.6666H18.3334V11.6666H21.6667V21.6666Z" />
          </svg>
        </div>

        <div class="px-4 py-3 -mx-3">
          <div class="mx-3">
            <span class="font-semibold" :class="{
              'text-emerald-500': toast.type === 'success',
              'text-blue-500': toast.type === 'info',
              'text-red-500': toast.type === 'error',
            }">
              {{ toast.type === 'success' ? 'Éxito' : toast.type === 'info' ? 'Info' : 'Error' }}
            </span>
            <p class="text-sm text-slate-600">
              {{ toast.message }}
            </p>
          </div>
        </div>

        <button @click="toast.show = false" class="ml-auto p-2 text-slate-400 hover:text-slate-600 focus:outline-none">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </Transition>
  </div>
</template>
