<template>
  <!-- Welcome Card -->
  <div
    class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl border border-slate-200 overflow-hidden transform transition-all duration-500 hover:shadow-2xl"
  >
    <div class="bg-gradient-to-r from-blue-600 via-blue-700 to-indigo-700 p-8">
      <div class="flex items-start justify-between">
        <div class="flex-1">
          <div
            class="inline-flex items-center gap-2 bg-white/20 backdrop-blur-sm px-3 py-1 rounded-full mb-4"
          >
            <div class="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
            <span class="text-xs font-semibold text-white">Sesión activa</span>
          </div>
          <h2 class="text-2xl md:text-3xl font-bold text-white mb-2">
            ¡Hola, {{ paymentData?.nombreSocio || 'Socio' }}!
          </h2>
          <p class="text-blue-100 text-base md:text-lg leading-relaxed">
            Aquí podrá realizar el pago de su cuota de socio de la Asociación civil casa del
            jubilado correspondiente al
            <span class="font-semibold text-white">{{ paymentData?.semestrePago }}</span>
            del año <span class="font-semibold text-white">{{ paymentData?.anioPago }}</span
            >.
          </p>
        </div>
        <div class="hidden md:block">
          <div
            class="w-16 h-16 bg-white/10 backdrop-blur-sm rounded-2xl flex items-center justify-center"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-8 w-8 text-white"
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
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Details -->
    <div class="p-6 bg-gradient-to-br from-slate-50 to-white">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="bg-white rounded-xl p-4 border border-slate-200 shadow-sm">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-blue-50 rounded-lg flex items-center justify-center">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 text-blue-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                />
              </svg>
            </div>
            <div>
              <p class="text-xs text-slate-500 font-medium">Período</p>
              <p class="text-sm font-bold text-slate-900">
                {{ paymentData?.semestrePago }}
              </p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl p-4 border border-slate-200 shadow-sm">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-indigo-50 rounded-lg flex items-center justify-center">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 text-indigo-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
            <div>
              <p class="text-xs text-slate-500 font-medium">Año</p>
              <p class="text-sm font-bold text-slate-900">{{ paymentData?.anioPago }}</p>
            </div>
          </div>
        </div>

        <div class="bg-gradient-to-br from-emerald-500 to-emerald-600 rounded-xl p-4 shadow-md">
          <div class="flex items-center gap-3">
            <div
              class="w-10 h-10 bg-white/20 backdrop-blur-sm rounded-lg flex items-center justify-center"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 text-white"
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
              <p class="text-xs text-emerald-100 font-medium">Monto a pagar</p>
              <p class="text-lg font-bold text-white">
                {{ formatMonto(paymentData?.monto) }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PaymentInfoCard',
  props: {
    paymentData: {
      type: Object,
      required: true,
    },
  },
  methods: {
    formatMonto(monto) {
      return Number(monto || 0).toLocaleString('es-AR', {
        style: 'currency',
        currency: 'ARS',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      })
    },
  },
}
</script>
