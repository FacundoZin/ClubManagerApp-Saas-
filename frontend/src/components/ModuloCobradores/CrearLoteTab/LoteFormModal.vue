<script setup>
import { reactive, ref } from 'vue'
import CobranzasService from '../../../services/CobranzasService'

const props = defineProps({
  isOpen: Boolean,
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  nombreLote: '',
  calleNorte: '',
  calleSur: '',
  calleEste: '',
  calleOeste: '',
})

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.nombreLote = ''
  form.calleNorte = ''
  form.calleSur = ''
  form.calleEste = ''
  form.calleOeste = ''
  errorMessage.value = ''
}

const handleSubmit = async () => {
  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const data = await CobranzasService.crearLote(form)
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
        class="relative transform overflow-hidden rounded-3xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <!-- Header -->
        <div class="bg-white px-6 py-6 border-b border-slate-100">
          <div class="sm:flex sm:items-start">
            <div
              class="mx-auto flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-cyan-100 sm:mx-0 sm:h-12 sm:w-12 shadow-sm ring-1 ring-cyan-200/50"
            >
              <svg
                class="h-6 w-6 text-cyan-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="2"
                stroke="currentColor"
                aria-hidden="true"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M9 6.75V15m6-6v8.25m.503 3.498l4.875-2.437c.381-.19.622-.58.622-1.006V4.82c0-.836-.88-1.38-1.628-1.006l-3.869 1.934c-.317.159-.69.159-1.006 0L9.503 3.252a1.125 1.125 0 00-1.006 0L3.622 5.689C3.24 5.88 3 6.27 3 6.695V19.18c0 .836.88 1.38 1.628 1.006l3.869-1.934c.317-.159.69-.159 1.006 0l4.994 2.497c.317.158.69.158 1.006 0z"
                />
              </svg>
            </div>
            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
              <h3
                class="text-lg font-bold leading-6 text-slate-900 tracking-tight"
                id="modal-title"
              >
                Crear Nuevo Lote
              </h3>
              <div class="mt-1">
                <p class="text-sm text-slate-500 font-medium">
                  Complete la información para registrar la zona de cobranza.
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-4 space-y-4">
            <div
              v-if="errorMessage"
              class="p-4 rounded-xl bg-red-50 text-red-700 text-sm mb-4 border border-red-100 font-bold flex items-center gap-2"
            >
              <svg class="w-5 h-5 text-red-400" fill="currentColor" viewBox="0 0 20 20">
                <path
                  fill-rule="evenodd"
                  d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
                  clip-rule="evenodd"
                />
              </svg>
              {{ errorMessage }}
            </div>

            <div>
              <label
                for="nombreLote"
                class="block text-[10px] font-black text-slate-400 uppercase tracking-widest mb-2"
                >Nombre del Lote *</label
              >
              <input
                type="text"
                id="nombreLote"
                v-model="form.nombreLote"
                required
                class="block w-full rounded-xl border-slate-200 shadow-sm focus:border-blue-500 focus:ring-2 focus:ring-blue-500 sm:text-sm px-4 py-3 border transition-all placeholder:text-slate-400"
                placeholder="Ej: Lote 1, Zona Norte, etc."
              />
            </div>

            <div class="space-y-4 pt-2">
              <p
                class="text-xs font-black text-slate-800 uppercase tracking-widest flex items-center"
              >
                <span class="w-1.5 h-1.5 rounded-full bg-cyan-500 mr-2"></span>
                Límites Geográficos
              </p>
              <div class="grid grid-cols-2 gap-4">
                <div>
                  <label
                    for="calleNorte"
                    class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1.5"
                    >Calle Norte</label
                  >
                  <input
                    type="text"
                    id="calleNorte"
                    v-model="form.calleNorte"
                    class="block w-full rounded-xl border-slate-200 shadow-sm focus:border-blue-500 focus:ring-2 focus:ring-blue-500 sm:text-sm px-4 py-3 border transition-all placeholder:text-slate-400"
                    placeholder="Calle superior"
                  />
                </div>
                <div>
                  <label
                    for="calleSur"
                    class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1.5"
                    >Calle Sur</label
                  >
                  <input
                    type="text"
                    id="calleSur"
                    v-model="form.calleSur"
                    class="block w-full rounded-xl border-slate-200 shadow-sm focus:border-blue-500 focus:ring-2 focus:ring-blue-500 sm:text-sm px-4 py-3 border transition-all placeholder:text-slate-400"
                    placeholder="Calle inferior"
                  />
                </div>
                <div>
                  <label
                    for="calleEste"
                    class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1.5"
                    >Calle Este</label
                  >
                  <input
                    type="text"
                    id="calleEste"
                    v-model="form.calleEste"
                    class="block w-full rounded-xl border-slate-200 shadow-sm focus:border-blue-500 focus:ring-2 focus:ring-blue-500 sm:text-sm px-4 py-3 border transition-all placeholder:text-slate-400"
                    placeholder="Calle derecha"
                  />
                </div>
                <div>
                  <label
                    for="calleOeste"
                    class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1.5"
                    >Calle Oeste</label
                  >
                  <input
                    type="text"
                    id="calleOeste"
                    v-model="form.calleOeste"
                    class="block w-full rounded-xl border-slate-200 shadow-sm focus:border-blue-500 focus:ring-2 focus:ring-blue-500 sm:text-sm px-4 py-3 border transition-all placeholder:text-slate-400"
                    placeholder="Calle izquierda"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Footer Actions -->
          <div class="bg-slate-50 px-4 py-3 flex flex-row-reverse gap-3 border-t border-slate-100">
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex flex-1 sm:flex-none w-full sm:w-auto justify-center items-center rounded-xl bg-cyan-600 px-4 py-2 text-sm font-bold text-white shadow-lg hover:bg-cyan-700 disabled:opacity-50 disabled:cursor-not-allowed transition-all transform active:scale-95"
            >
              <svg
                v-if="isSubmitting"
                class="animate-spin h-4 w-4 mr-2"
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
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
                ></path>
              </svg>
              {{ isSubmitting ? 'Creando...' : 'Crear Lote' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="inline-flex flex-1 sm:flex-none w-full sm:w-auto justify-center items-center rounded-xl bg-white px-4 py-2 text-sm font-bold text-slate-700 shadow-sm border border-slate-200 hover:bg-slate-50 transition-all"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
