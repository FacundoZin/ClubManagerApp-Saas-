<script setup>
defineProps({
  socio: {
    type: Object,
    required: true,
  },
})

defineEmits(['edit', 'delete', 'view'])
</script>

<template>
  <div
    class="bg-white rounded-2xl border border-slate-200 shadow-sm hover:shadow-md transition-all duration-300 overflow-hidden"
  >
    <div class="p-4">
      <div class="flex justify-between items-start">
        <div>
          <h3 class="text-base font-bold text-slate-900 line-clamp-1 content-center">
            {{ socio.nombre }} {{ socio.apellido }}
          </h3>
        </div>
        <span
          class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-bold"
          :class="socio.adeudaCuotas ? 'bg-red-100 text-red-800' : 'bg-green-100 text-green-800'"
        >
          <span
            class="w-1.5 h-1.5 rounded-full mr-1.5"
            :class="socio.adeudaCuotas ? 'bg-red-500' : 'bg-green-500'"
          ></span>
          {{ socio.adeudaCuotas ? 'Deudor' : 'Al día' }}
        </span>
      </div>

      <div class="mt-3 space-y-1.5">
        <div class="flex items-center text-xs text-slate-600">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-3.5 w-3.5 mr-2 text-slate-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
            />
          </svg>
          DNI: {{ socio.dni }}
        </div>
        <div class="flex items-center text-xs text-slate-600">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-3.5 w-3.5 mr-2 text-slate-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"
            />
          </svg>
          {{ socio.telefono || 'Sin teléfono' }}
        </div>
        <div class="flex items-center text-xs text-slate-600">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-3.5 w-3.5 mr-2 text-slate-400"
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
          <span class="truncate">
            {{
              socio.direcccion && socio.localidad
                ? socio.direcccion + ' (' + socio.localidad + ')'
                : 'Sin localidad registrada'
            }}
          </span>
        </div>
        <div v-if="socio.nombreLote" class="flex items-center text-xs text-slate-600">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-3.5 w-3.5 mr-2 text-slate-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"
            />
          </svg>
          Lote: {{ socio.nombreLote }}
        </div>
      </div>
    </div>

    <div class="bg-slate-50 px-4 py-2 border-t border-slate-100 flex justify-end gap-1">
      <button
        @click="$emit('view', socio)"
        class="text-[11px] font-bold text-slate-600 hover:text-blue-600 px-2 py-1 rounded-lg hover:bg-slate-200 transition-colors inline-flex items-center gap-1"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="w-3 h-3"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
          />
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"
          />
        </svg>
        Ver
      </button>
      <button
        @click="$emit('edit', socio)"
        class="text-[11px] font-bold text-blue-600 hover:text-blue-800 px-2 py-1 rounded-lg hover:bg-blue-50 transition-colors inline-flex items-center gap-1"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="w-3 h-3"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
          />
        </svg>
        Editar
      </button>
      <button
        @click="$emit('delete', socio)"
        class="text-[11px] font-bold text-red-600 hover:text-red-800 px-2 py-1 rounded-lg hover:bg-red-50 transition-colors inline-flex items-center gap-1"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="w-3 h-3"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
          />
        </svg>
        Baja
      </button>
    </div>
  </div>
</template>
