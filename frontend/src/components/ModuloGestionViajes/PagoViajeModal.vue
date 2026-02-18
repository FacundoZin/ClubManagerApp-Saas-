<script setup>
import { ref, onMounted } from 'vue'
import ViajesService from '../../services/viajesService'

const props = defineProps({
  isOpen: Boolean,
  inscripto: {
    type: Object,
    required: true,
  },
})

const emit = defineEmits(['close', 'save'])

const montoAbonar = ref(null)
const isSubmitting = ref(false)
const errorMessage = ref('')

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const handleSubmit = async () => {
  if (montoAbonar.value <= 0 || montoAbonar.value > props.inscripto.montoPendiente) {
    errorMessage.value = `El monto debe ser mayor a 0 y no superar ${formatCurrency(props.inscripto.montoPendiente)}`
    return
  }

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    await ViajesService.actualizarPago({
      idInscripto: props.inscripto.id,
      montoAbonado: montoAbonar.value,
    })
    emit('save')
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-[70] overflow-y-auto"
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
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-md border border-slate-200"
      >
        <div class="bg-white px-6 py-5 border-b border-slate-100 flex items-center justify-between">
          <h3 class="text-xl font-bold text-slate-900">Registrar Pago</h3>
          <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 p-2">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="px-6 py-6 space-y-5">
            <div
              v-if="errorMessage"
              class="p-4 bg-red-50 border border-red-100 rounded-xl text-red-700 text-sm"
            >
              {{ errorMessage }}
            </div>

            <!-- Read-only info -->
            <div class="p-4 bg-slate-50 rounded-xl border border-slate-200 space-y-3">
              <div>
                <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Socio</p>
                <p class="text-sm font-bold text-slate-700">{{ inscripto.nombreSocio }}</p>
              </div>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">
                    Ya Abonado
                  </p>
                  <p class="text-sm font-bold text-emerald-600">
                    {{ formatCurrency(inscripto.montoAbonado) }}
                  </p>
                </div>
                <div>
                  <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">
                    Saldo Pendiente
                  </p>
                  <p class="text-sm font-bold text-red-600">
                    {{ formatCurrency(inscripto.montoPendiente) }}
                  </p>
                </div>
              </div>
            </div>

            <div>
              <label for="montoPago" class="block text-sm font-medium text-slate-700 mb-2"
                >Monto a Abonar en $</label
              >
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-slate-500 sm:text-sm">$</span>
                </div>
                <input
                  type="text"
                  id="montoPago"
                  v-model="montoAbonar"
                  required
                  placeholder="Ej: 5000"
                  class="block w-full pl-7 pr-12 py-3 border border-slate-300 rounded-xl bg-white shadow-sm focus:ring-2 focus:ring-teal-500 focus:border-teal-500 sm:text-sm font-bold text-slate-900"
                  @input="montoAbonar = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
            </div>
          </div>

          <div
            class="bg-slate-50 px-6 py-4 flex flex-col sm:flex-row-reverse gap-3 border-t border-slate-200"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex justify-center px-6 py-3 bg-teal-600 text-white font-bold rounded-xl shadow-lg hover:bg-teal-700 transition-all disabled:opacity-50"
            >
              {{ isSubmitting ? 'Guardando...' : 'Registrar Pago' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="inline-flex justify-center px-6 py-3 bg-white text-slate-900 font-bold rounded-xl border border-slate-200 hover:bg-slate-50 transition-all"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
.no-spinner::-webkit-inner-spin-button,
.no-spinner::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.no-spinner {
  -moz-appearance: textfield;
  appearance: none;
}
</style>
