<script setup>
defineProps({
  articulo: {
    type: Object,
    required: true,
  },
})

defineEmits(['update-price'])
</script>

<template>
  <div
    class="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 hover:shadow-xl hover:-translate-y-1 transition-all duration-300 flex flex-col h-full group"
  >
    <div class="flex-1">
      <div
        class="p-4 bg-blue-50 rounded-2xl w-fit mb-5 shadow-sm ring-1 ring-black/5 group-hover:scale-110 transition-transform duration-300"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-6 w-6 text-blue-600"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
          />
        </svg>
      </div>
      <h3
        class="text-xl font-bold text-slate-900 mb-1 group-hover:text-blue-700 transition-colors duration-300"
      >
        {{ articulo.nombre }}
      </h3>
      <div class="flex items-center text-slate-500 mb-4">
        <span class="text-sm font-medium">Precio mensual:</span>
        <span class="ml-2 text-lg font-bold text-slate-900">${{ articulo.precioAlquiler }}</span>
      </div>
    </div>

    <div class="pt-4 border-t border-slate-100 mt-4">
      <button
        @click="$emit('update-price', articulo)"
        class="w-full inline-flex justify-center items-center px-4 py-2.5 text-sm font-bold text-blue-700 bg-blue-50 rounded-xl hover:bg-blue-100 transition-all border border-blue-100 shadow-sm"
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
            d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        Modificar Precio
      </button>
    </div>
  </div>
</template>
