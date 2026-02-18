<script setup>

const props = defineProps({
  viaje: {
    type: Object,
    required: true,
  },
  isExpanded: {
    type: Boolean,
    default: false,
  },
  variantes: {
    type: Array,
    default: () => [],
  },
  isLoadingVariantes: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['view', 'toggle-variantes', 'create-variante'])

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString + 'T00:00:00').toLocaleDateString('es-AR')
}

const getRegimenLabel = (value) => {
  const regimenLabels = { 0: 'Media Pensión', 1: 'Pensión Completa' }
  return regimenLabels[value] || 'N/A'
}
</script>

<template>
  <div class="bg-white rounded-2xl border border-slate-200 shadow-sm hover:shadow-md transition-shadow overflow-hidden">
    <div class="p-5">
      <div class="flex justify-between items-start mb-4">
        <div>
          <h3 class="text-lg font-bold text-slate-900 group-hover:text-teal-600 transition-colors">
            {{ viaje.titulo }}
          </h3>
          <div class="flex items-center text-slate-500 text-sm mt-1">
            <svg class="w-4 h-4 mr-1.5 text-teal-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            Salida: {{ formatDate(viaje.fechasalida) }}
          </div>
        </div>
        <div class="bg-teal-50 text-teal-700 text-xs font-semibold px-2.5 py-1 rounded-full border border-teal-100">
          {{ viaje.dias }} Días / {{ viaje.noches }} Noches
        </div>
      </div>

      <div class="grid grid-cols-2 gap-4 mb-6">
        <div class="p-3 bg-slate-50 rounded-xl border border-slate-100">
          <p class="text-[10px] uppercase tracking-wider font-bold text-slate-400 mb-0.5">
            Valor Base
          </p>
          <p class="text-sm font-bold text-slate-700">{{ formatCurrency(viaje.valorBase) }}</p>
        </div>
        <div class="p-3 bg-slate-50 rounded-xl border border-slate-100">
          <p class="text-[10px] uppercase tracking-wider font-bold text-slate-400 mb-0.5">
            % Comisión
          </p>
          <p class="text-sm font-bold text-slate-700">{{ viaje.porcentajeComision }}%</p>
        </div>
      </div>

      <div class="flex flex-wrap gap-2 pt-4 border-t border-slate-100">
        <button @click="$emit('toggle-variantes')"
          class="flex-1 inline-flex items-center justify-center px-4 py-2 text-sm font-semibold rounded-xl transition-all"
          :class="isExpanded
            ? 'bg-slate-100 text-slate-700'
            : 'bg-teal-50 text-teal-700 hover:bg-teal-100'
            ">
          <svg class="w-4 h-4 mr-2 transition-transform duration-300" :class="{ 'rotate-180': isExpanded }" fill="none"
            viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
          </svg>
          {{ isExpanded ? 'Cerrar' : 'Ver' }} variantes
        </button>

        <button @click="$emit('create-variante')"
          class="inline-flex items-center justify-center p-2 text-teal-600 bg-teal-50 hover:bg-teal-100 rounded-xl transition-all border border-teal-100"
          title="Agregar Variante">
          <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
          </svg>
        </button>

        <button @click="$emit('view')"
          class="inline-flex items-center justify-center p-2 text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-xl transition-all border border-blue-100"
          title="Ver Detalle Completo">
          <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
          </svg>
        </button>
      </div>
    </div>

    <!-- Expanded Section (Variantes) -->
    <div v-show="isExpanded" class="bg-slate-50 border-t border-slate-100 transition-all duration-300">
      <div v-if="isLoadingVariantes" class="p-8 flex justify-center">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-teal-600"></div>
      </div>

      <div v-else-if="variantes.length === 0" class="p-6 text-center">
        <p class="text-sm text-slate-500 italic">Este viaje aún no tiene variantes definidas.</p>
        <button @click="$emit('create-variante')" class="mt-2 text-xs font-bold text-teal-600 hover:text-teal-700">
          + Agregar la primera variante
        </button>
      </div>

      <div v-else class="p-4 flex flex-col gap-3">
        <div v-for="variante in variantes" :key="variante.id"
          class="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm hover:border-teal-200 hover:shadow-md transition-all group/var">
          <div class="flex justify-between items-start mb-4 gap-4">
            <h4 class="text-sm font-bold text-slate-900 leading-tight group-hover/var:text-teal-600 transition-colors">
              {{ variante.nombreVariante }}
            </h4>
            <span
              class="shrink-0 text-[10px] bg-slate-100 text-slate-600 px-2 py-1 rounded-lg font-bold border border-slate-200">
              {{ variante.tipoDeButaca }}
            </span>
          </div>

          <div class="grid grid-cols-2 gap-3 mb-4">
            <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-100 flex flex-col">
              <span class="text-[9px] uppercase tracking-wider font-bold text-slate-400 mb-1">Precio Final</span>
              <span class="text-sm font-bold text-teal-600">
                {{ formatCurrency(variante.valorViaje) }}
              </span>
            </div>
            <div class="p-2.5 bg-slate-50 rounded-xl border border-slate-100 flex flex-col">
              <span class="text-[9px] uppercase tracking-wider font-bold text-slate-400 mb-1">Seña</span>
              <span class="text-sm font-bold text-slate-700">
                {{ formatCurrency(variante.valorSeña) }}
              </span>
            </div>
          </div>

          <div class="flex items-center text-[10px] font-bold text-slate-400 uppercase tracking-widest">
            <svg class="w-3.5 h-3.5 mr-2 text-teal-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
            </svg>
            {{ getRegimenLabel(variante.regimen) }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
