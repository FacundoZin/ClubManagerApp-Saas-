<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ConfirmModal from '../../components/Common/ConfirmModal.vue'
import LoadingOverlay from '../../components/Common/LoadingOverlay.vue'
import AddItemModal from '../../components/ModuloAlquilerArticulos/Alquileres/AddItemModal.vue'
import RegisterPaymentModal from '../../components/ModuloAlquilerArticulos/Alquileres/RegisterPaymentModal.vue'
import ArticulosAlquiladosCard from '../../components/ModuloAlquilerArticulos/Articulos/ArticulosAlquiladosCard.vue'
import AlquilerService from '../../services/AlquilerService'

const route = useRoute()
const router = useRouter()
const alquilerId = route.params.id

const alquiler = ref(null)
const loading = ref(true)
const error = ref('')
const paymentError = ref('')

// Modals
const isAddItemModalOpen = ref(false)
const isConfirmFinalizeOpen = ref(false)
const isRegisterPaymentModalOpen = ref(false)

// Toast
const toast = ref({ show: false, message: '', type: 'success' })
const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

onMounted(() => {
  loadAlquiler()
})

const loadAlquiler = async (silent = false) => {
  if (!silent) loading.value = true
  error.value = ''
  try {
    const result = await AlquilerService.getById(alquilerId)
    alquiler.value = result
  } catch (e) {
    if (!silent) error.value = e.message
    else showToast(e.message, 'error')
  } finally {
    if (!silent) loading.value = false
  }
}

const handleItemUpdated = (message, type = 'success') => {
  showToast(message, type)
  if (type === 'success') {
    loadAlquiler(true) // Silent refresh
  }
}

const handleAddItem = async () => {
  isAddItemModalOpen.value = false
  showToast('Item agregado correctamente')
  loadAlquiler(true) // Silent refresh
}

const handleRegisterPayment = (message) => {
  showToast(message || 'Pago registrado correctamente')
  loadAlquiler()
}

const handleFinalize = async () => {
  try {
    await AlquilerService.finalize(alquilerId)
    isConfirmFinalizeOpen.value = false
    router.push({ name: 'alquiler-articulos', query: { success: 'finalizado' } })
  } catch (e) {
    showToast(e.message, 'error')
    isConfirmFinalizeOpen.value = false
  }
}

const goBack = () => router.push('/alquiler-articulos')

const meses = [
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

const getNombreMes = (mesNum) => {
  return meses[mesNum - 1] || mesNum
}

const allMonthsCombined = computed(() => {
  if (!alquiler.value) return []

  const paid = (alquiler.value.historialDePagos || []).map((p) => ({
    ...p,
    tipo: 'pagado',
  }))

  const pending = (alquiler.value.mesesAdeudados || []).map((m) => ({
    ...m,
    tipo: 'pendiente',
    id: `pending-${m.anio}-${m.mes}`,
  }))

  return [...paid, ...pending].sort((a, b) => {
    if (a.anio !== b.anio) return a.anio - b.anio
    return a.mes - b.mes
  })
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800 pb-20">
    <!-- Header Premium -->
    <div class="bg-white/80 backdrop-blur-md border-b border-slate-200 sticky top-0 z-30 shadow-sm">
      <div
        class="max-w-6xl mx-auto px-4 sm:px-6 py-4 flex flex-col sm:flex-row sm:items-center justify-between gap-4"
      >
        <div class="flex items-center gap-4">
          <button
            @click="goBack"
            class="group p-2 -ml-2 text-slate-400 hover:text-blue-600 rounded-xl hover:bg-blue-50 transition-all duration-300"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-6 w-6 transform group-hover:-translate-x-1 transition-transform"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2.5"
                d="M10 19l-7-7m0 0l7-7m-7 7h18"
              />
            </svg>
          </button>
          <div>
            <h1 class="text-xl font-black text-slate-900 tracking-tight">Detalle de Alquiler</h1>
            <p v-if="alquiler" class="text-xs font-bold text-slate-500 uppercase tracking-widest">
              Contrato #{{ alquilerId }}
            </p>
          </div>
        </div>
        <div v-if="alquiler" class="flex items-center gap-3">
          <span
            class="inline-flex items-center gap-1.5 px-4 py-1.5 rounded-full text-xs font-black uppercase tracking-widest shadow-sm border transition-all"
            :class="
              alquiler.estaAlDia
                ? 'bg-emerald-50 text-emerald-700 border-emerald-100'
                : 'bg-rose-50 text-rose-700 border-rose-100 animate-pulse'
            "
          >
            <span
              class="w-2 h-2 rounded-full"
              :class="alquiler.estaAlDia ? 'bg-emerald-500' : 'bg-rose-500'"
            ></span>
            {{ alquiler.estaAlDia ? 'Al día' : 'Con Deuda' }}
          </span>
        </div>
      </div>
    </div>

    <LoadingOverlay :show="loading" message="Sincronizando registros del alquiler..." />

    <div
      v-if="!loading && error"
      class="max-w-3xl mx-auto mt-12 p-8 bg-red-50 text-red-800 rounded-3xl border border-red-100 text-center font-bold flex items-center justify-center gap-3"
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="h-6 w-6 text-red-400"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
        />
      </svg>
      {{ error }}
    </div>

    <div v-else-if="!loading && alquiler" class="max-w-6xl mx-auto px-4 sm:px-6 mt-10 space-y-10">
      <!-- Info Cards Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Socio Header Card (New Style) -->
        <div
          class="lg:col-span-2 bg-white rounded-3xl shadow-sm border border-slate-200 overflow-hidden"
        >
          <div class="h-32 bg-gradient-to-br from-blue-600 to-indigo-700 relative">
            <div class="absolute -bottom-12 left-8 p-1 bg-white rounded-3xl shadow-xl">
              <div
                class="w-24 h-24 bg-blue-100 text-blue-700 rounded-[1.25rem] flex items-center justify-center text-3xl font-black ring-4 ring-white"
              >
                {{ alquiler.nombreSocio[0] }}{{ alquiler.apellidoSocio[0] }}
              </div>
            </div>
          </div>
          <div class="pt-16 pb-8 px-8">
            <div
              class="flex flex-col md:flex-row justify-between items-start md:items-center gap-6"
            >
              <div>
                <h2 class="text-3xl font-black text-slate-900 tracking-tight">
                  {{ alquiler.nombreSocio }} {{ alquiler.apellidoSocio }}
                </h2>
                <div class="flex items-center gap-4 mt-2">
                  <div class="flex items-center gap-1.5 text-slate-500">
                    <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M10 6H5a2 2 0 00-2 2v9a2 2 0 002 2h14a2 2 0 002-2V8a2 2 0 00-2-2h-5m-4 0V5a2 2 0 114 0v1m-4 0a2 2 0 104 0m-5 8a2 2 0 100-4 2 2 0 000 4zm5 3h1a2 2 0 012 2v1H2v-1a2 2 0 012-2h1"
                      />
                    </svg>
                    <span class="text-sm font-bold">{{ alquiler.dniSocio }}</span>
                  </div>
                  <div
                    v-if="alquiler.telefonoSocio"
                    class="flex items-center gap-1.5 text-slate-500"
                  >
                    <svg class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M3 5a2 2 0 012-2h2.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"
                      />
                    </svg>
                    <span class="text-sm font-bold">{{ alquiler.telefonoSocio }}</span>
                  </div>
                </div>
              </div>
              <div class="flex flex-col gap-1 items-end">
                <span class="text-[10px] font-black text-slate-400 uppercase tracking-widest"
                  >Dirección del Socio</span
                >
                <p class="text-base font-bold text-slate-700 text-right">
                  {{ alquiler.direccionSocio || 'No registrada' }}<br />
                  <span class="text-xs text-slate-400">{{
                    alquiler.localidadSocio || 'Sin localidad'
                  }}</span>
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Rental Meta & Quick Actions -->
        <div
          class="bg-white rounded-3xl shadow-sm border border-slate-200 p-8 flex flex-col justify-between overflow-hidden relative"
        >
          <div
            class="absolute bottom-0 right-0 w-32 h-32 bg-slate-50 rounded-full -mr-16 -mb-16 -z-10"
          ></div>

          <div>
            <h2
              class="text-xs font-black text-slate-400 uppercase tracking-widest mb-6 flex items-center gap-2"
            >
              <svg
                class="h-4 w-4 text-blue-500"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              Resumen del Alquiler
            </h2>
            <div class="space-y-4">
              <div class="flex justify-between items-center group">
                <span class="text-sm font-bold text-slate-400">Iniciado el:</span>
                <span
                  class="font-black text-slate-900 bg-slate-100 px-3 py-1 rounded-lg transition-transform group-hover:scale-105 duration-300"
                  >{{ alquiler.fechaAlquiler }}</span
                >
              </div>
              <div
                v-if="alquiler.observaciones"
                class="mt-4 p-4 bg-blue-50/50 border border-blue-100/50 rounded-2xl"
              >
                <p class="text-[10px] font-black text-blue-400 uppercase tracking-widest mb-1">
                  Observaciones
                </p>
                <p class="text-sm text-blue-700 font-medium italic italic leading-relaxed">
                  "{{ alquiler.observaciones }}"
                </p>
              </div>
            </div>
          </div>

          <div class="mt-8 space-y-3">
            <button
              @click="isRegisterPaymentModalOpen = true"
              class="w-full inline-flex justify-center items-center px-6 py-4 border border-transparent text-sm font-black uppercase tracking-widest rounded-2xl text-white bg-blue-600 hover:bg-blue-700 shadow-lg shadow-blue-100 transition-all transform active:scale-95 group"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-3 group-hover:scale-125 transition-transform"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2.5"
                  d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              Registrar Pago
            </button>
            <button
              @click="isConfirmFinalizeOpen = true"
              class="w-full inline-flex justify-center items-center px-6 py-4 border-2 border-slate-100 text-sm font-black uppercase tracking-widest rounded-2xl text-slate-600 bg-white hover:bg-slate-50 transition-all transform active:scale-95"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-3 text-slate-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2.5"
                  d="M5 13l4 4L19 7"
                />
              </svg>
              Finalizar Contrato
            </button>
            <div
              v-if="paymentError"
              class="mt-4 p-4 bg-red-50 border border-red-100 rounded-2xl text-xs font-bold text-red-800 flex items-center gap-3 animate-pulse"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 shrink-0 text-red-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              <span>{{ paymentError }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Articulos (Items) -->
      <ArticulosAlquiladosCard
        v-if="alquiler"
        :alquiler-id="alquilerId"
        :items="alquiler.items"
        @updated="handleItemUpdated"
        @add-item="isAddItemModalOpen = true"
      />

      <!-- Estado de Pagos (Historial y Deuda combinados) -->
      <div
        class="bg-white rounded-[2.5rem] shadow-sm border border-slate-200 overflow-hidden mb-12"
      >
        <div
          class="px-10 py-8 border-b border-slate-200 bg-slate-50/50 flex flex-col md:flex-row md:items-center justify-between gap-4"
        >
          <div>
            <h2 class="text-xl font-black text-slate-900 tracking-tight">Cronograma de Pagos</h2>
            <p class="text-xs font-bold text-slate-400 uppercase tracking-widest mt-1">
              Historial completo de cuotas mensuales
            </p>
          </div>
          <div
            v-if="alquiler.mesesAdeudados?.length > 0"
            class="w-fit text-[10px] font-black text-rose-700 bg-rose-100 px-4 py-1.5 rounded-full border border-rose-200 uppercase tracking-widest animate-pulse"
          >
            {{ alquiler.mesesAdeudados.length }} cuota(s) pendiente(s)
          </div>
        </div>

        <div class="p-8 sm:p-10">
          <div
            v-if="allMonthsCombined.length > 0"
            class="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-6"
          >
            <div
              v-for="pago in allMonthsCombined"
              :key="pago.id"
              class="group relative rounded-3xl p-5 text-center transition-all hover:shadow-xl hover:-translate-y-1 border-2 overflow-hidden"
              :class="
                pago.tipo === 'pagado'
                  ? 'bg-white border-emerald-100 hover:border-emerald-200'
                  : 'bg-white border-rose-100 hover:border-rose-200'
              "
            >
              <!-- Background accent -->
              <div
                class="absolute top-0 right-0 w-16 h-16 rounded-full -mr-8 -mt-8 opacity-10"
                :class="pago.tipo === 'pagado' ? 'bg-emerald-500' : 'bg-rose-500'"
              ></div>

              <div
                class="text-[9px] uppercase font-black tracking-widest mb-3 py-1 px-2 rounded-full inline-block"
                :class="
                  pago.tipo === 'pagado'
                    ? 'bg-emerald-50 text-emerald-600'
                    : 'bg-rose-50 text-rose-600'
                "
              >
                {{ pago.tipo === 'pagado' ? 'Pagado' : 'Pendiente' }}
              </div>
              <div class="text-xs uppercase font-black tracking-widest text-slate-500">
                {{ getNombreMes(pago.mes) }}
              </div>
              <div class="text-lg font-black text-slate-900 mb-1">
                {{ pago.anio }}
              </div>
              <div
                class="text-xl font-black mt-2 transition-transform group-hover:scale-110"
                :class="pago.tipo === 'pagado' ? 'text-emerald-600' : 'text-rose-600'"
              >
                <span class="text-sm opacity-50 font-bold">$</span>{{ pago.monto.toLocaleString() }}
              </div>
            </div>
          </div>
          <div
            v-else
            class="py-24 text-center bg-slate-50 border-2 border-dashed border-slate-200 rounded-[2rem]"
          >
            <div class="p-4 bg-white rounded-full w-fit mx-auto shadow-sm mb-4">
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
                  d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
            <h4 class="text-lg font-bold text-slate-800 tracking-tight">
              Sin actividad financiera
            </h4>
            <p class="text-sm text-slate-400 font-medium max-w-xs mx-auto mt-2">
              No se han registrado pagos ni deudas pendientes en este contrato hasta el momento.
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Modals -->
    <AddItemModal
      :is-open="isAddItemModalOpen"
      :alquiler-id="parseInt(alquilerId)"
      @close="isAddItemModalOpen = false"
      @save="handleAddItem"
    />

    <RegisterPaymentModal
      :is-open="isRegisterPaymentModalOpen"
      :alquiler-id="parseInt(alquilerId)"
      @close="isRegisterPaymentModalOpen = false"
      @saved="handleRegisterPayment"
    />

    <ConfirmModal
      :is-open="isConfirmFinalizeOpen"
      title="Finalizar Alquiler"
      message="¿Está seguro que desea finalizar este alquiler? Se registrará la devolución de todos los artículos."
      confirm-text="Finalizar Alquiler"
      type="info"
      @close="isConfirmFinalizeOpen = false"
      @confirm="handleFinalize"
    />

    <!-- Toast Premium -->
    <Transition
      enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-4 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition ease-in duration-200"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="toast.show"
        class="fixed bottom-10 right-10 z-50 flex w-full max-w-md overflow-hidden bg-white rounded-2xl shadow-2xl border border-slate-200 pointer-events-auto ring-1 ring-black ring-opacity-5"
      >
        <div
          class="flex items-center justify-center w-14 shrink-0"
          :class="{
            'bg-emerald-500': toast.type === 'success',
            'bg-red-500': toast.type === 'error',
          }"
        >
          <svg
            v-if="toast.type === 'success'"
            class="w-7 h-7 text-white"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2.5"
              d="M5 13l4 4L19 7"
            />
          </svg>
          <svg
            v-else
            class="w-7 h-7 text-white"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2.5"
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        </div>
        <div class="px-6 py-5">
          <span
            class="text-xs font-black uppercase tracking-widest mb-1 block"
            :class="{
              'text-emerald-600': toast.type === 'success',
              'text-red-600': toast.type === 'error',
            }"
          >
            {{ toast.type === 'success' ? 'Operación Exitosa' : 'Se produjo un error' }}
          </span>
          <p class="text-base font-bold text-slate-700 leading-tight">{{ toast.message }}</p>
        </div>
      </div>
    </Transition>
  </div>
</template>
