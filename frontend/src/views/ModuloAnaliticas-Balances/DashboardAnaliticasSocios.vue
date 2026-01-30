<template>
  <div class="space-y-6">
    <!-- Summary Ribbon -->
    <PanelResumenSocios />

    <!-- Main Chart -->
    <ChartContainer
      title="Tendencia de Socios"
      subtitle="Evolución del padrón total de socios activos"
      :loading="loading"
      :empty="movimientos.length === 0"
    >
      <template #extra>
        <div class="flex flex-col sm:flex-row sm:items-center gap-4">
          <!-- Legend -->
          <div
            class="hidden md:flex items-center gap-4 text-[10px] font-bold text-slate-400 uppercase tracking-wider"
          >
            <div class="flex items-center gap-1.5">
              <div class="w-2.5 h-2.5 rounded-full bg-[#0d9488]"></div>
              <span>Total Socios</span>
            </div>
          </div>

          <!-- Selector -->
          <div class="flex items-center gap-2 bg-slate-100/50 p-1 rounded-lg self-end sm:self-auto">
            <select
              v-model="filters.anio"
              @change="fetchData"
              class="text-[10px] font-bold bg-transparent border-none focus:ring-0 py-1 pl-2 pr-7"
            >
              <option v-for="y in years" :key="y" :value="y">{{ y }}</option>
            </select>
            <div class="w-px h-4 bg-slate-200"></div>
            <select
              v-model="filters.semestre"
              @change="fetchData"
              class="text-[10px] font-bold bg-transparent border-none focus:ring-0 py-1 pl-2 pr-7"
            >
              <option :value="null">Todo el año</option>
              <option :value="1">1º Semestre</option>
              <option :value="2">2º Semestre</option>
            </select>
          </div>
        </div>
      </template>

      <apexchart
        v-if="movimientos.length > 0"
        type="line"
        height="350"
        :options="chartOptions"
        :series="chartSeries"
      />
    </ChartContainer>

    <!-- Distribution Chart -->
    <ChartContainer
      title="Distribución Mensual"
      subtitle="Comparación de actividad por mes"
      :loading="loading"
      :empty="movimientos.length === 0"
    >
      <template #extra>
        <div
          class="flex items-center gap-4 text-[10px] font-bold text-slate-400 uppercase tracking-wider self-end sm:self-auto"
        >
          <div class="flex items-center gap-1.5">
            <div class="w-2.5 h-2.5 rounded-full bg-[#10b981]"></div>
            <span>Altas</span>
          </div>
          <div class="flex items-center gap-1.5">
            <div class="w-2.5 h-2.5 rounded-full bg-[#f43f5e]"></div>
            <span>Bajas</span>
          </div>
        </div>
      </template>

      <apexchart
        v-if="movimientos.length > 0"
        type="bar"
        height="350"
        :options="barChartOptions"
        :series="barChartSeries"
      />
    </ChartContainer>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, reactive } from 'vue'
import VueApexCharts from 'vue3-apexcharts'
import PanelResumenSocios from '../../components/ModuloAnaliticas-Balances/PanelResumenSocios.vue'
import ChartContainer from '../../components/ModuloAnaliticas-Balances/ChartContainer.vue'
import AnaliticasService from '../../services/AnaliticasService'

const apexchart = VueApexCharts
const currentYear = new Date().getFullYear()
const years = Array.from({ length: 5 }, (_, i) => currentYear - i)

const loading = ref(false)
const movimientos = ref([])

const filters = reactive({
  anio: currentYear,
  semestre: null,
})

const monthNames = [
  'Ene',
  'Feb',
  'Mar',
  'Abr',
  'May',
  'Jun',
  'Jul',
  'Ago',
  'Sep',
  'Oct',
  'Nov',
  'Dic',
]

const fetchData = async () => {
  loading.value = true
  try {
    const res = await AnaliticasService.getMovimientoSocios(filters.anio, filters.semestre)
    movimientos.value = res
  } catch (error) {
    console.error('Error al cargar datos de socios:', error)
  } finally {
    loading.value = false
  }
}

const chartOptions = computed(() => ({
  chart: {
    id: 'socios-trend',
    toolbar: { show: false },
    fontFamily: 'inherit',
    zoom: { enabled: false },
  },
  colors: ['#0d9488'],
  stroke: { curve: 'smooth', width: 4 },
  xaxis: {
    categories: movimientos.value.map((m) => monthNames[m.mes - 1]),
    axisBorder: { show: false },
    axisTicks: { show: false },
    labels: { style: { colors: '#64748b', fontWeight: 600 } },
  },
  yaxis: {
    labels: {
      style: { colors: '#64748b', fontWeight: 600 },
      formatter: (val) => Math.floor(val),
    },
  },
  tooltip: { theme: 'light' },
  legend: { show: false },
  grid: {
    borderColor: '#f1f5f9',
    xaxis: { lines: { show: true } },
  },
}))

const chartSeries = computed(() => [
  {
    name: 'Total Socios',
    type: 'line',
    data: movimientos.value.map((m) => m.cantidadDeSocios),
  },
])

const barChartOptions = computed(() => ({
  chart: {
    id: 'socios-bar',
    toolbar: { show: false },
    stacked: true,
    fontFamily: 'inherit',
  },
  plotOptions: {
    bar: {
      borderRadius: 6,
      columnWidth: '40%',
    },
  },
  colors: ['#10b981', '#f43f5e'],
  xaxis: {
    categories: movimientos.value.map((m) => monthNames[m.mes - 1]),
    axisBorder: { show: false },
    axisTicks: { show: false },
    labels: { style: { colors: '#64748b', fontWeight: 600 } },
  },
  yaxis: {
    labels: { style: { colors: '#64748b', fontWeight: 600 } },
  },
  legend: { show: false },
  tooltip: { theme: 'light' },
  grid: { borderColor: '#f1f5f9' },
}))

const barChartSeries = computed(() => [
  {
    name: 'Altas',
    data: movimientos.value.map((m) => m.cantidadAltas),
  },
  {
    name: 'Bajas',
    data: movimientos.value.map((m) => m.cantidadBajas * -1),
  },
])

onMounted(fetchData)
</script>
