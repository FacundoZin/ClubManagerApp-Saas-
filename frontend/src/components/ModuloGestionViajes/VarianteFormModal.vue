<script setup>
import { reactive, ref } from 'vue'
import ViajesService from '../../services/viajesService'

const props = defineProps({
  isOpen: Boolean,
  idViaje: Number,
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  nombreVariante: null,
  valorViaje: null,
  valorSeña: null,
  regimen: null,
  tipoDeButaca: null,
})

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.nombreVariante = null
  form.valorViaje = null
  form.valorSeña = null
  form.regimen = null
  form.tipoDeButaca = null
  errorMessage.value = ''
}

const handleSubmit = async () => {
  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const data = await ViajesService.createVarianteViaje({
      idViaje: props.idViaje,
      ...form,
    })
    emit('save', data)
    resetForm()
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isSubmitting.value = false
  }
}

const regimenOptions = [
  { value: 0, label: 'Media Pensión' },
  { value: 1, label: 'Pensión Completa' },
]
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
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4 border-b border-slate-100">
          <div class="sm:flex sm:items-start">
            <div
              class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-teal-100 sm:mx-0 sm:h-12 sm:w-12"
            >
              <svg
                class="h-6 w-6 text-teal-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M12 9v6m3-3H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
              <h3 class="text-xl font-bold leading-6 text-slate-900" id="modal-title">
                Agregar Variante de Viaje
              </h3>
              <div class="mt-1">
                <p class="text-sm font-medium text-slate-500">
                  Defina una variante específica (ej: Hotel A, Bus Semicama) para este viaje.
                </p>
              </div>
            </div>
          </div>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-5 sm:p-6 space-y-4">
            <div
              v-if="errorMessage"
              class="p-3 rounded-xl bg-red-50 border border-red-100 text-red-700 text-sm flex items-start gap-2"
            >
              <svg
                class="h-5 w-5 text-red-400 shrink-0"
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
              {{ errorMessage }}
            </div>

            <div class="space-y-1">
              <label for="nombreVariante" class="block text-sm font-bold text-slate-700"
                >Nombre de la Variante</label
              >
              <input
                type="text"
                id="nombreVariante"
                v-model="form.nombreVariante"
                required
                placeholder="Ej: Opción Económica / Hotel 4 Estrellas"
                class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label for="valorViaje" class="block text-sm font-bold text-slate-700"
                  >Valor Total ($)</label
                >
                <input
                  type="text"
                  id="valorViaje"
                  v-model="form.valorViaje"
                  required
                  placeholder="Ej: 150000"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                  @input="form.valorViaje = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
              <div class="space-y-1">
                <label for="valorSeña" class="block text-sm font-bold text-slate-700"
                  >Monto Seña ($)</label
                >
                <input
                  type="text"
                  id="valorSeña"
                  v-model="form.valorSeña"
                  required
                  placeholder="Ej: 30000"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                  @input="form.valorSeña = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
            </div>

            <div class="space-y-1">
              <label for="regimen" class="block text-sm font-bold text-slate-700">Régimen</label>
              <select
                id="regimen"
                v-model="form.regimen"
                required
                class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all bg-white"
              >
                <option :value="null" disabled>Seleccione un régimen</option>
                <option v-for="option in regimenOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </option>
              </select>
            </div>

            <div class="space-y-1">
              <label for="tipoDeButaca" class="block text-sm font-bold text-slate-700"
                >Tipo de Butaca / Transporte</label
              >
              <input
                type="text"
                id="tipoDeButaca"
                v-model="form.tipoDeButaca"
                required
                placeholder="Ej: Semicama / Cama / Avión"
                class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
              />
            </div>
          </div>

          <div
            class="bg-slate-50 px-4 py-3 flex gap-3 sm:flex-row-reverse sm:px-6 border-t border-slate-100"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex flex-1 justify-center items-center gap-2 rounded-xl bg-teal-600 px-4 py-2.5 text-sm font-bold text-white shadow-lg shadow-teal-100 hover:bg-teal-700 sm:ml-3 sm:w-auto sm:flex-none disabled:opacity-50 transition-all active:scale-95"
            >
              <svg
                v-if="isSubmitting"
                class="animate-spin h-4 w-4 text-white"
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
              {{ isSubmitting ? 'Guardando...' : 'Guardar Variante' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="inline-flex flex-1 justify-center rounded-xl bg-white px-4 py-2.5 text-sm font-bold text-slate-700 shadow-sm ring-1 ring-inset ring-slate-200 hover:bg-slate-50 sm:w-auto sm:flex-none transition-all active:scale-95"
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
