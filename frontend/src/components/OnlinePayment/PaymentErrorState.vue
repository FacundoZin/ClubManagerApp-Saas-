<template>
  <div class="max-w-2xl mx-auto">
    <div class="bg-white rounded-2xl shadow-xl border border-red-200 overflow-hidden">
      <div class="bg-gradient-to-r from-red-500 to-red-600 p-6">
        <div class="flex items-center gap-4">
          <div
            class="w-12 h-12 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-6 w-6 text-white"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
              />
            </svg>
          </div>
          <div>
            <h3 class="text-xl font-bold text-white">Error al cargar el pago</h3>
            <p class="text-red-100 text-sm mt-1">No pudimos procesar tu solicitud</p>
          </div>
        </div>
      </div>
      <div class="p-6">
        <div class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
          <p class="text-red-800 font-medium">{{ errorMessage }}</p>
        </div>
        <div class="flex flex-col sm:flex-row gap-3">
          <button
            @click="$emit('retry')"
            class="flex-1 px-6 py-3 bg-gradient-to-r from-red-500 to-red-600 text-white font-semibold rounded-lg hover:from-red-600 hover:to-red-700 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-0.5"
          >
            Reintentar
          </button>
          <button
            @click="$emit('go-back')"
            class="flex-1 px-6 py-3 bg-slate-100 text-slate-700 font-semibold rounded-lg hover:bg-slate-200 transition-all duration-300"
          >
            Volver
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PaymentErrorState',
  props: {
    errorMessage: {
      type: String,
      required: true,
    },
  },
}
</script>
