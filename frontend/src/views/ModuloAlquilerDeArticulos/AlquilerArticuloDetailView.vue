<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ConfirmModal from '../../components/Common/ConfirmModal.vue'
import LoadingOverlay from '../../components/Common/LoadingOverlay.vue'
import AddItemModal from '../../components/ModuloAlquilerArticulos/Alquileres/AddItemModal.vue'
import ArticulosAlquiladosCard from '../../components/ModuloAlquilerArticulos/Articulos/ArticulosAlquiladosCard.vue'
import AlquilerService from '../../services/AlquilerService'

const route = useRoute()
const router = useRouter()
const alquilerId = route.params.id

const alquiler = ref(null)
const loading = ref(true)
const error = ref('')
const paymentError = ref('')

// Modals
const isAddItemModalOpen = ref(false)
const isConfirmFinalizeOpen = ref(false)

// Toast
const toast = ref({ show: false, message: '', type: 'success' })
const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

onMounted(() => {
  loadAlquiler()
})

const loadAlquiler = async (silent = false) => {
  if (!silent) loading.value = true
  error.value = ''
  try {
    const result = await AlquilerService.getById(alquilerId)
    alquiler.value = result
  } catch (e) {
    if (!silent) error.value = e.message
    else showToast(e.message, 'error')
  } finally {
    if (!silent) loading.value = false
  }
}

const handleItemUpdated = (message, type = 'success') => {
  showToast(message, type)
  if (type === 'success') {
    loadAlquiler(true) // Silent refresh
  }
}

const handleAddItem = async () => {
  isAddItemModalOpen.value = false
  showToast('Item agregado correctamente')
  loadAlquiler(true) // Silent refresh
}

const handleRegisterPayment = async () => {
  paymentError.value = ''
  try {
    await AlquilerService.registerPayment(alquilerId)
    showToast('Pago registrado correctamente')
    loadAlquiler()
  } catch (e) {
    paymentError.value = e.message
  }
}

const handleFinalize = async () => {
  try {
    await AlquilerService.finalize(alquilerId)
    isConfirmFinalizeOpen.value = false
    router.push({ name: 'alquiler-articulos', query: { success: 'finalizado' } })
  } catch (e) {
    showToast(e.message, 'error')
    isConfirmFinalizeOpen.value = false
  }
}

const goBack = () => router.push('/alquiler-articulos')
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800 pb-12">
    <!-- Header Simple -->
    <div class="bg-white border-b border-slate-200 shadow-sm sticky top-0 z-30">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 py-4 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <button
            @click="goBack"
            class="p-2 -ml-2 text-slate-400 hover:text-slate-600 rounded-full hover:bg-slate-100 transition-colors"
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
                stroke-width="2"
                d="M10 19l-7-7m0 0l7-7m-7 7h18"
              />
            </svg>
          </button>
          <h1 class="text-xl font-bold text-slate-900">Detalle de Alquiler</h1>
        </div>
        <div v-if="alquiler">
          <span
            class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
            :class="
              alquiler.estaAlDia ? 'bg-emerald-100 text-emerald-800' : 'bg-rose-100 text-rose-800'
            "
          >
            {{ alquiler.estaAlDia ? 'Al día' : 'Con Deuda' }}
          </span>
        </div>
      </div>
    </div>

    <LoadingOverlay :show="loading" message="Cargando detalles del alquiler..." />

    <div
      v-if="!loading && error"
      class="max-w-3xl mx-auto mt-12 p-6 bg-red-50 text-red-700 rounded-lg text-center"
    >
      {{ error }}
    </div>

    <div v-else-if="!loading && alquiler" class="max-w-5xl mx-auto px-4 sm:px-6 mt-8 space-y-6">
      <!-- Info Cards Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Socio Info -->
        <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
          <h2
            class="text-sm font-medium text-slate-500 uppercase tracking-wider mb-4 border-b border-slate-100 pb-2"
          >
            Información del Socio
          </h2>
          <div class="space-y-4">
            <div class="flex justify-between items-center text-sm">
              <span class="text-slate-400 font-medium uppercase text-[10px] tracking-wider"
                >Nombre del Socio</span
              >
              <span class="font-bold text-slate-900"
                >{{ alquiler.nombreSocio }} {{ alquiler.apellidoSocio }}</span
              >
            </div>
            <div class="flex justify-between items-center text-sm">
              <span class="text-slate-400 font-medium uppercase text-[10px] tracking-wider"
                >Documento (DNI)</span
              >
              <span class="font-semibold text-slate-700">{{ alquiler.dniSocio }}</span>
            </div>
            <div class="flex justify-between items-center text-sm" v-if="alquiler.telefonoSocio">
              <span class="text-slate-400 font-medium uppercase text-[10px] tracking-wider"
                >Teléfono</span
              >
              <span class="font-semibold text-slate-700">{{ alquiler.telefonoSocio }}</span>
            </div>
            <div class="flex justify-between items-center text-sm" v-if="alquiler.direccionSocio">
              <span class="text-slate-400 font-medium uppercase text-[10px] tracking-wider"
                >Dirección</span
              >
              <span class="font-semibold text-slate-700">{{ alquiler.direccionSocio }}</span>
            </div>
            <div class="flex justify-between items-center text-sm" v-if="alquiler.localidadSocio">
              <span class="text-slate-400 font-medium uppercase text-[10px] tracking-wider"
                >Localidad</span
              >
              <span class="font-semibold text-slate-700">{{ alquiler.localidadSocio }}</span>
            </div>
          </div>
        </div>

        <!-- Rental Info & Actions -->
        <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6 flex flex-col">
          <h2
            class="text-sm font-medium text-slate-500 uppercase tracking-wider mb-4 border-b border-slate-100 pb-2"
          >
            Estado y Acciones
          </h2>
          <div class="flex-1 space-y-3">
            <div class="flex justify-between">
              <span class="text-slate-500">Fecha Inicio:</span>
              <span class="font-medium text-slate-900">{{ alquiler.fechaAlquiler }}</span>
            </div>
            <div
              v-if="alquiler.observaciones"
              class="text-sm text-slate-600 italic bg-slate-50 p-2 rounded"
            >
              "{{ alquiler.observaciones }}"
            </div>
          </div>

          <div class="mt-6 flex flex-col gap-3">
            <button
              @click="handleRegisterPayment"
              class="w-full inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-teal-700 bg-teal-100 hover:bg-teal-200 transition-colors"
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
                  d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              Registrar Pago
            </button>
            <button
              @click="isConfirmFinalizeOpen = true"
              class="w-full inline-flex justify-center items-center px-4 py-2 border border-slate-300 text-sm font-medium rounded-md text-slate-700 bg-white hover:bg-slate-50 transition-colors"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-2 text-slate-400"
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
              Finalizar Alquiler
            </button>
            <div
              v-if="paymentError"
              class="mt-2 p-2 bg-red-50 border border-red-100 rounded text-xs text-red-600 flex items-center gap-2"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-4 w-4 shrink-0"
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
              <span>{{ paymentError }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Articulos (Items) -->
      <ArticulosAlquiladosCard
        v-if="alquiler"
        :alquiler-id="alquilerId"
        :items="alquiler.items"
        @updated="handleItemUpdated"
        @add-item="isAddItemModalOpen = true"
      />

      <!-- Historial Pagos -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden mb-12">
        <div class="px-6 py-4 border-b border-slate-200 bg-slate-50">
          <h2 class="text-base font-bold text-slate-900">Historial de Pagos</h2>
        </div>
        <div
          class="p-6 grid grid-cols-2 sm:grid-cols-4 md:grid-cols-6 gap-4"
          v-if="alquiler.historialDePagos && alquiler.historialDePagos.length > 0"
        >
          <div
            v-for="pago in alquiler.historialDePagos"
            :key="pago.id"
            class="bg-emerald-50 border border-emerald-100 rounded-lg p-3 text-center"
          >
            <div class="text-xs text-emerald-600 uppercase font-semibold">
              {{ pago.mes }}/{{ pago.anio }}
            </div>
            <div class="text-lg font-bold text-emerald-700">${{ pago.monto }}</div>
          </div>
        </div>
        <div v-else class="p-8 text-center text-slate-400">
          No se han registrado pagos para este alquiler.
        </div>
      </div>
    </div>

    <!-- Modals -->
    <AddItemModal
      :is-open="isAddItemModalOpen"
      :alquiler-id="parseInt(alquilerId)"
      @close="isAddItemModalOpen = false"
      @save="handleAddItem"
    />

    <ConfirmModal
      :is-open="isConfirmFinalizeOpen"
      title="Finalizar Alquiler"
      message="¿Está seguro que desea finalizar este alquiler? Se registrará la devolución de todos los artículos."
      confirm-text="Finalizar Alquiler"
      type="info"
      @close="isConfirmFinalizeOpen = false"
      @confirm="handleFinalize"
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
