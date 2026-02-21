<script setup>
const emit = defineEmits(['create'])
</script>

<template>
  <div class="max-w-2xl mx-auto py-6">
    <div
      class="bg-white p-6 rounded-2xl border border-slate-200 shadow-xl text-center relative overflow-hidden"
    >
      <div
        class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-cyan-600 to-cyan-400"
      ></div>

      <div
        class="w-16 h-16 bg-cyan-50 text-cyan-600 rounded-xl flex items-center justify-center mx-auto mb-4 shadow-sm ring-1 ring-cyan-100/50"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-10 w-10"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M9 6.75V15m6-6v8.25m.503 3.498l4.875-2.437c.381-.19.622-.58.622-1.006V4.82c0-.836-.88-1.38-1.628-1.006l-3.869 1.934c-.317.159-.69.159-1.006 0L9.503 3.252a1.125 1.125 0 00-1.006 0L3.622 5.689C3.24 5.88 3 6.27 3 6.695V19.18c0 .836.88 1.38 1.628 1.006l3.869-1.934c.317-.159.69-.159 1.006 0l4.994 2.497c.317.158.69.158 1.006 0z"
          />
        </svg>
      </div>

      <h3 class="text-2xl font-black text-slate-900 mb-2 tracking-tight">Crear Nuevo Lote</h3>
      <p class="text-slate-500 mb-5 text-base max-w-md mx-auto leading-relaxed">
        Organice las cobranzas del club de manera eficiente mediante la segmentación por zonas
        geográficas.
      </p>

      <button
        @click="$emit('create')"
        class="inline-flex items-center px-6 py-2.5 border border-transparent text-sm font-black uppercase tracking-widest rounded-xl shadow-lg shadow-cyan-200 text-white bg-cyan-600 hover:bg-cyan-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-cyan-500 transition-all transform active:scale-95 group"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5 mr-3 group-hover:scale-125 transition-transform"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="3"
            d="M12 4v16m8-8H4"
          />
        </svg>
        Registrar Lote
      </button>
    </div>
  </div>
</template>
