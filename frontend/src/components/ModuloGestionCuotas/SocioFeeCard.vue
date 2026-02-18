<script setup>
import { computed, onMounted, ref, watch } from 'vue'

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
watch(
  () => props.socio.id,
  () => {
    selectedPeriods.value = [...(props.socio.periodosAdeudados || [])]
    emit('update-selection', selectedPeriods.value)
  },
)

const futurePeriods = computed(() => {
  const now = new Date()
  const currentYear = now.getFullYear()
  const currentMonth = now.getMonth() + 1 // 1-12
  const currentSemester = currentMonth <= 6 ? 1 : 2

  let p1, p2
  if (currentSemester === 1) {
    p1 = { anio: currentYear, semestre: 2 }
    p2 = { anio: currentYear + 1, semestre: 1 }
  } else {
    p1 = { anio: currentYear + 1, semestre: 1 }
    p2 = { anio: currentYear + 1, semestre: 2 }
  }

  // Filtrar si ya están en periodosAdeudados (por si acaso el backend los incluyó)
  const deudas = props.socio.periodosAdeudados || []
  return [p1, p2].filter(
    (fp) => !deudas.some((d) => d.anio === fp.anio && d.semestre === fp.semestre),
  )
})

const togglePeriod = (period) => {
  const index = selectedPeriods.value.findIndex(
    (p) => p.anio === period.anio && p.semestre === period.semestre,
  )
  if (index > -1) {
    selectedPeriods.value.splice(index, 1)
  } else {
    selectedPeriods.value.push(period)
  }
  emit('update-selection', selectedPeriods.value)
}

const isSelected = (period) => {
  return selectedPeriods.value.some((p) => p.anio === period.anio && p.semestre === period.semestre)
}
</script>

<template>
  <div
    class="bg-white rounded-2xl border border-slate-200 shadow-sm hover:shadow-md transition-all duration-300 overflow-hidden">
    <div class="p-5">
      <div class="flex justify-between items-start">
        <div>
          <h3 class="text-lg font-bold text-slate-900">{{ socio.nombre }} {{ socio.apellido }}</h3>
          <p class="text-sm text-slate-500 font-medium mt-1">DNI: {{ socio.dni }}</p>
        </div>
        <span
          class="inline-flex items-center rounded-full px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider border"
          :class="socio.adeudaCuotas ? 'bg-red-50 text-red-700 border-red-200' : 'bg-emerald-50 text-emerald-700 border-emerald-200'">
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
          <p class="text-xs font-bold text-slate-500 uppercase tracking-wider mb-2">
            Seleccionar periodos adeudados:
          </p>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
            <div v-for="(periodo, idx) in socio.periodosAdeudados" :key="idx" @click="togglePeriod(periodo)"
              class="flex items-center p-3 rounded-xl border cursor-pointer transition-all text-sm" :class="isSelected(periodo)
                  ? 'bg-emerald-50 border-emerald-200 text-emerald-700 shadow-sm'
                  : 'bg-slate-50 border-slate-200 text-slate-500 hover:border-slate-300'
                ">
              <input type="checkbox" :checked="isSelected(periodo)"
                class="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500 mr-2" @click.stop
                @change="togglePeriod(periodo)" />
              <span class="font-bold">{{ periodo.anio }} - {{ periodo.semestre }}º Semestre</span>
            </div>
          </div>
        </div>

        <!-- Periodos por Adelantado -->
        <div class="border-t border-slate-100 pt-3">
          <p class="text-xs font-bold text-slate-500 uppercase tracking-wider mb-2">
            Pago por adelantado:
          </p>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
            <div v-for="(periodo, idx) in futurePeriods" :key="'f-' + idx" @click="togglePeriod(periodo)"
              class="flex items-center p-3 rounded-xl border cursor-pointer transition-all text-sm" :class="isSelected(periodo)
                  ? 'bg-emerald-50 border-emerald-200 text-emerald-700 shadow-sm'
                  : 'bg-slate-50 border-slate-200 text-slate-500 hover:border-slate-300'
                ">
              <input type="checkbox" :checked="isSelected(periodo)"
                class="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500 mr-2" @click.stop
                @change="togglePeriod(periodo)" />
              <span class="font-bold">{{ periodo.anio }} - {{ periodo.semestre }}º Semestre</span>
            </div>
          </div>
        </div>

        <div class="flex items-center text-sm font-bold"
          :class="socio.adeudaCuotas ? 'text-red-600' : 'text-emerald-600'">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          {{
            socio.adeudaCuotas
              ? `Pendiente de pago (${socio.periodosAdeudados?.length || 0} periodos)`
              : 'Socio al día'
          }}
        </div>
      </div>
    </div>

    <div class="bg-slate-50 px-5 py-3.5 border-t border-slate-100 flex justify-end">
      <button @click="$emit('view', socio)"
        class="text-xs font-bold text-slate-600 hover:text-emerald-600 px-3 py-1.5 rounded-xl hover:bg-emerald-50 transition-all inline-flex items-center gap-1.5">
        <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
        </svg>
        Ver legajo completo
      </button>
    </div>
  </div>

</template>
