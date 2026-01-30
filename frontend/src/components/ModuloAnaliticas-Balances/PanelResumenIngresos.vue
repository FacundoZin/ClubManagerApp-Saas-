<template>
  <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden relative">
    <!-- Header with Selector -->
    <div
      class="px-4 py-3 border-b border-slate-200 bg-slate-50 flex flex-col sm:flex-row sm:items-center justify-between gap-4"
    >
      <div class="flex items-center gap-2">
        <div
          class="w-8 h-8 rounded-lg bg-teal-600/10 flex items-center justify-center border border-teal-600/10"
        >
          <CalendarIcon class="h-4 w-4 text-teal-600" />
        </div>
        <div>
          <h3 class="text-xs font-bold text-slate-700">Período de Análisis</h3>
          <p class="text-[10px] text-slate-500 font-medium">Filtrar estadísticas por fecha</p>
        </div>
      </div>

      <div class="flex flex-wrap items-center gap-2">
        <select
          v-model="filters.anio"
          @change="fetchData"
          class="text-xs font-bold bg-white border-slate-300 rounded-lg focus:ring-teal-500 focus:border-teal-500 py-1.5 pl-2 pr-8 transition-all shadow-sm"
        >
          <option v-for="y in years" :key="y" :value="y">{{ y }}</option>
        </select>

        <select
          v-model="filters.periodType"
          @change="fetchData"
          class="text-xs font-bold bg-white border-slate-300 rounded-lg focus:ring-teal-500 focus:border-teal-500 py-1.5 pl-2 pr-8 transition-all shadow-sm"
        >
          <option value="anual">Anual</option>
          <option value="semestral">Semestral</option>
          <option value="mensual">Mensual</option>
        </select>

        <select
          v-if="filters.periodType === 'semestral'"
          v-model="filters.semestre"
          @change="fetchData"
          class="text-xs font-bold bg-white border-slate-300 rounded-lg focus:ring-teal-500 focus:border-teal-500 py-1.5 pl-2 pr-8 transition-all shadow-sm"
        >
          <option :value="1">1º Sem</option>
          <option :value="2">2º Sem</option>
        </select>

        <select
          v-if="filters.periodType === 'mensual'"
          v-model="filters.mes"
          @change="fetchData"
          class="text-xs font-bold bg-white border-slate-300 rounded-lg focus:ring-teal-500 focus:border-teal-500 py-1.5 pl-2 pr-8 transition-all shadow-sm"
        >
          <option v-for="(name, index) in monthNames" :key="index" :value="index + 1">
            {{ name }}
          </option>
        </select>
      </div>
    </div>

    <!-- Loading Overlay -->
    <div
      v-if="loading"
      class="absolute inset-0 bg-white/60 z-20 flex items-center justify-center backdrop-blur-[1px]"
    >
      <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-teal-600"></div>
    </div>

    <div
      class="flex flex-col md:flex-row divide-y md:divide-y-0 md:divide-x divide-slate-100 relative z-10"
    >
      <!-- Total General -->
      <div
        class="flex-1 p-4 flex items-center gap-3.5 group transition-all duration-300 hover:bg-slate-50/80 cursor-default"
      >
        <div
          class="flex-shrink-0 w-10 h-10 rounded-xl bg-teal-600 flex items-center justify-center transition-all duration-300 shadow-sm shadow-teal-100"
        >
          <CurrencyDollarIcon class="h-5 w-5 text-white" />
        </div>
        <div class="min-w-0">
          <p class="text-[9px] font-bold text-slate-400 uppercase tracking-[0.1em] mb-0.5 truncate">
            Ingresos Totales
          </p>
          <div class="flex items-baseline gap-1.5 line-clamp-1">
            <span class="text-lg font-black text-slate-900 tracking-tight truncate">
              {{ formatCurrency(data.totalGeneral) }}
            </span>
          </div>
        </div>
      </div>

      <!-- Cuotas -->
      <div
        class="flex-1 p-4 flex items-center gap-3.5 group transition-all duration-300 hover:bg-slate-50/80 cursor-default"
      >
        <div
          class="flex-shrink-0 w-10 h-10 rounded-xl bg-sky-500 flex items-center justify-center transition-all duration-300 shadow-sm shadow-sky-100"
        >
          <CreditCardIcon class="h-5 w-5 text-white" />
        </div>
        <div class="min-w-0">
          <p class="text-[9px] font-bold text-slate-400 uppercase tracking-[0.1em] mb-0.5 truncate">
            Cobro de Cuotas
          </p>
          <div class="flex items-baseline gap-1.5 line-clamp-1">
            <span class="text-lg font-black text-slate-900 tracking-tight truncate">
              {{ formatCurrency(data.totalCuotas) }}
            </span>
          </div>
        </div>
      </div>

      <!-- Artículos -->
      <div
        class="flex-1 p-4 flex items-center gap-3.5 group transition-all duration-300 hover:bg-slate-50/80 cursor-default"
      >
        <div
          class="flex-shrink-0 w-10 h-10 rounded-xl bg-violet-600 flex items-center justify-center transition-all duration-300 shadow-sm shadow-violet-100"
        >
          <ShoppingBagIcon class="h-5 w-5 text-white" />
        </div>
        <div class="min-w-0">
          <p class="text-[9px] font-bold text-slate-400 uppercase tracking-[0.1em] mb-0.5 truncate">
            Alquiler Artículos
          </p>
          <div class="flex items-baseline gap-1.5 line-clamp-1">
            <span class="text-lg font-black text-slate-900 tracking-tight truncate">
              {{ formatCurrency(data.totalAlquilerArticulos) }}
            </span>
          </div>
        </div>
      </div>

      <!-- Salones -->
      <div
        class="flex-1 p-4 flex items-center gap-3.5 group transition-all duration-300 hover:bg-slate-50/80 cursor-default"
      >
        <div
          class="flex-shrink-0 w-10 h-10 rounded-xl bg-amber-500 flex items-center justify-center transition-all duration-300 shadow-sm shadow-amber-100"
        >
          <HomeIcon class="h-5 w-5 text-white" />
        </div>
        <div class="min-w-0">
          <p class="text-[9px] font-bold text-slate-400 uppercase tracking-[0.1em] mb-0.5 truncate">
            Reservas Salones
          </p>
          <div class="flex items-baseline gap-1.5 line-clamp-1">
            <span class="text-lg font-black text-slate-900 tracking-tight truncate">
              {{ formatCurrency(data.totalReservasSalones) }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import {
  CurrencyDollarIcon,
  CreditCardIcon,
  ShoppingBagIcon,
  HomeIcon,
  CalendarIcon,
} from '@heroicons/vue/24/outline'
import AnaliticasService from '../../services/AnaliticasService'

const loading = ref(false)
const data = ref({
  totalGeneral: 0,
  totalCuotas: 0,
  totalAlquilerArticulos: 0,
  totalReservasSalones: 0,
})

const currentYear = new Date().getFullYear()
const years = Array.from({ length: 5 }, (_, i) => currentYear - i)
const monthNames = [
  'Enero',
  'Febrero',
  'Marzo',
  'Abril',
  'Mayo',
  'Junio',
  'Julio',
  'Agosto',
  'Septiembre',
  'Octubre',
  'Noviembre',
  'Diciembre',
]

const filters = reactive({
  anio: currentYear,
  periodType: 'anual',
  mes: new Date().getMonth() + 1,
  semestre: new Date().getMonth() < 6 ? 1 : 2,
})

const fetchData = async () => {
  loading.value = true
  try {
    const res = await AnaliticasService.getResumenIngresos(
      filters.anio,
      filters.periodType === 'mensual' ? filters.mes : null,
      filters.periodType === 'semestral' ? filters.semestre : null,
    )
    data.value = res
  } catch (error) {
    console.error('Error al cargar resumen de ingresos:', error)
  } finally {
    loading.value = false
  }
}

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
    maximumFractionDigits: 0,
  }).format(value)
}

onMounted(fetchData)
</script>
