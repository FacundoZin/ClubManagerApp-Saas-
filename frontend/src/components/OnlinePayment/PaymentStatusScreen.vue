<template>
  <div class="max-w-xl mx-auto mt-12 animate-fadeIn">
    <div
      class="bg-white rounded-2xl shadow-lg border border-slate-100 overflow-hidden text-center pb-12"
    >
      <!-- Icon Header -->
      <div
        :class="[
          'p-10 flex flex-col items-center justify-center border-b border-slate-100',
          isApproved ? 'bg-blue-50/50' : status === 'rejected' ? 'bg-red-50/50' : 'bg-indigo-50/50',
        ]"
      >
        <div
          class="w-24 h-24 bg-white rounded-full flex items-center justify-center shadow-sm mb-4 relative"
        >
          <!-- Éxito -->
          <svg
            v-if="isApproved"
            xmlns="http://www.w3.org/2000/svg"
            class="h-12 w-12 text-blue-500"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <!-- Rechazado -->
          <svg
            v-else-if="status === 'rejected'"
            xmlns="http://www.w3.org/2000/svg"
            class="h-12 w-12 text-red-500"
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
          <!-- Procesando (Spinner) -->
          <div v-else class="relative w-12 h-12">
            <div
              class="absolute inset-0 border-4 border-indigo-100 border-t-indigo-600 rounded-full animate-spin"
            ></div>
          </div>
        </div>
        <h3 class="text-2xl font-bold text-slate-800">{{ title }}</h3>
      </div>

      <!-- Content -->
      <div class="px-8 mt-8">
        <p class="text-slate-600 text-lg leading-relaxed">
          {{ message }}
        </p>

        <div v-if="isApproved" class="mt-8 flex flex-col items-center gap-4">
          <div class="p-5 bg-blue-50 rounded-xl inline-block border border-blue-100">
            <p class="text-sm text-blue-800 font-medium flex items-center gap-2 justify-center">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              Su pago ha sido registrado correctamente en nuestro sistema.
            </p>
          </div>

          <button
            @click="$emit('download')"
            class="flex items-center gap-2 px-8 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-all duration-300 shadow-md transform hover:-translate-y-0.5"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"
              />
            </svg>
            Descargar Comprobante
          </button>
        </div>

        <div v-else-if="status === 'rejected'" class="mt-8">
          <button
            @click="$emit('retry')"
            class="px-8 py-3 bg-red-600 text-white font-semibold rounded-lg hover:bg-red-700 transition-all duration-300 shadow-md transform hover:-translate-y-0.5"
          >
            Intentar de nuevo
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PaymentStatusScreen',
  props: {
    status: {
      type: String,
      required: true, // 'approved', 'rejected', 'in_process'
    },
    errorMessage: {
      type: String,
      default: '',
    },
  },
  computed: {
    isApproved() {
      return this.status === 'approved'
    },
    title() {
      if (this.status === 'approved') return '¡Pago aprobado!'
      if (this.status === 'rejected') return 'Pago rechazado'
      return 'Procesando pago'
    },
    message() {
      if (this.status === 'approved')
        return 'Tu pago ha sido procesado con éxito. Gracias por tu aporte.'
      if (this.status === 'rejected') {
        return (
          this.errorMessage ||
          'Lo sentimos, el pago no pudo ser procesado. Por favor, intenta más tarde o utiliza otro medio de pago.'
        )
      }
      return 'Estamos verificando el estado de tu transacción...'
    },
  },
}
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
.animate-fadeIn {
  animation: fadeIn 0.5s ease-out;
}
</style>
