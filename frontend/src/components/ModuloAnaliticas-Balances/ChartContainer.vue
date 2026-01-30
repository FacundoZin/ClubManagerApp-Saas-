<template>
  <div class="bg-white rounded-2xl p-6 shadow-sm border border-slate-200 h-full">
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
      <div>
        <h3 class="text-xl font-bold text-slate-900 tracking-tight">{{ title }}</h3>
        <p v-if="subtitle" class="text-sm font-medium text-slate-500 mt-1 text-pretty">
          {{ subtitle }}
        </p>
      </div>
      <div class="flex items-center">
        <slot name="extra"></slot>
      </div>
    </div>

    <div class="relative h-[350px]">
      <div
        v-if="loading"
        class="absolute inset-0 flex items-center justify-center bg-white/60 z-10 rounded-xl backdrop-blur-[2px] transition-all"
      >
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-teal-600"></div>
      </div>
      <div
        v-if="empty && !loading"
        class="absolute inset-0 flex flex-col items-center justify-center text-slate-400 p-8 text-center"
      >
        <div class="w-16 h-16 bg-slate-50 rounded-full flex items-center justify-center mb-4">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-8 w-8 text-slate-300"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
            />
          </svg>
        </div>
        <p class="font-bold text-slate-600">No hay datos disponibles</p>
        <p class="text-sm mt-1">No se encontraron registros para este período</p>
      </div>
      <slot v-else></slot>
    </div>
  </div>
</template>

<script setup>
defineProps({
  title: String,
  subtitle: String,
  loading: Boolean,
  empty: Boolean,
})
</script>
