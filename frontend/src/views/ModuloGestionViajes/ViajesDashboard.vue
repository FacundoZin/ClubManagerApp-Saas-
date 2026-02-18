<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import InscripcionConfirmModal from '../../components/ModuloGestionViajes/InscripcionConfirmModal.vue'
import VarianteFormModal from '../../components/ModuloGestionViajes/VarianteFormModal.vue'
import ViajeCard from '../../components/ModuloGestionViajes/ViajeCard.vue'
import ViajeFormModal from '../../components/ModuloGestionViajes/ViajeFormModal.vue'
import SociosService from '../../services/SociosService'
import ViajesService from '../../services/viajesService'

const router = useRouter()

// State
const currentAction = ref('none') // 'none', 'create', 'list', 'inscribir'
const viajes = ref([])
const isLoading = ref(false)
const expandedViajeId = ref(null)
const expandedViajeVariantes = ref([])
const isLoadingVariantes = ref(false)

// Inscription Flow State
const searchDni = ref('')
const selectedSocio = ref(null)
const isSearchingSocio = ref(false)
const searchError = ref('')
const comboViajes = ref([])
const selectedViajeId = ref(null)

// Modal State
const isViajeModalOpen = ref(false)
const isVarianteModalOpen = ref(false)
const isInscripcionModalOpen = ref(false)
const targetViajeId = ref(null)

// Toast State
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

// Actions
const actions = [
  {
    id: 'create',
    title: 'Crear viaje',
    description: 'Registrar un nuevo destino base.',
    icon: 'M12 9v3m0 0v3m0-3h3m-3 0h-3m-9-4.721L12 1l7.71 4.279A2 2 0 0121 7.032V17a2 2 0 01-1.29 1.873L12 21l-7.71-2.127A2 2 0 013 17V7.032a2 2 0 011.29-1.753z',
    color: 'text-teal-600',
    bg: 'bg-teal-50',
    hoverBorder: 'group-hover:border-teal-200',
  },
  {
    id: 'list',
    title: 'Ver viajes disponibles',
    description: 'Listado de excursiones y turismo activo.',
    icon: 'M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7',
    color: 'text-emerald-600',
    bg: 'bg-emerald-50',
    hoverBorder: 'group-hover:border-emerald-200',
  },
  {
    id: 'inscribir',
    title: 'Inscribir socio a viaje',
    description: 'Registrar a un socio en una variante de viaje.',
    icon: 'M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z',
    color: 'text-blue-600',
    bg: 'bg-blue-50',
    hoverBorder: 'group-hover:border-blue-200',
  },
]

const selectAction = async (actionId) => {
  currentAction.value = actionId
  if (actionId === 'create') {
    isViajeModalOpen.value = true
  } else if (actionId === 'list') {
    fetchViajes()
  } else if (actionId === 'inscribir') {
    resetInscriptionFlow()
  }
}

const fetchViajes = async () => {
  isLoading.value = true
  try {
    viajes.value = await ViajesService.listarViajesDisponibles()
  } catch (error) {
    showToast(error.message, 'error')
  } finally {
    isLoading.value = false
  }
}

const toggleVariantes = async (viajeId) => {
  if (expandedViajeId.value === viajeId) {
    expandedViajeId.value = null
    expandedViajeVariantes.value = []
    return
  }

  expandedViajeId.value = viajeId
  isLoadingVariantes.value = true
  try {
    expandedViajeVariantes.value = await ViajesService.listarVariantesDeViaje(viajeId)
  } catch (error) {
    showToast(error.message, 'error')
  } finally {
    isLoadingVariantes.value = false
  }
}

const openCreateVariante = (viajeId) => {
  targetViajeId.value = viajeId
  isVarianteModalOpen.value = true
}

const handleSaveViaje = () => {
  isViajeModalOpen.value = false
  showToast('Viaje creado correctamente')
  if (currentAction.value === 'list') fetchViajes()
}

const handleSaveVariante = () => {
  isVarianteModalOpen.value = false
  showToast('Variante agregada correctamente')
  if (expandedViajeId.value === targetViajeId.value) {
    // Refresh variants if this travel is expanded
    toggleVariantes(targetViajeId.value)
    toggleVariantes(targetViajeId.value) // Toggle off and back on to refresh
  }
}

const viewDetail = (viajeId) => {
  router.push(`/viajes/${viajeId}`)
}

// Inscription Flow
const resetInscriptionFlow = () => {
  searchDni.value = ''
  selectedSocio.ref = null
  selectedSocio.value = null
  selectedViajeId.value = null
  comboViajes.value = []
  searchError.value = ''
}

const handleSearchSocio = async () => {
  if (!searchDni.value) return
  isSearchingSocio.value = true
  searchError.value = ''
  selectedSocio.value = null

  try {
    const socio = await SociosService.getByDni(searchDni.value)
    selectedSocio.value = socio
    // Load travels for step 2
    comboViajes.value = await ViajesService.getComboBoxViajes()
  } catch (error) {
    searchError.value = 'Socio no encontrado o error en la búsqueda.'
  } finally {
    isSearchingSocio.value = false
  }
}

const confirmInscripcion = () => {
  if (!selectedViajeId.value) return
  isInscripcionModalOpen.value = true
}

const handleFinishInscripcion = () => {
  isInscripcionModalOpen.value = false
  showToast('Inscripción realizada con éxito')
  resetInscriptionFlow()
  currentAction.value = 'none'
}

onMounted(() => {
  // Optional: check query params if redirected here
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb & Page Title -->
      <div class="mb-8">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <router-link to="/"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-teal-600">
                <svg class="w-3 h-3 mr-2.5" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    d="m19.707 9.293-2-2-7-7a1 1 0 0 0-1.414 0l-7 7-2 2a1 1 0 0 0 1.414 1.414L2 10.414V18a2 2 0 0 0 2 2h3a1 1 0 0 0 1-1v-4a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v4a1 1 0 0 0 1 1h3a2 2 0 0 0 2-2v-7.586l.293.293a1 1 0 0 0 1.414-1.414Z" />
                </svg>
                Inicio
              </router-link>
            </li>
            <li>
              <div class="flex items-center">
                <svg class="w-3 h-3 text-slate-400 mx-1" fill="none" viewBox="0 0 6 10" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="m1 9 4-4-4-4" />
                </svg>
                <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2">Gestión de Viajes</span>
              </div>
            </li>
          </ol>
        </nav>
        <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Gestión de Viajes</h2>
        <p class="text-slate-500 mt-1 text-lg">
          Administración de excursiones, variantes e inscripciones.
        </p>
      </div>

      <!-- Coordinated Transition for Main Views -->
      <Transition mode="out-in" enter-active-class="transition duration-400 ease-out"
        enter-from-class="opacity-0 translate-y-4" enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition duration-300 ease-in" leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-4">
        <!-- FIRST VIEW: Initial Action Selector -->
        <div v-if="currentAction === 'none'" key="selector"
          class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 mb-10">
          <button v-for="action in actions" :key="action.id" @click="selectAction(action.id)"
            class="group relative flex flex-col p-6 bg-white rounded-3xl border border-slate-200 shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 text-left overflow-hidden">
            <div
              class="absolute inset-0 bg-gradient-to-br from-slate-50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300">
            </div>

            <div class="relative z-10 flex items-start justify-between mb-5">
              <div
                class="p-4 rounded-2xl transition-all duration-300 shadow-sm ring-1 ring-black/5 group-hover:scale-110"
                :class="[action.bg, action.color]">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" :d="action.icon" />
                </svg>
              </div>
            </div>

            <div class="relative z-10 mt-auto">
              <h3
                class="text-xl font-bold text-slate-900 mb-1 group-hover:text-teal-700 transition-colors duration-300">
                {{ action.title }}
              </h3>
              <p class="text-sm text-slate-500 leading-relaxed font-medium">
                {{ action.description }}
              </p>
            </div>
          </button>
        </div>

        <!-- SECOND VIEW: Active Module (Navbar + Content Container) -->
        <div v-else key="content-view">
          <!-- Navigation Navbar -->
          <div class="flex flex-col sm:flex-row items-center gap-4 mb-8">
            <div
              class="flex flex-wrap items-center gap-2 p-1.5 bg-slate-200/50 rounded-[2rem] w-full sm:w-auto border border-slate-200/50 backdrop-blur-sm shadow-inner">
              <button v-for="action in actions" :key="action.id" @click="selectAction(action.id)"
                class="flex-1 sm:flex-none flex items-center justify-center px-5 py-2.5 rounded-[1.5rem] transition-all duration-300 font-bold text-sm"
                :class="[
                  currentAction === action.id
                    ? 'bg-white text-teal-600 shadow-md border border-slate-200 translate-y-[-1px] scale-[1.02]'
                    : 'text-slate-500 hover:text-slate-700 hover:bg-white/40'
                ]">
                <svg class="w-4 h-4 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="action.icon" />
                </svg>
                {{ action.title }}
              </button>
            </div>

            <button @click="currentAction = 'none'"
              class="hidden sm:flex items-center px-4 py-2 text-slate-400 hover:text-red-500 font-bold text-sm transition-colors group">
              <svg class="w-4 h-4 mr-2 group-hover:-translate-x-1 transition-transform" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
              Volver
            </button>
          </div>

          <!-- Dynamic Content Area -->
          <div class="bg-white rounded-3xl border border-slate-200 shadow-sm p-6 min-h-[400px] overflow-hidden">
            <!-- CREATE ACTION -->
            <div v-if="currentAction === 'create'"
              class="flex flex-col items-center justify-center h-full py-12 text-center animate-in fade-in zoom-in duration-300">
              <div class="p-4 bg-teal-50 rounded-full mb-4">
                <svg class="h-8 w-8 text-teal-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 9v3m0 0v3m0-3h3m-3 0h-3m-9-4.721L12 1l7.71 4.279A2 2 0 0121 7.032V17a2 2 0 01-1.29 1.873L12 21l-7.71-2.127A2 2 0 013 17V7.032a2 2 0 011.29-1.753z" />
                </svg>
              </div>
              <h3 class="text-lg font-medium text-slate-900">Crear nuevo viaje</h3>
              <p class="text-slate-500 mt-2 max-w-md">El formulario se ha abierto en una ventana emergente.</p>
              <button @click="isViajeModalOpen = true"
                class="mt-6 inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500">
                Abrir formulario
              </button>
            </div>

            <!-- LIST ACTION -->
            <div v-else-if="currentAction === 'list'" class="animate-in fade-in slide-in-from-bottom-4 duration-500">
              <div class="flex justify-between items-center mb-6">
                <h3 class="text-xl font-bold text-slate-900">Viajes Disponibles</h3>
                <button @click="fetchViajes"
                  class="text-sm font-bold text-teal-600 hover:text-teal-700 flex items-center">
                  <svg class="w-4 h-4 mr-1" :class="{ 'animate-spin': isLoading }" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                  Actualizar
                </button>
              </div>

              <div v-if="isLoading" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <div v-for="i in 3" :key="i" class="h-64 bg-slate-100 rounded-2xl animate-pulse"></div>
              </div>

              <div v-else-if="viajes.length === 0"
                class="flex flex-col items-center justify-center py-20 text-slate-400">
                <svg class="w-16 h-16 mb-4 opacity-20" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M3.055 11H5a2 2 0 012 2v1a2 2 0 002 2 2 2 0 012 2v2.945M8 3.935V5.5A2.5 2.5 0 0010.5 8h.5a2 2 0 012 2 2 2 0 104 0 2 2 0 012-2h1.064M15 20.488V18a2 2 0 012-2h3.064M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <p class="text-lg font-medium">No se encontraron viajes disponibles.</p>
              </div>

              <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <ViajeCard v-for="viaje in viajes" :key="viaje.id" :viaje="viaje"
                  :is-expanded="expandedViajeId === viaje.id" :variantes="expandedViajeVariantes"
                  :is-loading-variantes="isLoadingVariantes" @toggle-variantes="toggleVariantes(viaje.id)"
                  @create-variante="openCreateVariante(viaje.id)" @view="viewDetail(viaje.id)" />
              </div>
            </div>

            <!-- INSCRIBIR ACTION -->
            <div v-else-if="currentAction === 'inscribir'"
              class="max-w-3xl mx-auto py-4 animate-in fade-in slide-in-from-right-4 duration-500">
              <h3 class="text-xl font-bold text-slate-900 mb-8 flex items-center">
                <span
                  class="flex items-center justify-center w-8 h-8 rounded-full bg-blue-100 text-blue-600 text-sm mr-3">3</span>
                Inscripción de Socio
              </h3>
              <!-- ... resto del flujo de inscripción se mantiene igual ... -->
              <div class="mb-10 p-6 bg-slate-50 rounded-2xl border border-slate-200">
                <h4 class="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4">Paso 1: Identificar Socio
                </h4>
                <div class="flex gap-3">
                  <div class="relative flex-grow">
                    <input type="text" v-model="searchDni" @keyup.enter="handleSearchSocio"
                      placeholder="Ingrese DNI del socio"
                      class="block w-full pl-4 pr-10 py-3 border border-slate-300 rounded-xl leading-5 bg-white shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm" />
                    <div v-if="isSearchingSocio" class="absolute inset-y-0 right-3 flex items-center">
                      <svg class="animate-spin h-5 w-5 text-blue-500" fill="none" viewBox="0 0 24 24">
                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                        </circle>
                        <path class="opacity-75" fill="currentColor"
                          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                        </path>
                      </svg>
                    </div>
                  </div>
                  <button @click="handleSearchSocio" :disabled="isSearchingSocio || !searchDni"
                    class="px-6 py-3 bg-blue-600 text-white font-bold rounded-xl shadow-lg hover:bg-blue-700 transition-all disabled:opacity-50">Buscar</button>
                </div>

                <div v-if="searchError"
                  class="mt-4 text-sm text-red-600 font-medium bg-red-50 p-3 rounded-lg border border-red-100">
                  {{ searchError }}
                </div>

                <div v-if="selectedSocio"
                  class="mt-6 flex items-center p-4 bg-white rounded-xl border border-blue-100 shadow-sm animate-in fade-in slide-in-from-top-2">
                  <div class="w-12 h-12 rounded-full bg-blue-50 flex items-center justify-center text-blue-600 mr-4">
                    <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                        d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg></div>
                  <div>
                    <p class="text-xs text-slate-400 font-bold uppercase">Socio Encontrado</p>
                    <p class="text-lg font-bold text-slate-900">{{ selectedSocio.nombre }} {{ selectedSocio.apellido }}
                    </p>
                    <p class="text-sm text-slate-500">DNI: {{ selectedSocio.dni }}</p>
                  </div>
                  <button @click="resetInscriptionFlow" class="ml-auto p-2 text-slate-400 hover:text-red-500">
                    <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              </div>

              <!-- Step 2: Seleccionar Viaje -->
              <Transition enter-active-class="transition duration-300 ease-out"
                enter-from-class="opacity-0 translate-y-4" enter-to-class="opacity-100 translate-y-0">
                <div v-if="selectedSocio" class="mb-10 p-6 bg-slate-50 rounded-2xl border border-slate-200">
                  <h4 class="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4">
                    Paso 2: Elegir Destino
                  </h4>
                  <div class="space-y-4">
                    <label class="block text-sm font-medium text-slate-700">Viaje disponible</label>
                    <select v-model="selectedViajeId"
                      class="block w-full px-4 py-3 border border-slate-300 rounded-xl bg-white shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm">
                      <option :value="null" disabled>Seleccione un viaje...</option>
                      <option v-for="v in comboViajes" :key="v.idViaje" :value="v.idViaje">
                        {{ v.nombreViaje }}
                      </option>
                    </select>

                    <div class="flex justify-end mt-6">
                      <button @click="confirmInscripcion" :disabled="!selectedViajeId"
                        class="px-8 py-3 bg-blue-600 text-white font-bold rounded-xl shadow-lg hover:bg-blue-700 transition-all disabled:opacity-50 flex items-center">
                        Continuar a Variantes
                        <svg class="w-4 h-4 ml-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M14 5l7 7m0 0l-7 7m7-7H3" />
                        </svg>
                      </button>
                    </div>
                  </div>
                </div>
              </Transition>
            </div>
          </div>
        </div>
      </Transition>
    </main>

    <!-- Modales -->
    <ViajeFormModal :is-open="isViajeModalOpen" @close="isViajeModalOpen = false" @save="handleSaveViaje" />

    <VarianteFormModal :is-open="isVarianteModalOpen" :id-viaje="targetViajeId" @close="isVarianteModalOpen = false"
      @save="handleSaveVariante" />

    <InscripcionConfirmModal v-if="isInscripcionModalOpen" :is-open="isInscripcionModalOpen" :socio="selectedSocio"
      :id-viaje="selectedViajeId" @close="isInscripcionModalOpen = false" @save="handleFinishInscripcion" />

    <!-- Toast Notification -->
    <Transition enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
      enter-to-class="translate-y-0 opacity-100 sm:translate-x-0" leave-active-class="transition ease-in duration-100"
      leave-from-class="opacity-100" leave-to-class="opacity-0">
      <div v-if="toast.show"
        class="fixed bottom-5 right-5 z-50 flex w-full max-w-sm overflow-hidden bg-white rounded-lg shadow-2xl border border-slate-200 pointer-events-auto ring-1 ring-black ring-opacity-5">
        <div class="flex items-center justify-center w-12"
          :class="toast.type === 'success' ? 'bg-emerald-500' : 'bg-red-500'">
          <svg v-if="toast.type === 'success'" class="w-6 h-6 text-white fill-current" viewBox="0 0 40 40">
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM16.6667 28.3333L8.33337 20L10.6834 17.65L16.6667 23.6166L29.3167 10.9666L31.6667 13.3333L16.6667 28.3333Z" />
          </svg>
          <svg v-else class="w-6 h-6 text-white fill-current" viewBox="0 0 40 40">
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM21.6667 28.3333H18.3334V25H21.6667V28.3333ZM21.6667 21.6666H18.3334V11.6666H21.6667V21.6666Z" />
          </svg>
        </div>
        <div class="px-4 py-3 -mx-3 items-center flex">
          <div class="mx-3">
            <p class="text-sm font-bold" :class="toast.type === 'success' ? 'text-emerald-600' : 'text-red-600'">
              {{ toast.type === 'success' ? 'Éxito' : 'Error' }}
            </p>
            <p class="text-xs text-slate-500">{{ toast.message }}</p>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>
