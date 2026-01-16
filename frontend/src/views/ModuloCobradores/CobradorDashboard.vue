<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import CobradorSocioCard from '../../components/ModuloCobradores/CobradorSocioCard.vue'
import SocioUpdateModal from '../../components/ModuloGestionSocios/SocioUpdateModal.vue'
import LoteFormModal from '../../components/ModuloCobradores/LoteFormModal.vue'
import Pagination from '../../components/Common/Pagination.vue'
import ConfirmModal from '../../components/Common/ConfirmModal.vue'
import CobranzasService from '../../services/CobranzasService'
import CuotasService from '../../services/CuotasService'
import SociosService from '../../services/SociosService'

// Router
const router = useRouter()
const goHome = () => router.push('/')

// Tabs
const activeTab = ref('buscar-socios')

// Estado - Lotes
const lotes = ref([])
const loadingLotes = ref(false)

// Estado - Buscar Socios por Lote
const selectedLote = ref('')
const socios = ref([])
const loadingSocios = ref(false)
const errorSocios = ref('')

// Pagination state
const currentPage = ref(1)
const totalPages = ref(0)
const totalCount = ref(0)
const pageSize = ref(10)

// Modal Editar Socio
const isEditModalOpen = ref(false)
const selectedSocioId = ref(null)

// Modal Crear Lote
const isLoteModalOpen = ref(false)

// Estados para Modales de Confirmación
const confirmModal = ref({
  isOpen: false,
  title: '',
  message: '',
  type: 'info',
  action: null,
})

const closeConfirm = () => {
  confirmModal.value.isOpen = false
}

const handleConfirm = async () => {
  if (confirmModal.value.action) {
    await confirmModal.value.action()
  }
  closeConfirm()
}

// Toast
const toast = ref({ show: false, message: '', type: 'success' })
const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

// Cargar lotes
const loadLotes = async () => {
  loadingLotes.value = true
  try {
    const data = await CobranzasService.listarLotes()
    lotes.value = data
  } catch (e) {
    showToast('Error al cargar lotes: ' + e.message, 'error')
  } finally {
    loadingLotes.value = false
  }
}

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

// Handle page change
const handlePageChange = (page) => {
  buscarSocios(page)
}

// Manejar pago
const handlePago = (socio) => {
  confirmModal.value = {
    isOpen: true,
    title: 'Confirmar Pago',
    message: `¿Está seguro que desea registrar el pago de $${socio.deuda || '0.00'} para ${socio.nombre} ${socio.apellido}?`,
    type: 'info',
    action: async () => {
      try {
        const paymentData = {
          socioId: socio.id,
          monto: socio.deuda,
          formaPago: 0, // 0 = Cobrador
        }

        await CuotasService.registrarCuota(paymentData)
        showToast('Pago registrado exitosamente')
        buscarSocios(currentPage.value)
      } catch (e) {
        showToast(`Error al registrar pago: ${e.message}`, 'error')
      }
    },
  }
}

// Abrir modal de edición
const openEditModal = (socio) => {
  selectedSocioId.value = socio.id
  isEditModalOpen.value = true
}

// Manejar actualización de socio
const handleSocioUpdated = (updatedSocio) => {
  isEditModalOpen.value = false
  showToast('Socio actualizado correctamente')
  // Refrescamos la lista completa para asegurar que la UI refleje cambios como el lote
  buscarSocios(currentPage.value)
}

// Manejar eliminación de socio
const handleDelete = (socio) => {
  confirmModal.value = {
    isOpen: true,
    title: 'Dar de Baja Socio',
    message: `¿Está seguro de dar de baja a ${socio.nombre} ${socio.apellido}? Esta acción no se puede deshacer.`,
    type: 'danger',
    action: async () => {
      try {
        await SociosService.removeSocio(socio.id)
        showToast('Socio dado de baja exitosamente')
        // Al dar de baja, refrescamos la página actual o la anterior si esta quedó vacía
        const pageToLoad =
          socios.value.length === 1 && currentPage.value > 1
            ? currentPage.value - 1
            : currentPage.value
        buscarSocios(pageToLoad)
      } catch (e) {
        showToast(`Error al dar de baja: ${e.message}`, 'error')
      }
    },
  }
}

// Manejar creación de lote
const handleLoteCreated = () => {
  isLoteModalOpen.value = false
  showToast('Lote creado exitosamente')
  loadLotes()
}

// Cargar lotes al montar
onMounted(() => {
  loadLotes()
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
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
                  >Gestión de Cobranzas</span
                >
              </div>
            </li>
          </ol>
        </nav>
        <div class="flex justify-between items-end">
          <div>
            <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Gestión de Cobranzas</h2>
            <p class="text-slate-500 mt-1">Administre las cobranzas y lotes desde un solo lugar.</p>
          </div>
        </div>
      </div>

      <!-- Tabs -->
      <div class="border-b border-slate-200 mb-6">
        <nav class="-mb-px flex space-x-8" aria-label="Tabs">
          <button
            @click="activeTab = 'buscar-socios'"
            :class="[
              activeTab === 'buscar-socios'
                ? 'border-teal-500 text-teal-600'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors',
            ]"
          >
            Buscar Socios por Lote
          </button>
          <button
            @click="activeTab = 'crear-lote'"
            :class="[
              activeTab === 'crear-lote'
                ? 'border-teal-500 text-teal-600'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors',
            ]"
          >
            Crear Nuevo Lote
          </button>
        </nav>
      </div>

      <!-- Tab Content: BUSCAR SOCIOS POR LOTE -->
      <div v-if="activeTab === 'buscar-socios'">
        <!-- Selector de Lote -->
        <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6 mb-8">
          <div class="max-w-md">
            <label for="lote-select" class="block text-sm font-bold text-slate-700 mb-2"
              >Seleccionar Lote</label
            >
            <select
              id="lote-select"
              v-model="selectedLote"
              @change="buscarSocios()"
              class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 focus:ring-opacity-50 sm:text-base px-4 py-3 border transition-all"
            >
              <option value="">Seleccione un lote</option>
              <option v-for="lote in lotes" :key="lote.id" :value="lote.id">
                {{ lote.nombreLote }}
              </option>
            </select>
            <p v-if="loadingLotes" class="mt-2 text-xs text-slate-500 italic">Cargando lotes...</p>
            <p v-else class="mt-2 text-xs text-slate-500">
              {{ lotes.length }} lote(s) disponible(s)
            </p>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="loadingSocios" class="flex justify-center py-12">
          <svg
            class="animate-spin h-10 w-10 text-teal-600"
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

        <!-- Error State -->
        <div
          v-else-if="errorSocios"
          class="bg-red-50 border border-red-200 rounded-xl p-4 mb-6 text-red-700 flex items-center gap-3"
        >
          <svg class="h-6 w-6 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          {{ errorSocios }}
        </div>

        <!-- Lista de Socios -->
        <div v-else-if="socios.length > 0">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <CobradorSocioCard
              v-for="socio in socios"
              :key="socio.id"
              :socio="socio"
              @pay="handlePago"
              @edit="openEditModal"
              @delete="handleDelete"
            />
          </div>

          <!-- Pagination -->
          <div class="mt-8">
            <Pagination
              :current-page="currentPage"
              :total-pages="totalPages"
              :total-count="totalCount"
              :page-size="pageSize"
              @change-page="handlePageChange"
            />
          </div>
        </div>

        <!-- Empty State -->
        <div
          v-else-if="selectedLote && !loadingSocios"
          class="text-center py-12 bg-white rounded-xl border border-dashed border-slate-300"
        >
          <div
            class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-slate-100 mb-4"
          >
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
                d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
              />
            </svg>
          </div>
          <h3 class="text-lg font-medium text-slate-900">No hay socios deudores en este lote</h3>
          <p class="text-slate-500 mt-1">Seleccione otro lote o verifique más tarde.</p>
        </div>

        <div v-else class="text-center py-12 text-slate-500">
          Seleccione un lote para comenzar la búsqueda.
        </div>
      </div>

      <!-- Tab Content: CREAR NUEVO LOTE -->
      <div v-if="activeTab === 'crear-lote'">
        <div class="max-w-2xl mx-auto">
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
                  d="M9 6.75V15m6-6v8.25m.503 3.498l4.875-2.437c.381-.19.622-.58.622-1.006V4.82c0-.836-.88-1.38-1.628-1.006l-3.869 1.934c-.317.159-.69.159-1.006 0L9.503 3.252a1.125 1.125 0 00-1.006 0L3.622 5.689C3.24 5.88 3 6.27 3 6.695V19.18c0 .836.88 1.38 1.628 1.006l3.869-1.934c.317-.159.69-.159 1.006 0l4.994 2.497c.317.158.69.158 1.006 0z"
                />
              </svg>
            </div>
            <h3 class="text-xl font-bold text-slate-900 mb-2">Crear Nuevo Lote</h3>
            <p class="text-slate-500 mb-8">
              Registre un nuevo lote para organizar mejor las cobranzas por zonas geográficas.
            </p>

            <button
              @click="isLoteModalOpen = true"
              class="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-lg shadow-sm text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 transition-colors"
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
              Crear Lote
            </button>
          </div>
        </div>
      </div>
    </main>

    <!-- Modals -->
    <SocioUpdateModal
      :is-open="isEditModalOpen"
      :socio-id="selectedSocioId"
      @close="isEditModalOpen = false"
      @save="handleSocioUpdated"
    />

    <LoteFormModal
      :is-open="isLoteModalOpen"
      @close="isLoteModalOpen = false"
      @save="handleLoteCreated"
    />

    <ConfirmModal
      :is-open="confirmModal.isOpen"
      :title="confirmModal.title"
      :message="confirmModal.message"
      :type="confirmModal.type"
      @close="closeConfirm"
      @confirm="handleConfirm"
    />

    <!-- Toast Notification -->
    <div
      v-if="toast.show"
      class="fixed bottom-4 right-4 z-50 max-w-sm w-full bg-white shadow-lg rounded-lg pointer-events-auto ring-1 ring-black ring-opacity-5 overflow-hidden"
    >
      <div class="p-4">
        <div class="flex items-start">
          <div class="flex-shrink-0">
            <svg
              v-if="toast.type === 'success'"
              class="h-6 w-6 text-green-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
            <svg
              v-else
              class="h-6 w-6 text-red-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
          </div>
          <div class="ml-3 w-0 flex-1 pt-0.5">
            <p class="text-sm font-medium text-slate-900">{{ toast.message }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
