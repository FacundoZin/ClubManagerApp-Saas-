<script setup>
import { reactive, ref } from 'vue'
import ViajesService from '../../services/viajesService'

const props = defineProps({
  isOpen: Boolean,
  idViaje: Number,
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  nombreVariante: '',
  valorViaje: null,
  valorSeña: null,
  regimen: 0,
  tipoDeButaca: '',
})

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.nombreVariante = ''
  form.valorViaje = null
  form.valorSeña = null
  form.regimen = 0
  form.tipoDeButaca = ''
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
    class="fixed inset-0 z-[60] overflow-y-auto"
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
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4 border-b border-slate-100">
          <div class="sm:flex sm:items-start">
            <div
              class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-teal-100 sm:mx-0 sm:h-10 sm:w-10"
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
              <h3 class="text-lg font-semibold leading-6 text-slate-900" id="modal-title">
                Agregar Variante de Viaje
              </h3>
              <div class="mt-2">
                <p class="text-sm text-slate-500">
                  Defina una variante específica (ej: Hotel A, Bus Semicama) para este viaje.
                </p>
              </div>
            </div>
          </div>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-5 sm:p-6 space-y-4">
            <div v-if="errorMessage" class="p-3 rounded-md bg-red-50 text-red-700 text-sm mb-4">
              {{ errorMessage }}
            </div>

            <div>
              <label for="nombreVariante" class="block text-sm font-medium text-slate-700"
                >Nombre de la Variante</label
              >
              <input
                type="text"
                id="nombreVariante"
                v-model="form.nombreVariante"
                required
                placeholder="Ej: Opción Económica / Hotel 4 Estrellas"
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label for="valorViaje" class="block text-sm font-medium text-slate-700"
                  >Valor Total ($)</label
                >
                <input
                  type="text"
                  id="valorViaje"
                  v-model="form.valorViaje"
                  required
                  placeholder="Ej: 150000"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                  @input="form.valorViaje = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
              <div>
                <label for="valorSeña" class="block text-sm font-medium text-slate-700"
                  >Monto Seña ($)</label
                >
                <input
                  type="text"
                  id="valorSeña"
                  v-model="form.valorSeña"
                  required
                  placeholder="Ej: 30000"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                  @input="form.valorSeña = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
            </div>

            <div>
              <label for="regimen" class="block text-sm font-medium text-slate-700">Régimen</label>
              <select
                id="regimen"
                v-model="form.regimen"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border bg-white"
              >
                <option v-for="option in regimenOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </option>
              </select>
            </div>

            <div>
              <label for="tipoDeButaca" class="block text-sm font-medium text-slate-700"
                >Tipo de Butaca / Transporte</label
              >
              <input
                type="text"
                id="tipoDeButaca"
                v-model="form.tipoDeButaca"
                required
                placeholder="Ej: Semicama / Cama / Avión"
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
              />
            </div>
          </div>

          <div
            class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 border-t border-slate-200"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex w-full justify-center rounded-md bg-teal-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-teal-700 sm:ml-3 sm:w-auto disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {{ isSubmitting ? 'Guardando...' : 'Guardar Variante' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-slate-900 shadow-sm ring-1 ring-inset ring-slate-200 hover:bg-slate-50 sm:mt-0 sm:w-auto transition-colors"
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
