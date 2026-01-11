<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const socioId = route.params.id

const socio = ref(null)
const isLoading = ref(true)
const error = ref('')

const fetchSocioDetails = async () => {
  isLoading.value = true
  try {
    const response = await fetch(`http://localhost:5194/api/Socios/full/${socioId}`)
    if (!response.ok) {
      throw new Error('No se pudo cargar la información del socio')
    }
    socio.value = await response.json()
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
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString('es-AR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800 pb-12">
    <!-- Header -->
    <header class="bg-white border-b border-slate-200 sticky top-0 z-30 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16 items-center">
          <div class="flex items-center gap-4">
            <button
              @click="goBack"
              class="p-2 hover:bg-slate-100 rounded-lg transition-colors text-slate-500"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-6 w-6"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M10 19l-7-7m0 0l7-7m-7 7h18"
                />
              </svg>
            </button>
            <h1 class="text-xl font-bold text-slate-900">Detalle del Socio</h1>
          </div>
        </div>
      </div>
    </header>

    <main class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div v-if="isLoading" class="flex flex-col items-center justify-center py-24">
        <svg
          class="animate-spin h-12 w-12 text-blue-600 mb-4"
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
        >
          <circle
            class="opacity-25"
            cx="12"
            cy="12"
            r="10"
            stroke="currentColor"
            stroke-width="4"
          ></circle>
          <path
            class="opacity-75"
            fill="currentColor"
            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
          ></path>
        </svg>
        <p class="text-slate-500 font-medium">Cargando información detallada...</p>
      </div>

      <div
        v-else-if="error"
        class="bg-white p-8 rounded-2xl shadow-sm border border-slate-200 text-center"
      >
        <div
          class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-red-100 text-red-600 mb-4"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-8 w-8"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
            />
          </svg>
        </div>
        <h2 class="text-xl font-bold text-slate-900 mb-2">Error al cargar</h2>
        <p class="text-slate-500 mb-6">{{ error }}</p>
        <button
          @click="goBack"
          class="px-6 py-2 bg-slate-900 text-white rounded-xl font-bold hover:bg-slate-800 transition-colors"
        >
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
                  class="w-24 h-24 rounded-2xl bg-white shadow-xl flex items-center justify-center text-blue-600 border-4 border-white"
                >
                  <span class="text-3xl font-black"
                    >{{ socio.nombre[0] }}{{ socio.apellido[0] }}</span
                  >
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
                :class="
                  socio.adeudaCuotas ? 'bg-red-100 text-red-700' : 'bg-emerald-100 text-emerald-700'
                "
              >
                <span
                  class="w-2 h-2 rounded-full mr-2"
                  :class="socio.adeudaCuotas ? 'bg-red-500' : 'bg-emerald-500'"
                ></span>
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
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5"
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
              <div>
                <p class="text-xs font-bold text-slate-400 uppercase">Teléfono</p>
                <p class="text-slate-700 font-bold">{{ socio.telefono || 'No registrado' }}</p>
              </div>
            </div>

            <div class="flex items-start gap-4">
              <div class="p-2.5 rounded-xl bg-indigo-50 text-indigo-600">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm0 0c1.306 0 2.417.835 2.83 2M9 14a3.001 3.001 0 00-2.83 2M15 11h3m-3 4h2"
                  />
                </svg>
              </div>
              <div>
                <p class="text-xs font-bold text-slate-400 uppercase">DNI</p>
                <p class="text-slate-700 font-bold">{{ socio.dni }}</p>
              </div>
            </div>

            <div class="flex items-start gap-4">
              <div class="p-2.5 rounded-xl bg-amber-50 text-amber-600">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5"
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
              <div>
                <p class="text-xs font-bold text-slate-400 uppercase">Ubicación</p>
                <p class="text-slate-700 font-bold">{{ socio.direcccion || 'Sin dirección' }}</p>
                <p class="text-slate-500 text-sm font-medium">
                  {{ socio.lote ? 'Lote: ' + socio.lote : '' }} {{ socio.localidad }}
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Payment History Area -->
        <div class="lg:col-span-2 space-y-6">
          <div
            class="bg-white rounded-3xl shadow-sm border border-slate-200 overflow-hidden flex flex-col"
          >
            <div
              class="px-8 py-6 border-b border-slate-100 flex justify-between items-center bg-white sticky top-0 z-10"
            >
              <h3 class="text-lg font-black text-slate-900">Historial de Cuotas</h3>
              <span class="px-3 py-1 bg-slate-100 text-slate-600 rounded-lg text-xs font-black"
                >{{ socio.historialCuotas.length }} PAGOS</span
              >
            </div>

            <div class="overflow-x-auto">
              <table class="w-full text-left border-collapse">
                <thead>
                  <tr class="bg-slate-50/50">
                    <th
                      class="px-8 py-4 text-xs font-black text-slate-400 uppercase tracking-widest"
                    >
                      Fecha de Pago
                    </th>
                    <th
                      class="px-8 py-4 text-xs font-black text-slate-400 uppercase tracking-widest"
                    >
                      Importe
                    </th>
                    <th
                      class="px-8 py-4 text-xs font-black text-slate-400 uppercase tracking-widest"
                    >
                      Método
                    </th>
                    <th
                      class="px-8 py-4 text-xs font-black text-slate-400 uppercase tracking-widest text-right"
                    >
                      Estado
                    </th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-50">
                  <tr
                    v-for="cuota in socio.historialCuotas"
                    :key="cuota.id"
                    class="hover:bg-slate-50/80 transition-colors"
                  >
                    <td class="px-8 py-5 text-sm font-bold text-slate-700">
                      {{ formatDate(cuota.fechaPago) }}
                    </td>
                    <td class="px-8 py-5 text-sm font-black text-slate-900">
                      ${{ cuota.importe.toLocaleString('es-AR') }}
                    </td>
                    <td class="px-8 py-5 text-sm">
                      <span
                        class="px-2.5 py-1 rounded-lg bg-slate-100 text-slate-600 text-[10px] font-black uppercase"
                      >
                        {{ cuota.metodoPago }}
                      </span>
                    </td>
                    <td class="px-8 py-5 text-right">
                      <span class="inline-flex items-center text-emerald-600 font-black text-xs">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          class="h-4 w-4 mr-1"
                          viewBox="0 0 20 20"
                          fill="currentColor"
                        >
                          <path
                            fill-rule="evenodd"
                            d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                            clip-rule="evenodd"
                          />
                        </svg>
                        PAGADO
                      </span>
                    </td>
                  </tr>
                  <tr v-if="socio.historialCuotas.length === 0">
                    <td
                      colspan="4"
                      class="px-8 py-12 text-center text-slate-400 font-medium italic"
                    >
                      No se registran pagos realizados hasta la fecha.
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
</style>
