<template>
  <div class="space-y-6">
    <!-- Summary Ribbon -->
    <PanelResumenIngresos />

    <!-- Monthly Trend Chart -->
    <ChartContainer
      title="Historial de Ingresos"
      subtitle="Evolución mensual por categoría"
      :loading="loadingHistorial"
      :empty="historial.length === 0"
    >
      <template #extra>
        <div class="flex flex-col sm:flex-row sm:items-center gap-4">
          <!-- Legend -->
          <div
            class="hidden md:flex items-center gap-4 text-[10px] font-bold text-slate-400 uppercase tracking-wider"
          >
            <div class="flex items-center gap-1.5">
              <div class="w-2.5 h-2.5 rounded-full bg-[#0d9488]"></div>
              <span>Total</span>
            </div>
            <div class="flex items-center gap-1.5">
              <div class="w-2.5 h-2.5 rounded-full bg-[#10b981]"></div>
              <span>Cuotas</span>
            </div>
            <div class="flex items-center gap-1.5">
              <div class="w-2.5 h-2.5 rounded-full bg-[#f59e0b]"></div>
              <span>Artículos</span>
            </div>
            <div class="flex items-center gap-1.5">
              <div class="w-2.5 h-2.5 rounded-full bg-[#8b5cf6]"></div>
              <span>Salones</span>
            </div>
          </div>

          <!-- Selector -->
          <div class="flex items-center gap-2 bg-slate-100/50 p-1 rounded-lg self-end sm:self-auto">
            <select
              v-model="historialFilters.anio"
              @change="fetchHistorial"
              class="text-[10px] font-bold bg-transparent border-none focus:ring-0 py-1 pl-2 pr-7"
            >
              <option v-for="y in years" :key="y" :value="y">{{ y }}</option>
            </select>
            <div class="w-px h-4 bg-slate-200"></div>
            <select
              v-model="historialFilters.semestre"
              @change="fetchHistorial"
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
        v-if="historial.length > 0"
        type="line"
        height="350"
        :options="trendOptions"
        :series="trendSeries"
      />
    </ChartContainer>

    <div class="w-full">
      <!-- Payment Distribution Chart -->
      <ChartContainer
        title="Formas de Cobro"
        subtitle="Distribución total por canal de pago"
        :loading="loadingDistribucion"
        :empty="!distribucion.cobrador && !distribucion.linkDePago && !distribucion.sede"
      >
        <template #extra>
          <div class="flex items-center gap-2 bg-slate-100/50 p-1 rounded-lg self-end sm:self-auto">
            <select
              v-model="distribucionFilters.anio"
              @change="fetchDistribucion"
              class="text-[10px] font-bold bg-transparent border-none focus:ring-0 py-1 pl-2 pr-7"
            >
              <option v-for="y in years" :key="y" :value="y">{{ y }}</option>
            </select>
            <div class="w-px h-4 bg-slate-200"></div>
            <select
              v-model="distribucionFilters.semestre"
              @change="fetchDistribucion"
              class="text-[10px] font-bold bg-transparent border-none focus:ring-0 py-1 pl-2 pr-7"
            >
              <option :value="null">Todo el año</option>
              <option :value="1">1º Semestre</option>
              <option :value="2">2º Semestre</option>
            </select>
          </div>
        </template>

        <apexchart
          v-if="distribucion.cobrador || distribucion.linkDePago || distribucion.sede"
          type="bar"
          height="300"
          :options="paymentBarOptions"
          :series="paymentBarSeries"
        />
      </ChartContainer>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, reactive } from 'vue'
import VueApexCharts from 'vue3-apexcharts'
import PanelResumenIngresos from '../../components/ModuloAnaliticas-Balances/PanelResumenIngresos.vue'
import ChartContainer from '../../components/ModuloAnaliticas-Balances/ChartContainer.vue'
import AnaliticasService from '../../services/AnaliticasService'

const apexchart = VueApexCharts
const currentYear = new Date().getFullYear()
const years = Array.from({ length: 5 }, (_, i) => currentYear - i)

const loadingHistorial = ref(false)
const loadingDistribucion = ref(false)

const historial = ref([])
const historialFilters = reactive({
  anio: currentYear,
  semestre: null,
})

const distribucion = ref({
  cobrador: 0,
  linkDePago: 0,
  sede: 0,
})
const distribucionFilters = reactive({
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

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const fetchHistorial = async () => {
  loadingHistorial.value = true
  try {
    const res = await AnaliticasService.getHistorialIngresos(
      historialFilters.anio,
      historialFilters.semestre,
    )
    historial.value = res
  } catch (error) {
    console.error('Error al cargar historial de ingresos:', error)
  } finally {
    loadingHistorial.value = false
  }
}

const fetchDistribucion = async () => {
  loadingDistribucion.value = true
  try {
    const res = await AnaliticasService.getDistribucionPago(
      distribucionFilters.anio,
      distribucionFilters.semestre,
    )
    distribucion.value = res
  } catch (error) {
    console.error('Error al cargar distribución de pago:', error)
  } finally {
    loadingDistribucion.value = false
  }
}

const trendOptions = computed(() => ({
  chart: {
    id: 'ingresos-trend',
    toolbar: { show: false },
    fontFamily: 'inherit',
    stacked: false,
  },
  colors: ['#0d9488', '#10b981', '#f59e0b', '#8b5cf6'],
  stroke: { curve: 'smooth', width: 4 },
  xaxis: {
    categories: historial.value.map((h) => monthNames[h.mes - 1]),
    axisBorder: { show: false },
    axisTicks: { show: false },
    labels: { style: { colors: '#64748b', fontWeight: 600 } },
  },
  yaxis: {
    labels: {
      style: { colors: '#64748b', fontWeight: 600 },
      formatter: (val) => formatCurrency(val),
    },
  },
  tooltip: { theme: 'light' },
  legend: { show: false },
  grid: { borderColor: '#f1f5f9' },
}))

const trendSeries = computed(() => [
  { name: 'Total', data: historial.value.map((h) => h.total) },
  { name: 'Cuotas', data: historial.value.map((h) => h.cuotas) },
  { name: 'Artículos', data: historial.value.map((h) => h.alquilerArticulos) },
  { name: 'Salones', data: historial.value.map((h) => h.reservasSalones) },
])

const paymentBarOptions = computed(() => ({
  chart: {
    id: 'payment-bar',
    fontFamily: 'inherit',
    toolbar: { show: false },
  },
  plotOptions: {
    bar: {
      horizontal: false,
      borderRadius: 10,
      columnWidth: '50%',
      distributed: true,
      dataLabels: { position: 'top' },
    },
  },
  colors: ['#0d9488', '#10b981', '#f59e0b'],
  xaxis: {
    categories: ['Cobrador', 'Portal de Pagos', 'Sede'],
    axisBorder: { show: false },
    axisTicks: { show: false },
    labels: { style: { colors: '#64748b', fontWeight: 600 } },
  },
  yaxis: {
    max: 100,
    labels: {
      style: { colors: '#64748b', fontWeight: 600 },
      formatter: (val) => val.toFixed(0) + '%',
    },
  },
  grid: {
    borderColor: '#f1f5f9',
    xaxis: { lines: { show: false } },
    yaxis: { lines: { show: true } },
  },
  tooltip: {
    theme: 'light',
    y: { formatter: (val) => val.toFixed(1) + '%' },
  },
  dataLabels: {
    enabled: true,
    offsetY: -25,
    style: { colors: ['#475569'], fontWeight: 700 },
    formatter: (val) => val.toFixed(1) + '%',
  },
  legend: { show: false },
}))

const paymentBarSeries = computed(() => {
  const values = [
    distribucion.value.cobrador,
    distribucion.value.linkDePago,
    distribucion.value.sede,
  ]
  const total = values.reduce((acc, curr) => acc + curr, 0)
  return [
    {
      name: 'Porcentaje de Recaudación',
      data: values.map((v) => (total > 0 ? Number(((v / total) * 100).toFixed(1)) : 0)),
    },
  ]
})

onMounted(() => {
  fetchHistorial()
  fetchDistribucion()
})
</script>
