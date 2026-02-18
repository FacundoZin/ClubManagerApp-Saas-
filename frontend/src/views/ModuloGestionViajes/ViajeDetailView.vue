<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ViajesService from '../../services/viajesService'
import VarianteFormModal from '../../components/ModuloGestionViajes/VarianteFormModal.vue'
import InscripcionConfirmModal from '../../components/ModuloGestionViajes/InscripcionConfirmModal.vue'
import PagoViajeModal from '../../components/ModuloGestionViajes/PagoViajeModal.vue'
import ConfirmModal from '../../components/Common/ConfirmModal.vue'
import DataTable from '../../components/Common/DataTable.vue'

const route = useRoute()
const router = useRouter()
const viajeId = parseInt(route.params.id)

// State
const viaje = ref(null)
const isLoading = ref(true)
const error = ref('')

// Modal states
const isVarianteModalOpen = ref(false)
const isInscripcionModalOpen = ref(false)
const isPagoModalOpen = ref(false)
const isConfirmCancelOpen = ref(false)

const selectedInscripto = ref(null)
const selectedInscriptoId = ref(null)

// Toast state (using simple approach consistent with Dashboard)
const toast = ref({
  show: false,
  message: '',
  type: 'success',
})

const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => {
    toast.value.show = false
  }, 4000)
}

const fetchViajeCompleto = async () => {
  isLoading.value = true
  try {
    viaje.value = await ViajesService.verViajeCompleto(viajeId)
  } catch (err) {
    error.value = err.message
    showToast(err.message, 'error')
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  fetchViajeCompleto()
})

const formatCurrency = (value) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString + 'T00:00:00').toLocaleDateString('es-AR')
}

const getRegimenLabel = (value) => {
  const regimenLabels = { 0: 'Media Pensión', 1: 'Pensión Completa' }
  return regimenLabels[value] || 'N/A'
}

const handleOpenPago = (inscripto) => {
  selectedInscripto.value = inscripto
  isPagoModalOpen.value = true
}

const handleOpenCancel = (id) => {
  selectedInscriptoId.value = id
  isConfirmCancelOpen.value = true
}

const confirmCancelInscripcion = async () => {
  try {
    await ViajesService.cancelarInscripcion(selectedInscriptoId.value)
    showToast('Inscripción cancelada correctamente')
    fetchViajeCompleto()
  } catch (err) {
    showToast(err.message, 'error')
  } finally {
    isConfirmCancelOpen.value = false
  }
}

const handleSaveAction = () => {
  isVarianteModalOpen.value = false
  isInscripcionModalOpen.value = false
  isPagoModalOpen.value = false
  showToast('Operación realizada con éxito')
  fetchViajeCompleto()
}

const goBack = () => {
  router.push('/viajes')
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Loading State -->
      <div v-if="isLoading" class="flex flex-col items-center justify-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-teal-600 mb-4"></div>
        <p class="text-slate-500 font-medium">Cargando detalles del viaje...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="bg-white p-8 rounded-2xl border border-slate-200 text-center">
        <div
          class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-red-100 text-red-600 mb-4"
        >
          <svg class="w-8 h-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        </div>
        <h3 class="text-xl font-bold text-slate-900 mb-2">Error al cargar el viaje</h3>
        <p class="text-slate-500 mb-6">{{ error }}</p>
        <button @click="goBack" class="px-6 py-2 bg-slate-900 text-white rounded-xl font-bold">
          Volver atrás
        </button>
      </div>

      <div v-else>
        <!-- Breadcrumb & Header -->
        <div class="mb-8">
          <nav class="flex mb-2" aria-label="Breadcrumb">
            <ol class="inline-flex items-center space-x-1 md:space-x-3">
              <li class="inline-flex items-center">
                <router-link
                  to="/"
                  class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-teal-600"
                >
                  <svg class="w-3 h-3 mr-2.5" fill="currentColor" viewBox="0 0 20 20">
                    <path
                      d="m19.707 9.293-2-2-7-7a1 1 0 0 0-1.414 0l-7 7-2 2a1 1 0 0 0 1.414 1.414L2 10.414V18a2 2 0 0 0 2 2h3a1 1 0 0 0 1-1v-4a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v4a1 1 0 0 0 1 1h3a2 2 0 0 0 2-2v-7.586l.293.293a1 1 0 0 0 1.414-1.414Z"
                    />
                  </svg>
                  Inicio
                </router-link>
              </li>
              <li>
                <div class="flex items-center">
                  <svg
                    class="w-3 h-3 text-slate-400 mx-1"
                    fill="none"
                    viewBox="0 0 6 10"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="m1 9 4-4-4-4"
                    />
                  </svg>
                  <router-link
                    to="/viajes"
                    class="ml-1 text-sm font-medium text-slate-500 hover:text-teal-600 md:ml-2"
                    >Gestión de Viajes</router-link
                  >
                </div>
              </li>
              <li>
                <div class="flex items-center">
                  <svg
                    class="w-3 h-3 text-slate-400 mx-1"
                    fill="none"
                    viewBox="0 0 6 10"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="m1 9 4-4-4-4"
                    />
                  </svg>
                  <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2">{{
                    viaje.titulo
                  }}</span>
                </div>
              </li>
            </ol>
          </nav>

          <div class="flex flex-col md:flex-row md:items-end justify-between gap-4">
            <div>
              <h2 class="text-3xl font-bold text-slate-900 tracking-tight">{{ viaje.titulo }}</h2>
              <div class="flex flex-wrap items-center gap-4 mt-2 text-slate-500 font-medium">
                <span class="flex items-center">
                  <svg
                    class="w-4 h-4 mr-1.5 text-teal-500"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                    />
                  </svg>
                  Salida: {{ formatDate(viaje.fechasalida) }}
                </span>
                <span class="flex items-center">
                  <svg
                    class="w-4 h-4 mr-1.5 text-teal-500"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                  {{ viaje.dias }} Días / {{ viaje.noches }} Noches
                </span>
                <span class="flex items-center">
                  <svg
                    class="w-4 h-4 mr-1.5 text-teal-500"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                  Precio Base: {{ formatCurrency(viaje.valorBase) }}
                </span>
              </div>
            </div>
            <div class="flex gap-2">
              <button
                @click="isVarianteModalOpen = true"
                class="px-4 py-2.5 bg-white text-teal-600 font-bold rounded-xl border border-teal-200 hover:bg-teal-50 shadow-sm transition-all flex items-center"
              >
                <svg class="w-5 h-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 9v3m0 0v3m0-3h3m-3 0h-3m-9-4.721L12 1l7.71 4.279A2 2 0 0121 7.032V17a2 2 0 01-1.29 1.873L12 21l-7.71-2.127A2 2 0 013 17V7.032a2 2 0 011.29-1.753z"
                  />
                </svg>
                Nueva Variante
              </button>
              <button
                @click="isInscripcionModalOpen = true"
                class="px-4 py-2.5 bg-teal-600 text-white font-bold rounded-xl hover:bg-teal-700 shadow-lg transition-all flex items-center"
              >
                <svg class="w-5 h-5 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"
                  />
                </svg>
                Inscribir Socio
              </button>
            </div>
          </div>
        </div>

        <!-- Financial Stats Grid -->
        <div class="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-7 gap-4 mb-10">
          <div
            class="bg-white p-4 rounded-2xl border border-slate-200 flex flex-col items-center text-center shadow-sm"
          >
            <span class="text-[10px] uppercase font-bold text-slate-400 mb-2">Inscriptos</span>
            <span class="text-xl font-black text-slate-900">{{ viaje.totalInscriptos }}</span>
          </div>
          <div
            class="bg-white p-4 rounded-2xl border border-slate-200 flex flex-col items-center text-center shadow-sm"
          >
            <span class="text-[10px] uppercase font-bold text-slate-400 mb-2">Cancelados</span>
            <span class="text-xl font-black text-slate-900">{{ viaje.totalCancelados }}</span>
          </div>
          <div
            class="bg-emerald-50 p-4 rounded-2xl border border-emerald-100 flex flex-col items-center text-center shadow-sm"
          >
            <span class="text-[10px] uppercase font-bold text-emerald-600 mb-2">Recaudado</span>
            <span class="text-lg font-black text-emerald-700">{{
              formatCurrency(viaje.totalRecaudado)
            }}</span>
          </div>
          <div
            class="bg-rose-50 p-4 rounded-2xl border border-rose-100 flex flex-col items-center text-center shadow-sm"
          >
            <span class="text-[10px] uppercase font-bold text-rose-500 mb-2">Pendiente</span>
            <span class="text-lg font-black text-rose-700">{{
              formatCurrency(viaje.totalPendiente)
            }}</span>
          </div>
          <div
            class="bg-blue-50 p-4 rounded-2xl border border-blue-100 flex flex-col items-center text-center shadow-sm"
          >
            <span class="text-[10px] uppercase font-bold text-blue-500 mb-2">Comisión</span>
            <span class="text-lg font-black text-blue-700">{{
              formatCurrency(viaje.montoComision)
            }}</span>
          </div>
          <div
            class="bg-slate-900 p-4 rounded-2xl flex flex-col items-center text-center shadow-lg"
          >
            <span class="text-[10px] uppercase font-bold text-slate-400 mb-2">A Agencia</span>
            <span class="text-lg font-black text-white">{{
              formatCurrency(viaje.montoParaAgencia)
            }}</span>
          </div>
          <div
            class="bg-teal-50 p-4 rounded-2xl border border-teal-100 flex flex-col items-center text-center shadow-sm"
          >
            <span class="text-[10px] uppercase font-bold text-teal-600 mb-2">Liberados</span>
            <span class="text-xl font-black text-teal-700">{{ viaje.totalLiberados }}</span>
          </div>
        </div>

        <!-- Variantes & Inscriptos Section -->
        <div class="space-y-8">
          <div
            v-for="variante in viaje.variantes"
            :key="variante.id"
            class="bg-white rounded-2xl border border-slate-200 overflow-hidden shadow-sm"
          >
            <!-- Variante Header -->
            <div
              class="bg-slate-50 px-6 py-4 border-b border-slate-200 flex flex-col sm:flex-row sm:items-center justify-between gap-4"
            >
              <div class="flex items-center">
                <div
                  class="w-10 h-10 rounded-full bg-teal-100 text-teal-600 flex items-center justify-center mr-3 font-bold"
                >
                  {{ variante.id }}
                </div>
                <div>
                  <h3 class="text-lg font-bold text-slate-900 leading-tight">
                    Variante: {{ variante.nombreVariante }}
                  </h3>
                  <p class="text-xs text-slate-500">
                    {{ getRegimenLabel(variante.regimen) }} — {{ variante.tipoDeButaca }}
                  </p>
                </div>
              </div>
              <div class="flex gap-4">
                <div class="text-right">
                  <p class="text-[10px] uppercase font-bold text-slate-400">Valor Viaje</p>
                  <p class="text-sm font-bold text-slate-700">
                    {{ formatCurrency(variante.valorViaje) }}
                  </p>
                </div>
                <div class="text-right">
                  <p class="text-[10px] uppercase font-bold text-slate-400">Seña</p>
                  <p class="text-sm font-bold text-slate-700">
                    {{ formatCurrency(variante.valorSeña) }}
                  </p>
                </div>
              </div>
            </div>

            <!-- Inscriptos Table -->
            <DataTable
              :headers="[
                { label: 'Socio', key: 'socio' },
                { label: 'DNI / Teléfono', key: 'dni' },
                {
                  label: 'Abonado',
                  key: 'montoAbonado',
                  textRight: true,
                  class: 'font-bold text-emerald-600',
                },
                {
                  label: 'Pendiente',
                  key: 'montoPendiente',
                  textRight: true,
                  class: 'font-bold text-rose-600',
                },
                { label: 'Estado', key: 'estado' },
                { label: 'Acciones', key: 'acciones', textRight: true },
              ]"
              :items="variante.inscriptos"
              emptyMessage="Aún no hay socios inscriptos en esta variante."
            >
              <template #cell(socio)="{ item }">
                <span
                  class="font-bold text-slate-700"
                  :class="{ 'opacity-50 grayscale': item.cancelado }"
                  >{{ item.nombreSocio }}</span
                >
              </template>
              <template #cell(dni)="{ item }">
                <div class="text-xs" :class="{ 'opacity-50 grayscale': item.cancelado }">
                  <p class="text-slate-600 font-medium">{{ item.dniSocio }}</p>
                  <p class="text-slate-400">{{ item.telefonoSocio || 'S/T' }}</p>
                </div>
              </template>
              <template #cell(montoAbonado)="{ item }">
                <span :class="{ 'opacity-50 grayscale': item.cancelado }">
                  {{ formatCurrency(item.montoAbonado) }}
                </span>
              </template>
              <template #cell(montoPendiente)="{ item }">
                <span :class="{ 'opacity-50 grayscale': item.cancelado }">
                  {{ formatCurrency(item.montoPendiente) }}
                </span>
              </template>
              <template #cell(estado)="{ item }">
                <div :class="{ 'opacity-50 grayscale': item.cancelado }">
                  <span
                    v-if="item.cancelado"
                    class="inline-flex items-center px-2 py-0.5 rounded text-xs font-bold bg-red-100 text-red-700"
                    >Cancelado</span
                  >
                  <span
                    v-else-if="item.montoPendiente === 0"
                    class="inline-flex items-center px-2 py-0.5 rounded text-xs font-bold bg-emerald-100 text-emerald-700"
                    >Saldado</span
                  >
                  <span
                    v-else
                    class="inline-flex items-center px-2 py-0.5 rounded text-xs font-bold bg-blue-100 text-blue-700"
                    >Activo</span
                  >
                </div>
              </template>
              <template #cell(acciones)="{ item }">
                <div v-if="!item.cancelado" class="flex justify-end gap-2">
                  <button
                    v-if="item.montoPendiente > 0"
                    @click="handleOpenPago(item)"
                    class="p-2 text-emerald-600 hover:bg-emerald-50 rounded-lg transition-colors"
                    title="Registrar Pago"
                  >
                    <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                  </button>
                  <button
                    @click="handleOpenCancel(item.id)"
                    class="p-2 text-red-400 hover:bg-red-50 rounded-lg transition-colors"
                    title="Cancelar Inscripción"
                  >
                    <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                  </button>
                </div>
              </template>
            </DataTable>
          </div>
        </div>
      </div>
    </main>

    <!-- Modals -->
    <VarianteFormModal
      :is-open="isVarianteModalOpen"
      :id-viaje="viajeId"
      @close="isVarianteModalOpen = false"
      @save="handleSaveAction"
    />

    <InscripcionConfirmModal
      v-if="isInscripcionModalOpen"
      :is-open="isInscripcionModalOpen"
      :socio="null"
      :id-viaje="viajeId"
      @close="isInscripcionModalOpen = false"
      @save="handleSaveAction"
    />

    <PagoViajeModal
      v-if="isPagoModalOpen"
      :is-open="isPagoModalOpen"
      :inscripto="selectedInscripto"
      @close="isPagoModalOpen = false"
      @save="handleSaveAction"
    />

    <ConfirmModal
      :is-open="isConfirmCancelOpen"
      title="Cancelar Inscripción"
      message="¿Está seguro que desea cancelar esta inscripción? Esta acción no se puede deshacer y el socio dejará de figurar como activo en el viaje."
      confirm-text="Confirmar Cancelación"
      type="danger"
      @close="isConfirmCancelOpen = false"
      @confirm="confirmCancelInscripcion"
    />

    <!-- Toast Notification -->
    <Transition
      enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
      enter-to-class="translate-y-0 opacity-100 sm:translate-x-0"
      leave-active-class="transition ease-in duration-100"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="toast.show"
        class="fixed bottom-5 right-5 z-50 flex w-full max-w-sm overflow-hidden bg-white rounded-lg shadow-2xl border border-slate-200 pointer-events-auto ring-1 ring-black ring-opacity-5"
      >
        <div
          class="flex items-center justify-center w-12"
          :class="toast.type === 'success' ? 'bg-emerald-500' : 'bg-red-500'"
        >
          <svg
            v-if="toast.type === 'success'"
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 40 40"
          >
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM16.6667 28.3333L8.33337 20L10.6834 17.65L16.6667 23.6166L29.3167 10.9666L31.6667 13.3333L16.6667 28.3333Z"
            />
          </svg>
          <svg v-else class="w-6 h-6 text-white fill-current" viewBox="0 0 40 40">
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM21.6667 28.3333H18.3334V25H21.6667V28.3333ZM21.6667 21.6666H18.3334V11.6666H21.6667V21.6666Z"
            />
          </svg>
        </div>
        <div class="px-4 py-3 -mx-3 items-center flex">
          <div class="mx-3">
            <p
              class="text-sm font-bold"
              :class="toast.type === 'success' ? 'text-emerald-600' : 'text-red-600'"
            >
              {{ toast.type === 'success' ? 'Éxito' : 'Error' }}
            </p>
            <p class="text-xs text-slate-500">{{ toast.message }}</p>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>
