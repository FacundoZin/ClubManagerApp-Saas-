<script setup>
import { reactive, ref } from 'vue'
import ViajesService from '../../services/viajesService'

const props = defineProps({
  isOpen: Boolean,
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  titulo: '',
  dias: null,
  noches: null,
  fechaSalida: '',
  ventasParaLiberado: null,
  valorBase: null,
  porcentajeComision: null,
})

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.titulo = ''
  form.dias = null
  form.noches = null
  form.fechaSalida = ''
  form.ventasParaLiberado = null
  form.valorBase = null
  form.porcentajeComision = null
  errorMessage.value = ''
}

const handleSubmit = async () => {
  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const data = await ViajesService.createViaje(form)
    emit('save', data)
    resetForm()
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
    class="fixed inset-0 z-[60] overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <!-- Background backdrop with blur -->
    <div
      class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <!-- Header -->
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
                  d="M6 12L3.269 3.126A59.768 59.768 0 0121.485 12 59.77 59.77 0 013.27 20.876L5.999 12zm0 0h7.5"
                />
              </svg>
            </div>
            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
              <h3 class="text-lg font-semibold leading-6 text-slate-900" id="modal-title">
                Crear Nuevo Viaje Base
              </h3>
              <div class="mt-2">
                <p class="text-sm text-slate-500">
                  Defina los parámetros generales del viaje. Luego podrá agregar variantes.
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-5 sm:p-6 space-y-4">
            <div v-if="errorMessage" class="p-3 rounded-md bg-red-50 text-red-700 text-sm mb-4">
              {{ errorMessage }}
            </div>

            <div>
              <label for="titulo" class="block text-sm font-medium text-slate-700"
                >Título del Viaje</label
              >
              <input
                type="text"
                id="titulo"
                v-model="form.titulo"
                required
                placeholder="Ej: Cataratas del Iguazú"
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label for="dias" class="block text-sm font-medium text-slate-700">Días</label>
                <input
                  type="text"
                  id="dias"
                  v-model="form.dias"
                  required
                  placeholder="Ej: 5"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                  @input="form.dias = $event.target.value.replace(/[^0-9]/g, '')"
                />
              </div>
              <div>
                <label for="noches" class="block text-sm font-medium text-slate-700">Noches</label>
                <input
                  type="text"
                  id="noches"
                  v-model="form.noches"
                  required
                  placeholder="Ej: 4"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                  @input="form.noches = $event.target.value.replace(/[^0-9]/g, '')"
                />
              </div>
            </div>

            <div>
              <label for="fechaSalida" class="block text-sm font-medium text-slate-700"
                >Fecha de Salida</label
              >
              <input
                type="date"
                id="fechaSalida"
                v-model="form.fechaSalida"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label for="valorBase" class="block text-sm font-medium text-slate-700"
                  >Valor Base ($)</label
                >
                <input
                  type="text"
                  id="valorBase"
                  v-model="form.valorBase"
                  required
                  placeholder="Ej: 150000"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                  @input="form.valorBase = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
              <div>
                <label for="porcentajeComision" class="block text-sm font-medium text-slate-700"
                  >% Comisión</label
                >
                <input
                  type="text"
                  id="porcentajeComision"
                  v-model="form.porcentajeComision"
                  required
                  placeholder="Ej: 15"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                  @input="form.porcentajeComision = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
            </div>

            <div>
              <label for="ventasParaLiberado" class="block text-sm font-medium text-slate-700"
                >Ventas para Liberado (Opcional)</label
              >
              <input
                type="text"
                id="ventasParaLiberado"
                v-model="form.ventasParaLiberado"
                placeholder="Ej: 15"
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                @input="form.ventasParaLiberado = $event.target.value.replace(/[^0-9]/g, '')"
              />
            </div>
          </div>

          <!-- Footer Actions -->
          <div
            class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 border-t border-slate-200"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex w-full justify-center rounded-md bg-teal-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-teal-700 sm:ml-3 sm:w-auto disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {{ isSubmitting ? 'Guardando...' : 'Crear Viaje' }}
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
