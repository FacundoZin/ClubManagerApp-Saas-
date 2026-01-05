<script setup>
defineProps({
  socio: {
    type: Object,
    required: true,
  },
})

defineEmits(['view'])
</script>

<template>
  <div
    class="bg-white rounded-lg border border-slate-200 shadow-sm hover:shadow-md transition-shadow duration-200 overflow-hidden"
  >
    <div class="p-5">
      <div class="flex justify-between items-start">
        <div>
          <h3 class="text-lg font-bold text-slate-900">{{ socio.nombre }} {{ socio.apellido }}</h3>
          <p class="text-sm text-slate-500 font-medium mt-1">DNI: {{ socio.dni }}</p>
        </div>
        <span
          class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
          :class="socio.adeudaCuotas ? 'bg-red-100 text-red-800' : 'bg-green-100 text-green-800'"
        >
          {{ socio.adeudaCuotas ? 'Deudor' : 'Al día' }}
        </span>
      </div>

      <div class="mt-4 space-y-2">
        <div v-if="socio.localidad" class="flex items-center text-sm text-slate-600">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4 mr-2 text-slate-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
            />
          </svg>
          {{ socio.localidad }}
        </div>
        <div
          class="flex items-center text-sm font-medium"
          :class="socio.adeudaCuotas ? 'text-red-600' : 'text-green-600'"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4 mr-2"
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
          {{ socio.adeudaCuotas ? 'Pendiente de pago' : 'Cuota semestre actual pagada' }}
        </div>
      </div>
    </div>

    <div class="bg-slate-50 px-5 py-3 border-t border-slate-100 flex justify-end">
      <button
        @click="$emit('view', socio)"
        class="text-xs font-medium text-slate-600 hover:text-blue-600 px-3 py-1 rounded hover:bg-slate-200 transition-colors"
      >
        Ver legajo completo
      </button>
    </div>
  </div>
</template>
