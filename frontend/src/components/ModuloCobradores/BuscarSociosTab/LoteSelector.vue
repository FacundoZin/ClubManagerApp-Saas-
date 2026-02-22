<script setup>
defineProps({
  lotes: {
    type: Array,
    required: true,
  },
  modelValue: {
    type: [String, Number],
    default: '',
  },
  loading: {
    type: Boolean,
    default: false,
  },
})

defineEmits(['update:modelValue', 'change'])
</script>

<template>
  <div class="w-full lg:max-w-xs shrink-0">
    <label
      for="lote-select"
      class="block text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-2 px-1"
    >
      Lote de Cobranza
    </label>
    <div class="relative group">
      <select
        id="lote-select"
        :value="modelValue"
        @change="
          ($emit('update:modelValue', $event.target.value), $emit('change', $event.target.value))
        "
        class="appearance-none block w-full rounded-lg border-slate-200 bg-white text-slate-700 font-bold sm:text-sm px-3 py-2 border focus:border-cyan-500 focus:ring-4 focus:ring-cyan-500/5 transition-all cursor-pointer pr-10 hover:border-slate-300 shadow-sm"
      >
        <option value="">Seleccione un lote...</option>
        <option v-for="lote in lotes" :key="lote.id" :value="lote.id">
          {{ lote.nombreLote }}
        </option>
      </select>
      <div
        class="absolute inset-y-0 right-0 flex items-center px-3 pointer-events-none text-slate-400 group-hover:text-slate-600 transition-colors"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M19 9l-7 7-7-7"
          />
        </svg>
      </div>
    </div>

    <div class="mt-2.5 px-1">
      <p v-if="loading" class="text-xs text-slate-400 italic flex items-center gap-2">
        <svg
          class="animate-spin h-3 w-3 text-cyan-600"
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
        >
          <circle
            class="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            stroke-width="4"
          ></circle>
          <path
            class="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
          ></path>
        </svg>
        Cargando lotes...
      </p>
      <p v-else class="text-[10px] font-bold text-slate-400 uppercase tracking-tight">
        {{ lotes.length }} Lotes disponibles
      </p>
    </div>
  </div>
</template>
