<script setup>
import { reactive, ref, watch } from 'vue'
import ArticuloService from '../../../services/ArticuloService'

const props = defineProps({
  isOpen: Boolean,
  articulo: Object,
})

const emit = defineEmits(['close', 'save'])

const nuevoPrecio = ref(0)
const isSubmitting = ref(false)
const errorMessage = ref('')

watch(
  () => props.articulo,
  (newVal) => {
    if (newVal) {
      nuevoPrecio.value = newVal.precioAlquiler
    }
  },
)

const handleSubmit = async () => {
  if (!props.articulo || nuevoPrecio.value <= 0) {
    errorMessage.value = 'Ingrese un precio válido.'
    return
  }

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const result = await ArticuloService.updatePrecio(props.articulo.id, nuevoPrecio.value)
    emit('save', result)
    // Reset isn't strictly needed as modal closes, but good practice
  } catch (e) {
    errorMessage.value = e.message
  } finally {
    isSubmitting.value = false
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
      class="fixed inset-0 bg-slate-900/30 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-sm border border-slate-200"
      >
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
          <h3 class="text-lg font-semibold leading-6 text-slate-900 mb-2">Modificar Precio</h3>
          <p class="text-sm text-slate-500 mb-4">
            Actualizar el precio de alquiler para: <strong>{{ articulo?.nombre }}</strong>
          </p>

          <div v-if="errorMessage" class="text-red-500 text-sm mb-3">{{ errorMessage }}</div>

          <label class="block text-sm font-medium text-slate-700">Nuevo Precio Mensual</label>
          <div class="relative mt-1 rounded-md shadow-sm">
            <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
              <span class="text-slate-500 sm:text-sm">$</span>
            </div>
            <input
              type="number"
              v-model="nuevoPrecio"
              min="1"
              class="block w-full rounded-md border-slate-300 pl-7 pr-3 focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
            />
          </div>
        </div>

        <div class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
          <button
            type="button"
            @click="handleSubmit"
            :disabled="isSubmitting"
            class="inline-flex w-full justify-center rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 sm:ml-3 sm:w-auto disabled:opacity-50 transition-colors"
          >
            {{ isSubmitting ? 'Guardando...' : 'Confirmar' }}
          </button>
          <button
            type="button"
            @click="$emit('close')"
            class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-slate-900 shadow-sm ring-1 ring-inset ring-slate-300 hover:bg-slate-50 sm:mt-0 sm:w-auto transition-colors"
          >
            Cancelar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
