<script setup>
defineProps({
  alquiler: {
    type: Object,
    required: true,
  },
})

defineEmits(['view-detail'])
</script>

<template>
  <div
    class="bg-white rounded-2xl border border-slate-200 shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 relative overflow-hidden flex flex-col h-full group"
  >
    <!-- Encabezado: Nombre y Estado -->
    <div class="p-6 pb-0 flex justify-between items-start mb-5">
      <h3
        class="text-xl font-bold text-slate-900 leading-tight group-hover:text-blue-700 transition-colors duration-300"
      >
        {{ alquiler.nombreSocio }} {{ alquiler.apellidoSocio }}
      </h3>
      <div v-if="alquiler.estaAlDia !== undefined">
        <span
          class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold"
          :class="
            alquiler.estaAlDia ? 'bg-emerald-100 text-emerald-800' : 'bg-rose-100 text-rose-800'
          "
        >
          <span
            class="w-1.5 h-1.5 rounded-full mr-1.5"
            :class="alquiler.estaAlDia ? 'bg-emerald-500' : 'bg-rose-500'"
          ></span>
          {{ alquiler.estaAlDia ? 'Al día' : 'Deuda' }}
        </span>
      </div>
    </div>

    <!-- Bloque de Información Unificado -->
    <div class="px-6 space-y-3 mb-6 flex-1">
      <!-- Fecha de Inicio -->
      <div class="flex items-center text-sm text-slate-600">
        <div class="p-2 rounded-lg bg-slate-50 mr-3 text-slate-400">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4"
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
        <span class="font-bold text-slate-400 uppercase text-[10px] tracking-widest mr-2"
          >Iniciado:</span
        >
        <span class="font-medium text-slate-700">{{ alquiler.fechaAlquiler }}</span>
      </div>

      <!-- DNI -->
      <div class="flex items-center text-sm text-slate-600">
        <div class="p-2 rounded-lg bg-slate-50 mr-3 text-slate-400">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm5 3h1a2 2 0 012 2v1H2v-1a2 2 0 012-2h1"
            />
          </svg>
        </div>
        <span class="font-bold text-slate-400 uppercase text-[10px] tracking-widest mr-2"
          >DNI:</span
        >
        <span class="font-medium text-slate-700">{{ alquiler.dniSocio }}</span>
      </div>

      <!-- Dirección -->
      <div v-if="alquiler.direccionSocio" class="flex items-start text-sm text-slate-600">
        <div class="p-2 rounded-lg bg-slate-50 mr-3 text-slate-400">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4"
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
        <span class="font-medium text-slate-700 text-sm mt-1"
          >{{ alquiler.direccionSocio
          }}{{ alquiler.localidadSocio ? `, ${alquiler.localidadSocio}` : '' }}</span
        >
      </div>

      <!-- Teléfono -->
      <div v-if="alquiler.telefonoSocio" class="flex items-center text-sm text-slate-600">
        <div class="p-2 rounded-lg bg-slate-50 mr-3 text-slate-400">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4"
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
        </div>
        <span class="font-medium text-slate-700">{{ alquiler.telefonoSocio }}</span>
      </div>
    </div>

    <!-- Botón de Acción -->
    <div class="bg-slate-50 px-6 py-4 border-t border-slate-100 mt-auto">
      <button
        @click="$emit('view-detail', alquiler.id)"
        class="w-full inline-flex justify-center items-center px-4 py-3 text-sm font-bold text-indigo-700 bg-indigo-50 rounded-xl hover:bg-indigo-600 hover:text-white transition-all duration-300 shadow-sm border border-indigo-100"
      >
        Ver Alquiler Completo
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-4 w-4 ml-2 group-hover:translate-x-1 transition-transform"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M14 5l7 7m0 0l-7 7m7-7H3"
          />
        </svg>
      </button>
    </div>
  </div>
</template>
