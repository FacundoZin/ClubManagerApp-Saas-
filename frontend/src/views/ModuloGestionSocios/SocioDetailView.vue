<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import SociosService from '../../services/SociosService'

const route = useRoute()
const router = useRouter()
const socioId = route.params.id

const socio = ref(null)
const isLoading = ref(true)
const error = ref('')

const fetchSocioDetails = async () => {
  isLoading.value = true
  try {
    const data = await SociosService.getFullDetail(socioId)
    // Ordenar historial por año y semestre descendente para ver lo más reciente primero
    if (data.historialCuotas) {
      data.historialCuotas.sort((a, b) => {
        if (a.anio !== b.anio) return b.anio - a.anio
        return b.semestre - a.semestre
      })
    }
    socio.value = data
  } catch (err) {
    error.value = err.message
  } finally {
    isLoading.value = false
  }
}

const goBack = () => {
  router.back()
}

onMounted(fetchSocioDetails)

const formatDate = (dateString) => {
  if (!dateString) return 'Pendiente de Pago'
  return new Date(dateString).toLocaleDateString('es-AR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

const getMetodoPagoLabel = (metodo) => {
  if (metodo === null || metodo === undefined) return '-'
  const labels = {
    0: 'Cobrador',
    1: 'Link de Pago',
    2: 'Sede',
  }
  return labels[metodo] || 'Otro'
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800 pb-12">
    <!-- Header -->
    <header class="bg-white border-b border-slate-200 sticky top-0 z-30 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16 items-center">
          <div class="flex items-center gap-4">
            <button @click="goBack" class="p-2 hover:bg-slate-100 rounded-lg transition-colors text-slate-500">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
            </button>
            <h1 class="text-xl font-bold text-slate-900">Detalle del Socio</h1>
          </div>
        </div>
      </div>
    </header>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="isLoading" class="flex flex-col items-center justify-center py-24">
        <svg class="animate-spin h-12 w-12 text-blue-600 mb-4" xmlns="http://www.w3.org/2000/svg" fill="none"
          viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
          </path>
        </svg>
        <p class="text-slate-500 font-medium">Cargando información detallada...</p>
      </div>

      <div v-else-if="error" class="bg-white p-8 rounded-2xl shadow-sm border border-slate-200 text-center">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-red-100 text-red-600 mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <h2 class="text-xl font-bold text-slate-900 mb-2">Error al cargar</h2>
        <p class="text-slate-500 mb-6">{{ error }}</p>
        <button @click="goBack"
          class="px-6 py-2 bg-slate-900 text-white rounded-xl font-bold hover:bg-slate-800 transition-colors">
          Volver a Gestión
        </button>
      </div>

      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Profile Summary Card -->
        <div class="lg:col-span-1 space-y-6">
          <div class="bg-white rounded-3xl shadow-sm border border-slate-200 overflow-hidden">
            <div class="h-32 bg-gradient-to-br from-blue-600 to-indigo-700 relative">
              <div class="absolute -bottom-10 left-1/2 -translate-x-1/2">
                <div
                  class="w-24 h-24 rounded-2xl bg-white shadow-xl flex items-center justify-center text-blue-600 border-4 border-white">
                  <span class="text-3xl font-black">{{ socio.nombre[0] }}{{ socio.apellido[0] }}</span>
                </div>
              </div>
            </div>
            <div class="pt-14 pb-8 px-6 text-center">
              <h2 class="text-2xl font-black text-slate-900">
                {{ socio.nombre }} {{ socio.apellido }}
              </h2>
              <p class="text-slate-500 font-bold uppercase tracking-wider text-xs mt-1">
                Socio #{{ socio.id }}
              </p>

              <div
                class="mt-6 inline-flex items-center px-4 py-1.5 rounded-full text-sm font-black ring-4 ring-white shadow-sm"
                :class="socio.adeudaCuotas ? 'bg-red-100 text-red-700' : 'bg-emerald-100 text-emerald-700'
                  ">
                <span class="w-2 h-2 rounded-full mr-2"
                  :class="socio.adeudaCuotas ? 'bg-red-500' : 'bg-emerald-500'"></span>
                {{ socio.adeudaCuotas ? 'DEUDOR' : 'CUOTA AL DÍA' }}
              </div>
            </div>
            <div class="border-t border-slate-100 px-6 py-4 bg-slate-50/50">
              <div class="flex justify-between items-center text-sm font-bold">
                <span class="text-slate-400 uppercase tracking-tighter">Desde</span>
                <span class="text-slate-600">{{ formatDate(socio.fechaAsociacion) }}</span>
              </div>
            </div>
          </div>

          <div class="bg-white rounded-3xl shadow-sm border border-slate-200 p-6 space-y-6">
            <h3 class="text-sm font-black text-slate-400 uppercase tracking-widest">
              Información de contacto
            </h3>

            <div class="flex items-start gap-4">
              <div class="p-2.5 rounded-xl bg-blue-50 text-blue-600">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                </svg>
              </div>
              <div>
                <p class="text-xs font-bold text-slate-400 uppercase">Teléfono</p>
                <p class="text-slate-700 font-bold">{{ socio.telefono || 'No registrado' }}</p>
              </div>
            </div>

            <div class="flex items-start gap-4">
              <div class="p-2.5 rounded-xl bg-indigo-50 text-indigo-600">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm0 0c1.306 0 2.417.835 2.83 2M9 14a3.001 3.001 0 00-2.83 2M15 11h3m-3 4h2" />
                </svg>
              </div>
              <div>
                <p class="text-xs font-bold text-slate-400 uppercase">DNI</p>
                <p class="text-slate-700 font-bold">{{ socio.dni }}</p>
              </div>
            </div>

            <div class="flex items-start gap-4">
              <div class="p-2.5 rounded-xl bg-amber-50 text-amber-600">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
              </div>
              <div>
                <p class="text-xs font-bold text-slate-400 uppercase">Ubicación</p>
                <p class="text-slate-700 font-bold">{{ socio.direccion || 'Sin dirección' }}</p>
                <p class="text-slate-500 text-sm font-medium">
                  {{ socio.nombreLote ? 'Lote: ' + socio.nombreLote : '' }} {{ socio.localidad }}
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Payment History Area -->
        <div class="lg:col-span-2 space-y-6">
          <div class="bg-white rounded-3xl shadow-sm border border-slate-200 overflow-hidden flex flex-col">
            <div class="px-8 py-6 border-b border-slate-100 flex justify-between items-center bg-white">
              <h3 class="text-lg font-black text-slate-900">Historial de Cuotas</h3>
              <span class="px-3 py-1 bg-slate-100 text-slate-600 rounded-lg text-xs font-black">{{
                socio.historialCuotas.length }} PERÍODOS</span>
            </div>

            <div class="overflow-x-auto scrollbar-hide">
              <table class="w-full text-left border-collapse min-w-max md:min-w-full">
                <thead>
                  <tr class="bg-slate-50/50 hidden md:table-row">
                    <th
                      class="px-8 md:px-4 py-4 text-xs font-black text-slate-400 uppercase tracking-widest whitespace-nowrap md:border-r md:border-slate-200">
                      Período
                    </th>
                    <th
                      class="px-8 md:px-4 py-4 text-xs font-black text-slate-400 uppercase tracking-widest whitespace-nowrap text-center md:border-r md:border-slate-200">
                      Fecha de Pago
                    </th>
                    <th
                      class="px-8 md:px-4 py-4 text-xs font-black text-slate-400 uppercase tracking-widest whitespace-nowrap text-center md:border-r md:border-slate-200">
                      Importe
                    </th>
                    <th
                      class="px-8 md:px-4 py-4 text-xs font-black text-slate-400 uppercase tracking-widest whitespace-nowrap text-center md:border-r md:border-slate-200">
                      Método
                    </th>
                    <th
                      class="px-8 md:px-4 py-4 text-xs font-black text-slate-400 uppercase tracking-widest text-right whitespace-nowrap">
                      Estado
                    </th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100">
                  <tr v-for="(cuota, index) in socio.historialCuotas" :key="index"
                    class="hover:bg-slate-50/80 transition-colors flex flex-col md:table-row py-6 md:py-0">

                    <!-- Period & State (Mobile Header) -->
                    <td
                      class="px-8 md:px-4 md:py-5 text-sm font-bold text-slate-600 whitespace-nowrap pb-3 md:pb-5 md:border-r md:border-slate-100">
                      <div class="flex items-center justify-between md:justify-start gap-4">
                        <div class="flex items-center gap-3">
                          <svg v-if="!cuota.pagado" xmlns="http://www.w3.org/2000/svg"
                            class="h-4 w-4 text-red-500 flex-shrink-0" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                          </svg>
                          <svg v-else xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-emerald-500 flex-shrink-0"
                            viewBox="0 0 20 20" fill="currentColor">
                            <path fill-rule="evenodd"
                              d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                              clip-rule="evenodd" />
                          </svg>
                          <span class="text-slate-900 font-black text-base md:text-sm">
                            {{ cuota.semestre === 1 ? 'Primer Semestre' : 'Segundo Semestre' }} {{ cuota.anio }}
                          </span>
                        </div>
                        <span class="md:hidden">
                          <span class="inline-flex items-center font-black text-[10px] tracking-tight"
                            :class="cuota.pagado ? 'text-emerald-500' : 'text-red-500'">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 mr-1" viewBox="0 0 20 20"
                              fill="currentColor">
                              <path v-if="cuota.pagado" fill-rule="evenodd"
                                d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                                clip-rule="evenodd" />
                              <path v-else fill-rule="evenodd"
                                d="M10 18a8 8 0 100-16 8 8 0 000 16zM8 7a1 1 0 00-1 1v4a1 1 0 001 1h4a1 1 0 001-1V8a1 1 0 00-1-1H8z"
                                clip-rule="evenodd" />
                            </svg>
                            {{ cuota.pagado ? 'PAGADO' : 'PENDIENTE' }}
                          </span>
                        </span>
                      </div>
                    </td>

                    <!-- Mobile Details Row: Fecha -->
                    <td
                      class="px-8 md:px-4 md:py-5 text-sm font-bold text-slate-700 whitespace-nowrap pt-2 md:pt-5 text-center md:border-r md:border-slate-100">
                      <div class="flex justify-between md:block">
                        <span
                          class="md:hidden text-slate-400 font-bold text-[10px] uppercase tracking-widest">Fecha:</span>
                        <template v-if="cuota.pagado">
                          {{ formatDate(cuota.fechaDePago) }}
                        </template>
                        <template v-else>
                          <span class="text-slate-300">-</span>
                        </template>
                      </div>
                    </td>

                    <!-- Mobile Details Row: Importe -->
                    <td
                      class="px-8 md:px-4 md:py-5 text-sm font-black text-slate-900 whitespace-nowrap pt-2 md:pt-5 text-center md:border-r md:border-slate-100">
                      <div class="flex justify-between md:block">
                        <span
                          class="md:hidden text-slate-400 font-bold text-[10px] uppercase tracking-widest">Importe:</span>
                        <template v-if="cuota.pagado">
                          {{
                            cuota.importePagado
                              ? '$' + cuota.importePagado.toLocaleString('es-AR')
                              : '-'
                          }}
                        </template>
                        <template v-else>
                          <span class="text-slate-300">-</span>
                        </template>
                      </div>
                    </td>

                    <!-- Mobile Details Row: Método -->
                    <td
                      class="px-8 md:px-4 md:py-5 text-sm uppercase pt-2 md:pt-5 text-center pb-2 md:pb-5 md:border-r md:border-slate-100">
                      <div class="flex justify-between md:block">
                        <span
                          class="md:hidden text-slate-400 font-bold text-[10px] uppercase tracking-widest">Método:</span>
                        <span v-if="cuota.pagado"
                          class="px-2.5 py-1 rounded-lg bg-slate-100 text-slate-600 text-[10px] font-black uppercase md:inline-block">
                          {{ getMetodoPagoLabel(cuota.metodoDePago) }}
                        </span>
                        <span v-else class="text-slate-300 font-normal">-</span>
                      </div>
                    </td>

                    <!-- Desktop State Column -->
                    <td class="px-8 md:px-4 py-5 text-right whitespace-nowrap hidden md:table-cell">
                      <span class="inline-flex items-center font-black text-xs text-slate-600">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-1 flex-shrink-0"
                          :class="cuota.pagado ? 'text-emerald-500' : 'text-red-500'" viewBox="0 0 20 20"
                          fill="currentColor">
                          <path v-if="cuota.pagado" fill-rule="evenodd"
                            d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                            clip-rule="evenodd" />
                          <path v-else fill-rule="evenodd"
                            d="M10 18a8 8 0 100-16 8 8 0 000 16zM8 7a1 1 0 00-1 1v4a1 1 0 001 1h4a1 1 0 001-1V8a1 1 0 00-1-1H8z"
                            clip-rule="evenodd" />
                        </svg>
                        {{ cuota.pagado ? 'PAGADO' : 'PENDIENTE' }}
                      </span>
                    </td>
                  </tr>
                  <tr v-if="socio.historialCuotas.length === 0">
                    <td colspan="5" class="px-8 py-12 text-center text-slate-400 font-medium italic">
                      No se registran periodos para este socio.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.font-sans {
  font-family:
    'Outfit',
    'Inter',
    system-ui,
    -apple-system,
    sans-serif;
}

/* Hide scrollbar but keep functionality */
.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
