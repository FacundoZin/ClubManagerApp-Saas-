<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import SalonService from '../../services/SalonService'

// Componentes del Módulo
import ConfirmModal from '../../components/Common/ConfirmModal.vue'
import AvailabilityModal from '../../components/ModuloReservaSalones/AvailabilityModal.vue'
import ReservaCard from '../../components/ModuloReservaSalones/ReservaCard.vue'
import ReservaDetailsModal from '../../components/ModuloReservaSalones/ReservaDetailsModal.vue'
import ReservaFormModal from '../../components/ModuloReservaSalones/ReservaFormModal.vue'
import ReservaList from '../../components/ModuloReservaSalones/ReservaList.vue'
import Pagination from '../../components/Common/Pagination.vue'

// -- GLOBAL STATE --
const router = useRouter()
const currentAction = ref('none')
const salones = ref([])
const errorGlobal = ref('')

// -- TOAST STATE --
const toast = ref({ show: false, message: '', type: 'success' })
const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

// -- MODALS STATE --
const isFormModalOpen = ref(false)
const isAvailabilityModalOpen = ref(false)
const isDetailsModalOpen = ref(false)
const isConfirmDeleteOpen = ref(false)
const availabilityResult = ref(null)
const availabilityLoading = ref(false)
const reservationToDelete = ref(null)
const selectedReservaId = ref(null)

// -- SELECTION STATE --
const selectedSalonId = ref('')
const selectedDate = ref('')
const reservationsList = ref([])
const searchResult = ref(null)
const searchPerformed = ref(false)
const isLoadingData = ref(false)
const dataError = ref('')

// -- PAGINATION STATE --
const currentPage = ref(1)
const pageSize = ref(12)
const totalCount = ref(0)
const totalPages = ref(0)

// -- ACTIONS CONFIG --
const actions = [
  {
    id: 'list',
    title: 'Listar Reservas',
    description: 'Ver todas las reservas de un salón.',
    icon: 'M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10',
    color: 'text-indigo-600',
    bg: 'bg-indigo-50',
    hoverBorder: 'group-hover:border-indigo-200',
  },
  {
    id: 'check',
    title: 'Verificar Disponibilidad',
    description: 'Consultar si un salón está libre en una fecha.',
    icon: 'M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z',
    color: 'text-emerald-600',
    bg: 'bg-emerald-50',
    hoverBorder: 'group-hover:border-emerald-200',
  },
  {
    id: 'search',
    title: 'Buscar Reserva',
    description: 'Buscar reservas por fecha y salón.',
    icon: 'M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z',
    color: 'text-violet-600',
    bg: 'bg-violet-50',
    hoverBorder: 'group-hover:border-violet-200',
  },
  {
    id: 'create',
    title: 'Nueva Reserva',
    description: 'Registrar una nueva reserva de salón.',
    icon: 'M12 4v16m8-8H4',
    color: 'text-blue-600',
    bg: 'bg-blue-50',
    hoverBorder: 'group-hover:border-blue-200',
  },
]

// -- INIT --
onMounted(async () => {
  try {
    salones.value = await SalonService.getAllSalons()
  } catch (e) {
    errorGlobal.value = 'Error al cargar los salones. Por favor recargue la página.'
    console.error(e)
  }
})

// -- HANDLERS --
const selectAction = (actionId) => {
  currentAction.value = actionId
  // Reset local states
  selectedSalonId.value = ''
  selectedDate.value = ''
  reservationsList.value = []
  searchResult.value = null
  searchPerformed.value = false
  dataError.value = ''

  if (actionId === 'create') {
    isFormModalOpen.value = true
  }
  // Reset pagination
  currentPage.value = 1
}

const goHome = () => router.push('/')

// LIST
const handleListReservas = async () => {
  if (!selectedSalonId.value) return
  isLoadingData.value = true
  dataError.value = ''
  reservationsList.value = []

  try {
    const result = await SalonService.getReservasBySalon(
      selectedSalonId.value,
      currentPage.value,
      pageSize.value,
    )
    reservationsList.value = result.items
    totalCount.value = result.totalCount
    totalPages.value = result.totalPages
  } catch (e) {
    dataError.value = e.message
  } finally {
    isLoadingData.value = false
  }
}

const handlePageChange = (newPage) => {
  currentPage.value = newPage
  handleListReservas()
}

const handleSalonChange = () => {
  currentPage.value = 1
  handleListReservas()
}

// CHECK AVAILABILITY
const handleCheckAvailability = async () => {
  if (!selectedSalonId.value || !selectedDate.value) return

  isAvailabilityModalOpen.value = true
  availabilityLoading.value = true
  availabilityResult.value = null

  try {
    const result = await SalonService.checkAvailability(selectedDate.value, selectedSalonId.value)
    availabilityResult.value = result
  } catch (e) {
    availabilityResult.value = { exit: false, errormessage: e.message }
  } finally {
    availabilityLoading.value = false
  }
}

// SEARCH ONE
const handleSearchReserva = async () => {
  if (!selectedSalonId.value || !selectedDate.value) return
  isLoadingData.value = true
  dataError.value = ''
  searchResult.value = null
  searchPerformed.value = true

  try {
    const result = await SalonService.getReservaByFecha(selectedDate.value, selectedSalonId.value)
    searchResult.value = result
  } catch (e) {
    dataError.value = e.message
  } finally {
    isLoadingData.value = false
  }
}

// CREATE
const handleSaveReserva = (newReserva) => {
  isFormModalOpen.value = false
  showToast('Reserva creada exitosamente')
  // Optionally refresh list if we were in list mode and same salon
  if (currentAction.value === 'list' && selectedSalonId.value == newReserva.salonId) {
    handleListReservas()
  }
}

// ACTIONS (DELETE/VIEW)
const handleDeleteRequest = (reserva) => {
  reservationToDelete.value = reserva
  isConfirmDeleteOpen.value = true
}

const confirmDelete = async () => {
  if (!reservationToDelete.value) return
  try {
    await SalonService.cancelReserva(
      reservationToDelete.value.id || reservationToDelete.value.idReserva,
    )
    showToast('Reserva cancelada correctamente')

    // Refresh Current View
    if (currentAction.value === 'list') await handleListReservas()
    if (currentAction.value === 'search') await handleSearchReserva()
  } catch (e) {
    showToast(e.message, 'error')
  } finally {
    isConfirmDeleteOpen.value = false
    reservationToDelete.value = null
  }
}

const handleViewDetails = (id) => {
  selectedReservaId.value = id
  isDetailsModalOpen.value = true
}

const refreshData = async () => {
  if (currentAction.value === 'list') await handleListReservas()
  if (currentAction.value === 'search') await handleSearchReserva()
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <!-- Header -->
    <!-- Header -->

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb -->
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
                  >Reservas Salones</span
                >
              </div>
            </li>
          </ol>
        </nav>
        <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Gestión de Reservas</h2>
        <p class="text-slate-500 mt-1 text-lg">Administre las reservas de los espacios del club.</p>
      </div>

      <!-- Action Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-10">
        <button
          v-for="action in actions"
          :key="action.id"
          @click="selectAction(action.id)"
          class="group relative flex flex-col p-6 bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-lg hover:-translate-y-1 transition-all duration-300 text-left overflow-hidden"
          :class="[
            action.hoverBorder,
            currentAction === action.id ? 'ring-2 ring-blue-500 border-transparent' : '',
          ]"
        >
          <div
            class="absolute inset-0 bg-gradient-to-br from-slate-50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"
          ></div>

          <div class="relative z-10 flex items-start justify-between mb-5">
            <div
              class="p-3.5 rounded-lg transition-colors duration-300 shadow-sm ring-1 ring-black/5"
              :class="[action.bg, action.color]"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-6 w-6"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="1.5"
                  :d="action.icon"
                />
              </svg>
            </div>
          </div>

          <div class="relative z-10 mt-auto">
            <h3
              class="text-lg font-bold text-slate-900 mb-1 group-hover:text-blue-700 transition-colors duration-300"
            >
              {{ action.title }}
            </h3>
            <p class="text-xs text-slate-500 leading-relaxed font-medium">
              {{ action.description }}
            </p>
          </div>
        </button>
      </div>

      <!-- Content Area -->
      <div
        v-if="currentAction !== 'none'"
        class="bg-white rounded-xl border border-slate-200 shadow-sm p-6 min-h-[400px]"
      >
        <!-- CREATE PLACEHOLDER -->
        <div
          v-if="currentAction === 'create'"
          class="flex flex-col items-center justify-center h-full py-12 text-center"
        >
          <div class="p-4 bg-blue-50 rounded-full mb-4">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-8 w-8 text-blue-600"
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
          </div>
          <h3 class="text-lg font-medium text-slate-900">Nueva Reserva</h3>
          <p class="text-slate-500 mt-2">
            El formulario está abierto. Si lo cerró, ábralo nuevamente.
          </p>
          <button
            @click="isFormModalOpen = true"
            class="mt-6 inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700"
          >
            Abrir formulario
          </button>
        </div>

        <!-- LIST ACTION -->
        <div v-if="currentAction === 'list'">
          <div class="flex flex-col sm:flex-row gap-4 mb-6">
            <div class="w-full sm:w-1/3">
              <label class="block text-sm font-medium text-slate-700 mb-1">Seleccionar Salón</label>
              <select
                v-model="selectedSalonId"
                @change="handleSalonChange"
                class="block w-full rounded-md border-slate-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm border px-3 py-2"
              >
                <option value="" disabled>-- Elija un salón --</option>
                <option v-for="salon in salones" :key="salon.id" :value="salon.id">
                  {{ salon.nombre }}
                </option>
              </select>
            </div>
          </div>

          <div v-if="isLoadingData" class="flex justify-center py-12">
            <svg
              class="animate-spin h-8 w-8 text-indigo-600"
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
          <div v-else-if="dataError" class="text-red-500 text-center py-12">{{ dataError }}</div>
          <div v-else-if="selectedSalonId">
            <ReservaList
              :reservas="reservationsList"
              @view-details="handleViewDetails"
              @cancel="handleDeleteRequest"
            />
            <!-- Pagination -->
            <div v-if="reservationsList.length > 0" class="mt-6">
              <Pagination
                :current-page="currentPage"
                :total-pages="totalPages"
                :total-count="totalCount"
                :page-size="pageSize"
                @change-page="handlePageChange"
              />
            </div>
          </div>
          <div v-else class="text-center py-12 text-slate-400">
            Seleccione un salón para ver sus reservas.
          </div>
        </div>

        <!-- CHECK AVAILABILITY -->
        <div v-if="currentAction === 'check'">
          <div class="flex flex-col sm:flex-row gap-4 items-end mb-6 max-w-2xl mx-auto mt-8">
            <div class="w-full sm:w-1/2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Salón</label>
              <select
                v-model="selectedSalonId"
                class="block w-full rounded-md border-slate-300 shadow-sm focus:border-emerald-500 focus:ring-emerald-500 sm:text-sm border px-3 py-2"
              >
                <option value="" disabled>-- Elija un salón --</option>
                <option v-for="salon in salones" :key="salon.id" :value="salon.id">
                  {{ salon.nombre }}
                </option>
              </select>
            </div>
            <div class="w-full sm:w-1/2">
              <label class="block text-sm font-medium text-slate-700 mb-1">Fecha</label>
              <input
                type="date"
                v-model="selectedDate"
                class="block w-full rounded-md border-slate-300 shadow-sm focus:border-emerald-500 focus:ring-emerald-500 sm:text-sm border px-3 py-2"
              />
            </div>
            <div>
              <button
                @click="handleCheckAvailability"
                :disabled="!selectedSalonId || !selectedDate"
                class="w-full sm:w-auto px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-emerald-600 hover:bg-emerald-700 disabled:opacity-50"
              >
                Verificar
              </button>
            </div>
          </div>
          <div class="text-center text-slate-500 text-sm mt-8">
            Seleccione un salón y una fecha para comprobar si está libre.
          </div>
        </div>

        <!-- SEARCH -->
        <div v-if="currentAction === 'search'">
          <div class="flex flex-col sm:flex-row gap-4 items-end mb-6 max-w-3xl mx-auto">
            <div class="w-full sm:w-1/3">
              <label class="block text-sm font-medium text-slate-700 mb-1">Salón</label>
              <select
                v-model="selectedSalonId"
                class="block w-full rounded-md border-slate-300 shadow-sm focus:border-violet-500 focus:ring-violet-500 sm:text-sm border px-3 py-2"
              >
                <option value="" disabled>-- Elija un salón --</option>
                <option v-for="salon in salones" :key="salon.id" :value="salon.id">
                  {{ salon.nombre }}
                </option>
              </select>
            </div>
            <div class="w-full sm:w-1/3">
              <label class="block text-sm font-medium text-slate-700 mb-1">Fecha</label>
              <input
                type="date"
                v-model="selectedDate"
                class="block w-full rounded-md border-slate-300 shadow-sm focus:border-violet-500 focus:ring-violet-500 sm:text-sm border px-3 py-2"
              />
            </div>
            <div class="w-full sm:w-auto">
              <button
                @click="handleSearchReserva"
                :disabled="!selectedSalonId || !selectedDate || isLoadingData"
                class="w-full sm:w-auto px-6 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-violet-600 hover:bg-violet-700 disabled:opacity-50"
              >
                {{ isLoadingData ? 'Buscando...' : 'Buscar' }}
              </button>
            </div>
          </div>

          <div v-if="dataError" class="text-red-500 text-center py-6">{{ dataError }}</div>

          <div v-if="searchResult && !isLoadingData" class="max-w-2xl mx-auto mt-8">
            <h4 class="text-sm font-medium text-slate-500 mb-3 uppercase tracking-wider">
              Resultado de la búsqueda
            </h4>
            <ReservaCard
              :reserva="searchResult"
              @delete="handleDeleteRequest"
              @view="handleViewDetails"
            />
          </div>

          <div
            v-else-if="searchPerformed && !searchResult && !isLoadingData"
            class="text-center py-12"
          >
            <div class="inline-flex items-center justify-center p-4 bg-slate-50 rounded-full mb-3">
              <svg
                class="h-8 w-8 text-slate-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
                />
              </svg>
            </div>
            <p class="text-slate-500 max-w-xs mx-auto">
              No hay reservas registradas para
              <span class="font-semibold text-slate-700">{{
                salones.find((s) => s.id === selectedSalonId)?.nombre || 'este salón'
              }}</span>
              el día
              <span class="font-semibold text-slate-700">{{
                new Date(selectedDate + 'T00:00:00').toLocaleDateString('es-AR', {
                  day: 'numeric',
                  month: 'long',
                  year: 'numeric',
                })
              }}</span
              >.
            </p>
          </div>
        </div>
      </div>
    </main>

    <!-- Modals -->
    <ReservaFormModal
      :is-open="isFormModalOpen"
      :salones="salones"
      @close="isFormModalOpen = false"
      @save="handleSaveReserva"
    />
    <AvailabilityModal
      :is-open="isAvailabilityModalOpen"
      :loading="availabilityLoading"
      :result="availabilityResult"
      @close="isAvailabilityModalOpen = false"
    />

    <ConfirmModal
      :is-open="isConfirmDeleteOpen"
      title="Cancelar Reserva"
      message="¿Está seguro que desea cancelar esta reserva? Esta acción no se puede deshacer."
      confirm-text="Si, Cancelar"
      type="danger"
      @close="isConfirmDeleteOpen = false"
      @confirm="confirmDelete"
    />

    <ReservaDetailsModal
      :is-open="isDetailsModalOpen"
      :reserva-id="selectedReservaId"
      @close="isDetailsModalOpen = false"
      @payment-registered="refreshData"
    />

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
            'bg-red-500': toast.type === 'error',
          }"
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
              'text-emerald-500': toast.type === 'success',
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
