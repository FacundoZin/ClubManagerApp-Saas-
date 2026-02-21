<script setup>
import { ref, computed } from 'vue'

const props = defineProps({
  socio: {
    type: Object,
    required: true,
  },
})

const emit = defineEmits(['pay', 'edit', 'delete'])

// Estado local para periodos seleccionados
const selectedPeriods = ref([])

// Inicializar con todos los periodos seleccionados por defecto (solo los adeudados)
selectedPeriods.value = [...(props.socio.periodosAdeudados || [])]

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

  // Filtrar si ya están en periodosAdeudados
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
}

const isSelected = (period) => {
  return selectedPeriods.value.some((p) => p.anio === period.anio && p.semestre === period.semestre)
}

const handlePay = () => {
  if (selectedPeriods.value.length === 0) {
    alert('Debe seleccionar al menos un periodo para cobrar.')
    return
  }
  emit('pay', { socio: props.socio, periodos: selectedPeriods.value })
}
</script>

<template>
  <div
    class="bg-white rounded-lg border border-slate-200 shadow-sm hover:shadow-md transition-shadow duration-200 overflow-hidden flex flex-col h-full"
  >
    <div class="p-3 flex-grow">
      <div class="flex flex-col sm:flex-row justify-between items-start gap-2">
        <div class="w-full">
          <div class="flex items-center justify-between gap-2">
            <h3 class="text-base font-bold text-slate-900 truncate">
              {{ socio.nombre }} {{ socio.apellido }}
            </h3>
            <span
              class="inline-flex shrink-0 items-center rounded-full px-2.5 py-0.5 text-xs font-semibold"
              :class="
                socio.periodosAdeudados?.length > 0
                  ? 'bg-red-100 text-red-800'
                  : 'bg-green-100 text-green-800'
              "
            >
              {{ socio.periodosAdeudados?.length > 0 ? 'Deudor' : 'Al día' }}
            </span>
          </div>
          <p class="text-sm text-slate-500 font-medium mt-1">DNI: {{ socio.dni }}</p>
        </div>
      </div>

      <div class="mt-2 space-y-2">
        <!-- Selección de Periodos -->
        <div v-if="socio.periodosAdeudados?.length > 0" class="border-t border-slate-100 pt-3 mt-3">
          <p class="text-xs font-bold text-slate-500 uppercase tracking-wider mb-2">
            Periodos adeudados:
          </p>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
            <div
              v-for="(periodo, idx) in socio.periodosAdeudados"
              :key="idx"
              @click="togglePeriod(periodo)"
              class="flex items-center p-2 rounded-md border cursor-pointer transition-all text-[11px] sm:text-xs"
              :class="
                isSelected(periodo)
                  ? 'bg-green-50 border-green-200 text-green-700'
                  : 'bg-slate-50 border-slate-200 text-slate-500'
              "
            >
              <input
                type="checkbox"
                :checked="isSelected(periodo)"
                class="h-3 w-3 rounded border-slate-300 text-green-600 focus:ring-green-500 mr-2"
                @click.stop
                @change="togglePeriod(periodo)"
              />
              <span class="font-medium">{{ periodo.anio }} - {{ periodo.semestre }}º Sem</span>
            </div>
          </div>
        </div>

        <!-- Periodos por Adelantado -->
        <div class="border-t border-slate-100 pt-3 mt-3">
          <p class="text-xs font-bold text-slate-500 uppercase tracking-wider mb-2">
            Pago por adelantado:
          </p>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
            <div
              v-for="(periodo, idx) in futurePeriods"
              :key="'f-' + idx"
              @click="togglePeriod(periodo)"
              class="flex items-center p-2 rounded-md border cursor-pointer transition-all text-[11px] sm:text-xs"
              :class="
                isSelected(periodo)
                  ? 'bg-green-50 border-green-200 text-green-700'
                  : 'bg-slate-50 border-slate-200 text-slate-500'
              "
            >
              <input
                type="checkbox"
                :checked="isSelected(periodo)"
                class="h-3 w-3 rounded border-slate-300 text-green-600 focus:ring-green-500 mr-2"
                @click.stop
                @change="togglePeriod(periodo)"
              />
              <span class="font-medium">{{ periodo.anio }} - {{ periodo.semestre }}º Sem</span>
            </div>
          </div>
          <p
            v-if="selectedPeriods.length === 0"
            class="text-[10px] text-red-500 mt-1 font-medium italic"
          >
            Seleccione al menos un periodo
          </p>
        </div>

        <div class="flex items-center text-sm text-slate-600 border-t border-slate-100 pt-3">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4 mr-2 text-slate-400"
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
        <div class="flex items-center text-sm text-slate-600">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-4 w-4 mr-2 text-slate-400"
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
          {{ socio.direcccion || socio.localidad || 'Sin dirección' }}
        </div>
      </div>
    </div>

    <div class="bg-slate-50 px-3 py-2 border-t border-slate-100 flex flex-col gap-2">
      <button
        @click="handlePay"
        :disabled="selectedPeriods.length === 0"
        class="w-full inline-flex justify-center items-center rounded-lg bg-green-600 px-3 py-2 text-sm font-semibold text-white hover:bg-green-500 shadow-sm transition-all disabled:opacity-50 disabled:bg-slate-400"
      >
        Registrar pago ({{ selectedPeriods.length }})
      </button>
      <div class="flex gap-2">
        <button
          @click="$emit('edit', socio)"
          class="flex-1 inline-flex justify-center items-center rounded-lg bg-white px-3 py-2 text-sm font-semibold text-slate-700 shadow-sm ring-1 ring-inset ring-slate-300 hover:bg-slate-50 transition-all"
        >
          Editar socio
        </button>
        <button
          @click="$emit('delete', socio)"
          class="flex-1 inline-flex justify-center items-center rounded-lg bg-white px-3 py-2 text-sm font-semibold text-red-600 shadow-sm ring-1 ring-inset ring-red-200 hover:bg-red-50 transition-all"
        >
          Dar de baja
        </button>
      </div>
    </div>
  </div>
</template>
