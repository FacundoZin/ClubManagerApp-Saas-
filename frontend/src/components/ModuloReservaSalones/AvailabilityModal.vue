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
  <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog"
    aria-modal="true">
    <div class="fixed inset-0 bg-slate-900/30 backdrop-blur-sm transition-opacity" @click="$emit('close')"></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-sm border border-slate-200">
        <div class="bg-white px-4 pb-4 pt-5 sm:p-8 sm:pb-6">
          <div v-if="loading" class="flex flex-col items-center justify-center py-8">
            <svg class="animate-spin h-12 w-12 text-blue-600 mb-4" xmlns="http://www.w3.org/2000/svg" fill="none"
              viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
              <path class="opacity-75" fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
              </path>
            </svg>
            <p class="text-sm font-bold text-slate-500 uppercase tracking-widest">Verificando...</p>
          </div>

          <div v-else class="animate-in fade-in zoom-in duration-300">
            <div
              class="mx-auto flex h-16 w-16 flex-shrink-0 items-center justify-center rounded-2xl sm:mx-auto sm:h-16 sm:w-16 mb-6 shadow-sm border"
              :class="result?.data?.disponible ? 'bg-emerald-50 text-emerald-600 border-emerald-100' : 'bg-red-50 text-red-600 border-red-100'">
              <svg v-if="result?.data?.disponible" class="h-10 w-10" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7" />
              </svg>
              <svg v-else class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </div>

            <div class="text-center px-2">
              <h3 class="text-xl font-bold text-slate-900 mb-3">
                {{ result?.data?.disponible ? '¡Disponible!' : 'No Disponible' }}
              </h3>
              <p class="text-sm text-slate-500 font-medium leading-relaxed">
                {{
                  result?.data?.mensaje || result?.errormessage || 'Información de disponibilidad.'
                }}
              </p>
              <div v-if="result?.data?.disponible" class="mt-4 p-2 bg-emerald-50 rounded-lg inline-block">
                <p class="text-xs text-emerald-700 font-bold uppercase tracking-wider">
                  Salón libre
                </p>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-slate-50 px-6 py-5 sm:flex sm:px-8 border-t border-slate-200">
          <button type="button" @click="$emit('close')"
            class="inline-flex w-full justify-center rounded-xl bg-blue-600 px-6 py-3 text-sm font-bold text-white shadow-lg shadow-blue-200 hover:bg-blue-700 transition-all duration-300 hover:scale-[1.02] active:scale-[0.98]">
            Aceptar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
