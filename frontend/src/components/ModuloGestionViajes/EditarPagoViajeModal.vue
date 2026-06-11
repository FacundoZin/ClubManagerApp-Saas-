<script setup>
import { ref, watch } from 'vue'
import ViajesService from '../../services/viajesService'

const props = defineProps({
  isOpen: Boolean,
  inscripto: {
    type: Object,
    required: true,
  },
  variante: {
    type: Object,
    required: true,
  },
})

const emit = defineEmits(['close', 'save'])

const nuevoMonto = ref(props.inscripto ? props.inscripto.montoAbonado : 0)
const motivo = ref('')
const errorMessage = ref('')
const isSubmitting = ref(false)

watch(
  () => props.inscripto,
  (newValue) => {
    if (newValue) {
      nuevoMonto.value = newValue.montoAbonado
      motivo.value = ''
      errorMessage.value = ''
    }
  },
)

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const handleSubmit = async () => {
  const monto = Number(nuevoMonto.value)

  if (isNaN(monto)) {
    errorMessage.value = 'Por favor ingrese un importe válido.'
    return
  }

  if (monto < 0) {
    errorMessage.value = 'El importe no puede ser negativo.'
    return
  }

  if (monto > props.variante.valorViaje) {
    errorMessage.value = `El importe no puede ser mayor al valor total del viaje (${formatCurrency(props.variante.valorViaje)}).`
    return
  }

  if (monto === props.inscripto.montoAbonado) {
    errorMessage.value = 'El nuevo importe debe ser distinto al importe actual.'
    return
  }

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    await ViajesService.editarPago({
      idInscripto: props.inscripto.id,
      nuevoMontoAbonado: monto,
      motivoModificacion: motivo.value.trim(),
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
  <div v-if="isOpen" class="fixed inset-0 z-[70] overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
    <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity" @click="$emit('close')"></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200">
        <div class="bg-white px-6 py-5 border-b border-slate-100 flex items-center justify-between">
          <div>
            <h3 class="text-xl font-bold text-slate-900">Editar Seña / Pago</h3>
            <p class="text-sm text-slate-500">Ajuste el importe registrado para este pasajero.</p>
          </div>
          <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 p-2">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="px-6 py-6 space-y-5">
            <div v-if="errorMessage" class="p-4 bg-red-50 border border-red-100 rounded-xl text-red-700 text-sm">
              {{ errorMessage }}
            </div>

            <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <div class="p-4 bg-slate-50 rounded-xl border border-slate-200">
                <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Pasajero</p>
                <p class="text-sm font-bold text-slate-900">{{ props.inscripto.apellido }} {{ props.inscripto.nombre }}</p>
              </div>
              <div class="p-4 bg-slate-50 rounded-xl border border-slate-200">
                <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Variante</p>
                <p class="text-sm font-bold text-slate-900">{{ props.variante.nombreVariante }}</p>
              </div>
            </div>

            <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
              <div class="p-4 bg-slate-50 rounded-xl border border-slate-200">
                <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Importe actual</p>
                <p class="text-sm font-bold text-emerald-700">{{ formatCurrency(props.inscripto.montoAbonado) }}</p>
              </div>
              <div class="p-4 bg-slate-50 rounded-xl border border-slate-200">
                <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Valor total del viaje</p>
                <p class="text-sm font-bold text-slate-700">{{ formatCurrency(props.variante.valorViaje) }}</p>
              </div>
              <div class="p-4 bg-slate-50 rounded-xl border border-slate-200">
                <p class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Límite máximo</p>
                <p class="text-sm font-bold text-slate-700">{{ formatCurrency(props.variante.valorViaje) }}</p>
              </div>
            </div>

            <div>
              <label for="nuevoMonto" class="block text-sm font-medium text-slate-700 mb-2">Nuevo importe</label>
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-slate-500 sm:text-sm">$</span>
                </div>
                <input
                  id="nuevoMonto"
                  type="text"
                  v-model="nuevoMonto"
                  class="block w-full pl-7 pr-4 py-3 border border-slate-300 rounded-xl bg-white shadow-sm focus:ring-2 focus:ring-teal-500 focus:border-teal-500 sm:text-sm text-slate-900"
                  placeholder="Ej: 250000"
                  @input="nuevoMonto = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
            </div>

            <div>
              <label for="motivo" class="block text-sm font-medium text-slate-700 mb-2">Motivo de modificación (opcional)</label>
              <textarea
                id="motivo"
                v-model="motivo"
                rows="3"
                class="block w-full px-4 py-3 border border-slate-300 rounded-xl bg-white shadow-sm focus:ring-2 focus:ring-teal-500 focus:border-teal-500 sm:text-sm text-slate-900"
                placeholder="Ej: Corrección de seña registrada en el sistema"
              ></textarea>
            </div>
          </div>

          <div class="bg-slate-50 px-6 py-4 flex flex-col sm:flex-row-reverse gap-3 border-t border-slate-200">
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex justify-center px-6 py-3 bg-teal-600 text-white font-bold rounded-xl shadow-lg hover:bg-teal-700 transition-all disabled:opacity-50"
            >
              {{ isSubmitting ? 'Guardando...' : 'Guardar cambios' }}
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
