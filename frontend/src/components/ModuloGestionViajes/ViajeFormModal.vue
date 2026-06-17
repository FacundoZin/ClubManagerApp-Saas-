<script setup>
import { reactive, ref } from 'vue'
import ViajesService from '../../services/viajesService'

const props = defineProps({
  isOpen: Boolean,
  viaje: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  id: null,
  titulo: null,
  dias: null,
  noches: null,
  fechaSalida: null,
  ventasParaLiberado: null,
  valorBase: null,
  porcentajeComision: null,
})

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.id = props.viaje?.id || null
  form.titulo = props.viaje?.titulo || null
  form.dias = props.viaje?.dias || null
  form.noches = props.viaje?.noches || null
  form.fechaSalida = props.viaje?.fechasalida || null
  form.ventasParaLiberado = props.viaje?.ventasParaLiberado || null
  form.valorBase = props.viaje?.valorBase || null
  form.porcentajeComision = props.viaje?.porcentajeComision || null
  errorMessage.value = ''
}

import { watch } from 'vue'
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    resetForm()
  }
})

const handleSubmit = async () => {
  isSubmitting.value = true
  errorMessage.value = ''

  try {
    if (props.viaje) {
      await ViajesService.updateViaje(form)
      emit('save')
    } else {
      const data = await ViajesService.createViaje(form)
      emit('save', data)
    }
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
    class="fixed inset-0 z-50 overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <!-- Background backdrop with blur -->
    <div
      class="fixed inset-0 bg-slate-900/30 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <!-- Header -->
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
                  d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L10.582 16.07a4.5 4.5 0 01-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 011.13-1.897l8.932-8.931zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0115.75 21H5.25A2.25 2.25 0 013 18.75V8.25A2.25 2.25 0 015.25 6H10"
                />
              </svg>
            </div>
            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
              <h3 class="text-xl font-bold leading-6 text-slate-900" id="modal-title">
                {{ viaje ? 'Editar Viaje Base' : 'Crear Nuevo Viaje Base' }}
              </h3>
              <div class="mt-1">
                <p class="text-sm font-medium text-slate-500">
                   {{ viaje ? 'Actualice los parámetros generales del viaje.' : 'Defina los parámetros generales del viaje. Luego podrá agregar variantes.' }}
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Form -->
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
              <label for="titulo" class="block text-sm font-bold text-slate-700"
                >Título del Viaje</label
              >
              <input
                type="text"
                id="titulo"
                v-model="form.titulo"
                required
                placeholder="Ej: Cataratas del Iguazú"
                class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label for="dias" class="block text-sm font-bold text-slate-700">Días</label>
                <input
                  type="text"
                  id="dias"
                  v-model="form.dias"
                  required
                  placeholder="Ej: 5"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                  @input="form.dias = $event.target.value.replace(/[^0-9]/g, '')"
                />
              </div>
              <div class="space-y-1">
                <label for="noches" class="block text-sm font-bold text-slate-700">Noches</label>
                <input
                  type="text"
                  id="noches"
                  v-model="form.noches"
                  required
                  placeholder="Ej: 4"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                  @input="form.noches = $event.target.value.replace(/[^0-9]/g, '')"
                />
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label for="fechaSalida" class="block text-sm font-bold text-slate-700"
                  >Fecha de Salida</label
                >
                <input
                  type="date"
                  id="fechaSalida"
                  v-model="form.fechaSalida"
                  required
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                />
              </div>
              <div class="space-y-1">
                <label for="ventasParaLiberado" class="block text-sm font-bold text-slate-700"
                  >Ventas para Liberado</label
                >
                <input
                  type="text"
                  id="ventasParaLiberado"
                  v-model="form.ventasParaLiberado"
                  placeholder="Ej: 15"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                  @input="form.ventasParaLiberado = $event.target.value.replace(/[^0-9]/g, '')"
                />
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label for="valorBase" class="block text-sm font-bold text-slate-700"
                  >Valor Base ($)</label
                >
                <input
                  type="text"
                  id="valorBase"
                  v-model="form.valorBase"
                  required
                  placeholder="Ej: 150000"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                  @input="form.valorBase = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
              <div class="space-y-1">
                <label for="porcentajeComision" class="block text-sm font-bold text-slate-700"
                  >% Comisión</label
                >
                <input
                  type="text"
                  id="porcentajeComision"
                  v-model="form.porcentajeComision"
                  required
                  placeholder="Ej: 15"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-teal-500 focus:ring focus:ring-teal-200 sm:text-sm px-3 py-2 border transition-all"
                  @input="form.porcentajeComision = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
            </div>
          </div>

          <!-- Footer Actions -->
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
              {{ isSubmitting ? 'Guardando...' : (viaje ? 'Actualizar Viaje' : 'Crear Viaje') }}

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
