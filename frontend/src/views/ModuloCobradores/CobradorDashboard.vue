<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import ConfirmModal from '../../components/Common/ConfirmModal.vue'
import BuscarSociosTab from '../../components/ModuloCobradores/BuscarSociosTab/BuscarSociosTab.vue'
import CrearLoteTab from '../../components/ModuloCobradores/CrearLoteTab/CrearLoteTab.vue'
import LoteFormModal from '../../components/ModuloCobradores/CrearLoteTab/LoteFormModal.vue'
import HistorialCobradoresTab from '../../components/ModuloCobradores/HistorialCobradoresTab/HistorialCobradoresTab.vue'
import SocioUpdateModal from '../../components/ModuloGestionSocios/SocioUpdateModal.vue'
import CobranzasService from '../../services/CobranzasService'
import CuotasService from '../../services/CuotasService'
import SociosService from '../../services/SociosService'

// Router
const router = useRouter()
const goHome = () => router.push('/')

// Tabs
const activeTab = ref('buscar-socios')
const buscarSociosTabRef = ref(null)

const handleTabChange = (tab) => {
  activeTab.value = tab
}

// Estado Global - Lotes (Compartido)
const lotes = ref([])
const loadingLotes = ref(false)

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

// Modales de Confirmación
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

// Modal Editar Socio
const isEditModalOpen = ref(false)
const selectedSocioId = ref(null)

const openEditModal = (socio) => {
  selectedSocioId.value = socio.id
  isEditModalOpen.value = true
}

const handleSocioUpdated = () => {
  isEditModalOpen.value = false
  showToast('Socio actualizado correctamente')
  buscarSociosTabRef.value?.refresh()
}

// Modal Crear Lote
const isLoteModalOpen = ref(false)
const handleLoteCreated = () => {
  isLoteModalOpen.value = false
  showToast('Lote creado exitosamente')
  loadLotes()
}

// Manejar pago
const handlePago = ({ socio, periodos }) => {
  confirmModal.value = {
    isOpen: true,
    title: 'Confirmar Pago',
    message: `¿Está seguro que desea registrar el pago de ${periodos.length} periodo(s) para ${socio.nombre} ${socio.apellido}?`,
    type: 'info',
    action: async () => {
      try {
        const paymentData = {
          socioId: socio.id,
          formaPago: 0, // 0 = Cobrador
          periodos: periodos,
        }

        await CuotasService.registrarPagoCobrador(paymentData)
        showToast('Pagos registrados exitosamente')
        buscarSociosTabRef.value?.refresh()
      } catch (e) {
        showToast(`Error al registrar pago: ${e.message}`, 'error')
      }
    },
  }
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
        buscarSociosTabRef.value?.refresh()
      } catch (e) {
        showToast(`Error al dar de baja: ${e.message}`, 'error')
      }
    },
  }
}

// Cargar lotes al montar
onMounted(() => {
  loadLotes()
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
      <!-- Breadcrumb -->
      <div class="mb-6">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a href="#" @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-teal-600">
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
                <svg class="w-3 h-3 text-slate-400 mx-1" xmlns="http://www.w3.org/2000/svg" fill="none"
                  viewBox="0 0 6 10">
                  <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="m1 9 4-4-4-4" />
                </svg>
                <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2">Gestión de Cobranzas</span>
              </div>
            </li>
          </ol>
        </nav>
        <div class="flex flex-col sm:flex-row justify-between items-start sm:items-end gap-4">
          <div>
            <h2 class="text-2xl sm:text-3xl font-bold text-slate-900 tracking-tight">
              Gestión de Cobranzas
            </h2>
            <p class="text-slate-500 mt-1 text-sm sm:text-base">
              Administre las cobranzas y lotes desde un solo lugar.
            </p>
          </div>
        </div>
      </div>

      <!-- Tabs -->
      <div class="border-b border-slate-200 mb-6 overflow-x-auto scrollbar-hide">
        <nav class="-mb-px flex space-x-6 sm:space-x-8 min-w-max" aria-label="Tabs">
          <button @click="handleTabChange('buscar-socios')" :class="[
            activeTab === 'buscar-socios'
              ? 'border-teal-500 text-teal-600'
              : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
            'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors flex-shrink-0',
          ]">
            Buscar Socios por Lote
          </button>
          <button @click="handleTabChange('crear-lote')" :class="[
            activeTab === 'crear-lote'
              ? 'border-teal-500 text-teal-600'
              : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
            'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors flex-shrink-0',
          ]">
            Crear Nuevo Lote
          </button>
          <button @click="handleTabChange('historial-cobradores')" :class="[
            activeTab === 'historial-cobradores'
              ? 'border-teal-500 text-teal-600'
              : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
            'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-colors flex-shrink-0',
          ]">
            Historial de Cobradores
          </button>
        </nav>
      </div>

      <!-- Tab Content -->
      <div class="min-h-[400px]">
        <BuscarSociosTab v-if="activeTab === 'buscar-socios'" ref="buscarSociosTabRef" :lotes="lotes"
          :loading-lotes="loadingLotes" @pay="handlePago" @edit="openEditModal" @delete="handleDelete" />

        <CrearLoteTab v-if="activeTab === 'crear-lote'" @create="isLoteModalOpen = true" />

        <HistorialCobradoresTab v-if="activeTab === 'historial-cobradores'" :show-toast="showToast" />
      </div>
    </main>

    <!-- Modals -->
    <SocioUpdateModal :is-open="isEditModalOpen" :socio-id="selectedSocioId" @close="isEditModalOpen = false"
      @save="handleSocioUpdated" />

    <LoteFormModal :is-open="isLoteModalOpen" @close="isLoteModalOpen = false" @save="handleLoteCreated" />

    <ConfirmModal :is-open="confirmModal.isOpen" :title="confirmModal.title" :message="confirmModal.message"
      :type="confirmModal.type" @close="closeConfirm" @confirm="handleConfirm" />

    <!-- Toast Notification -->
    <Transition enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
      enter-to-class="translate-y-0 opacity-100 sm:translate-x-0" leave-active-class="transition ease-in duration-100"
      leave-from-class="opacity-100" leave-to-class="opacity-0">
      <div v-if="toast.show"
        class="fixed bottom-4 right-4 z-50 max-w-sm w-full bg-white shadow-lg rounded-lg pointer-events-auto ring-1 ring-black ring-opacity-5 overflow-hidden">
        <div class="p-4">
          <div class="flex items-start">
            <div class="flex-shrink-0">
              <svg v-if="toast.type === 'success'" class="h-6 w-6 text-green-400" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <svg v-else class="h-6 w-6 text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div class="ml-3 w-0 flex-1 pt-0.5">
              <p class="text-sm font-medium text-slate-900">{{ toast.message }}</p>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* Ocultar scrollbar pero mantener funcionalidad */
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

/* Estilo para los botones de las tabs en mobile */
@media (max-width: 640px) {
  nav button {
    padding-left: 0.5rem;
    padding-right: 0.5rem;
    font-size: 0.8rem;
  }
}
</style>
