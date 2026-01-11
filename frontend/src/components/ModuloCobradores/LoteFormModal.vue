<script setup>
import { reactive, ref } from 'vue'
import CobranzasService from '../../services/CobranzasService'

const props = defineProps({
  isOpen: Boolean,
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  nombreLote: '',
  calle1: '',
  calle2: '',
  calle3: '',
  calle4: '',
})

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.nombreLote = ''
  form.calle1 = ''
  form.calle2 = ''
  form.calle3 = ''
  form.calle4 = ''
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
        class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
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
              <h3 class="text-lg font-semibold leading-6 text-slate-900" id="modal-title">
                Crear Nuevo Lote
              </h3>
              <div class="mt-2">
                <p class="text-sm text-slate-500">
                  Complete la información para registrar un nuevo lote en el sistema.
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
              <label for="nombreLote" class="block text-sm font-medium text-slate-700"
                >Nombre del Lote *</label
              >
              <input
                type="text"
                id="nombreLote"
                v-model="form.nombreLote"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                placeholder="Ej: Lote 1, Zona Norte, etc."
              />
            </div>

            <div class="space-y-3">
              <p class="text-sm font-medium text-slate-700">Calles que delimitan el lote</p>
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label for="calle1" class="block text-xs font-medium text-slate-600"
                    >Calle 1</label
                  >
                  <input
                    type="text"
                    id="calle1"
                    v-model="form.calle1"
                    class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                    placeholder="Nombre de calle"
                  />
                </div>
                <div>
                  <label for="calle2" class="block text-xs font-medium text-slate-600"
                    >Calle 2</label
                  >
                  <input
                    type="text"
                    id="calle2"
                    v-model="form.calle2"
                    class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                    placeholder="Nombre de calle"
                  />
                </div>
                <div>
                  <label for="calle3" class="block text-xs font-medium text-slate-600"
                    >Calle 3</label
                  >
                  <input
                    type="text"
                    id="calle3"
                    v-model="form.calle3"
                    class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                    placeholder="Nombre de calle"
                  />
                </div>
                <div>
                  <label for="calle4" class="block text-xs font-medium text-slate-600"
                    >Calle 4</label
                  >
                  <input
                    type="text"
                    id="calle4"
                    v-model="form.calle4"
                    class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-teal-500 focus:ring-teal-500 sm:text-sm px-3 py-2 border"
                    placeholder="Nombre de calle"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Footer Actions -->
          <div
            class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 border-t border-slate-200"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex w-full justify-center rounded-md bg-teal-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-teal-500 sm:ml-3 sm:w-auto disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {{ isSubmitting ? 'Creando...' : 'Crear Lote' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-slate-900 shadow-sm ring-1 ring-inset ring-slate-300 hover:bg-slate-50 sm:mt-0 sm:w-auto transition-colors"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
