<script setup>
import { ref, computed, watch } from 'vue'
import SalonService from '../../services/SalonService'

const props = defineProps({
  isOpen: Boolean,
  reservaId: {
    type: [Number, String],
    default: null,
  },
})

const emit = defineEmits(['close', 'payment-registered'])

const reserva = ref(null)
const isLoading = ref(false)
const error = ref('')

// Payment State
const showPaymentInput = ref(false)
const montoAPagar = ref(0)
const isRecordingPayment = ref(false)
const toast = ref({ show: false, message: '', type: 'success' })

const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

const fetchReservaDetails = async () => {
  if (!props.reservaId) return

  isLoading.value = true
  error.value = ''
  try {
    reserva.value = await SalonService.getReservaById(props.reservaId)
  } catch (e) {
    error.value = 'No se pudo cargar la información de la reserva.'
    console.error(e)
  } finally {
    isLoading.value = false
  }
}

watch(
  () => props.isOpen,
  (newVal) => {
    if (newVal && props.reservaId) {
      fetchReservaDetails()
    } else if (!newVal) {
      reserva.value = null
    }
  },
)

const isPagado = computed(() => {
  if (!reserva.value) return false
  return reserva.value.totalPagado >= reserva.value.importe
})

const saldoPendiente = computed(() => {
  if (!reserva.value) return 0
  return Math.max(0, reserva.value.importe - reserva.value.totalPagado)
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-AR', { style: 'currency', currency: 'ARS' }).format(value)
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString + 'T00:00:00')
  return date.toLocaleDateString('es-AR', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
}

const handleRegistrarPago = async () => {
  if (montoAPagar.value <= 0) {
    showToast('El monto debe ser mayor a 0', 'error')
    return
  }

  if (montoAPagar.value > saldoPendiente.value) {
    showToast('El monto no puede superar el saldo pendiente', 'error')
    return
  }

  isRecordingPayment.value = true
  try {
    await SalonService.actualizarPago(props.reservaId, montoAPagar.value)
    showToast('Pago registrado correctamente')
    // Refresh details
    await fetchReservaDetails()
    // Notify parent to refresh list
    emit('payment-registered')
    // Reset input
    showPaymentInput.value = false
    montoAPagar.value = 0
  } catch (e) {
    console.error(e)
    showToast(e.message, 'error')
  } finally {
    isRecordingPayment.value = false
  }
}
</script>

<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-50 overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <div
      class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-2xl border border-slate-200"
      >
        <!-- Loading State -->
        <div v-if="isLoading" class="flex flex-col items-center justify-center py-20">
          <div class="relative w-16 h-16">
            <div class="absolute inset-0 border-4 border-blue-100 rounded-full"></div>
            <div
              class="absolute inset-0 border-4 border-blue-600 rounded-full border-t-transparent animate-spin"
            ></div>
          </div>
          <p class="mt-4 text-slate-500 font-medium">Cargando detalles...</p>
        </div>

        <!-- Error State -->
        <div v-else-if="error" class="p-8 text-center">
          <div
            class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-red-100 mb-4"
          >
            <svg class="h-6 w-6 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
              />
            </svg>
          </div>
          <h3 class="text-lg font-bold text-slate-900">{{ error }}</h3>
          <button
            @click="$emit('close')"
            class="mt-6 px-4 py-2 bg-slate-100 text-slate-700 rounded-lg hover:bg-slate-200 transition-colors"
          >
            Cerrar
          </button>
        </div>

        <!-- Content -->
        <div v-else-if="reserva">
          <!-- Header -->
          <div class="relative bg-slate-50 px-6 py-6 border-b border-slate-200">
            <div class="flex justify-between items-start">
              <div>
                <span
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold uppercase tracking-wider mb-2"
                  :class="
                    isPagado ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'
                  "
                >
                  {{ isPagado ? 'Pagado' : 'Pago Pendiente' }}
                </span>
                <h3 class="text-2xl font-bold text-slate-900 leading-tight">
                  {{ reserva.titulo || 'Sin Título' }}
                </h3>
                <p class="text-slate-500 mt-1 flex items-center gap-1.5">
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
                      d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                    />
                  </svg>
                  {{ formatDate(reserva.fechaAlquiler) }}
                </p>
              </div>
              <button
                @click="$emit('close')"
                class="p-2 text-slate-400 hover:text-slate-600 hover:bg-slate-200 rounded-full transition-all"
              >
                <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
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

          <div class="px-6 py-8 space-y-8">
            <!-- Info Grid -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
              <!-- Salon Section -->
              <div class="space-y-4">
                <h4
                  class="text-xs font-bold text-slate-400 uppercase tracking-widest flex items-center gap-2"
                >
                  <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"
                    />
                  </svg>
                  Ubicación del Evento
                </h4>
                <div class="bg-indigo-50/50 rounded-xl p-4 border border-indigo-100">
                  <p class="text-lg font-bold text-indigo-900">{{ reserva.nombreSalon }}</p>
                  <p class="text-sm text-indigo-700 mt-1">{{ reserva.direccionSalon }}</p>
                </div>
              </div>

              <!-- Borrower Section -->
              <div class="space-y-4">
                <h4
                  class="text-xs font-bold text-slate-400 uppercase tracking-widest flex items-center gap-2"
                >
                  <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                    />
                  </svg>
                  Datos del Socio
                </h4>
                <div class="bg-slate-50 rounded-xl p-4 border border-slate-200">
                  <p class="text-base font-bold text-slate-900">
                    {{ reserva.nombreSocio }} {{ reserva.apellidoSocio }}
                  </p>
                  <div class="mt-2 space-y-1">
                    <p
                      v-if="reserva.telefonoSocio"
                      class="text-sm text-slate-600 flex items-center gap-2"
                    >
                      <svg
                        class="h-3.5 w-3.5 text-slate-400"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                      >
                        <path
                          d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"
                        />
                      </svg>
                      {{ reserva.telefonoSocio }}
                    </p>
                    <p
                      v-if="reserva.direccionSocio"
                      class="text-sm text-slate-600 flex items-center gap-2"
                    >
                      <svg
                        class="h-3.5 w-3.5 text-slate-400"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                      >
                        <path
                          d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
                        />
                        <path d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                      {{ reserva.direccionSocio }}, {{ reserva.localidad }}
                    </p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Financial Summary -->
            <div class="space-y-4">
              <h4
                class="text-xs font-bold text-slate-400 uppercase tracking-widest flex items-center gap-2"
              >
                <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z"
                  />
                </svg>
                Resumen de Pago
              </h4>
              <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div class="p-4 rounded-xl border-2 border-slate-100 bg-white">
                  <p class="text-xs text-slate-500 font-medium">Importe Total</p>
                  <p class="text-xl font-bold text-slate-900 mt-1">
                    {{ formatCurrency(reserva.importe) }}
                  </p>
                </div>
                <div class="p-4 rounded-xl border-2 border-emerald-100 bg-emerald-50/30">
                  <p class="text-xs text-emerald-600 font-medium">Abonado</p>
                  <p class="text-xl font-bold text-emerald-700 mt-1">
                    {{ formatCurrency(reserva.totalPagado) }}
                  </p>
                </div>
                <div class="p-4 rounded-xl border-2 border-amber-100 bg-amber-50/30">
                  <p class="text-xs text-amber-600 font-medium">Saldo Pendiente</p>
                  <p class="text-xl font-bold text-amber-700 mt-1">
                    {{ formatCurrency(saldoPendiente) }}
                  </p>
                </div>
              </div>
            </div>
            <!-- Payment History -->
            <div
              v-if="reserva.historialPagos && reserva.historialPagos.length > 0"
              class="space-y-4"
            >
              <h4
                class="text-xs font-bold text-slate-400 uppercase tracking-widest flex items-center gap-2"
              >
                <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
                Historial de Pagos
              </h4>
              <div class="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
                <table class="min-w-full divide-y divide-slate-200">
                  <thead class="bg-slate-50/80">
                    <tr>
                      <th
                        scope="col"
                        class="px-6 py-3 text-left text-xs font-bold text-slate-500 uppercase tracking-wider"
                      >
                        Fecha
                      </th>
                      <th
                        scope="col"
                        class="px-6 py-3 text-right text-xs font-bold text-slate-500 uppercase tracking-wider"
                      >
                        Monto
                      </th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-200">
                    <tr
                      v-for="(pago, index) in reserva.historialPagos"
                      :key="index"
                      class="hover:bg-slate-50/50 transition-colors"
                    >
                      <td class="px-6 py-3 whitespace-nowrap text-sm text-slate-700 font-medium">
                        {{ formatDate(pago.fecha) }}
                      </td>
                      <td
                        class="px-6 py-3 whitespace-nowrap text-sm text-right text-emerald-600 font-bold bg-emerald-50/10"
                      >
                        {{ formatCurrency(pago.monto) }}
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- Payment Input Section (Conditional) -->
          <div
            v-if="showPaymentInput"
            class="px-6 py-6 bg-blue-50/50 border-t border-blue-100 ring-1 ring-inset ring-blue-600/10"
          >
            <div class="flex flex-col sm:flex-row items-end gap-5">
              <div class="flex-1 w-full group">
                <label class="block text-sm font-bold text-blue-900 mb-2 px-1"
                  >Monto a registrar</label
                >
                <div
                  class="relative group-focus-within:scale-[1.01] transition-transform duration-200"
                >
                  <span class="absolute left-4 top-1/2 -translate-y-1/2 text-blue-500 font-bold"
                    >$</span
                  >
                  <input
                    type="number"
                    v-model="montoAPagar"
                    placeholder="0.00"
                    step="0.01"
                    min="0"
                    :max="saldoPendiente"
                    class="block w-full pl-10 pr-4 py-3 bg-white border-2 border-blue-100 rounded-2xl focus:border-blue-500 focus:ring-4 focus:ring-blue-500/10 transition-all outline-none text-blue-900 font-bold text-lg shadow-sm"
                  />
                </div>
              </div>
              <div class="flex gap-3 w-full sm:w-auto">
                <button
                  type="button"
                  @click="showPaymentInput = false"
                  class="flex-1 sm:flex-none px-6 py-3 bg-white text-slate-600 font-bold rounded-2xl border-2 border-slate-200 hover:bg-slate-50 hover:border-slate-300 transition-all active:scale-95 shadow-sm"
                >
                  Cancelar
                </button>
                <button
                  type="button"
                  @click="handleRegistrarPago"
                  :disabled="isRecordingPayment || montoAPagar <= 0"
                  class="flex-1 sm:flex-none px-8 py-3 bg-blue-600 text-white font-bold rounded-2xl hover:bg-blue-700 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-lg shadow-blue-500/30 flex items-center justify-center gap-2 min-w-[160px]"
                >
                  <svg
                    v-if="isRecordingPayment"
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
                  <span v-else>Confirmar Pago</span>
                </button>
              </div>
            </div>
          </div>

          <!-- Footer -->
          <div class="bg-slate-50 px-6 py-5 flex justify-end gap-3 border-t border-slate-200">
            <button
              type="button"
              @click="$emit('close')"
              class="px-5 py-2.5 bg-white border border-slate-300 text-slate-700 font-semibold rounded-xl hover:bg-slate-100 transition-all shadow-sm"
            >
              Cerrar
            </button>
            <button
              v-if="!isPagado && !showPaymentInput"
              type="button"
              @click="showPaymentInput = true"
              class="px-5 py-2.5 bg-blue-600 text-white font-semibold rounded-xl hover:bg-blue-700 transition-all shadow-md shadow-blue-200"
            >
              Registrar Otro Pago
            </button>
          </div>

          <!-- Inner Toast Notification -->
          <Transition
            enter-active-class="transform ease-out duration-300 transition"
            enter-from-class="translate-y-2 opacity-0"
            enter-to-class="translate-y-0 opacity-100"
            leave-active-class="transition ease-in duration-200"
            leave-from-class="opacity-100"
            leave-to-class="opacity-0"
          >
            <div
              v-if="toast.show"
              class="fixed bottom-6 left-1/2 -translate-x-1/2 z-[60] flex w-[90%] max-w-sm overflow-hidden bg-white rounded-2xl shadow-2xl border border-slate-200 ring-1 ring-black/5"
            >
              <div
                class="flex items-center justify-center w-12 shrink-0"
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
                    stroke-width="2.5"
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
                    stroke-width="2.5"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </div>
              <div class="px-4 py-3 min-w-0">
                <p
                  class="text-sm font-bold mb-0.5"
                  :class="{
                    'text-emerald-700': toast.type === 'success',
                    'text-red-700': toast.type === 'error',
                  }"
                >
                  {{ toast.type === 'success' ? 'Completado' : 'Error' }}
                </p>
                <p class="text-xs text-slate-600 font-medium truncate">{{ toast.message }}</p>
              </div>
            </div>
          </Transition>
        </div>
      </div>
    </div>
  </div>
</template>
