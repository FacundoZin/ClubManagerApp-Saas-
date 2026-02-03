<script setup>
import { onMounted, ref, watch } from 'vue'

const props = defineProps({
  socio: {
    type: Object,
    required: true,
  },
})

const emit = defineEmits(['view', 'update-selection'])

const selectedPeriods = ref([])

// Inicializar con todos por defecto
onMounted(() => {
  if (props.socio.periodosAdeudados) {
    selectedPeriods.value = [...props.socio.periodosAdeudados]
    emit('update-selection', selectedPeriods.value)
  }
})

// Vigilamos si cambia el socio para resetear selección
watch(() => props.socio.id, () => {
  selectedPeriods.value = [...(props.socio.periodosAdeudados || [])]
  emit('update-selection', selectedPeriods.value)
})

const togglePeriod = (period) => {
  const index = selectedPeriods.value.findIndex(p => p.anio === period.anio && p.semestre === period.semestre)
  if (index > -1) {
    selectedPeriods.value.splice(index, 1)
  } else {
    selectedPeriods.value.push(period)
  }
  emit('update-selection', selectedPeriods.value)
}

const isSelected = (period) => {
  return selectedPeriods.value.some(p => p.anio === period.anio && p.semestre === period.semestre)
}
</script>

<template>
  <div
    class="bg-white rounded-lg border border-slate-200 shadow-sm hover:shadow-md transition-shadow duration-200 overflow-hidden">
    <div class="p-5">
      <div class="flex justify-between items-start">
        <div>
          <h3 class="text-lg font-bold text-slate-900">{{ socio.nombre }} {{ socio.apellido }}</h3>
          <p class="text-sm text-slate-500 font-medium mt-1">DNI: {{ socio.dni }}</p>
        </div>
        <span class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
          :class="socio.adeudaCuotas ? 'bg-red-100 text-red-800' : 'bg-green-100 text-green-800'">
          {{ socio.adeudaCuotas ? 'Deudor' : 'Al día' }}
        </span>
      </div>

      <div class="mt-4 space-y-4">
        <div v-if="socio.localidad" class="flex items-center text-sm text-slate-600">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2 text-slate-400" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
          </svg>
          {{ socio.localidad }}
        </div>

        <!-- Lista de Periodos Adeudados -->
        <div v-if="socio.periodosAdeudados && socio.periodosAdeudados.length > 0"
          class="border-t border-slate-100 pt-3">
          <p class="text-xs font-bold text-slate-500 uppercase tracking-wider mb-2">Seleccionar periodos a pagar:</p>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
            <div v-for="(periodo, idx) in socio.periodosAdeudados" :key="idx" @click="togglePeriod(periodo)"
              class="flex items-center p-2 rounded-md border cursor-pointer transition-all text-sm"
              :class="isSelected(periodo) ? 'bg-indigo-50 border-indigo-200 text-indigo-700' : 'bg-slate-50 border-slate-200 text-slate-500'">
              <input type="checkbox" :checked="isSelected(periodo)"
                class="h-4 w-4 rounded border-slate-300 text-indigo-600 focus:ring-indigo-500 mr-2" @click.stop
                @change="togglePeriod(periodo)" />
              <span class="font-medium">{{ periodo.anio }} - {{ periodo.semestre }}º Semestre</span>
            </div>
          </div>
        </div>

        <div class="flex items-center text-sm font-medium"
          :class="socio.adeudaCuotas ? 'text-red-600' : 'text-green-600'">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          {{
            socio.adeudaCuotas ? `Pendiente de pago (${socio.periodosAdeudados?.length || 0} periodos)` : 'Socio aldía'
          }}
        </div>
      </div>
    </div>

    <div class="bg-slate-50 px-5 py-3 border-t border-slate-100 flex justify-end">
      <button @click="$emit('view', socio)"
        class="text-xs font-medium text-slate-600 hover:text-blue-600 px-3 py-1 rounded hover:bg-slate-200 transition-colors">
        Ver legajo completo
      </button>
    </div>
  </div>
</template>
