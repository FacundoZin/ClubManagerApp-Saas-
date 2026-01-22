<template>
  <header
    class="bg-white/80 backdrop-blur-md border-b border-slate-200 sticky top-0 z-30 shadow-sm"
  >
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16 items-center">
        <div class="flex items-center gap-3">
          <div
            class="w-9 h-9 bg-gradient-to-br from-blue-600 to-blue-700 rounded-lg flex items-center justify-center shadow-md text-white"
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
                d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
          </div>
          <div>
            <h1 class="text-lg font-bold text-slate-900 tracking-tight leading-none">
              Pago de Cuota
            </h1>
            <span class="text-xs text-slate-500 font-medium"
              >Asociación civil casa del jubilado</span
            >
          </div>
        </div>

        <div class="flex items-center gap-2">
          <div
            class="px-3 py-1.5 bg-blue-50 text-blue-700 rounded-lg text-xs font-semibold border border-blue-200"
          >
            Pago Seguro
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script>
export default {
  name: 'PaymentHeader',
}
</script>
