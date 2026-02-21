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
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb & Page Title -->
      <div class="mb-4">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a
                href="#"
                @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-cyan-600"
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
        <h2 class="text-2xl font-bold text-slate-900 tracking-tight">Gestión de Cobranzas</h2>
        <p class="text-slate-500 mt-1 text-base">
          Administre las cobranzas y lotes desde un solo lugar.
        </p>
      </div>

      <!-- Navigation Pill-Navbar -->
      <div class="flex flex-col sm:flex-row items-center gap-4 mb-8">
        <div
          class="flex flex-wrap items-center gap-2 p-1.5 bg-slate-200/50 rounded-[2rem] w-full sm:w-auto border border-slate-200/50 backdrop-blur-sm shadow-inner"
        >
          <button
            v-for="tab in [
              {
                id: 'buscar-socios',
                label: 'Buscar Socios por Lote',
                icon: 'M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z',
              },
              {
                id: 'crear-lote',
                label: 'Crear Nuevo Lote',
                icon: 'M12 4v16m8-8H4',
              },
              {
                id: 'historial-cobradores',
                label: 'Historial de Cobradores',
                icon: 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z',
              },
            ]"
            :key="tab.id"
            @click="handleTabChange(tab.id)"
            class="flex-1 sm:flex-none flex items-center justify-center px-4 py-2 rounded-[1.5rem] transition-all duration-300 font-bold text-sm"
            :class="[
              activeTab === tab.id
                ? 'bg-white text-cyan-600 shadow-md border border-slate-200 translate-y-[-1px] scale-[1.02]'
                : 'text-slate-500 hover:text-slate-700 hover:bg-white/40',
            ]"
          >
            <svg class="w-4 h-4 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="tab.icon" />
            </svg>
            {{ tab.label }}
          </button>
        </div>

        <button
          @click="goHome"
          class="hidden sm:flex items-center px-4 py-2 text-slate-400 hover:text-red-500 font-bold text-sm transition-colors group"
        >
          <svg
            class="w-4 h-4 mr-2 group-hover:-translate-x-1 transition-transform"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M10 19l-7-7m0 0l7-7m-7 7h18"
            />
          </svg>
          Volver
        </button>
      </div>

      <div
        class="bg-white rounded-2xl border border-slate-200 shadow-sm p-4 min-h-[400px] overflow-hidden"
      >
        <BuscarSociosTab
          v-if="activeTab === 'buscar-socios'"
          ref="buscarSociosTabRef"
          :lotes="lotes"
          :loading-lotes="loadingLotes"
          @pay="handlePago"
          @edit="openEditModal"
          @delete="handleDelete"
        />

        <CrearLoteTab v-if="activeTab === 'crear-lote'" @create="isLoteModalOpen = true" />

        <HistorialCobradoresTab
          v-if="activeTab === 'historial-cobradores'"
          :show-toast="showToast"
        />
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
                'text-red-500': toast.type === 'error',
              }"
            >
              {{ toast.type === 'success' ? 'Éxito' : 'Error' }}
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

<style scoped>
.font-sans {
  font-family:
    'Outfit',
    'Inter',
    system-ui,
    -apple-system,
    sans-serif;
}

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
