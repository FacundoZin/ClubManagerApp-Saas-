<script setup>
defineProps({
  isOpen: Boolean,
  result: {
    type: Object,
    default: null, // Should contain { available: boolean, message: string } or similar
  },
  loading: Boolean,
})

defineEmits(['close'])
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
          <div v-if="loading" class="flex flex-col items-center justify-center py-6">
            <svg
              class="animate-spin h-10 w-10 text-blue-600 mb-3"
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
            <p class="text-sm text-slate-500">Verificando disponibilidad...</p>
          </div>

          <div v-else>
            <div
              class="mx-auto flex h-16 w-16 flex-shrink-0 items-center justify-center rounded-full sm:mx-0 sm:h-12 sm:w-12 mb-4 mx-auto"
              :class="result?.data?.disponible ? 'bg-emerald-100' : 'bg-red-100'"
            >
              <svg
                v-if="result?.data?.disponible"
                class="h-8 w-8 text-emerald-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M5 13l4 4L19 7"
                />
              </svg>
              <svg
                v-else
                class="h-8 w-8 text-red-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </div>

            <div class="text-center">
              <h3 class="text-lg font-bold text-slate-900 mb-2">
                {{ result?.data?.disponible ? 'Disponible' : 'No Disponible' }}
              </h3>
              <p class="text-sm text-slate-600">
                {{
                  result?.data?.mensaje || result?.errormessage || 'Información de disponibilidad.'
                }}
              </p>
              <p v-if="result?.data?.disponible" class="text-xs text-emerald-600 mt-2 font-medium">
                El salón está libre para la fecha seleccionada.
              </p>
            </div>
          </div>
        </div>

        <div class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
          <button
            type="button"
            @click="$emit('close')"
            class="inline-flex w-full justify-center rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 sm:ml-3 sm:w-auto transition-colors"
          >
            Aceptar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
