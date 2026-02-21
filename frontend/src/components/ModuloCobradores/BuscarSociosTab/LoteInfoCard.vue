<script setup>
defineProps({
  lote: {
    type: Object,
    required: true,
  },
})
</script>

<template>
  <div v-if="lote" class="flex-1 animate-in fade-in slide-in-from-left-2 duration-300">
    <div
      class="bg-slate-50/50 p-3 rounded-xl border border-slate-100 flex flex-col md:flex-row md:items-center gap-4"
    >
      <!-- Título y Icono -->
      <div
        class="flex items-center gap-4 border-b md:border-b-0 md:border-r border-slate-200 pb-4 md:pb-0 md:pr-8"
      >
        <div
          class="w-10 h-10 bg-white rounded-lg shadow-sm border border-slate-100 flex items-center justify-center text-cyan-600 shrink-0"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-6 w-6"
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
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"
            />
          </svg>
        </div>
        <div>
          <h4 class="text-sm font-bold text-slate-900 leading-tight">
            {{ lote.nombreLote }}
          </h4>
          <p class="text-[10px] text-cyan-600 font-bold uppercase tracking-widest mt-0.5">
            Zona Establecida
          </p>
        </div>
      </div>

      <!-- Grilla de Calles -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-x-8 gap-y-4 flex-1">
        <div class="flex flex-col">
          <span class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1"
            >Calle Norte</span
          >
          <span class="text-sm font-bold text-slate-700">{{ lote.calleNorte || '-' }}</span>
        </div>
        <div class="flex flex-col">
          <span class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1"
            >Calle Sur</span
          >
          <span class="text-sm font-bold text-slate-700">{{ lote.calleSur || '-' }}</span>
        </div>
        <div class="flex flex-col">
          <span class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1"
            >Calle Este</span
          >
          <span class="text-sm font-bold text-slate-700">{{ lote.calleEste || '-' }}</span>
        </div>
        <div class="flex flex-col">
          <span class="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-1"
            >Calle Oeste</span
          >
          <span class="text-sm font-bold text-slate-700">{{ lote.calleOeste || '-' }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
