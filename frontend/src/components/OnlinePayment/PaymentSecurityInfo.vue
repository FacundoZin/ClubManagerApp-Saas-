<template>
  <div class="bg-blue-50/50 backdrop-blur-sm border border-blue-200 rounded-xl p-4">
    <div class="flex items-start gap-3">
      <div
        class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center flex-shrink-0 mt-0.5"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-4 w-4 text-blue-600"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
          />
        </svg>
      </div>
      <div class="flex-1">
        <h4 class="text-sm font-semibold text-blue-900 mb-1">Pago 100% seguro</h4>
        <p class="text-xs text-blue-700 leading-relaxed">
          Tus datos están protegidos. Procesado por Mercado Pago, cumpliendo con los más altos
          estándares de seguridad.
        </p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PaymentSecurityInfo',
}
</script>
