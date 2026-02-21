<script setup>
import { onMounted, ref } from 'vue'
import CobranzasService from '../../../services/CobranzasService'

const props = defineProps({
  showToast: {
    type: Function,
    required: true,
  },
})

// Estado
const cobradores = ref([])
const loadingCobradores = ref(false)
const selectedCobrador = ref(null)
const historialData = ref(null)
const loadingHistorial = ref(false)
const period = ref({
  mes: new Date().getMonth() + 1,
  anio: new Date().getFullYear(),
})

const loadCobradores = async () => {
  loadingCobradores.value = true
  try {
    const data = await CobranzasService.verListadoDeCobradores()
    cobradores.value = data
  } catch (e) {
    props.showToast('Error al cargar cobradores: ' + e.message, 'error')
  } finally {
    loadingCobradores.value = false
  }
}

const loadHistorial = async () => {
  if (!selectedCobrador.value) return
  loadingHistorial.value = true
  try {
    const data = await CobranzasService.verHistorialDeCobradorByMes(
      selectedCobrador.value.idCobrador,
      period.value.mes,
      period.value.anio,
    )
    historialData.value = data
  } catch (e) {
    props.showToast('Error al cargar historial: ' + e.message, 'error')
  } finally {
    loadingHistorial.value = false
  }
}

const selectCobrador = (cobrador) => {
  selectedCobrador.value = cobrador
  loadHistorial()
}

const handlePeriodChange = () => {
  loadHistorial()
}

onMounted(() => {
  loadCobradores()
})
</script>

<template>
  <div class="animate-in fade-in duration-500">
    <div
      v-if="loadingCobradores || cobradores.length > 0"
      class="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden"
    >
      <div class="grid grid-cols-1 lg:grid-cols-4 min-h-[300px] lg:min-h-[400px]">
        <!-- Lateral: Listado de Cobradores -->
        <div
          class="lg:col-span-1 border-r border-slate-100 bg-slate-50/30 flex flex-col max-h-[400px] lg:max-h-none"
        >
          <div class="p-5 border-b border-slate-100 bg-white/50 backdrop-blur-sm">
            <h3
              class="text-xs font-black text-slate-400 uppercase tracking-widest flex items-center gap-2"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-4 w-4 text-cyan-500"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path
                  d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a8.113 8.113 0 00-4.03-7.062 8.054 8.054 0 00-4.193.684A4.6 4.6 0 003 14.113V17h9.93zM18.477 14.521c.384-2.625-1.455-5.147-4.135-5.607a7.977 7.977 0 00-3.328.096 8.039 8.039 0 013.355 5.8 8.016 8.016 0 01-.127 1.291h4.235v-.58z"
                />
              </svg>
              Equipo de Cobranza
            </h3>
          </div>

          <div class="flex-1 overflow-y-auto p-4 space-y-2">
            <div v-if="loadingCobradores" class="flex justify-center py-12">
              <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-cyan-600"></div>
            </div>

            <template v-else>
              <button
                v-for="cobrador in cobradores"
                :key="cobrador.idCobrador"
                @click="selectCobrador(cobrador)"
                :class="[
                  selectedCobrador?.idCobrador === cobrador.idCobrador
                    ? 'bg-cyan-600 text-white shadow-lg shadow-cyan-200 ring-1 ring-cyan-600 translate-x-1'
                    : 'bg-white text-slate-600 hover:bg-white hover:shadow-md hover:border-cyan-100 border border-slate-100',
                  'w-full text-left px-4 py-3.5 rounded-2xl font-bold transition-all duration-300 flex items-center gap-3 group',
                ]"
              >
                <div
                  :class="[
                    selectedCobrador?.idCobrador === cobrador.idCobrador
                      ? 'bg-white/20'
                      : 'bg-cyan-50 text-cyan-600 group-hover:bg-cyan-100',
                    'w-10 h-10 rounded-xl flex items-center justify-center text-sm font-black uppercase shrink-0 transition-colors',
                  ]"
                >
                  {{ cobrador.nombreCompleto.charAt(0) }}
                </div>
                <span class="truncate text-sm tracking-tight">{{ cobrador.nombreCompleto }}</span>
              </button>
            </template>
          </div>
        </div>

        <!-- Principal: Historial -->
        <div class="lg:col-span-3 flex flex-col bg-white">
          <div
            v-if="!selectedCobrador"
            class="flex-1 flex flex-col items-center justify-center p-6 text-center"
          >
            <div
              class="w-20 h-20 bg-slate-50 rounded-3xl flex items-center justify-center mb-6 shadow-inner border border-slate-100"
            >
              <svg
                class="h-10 w-10 text-slate-300"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="1.5"
                  d="M15 15l-2 5L9 9l11 4-5 2zm0 0l5 5M7.188 2.239l.777 2.897M5.136 7.965l-2.898-.777M13.95 4.05l-2.122 2.122m-5.657 5.656l-2.12 guest-2.122"
                />
              </svg>
            </div>
            <h3 class="text-xl font-bold text-slate-900 tracking-tight">Seleccione un cobrador</h3>
            <p class="max-w-[280px] mx-auto mt-2 text-slate-500 font-medium">
              Elija un cobrador de la lista lateral para visualizar su actividad y desempeño
              mensual.
            </p>
          </div>

          <div
            v-else
            class="flex flex-col h-full animate-in fade-in slide-in-from-right-4 duration-500"
          >
            <!-- Header del Historial -->
            <div
              class="p-6 border-b border-slate-100 bg-slate-50/30 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-6"
            >
              <div>
                <h3 class="text-xl font-bold text-slate-900 tracking-tight">
                  Actividad de {{ selectedCobrador.nombreCompleto }}
                </h3>
                <div class="flex items-center gap-3 mt-1.5">
                  <span
                    class="text-[10px] font-black px-2 py-0.5 bg-cyan-100 text-cyan-700 rounded-lg uppercase tracking-widest"
                    >Total recaudado</span
                  >
                  <span class="text-lg font-black text-slate-900"
                    >${{ historialData?.montoTotalCobrado?.toLocaleString() || '0' }}</span
                  >
                </div>
              </div>

              <!-- Formulario Periodo Compacto -->
              <div
                class="flex items-center gap-2 bg-white p-2 rounded-2xl border border-slate-200 shadow-sm"
              >
                <div class="relative group">
                  <select
                    v-model="period.mes"
                    @change="handlePeriodChange"
                    class="appearance-none bg-transparent text-sm font-bold text-slate-700 border-none focus:ring-0 cursor-pointer pl-4 pr-8 py-1 transition-colors hover:text-cyan-600"
                  >
                    <option v-for="m in 12" :key="m" :value="m">
                      {{ new Date(0, m - 1).toLocaleString('es-ES', { month: 'long' }) }}
                    </option>
                  </select>
                  <div
                    class="absolute right-2 inset-y-0 flex items-center pointer-events-none text-slate-400"
                  >
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M19 9l-7 7-7-7"
                      />
                    </svg>
                  </div>
                </div>
                <div class="h-5 w-px bg-slate-200"></div>
                <input
                  type="number"
                  v-model="period.anio"
                  @change="handlePeriodChange"
                  class="w-20 bg-transparent text-sm font-black text-slate-700 border-none focus:ring-0 text-center transition-colors hover:text-cyan-600"
                />
                <button
                  @click="loadHistorial"
                  class="bg-cyan-600 text-white p-2.5 rounded-xl hover:bg-cyan-700 transition-all shadow-md active:scale-95 group"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-4 w-4 group-active:rotate-180 transition-transform duration-500"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="3"
                      d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
                    />
                  </svg>
                </button>
              </div>
            </div>

            <!-- Tabla Content -->
            <div class="flex-1 overflow-x-auto">
              <div v-if="loadingHistorial" class="p-10 flex flex-col items-center">
                <div class="animate-spin rounded-full h-12 w-12 border-b-4 border-cyan-600"></div>
                <p class="mt-6 text-sm text-slate-500 font-bold uppercase tracking-widest">
                  Actualizando registros...
                </p>
              </div>

              <template v-else-if="historialData && historialData.cobrosRealizados.length > 0">
                <table class="w-full text-left border-collapse">
                  <thead>
                    <tr class="bg-slate-50/50 border-b border-slate-100">
                      <th
                        class="px-8 py-5 text-[10px] font-black text-slate-400 uppercase tracking-widest"
                      >
                        Fecha de Cobro
                      </th>
                      <th
                        class="px-8 py-5 text-[10px] font-black text-slate-400 uppercase tracking-widest"
                      >
                        Nombre del Socio
                      </th>
                      <th
                        class="px-8 py-5 text-[10px] font-black text-slate-400 uppercase tracking-widest text-right"
                      >
                        Monto Cobrado
                      </th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-50">
                    <tr
                      v-for="(cobro, idx) in historialData.cobrosRealizados"
                      :key="idx"
                      class="hover:bg-cyan-50/30 transition-colors group"
                    >
                      <td
                        class="px-8 py-5 whitespace-nowrap text-sm font-bold text-slate-500 group-hover:text-slate-700 transition-colors"
                      >
                        {{ new Date(cobro.fechaCobro).toLocaleDateString() }}
                      </td>
                      <td class="px-8 py-5 whitespace-nowrap">
                        <div
                          class="text-base font-bold text-slate-900 group-hover:text-cyan-700 transition-colors"
                        >
                          {{ cobro.nombreSocio }}
                        </div>
                      </td>
                      <td class="px-8 py-5 whitespace-nowrap text-right">
                        <span
                          class="text-sm font-black text-cyan-700 bg-cyan-50 px-4 py-2 rounded-xl group-hover:bg-cyan-600 group-hover:text-white transition-all shadow-sm group-hover:shadow-md border border-cyan-100"
                        >
                          ${{ cobro.montoCobrado.toLocaleString() }}
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </template>

              <div v-else class="p-10 text-center">
                <div
                  class="w-20 h-20 bg-slate-50 rounded-3xl border border-dashed border-slate-200 flex items-center justify-center mx-auto mb-6"
                >
                  <svg
                    class="h-10 w-10 text-slate-300"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                    />
                  </svg>
                </div>
                <h4 class="text-xl font-bold text-slate-900 tracking-tight">
                  Sin registros hallados
                </h4>
                <p class="text-slate-500 mt-2 max-w-[280px] mx-auto font-medium">
                  No se han registrado cobranzas para este periodo. Intente seleccionando otro mes o
                  año.
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div
      v-else
      class="bg-white border-2 border-dashed border-slate-200 rounded-3xl p-10 text-center animate-in zoom-in duration-300 shadow-sm"
    >
      <div
        class="w-20 h-20 bg-slate-50 text-slate-200 rounded-3xl flex items-center justify-center mx-auto mb-6 shadow-inner"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-10 w-10"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="1.5"
            d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
          />
        </svg>
      </div>
      <h3 class="text-xl font-black text-slate-900 mb-2 tracking-tight">
        No hay cobradores registrados
      </h3>
      <p class="text-slate-500 max-w-sm mx-auto text-base font-medium leading-relaxed">
        Todavía no se han asignado cobradores al sistema. Comience registrando uno nuevo.
      </p>
    </div>
  </div>
</template>
