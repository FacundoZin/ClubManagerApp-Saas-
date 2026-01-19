<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import SocioFeeCard from '../../components/ModuloGestionCuotas/SocioFeeCard.vue'
import CuotasService from '../../services/CuotasService'
import SociosService from '../../services/SociosService'

const router = useRouter()

// Toast State
const toast = ref({
  show: false,
  message: '',
  type: 'success',
})

const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  const duration = type === 'error' ? 6000 : 3000
  setTimeout(() => {
    toast.value.show = false
  }, duration)
}

// State
const currentAction = ref('none') // 'none', 'pay', 'update'
const searchDni = ref('')
const searchResult = ref(null)
const searchError = ref('')
const isSearching = ref(false)
const isProcessing = ref(false)

const formaPagoSelected = ref('2') // Default Sede (2)
const nuevoValorCuota = ref(0)

const actions = [
  {
    id: 'pay',
    title: 'Registrar pago de cuota',
    description: 'Buscar un socio por DNI y registrar el pago del semestre actual.',
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    color: 'text-emerald-600',
    bg: 'bg-emerald-50',
    hoverBorder: 'group-hover:border-emerald-200',
  },
  {
    id: 'update',
    title: 'Actualizar valor de cuota',
    description: 'Modificar el monto de la cuota social vigente para todos los socios.',
    icon: 'M12 4v16m8-8H4',
    color: 'text-blue-600',
    bg: 'bg-blue-50',
    hoverBorder: 'group-hover:border-blue-200',
  },
]

const selectAction = (actionId) => {
  currentAction.value = actionId
  searchResult.value = null
  searchDni.value = ''
  searchError.value = ''
  nuevoValorCuota.value = 0
}

const goHome = () => {
  router.push('/')
}

const handleSearch = async () => {
  if (!searchDni.value) return

  isSearching.value = true
  searchError.value = ''
  searchResult.value = null

  try {
    const data = await SociosService.getByDni(searchDni.value)

    if (!data.adeudaCuotas) {
      searchError.value = 'Este socio ya pagó su cuota'
      searchResult.value = data
      return
    }

    searchResult.value = data
  } catch (error) {
    searchError.value = error.message
  } finally {
    isSearching.value = false
  }
}

const handleRegisterPayment = async () => {
  if (!searchResult.value) return

  isProcessing.value = true
  try {
    await CuotasService.registrarCuota({
      socioId: searchResult.value.id,
      formaPago: parseInt(formaPagoSelected.value),
    })

    showToast('El pago se registró correctamente')
    searchResult.value = null
    searchDni.value = ''
  } catch (error) {
    showToast(error.message || 'Algo salió mal al registrar el pago', 'error')
  } finally {
    isProcessing.value = false
  }
}

const handleUpdateValue = async () => {
  if (nuevoValorCuota.value <= 0) {
    showToast('El valor debe ser mayor a 0', 'error')
    return
  }

  isProcessing.value = true
  try {
    await CuotasService.actualizarValor(nuevoValorCuota.value)

    showToast('El valor de la cuota se actualizó correctamente')
    nuevoValorCuota.value = 0
  } catch (error) {
    showToast(error.message || 'Algo salió mal lo sentimos', 'error')
  } finally {
    isProcessing.value = false
  }
}

const handleView = (socio) => {
  router.push(`/socios/${socio.id}`)
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <!-- Header Institucional -->
    <!-- Header Institucional -->

    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb & Page Title -->
      <div class="mb-8">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a
                href="#"
                @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-blue-600"
              >
                <svg
                  class="w-3 h-3 mr-2.5"
                  aria-hidden="true"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="currentColor"
                  viewBox="0 0 20 20"
                >
                  <path
                    d="m19.707 9.293-2-2-7-7a1 1 0 0 0-1.414 0l-7 7-2 2a1 1 0 0 0 1.414 1.414L2 10.414V18a2 2 0 0 0 2 2h3a1 1 0 0 0 1-1v-4a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v4a1 1 0 0 0 1 1h3a2 2 0 0 0 2-2v-7.586l.293.293a1 1 0 0 0 1.414-1.414Z"
                  />
                </svg>
                Inicio
              </a>
            </li>
            <li>
              <div class="flex items-center">
                <svg
                  class="w-3 h-3 text-slate-400 mx-1"
                  aria-hidden="true"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 6 10"
                >
                  <path
                    stroke="currentColor"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="m1 9 4-4-4-4"
                  />
                </svg>
                <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2"
                  >Gestión de Cuotas</span
                >
              </div>
            </li>
          </ol>
        </nav>
        <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Gestión de Cuotas</h2>
        <p class="text-slate-500 mt-1 text-lg">
          Administre el registro de pagos y el valor de la cuota social.
        </p>
      </div>

      <!-- Action Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-6 mb-10">
        <button
          v-for="action in actions"
          :key="action.id"
          @click="selectAction(action.id)"
          class="group relative flex flex-col p-6 bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-lg hover:-translate-y-1 transition-all duration-300 text-left overflow-hidden"
          :class="[
            action.hoverBorder,
            currentAction === action.id ? 'ring-2 ring-blue-500 border-transparent' : '',
          ]"
        >
          <div
            class="absolute inset-0 bg-gradient-to-br from-slate-50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"
          ></div>

          <div class="relative z-10 flex items-start justify-between mb-5">
            <div
              class="p-3.5 rounded-lg transition-colors duration-300 shadow-sm ring-1 ring-black/5"
              :class="[action.bg, action.color]"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-7 w-7"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="1.5"
                  :d="action.icon"
                />
              </svg>
            </div>
          </div>

          <div class="relative z-10 mt-auto">
            <h3
              class="text-lg font-bold text-slate-900 mb-1 group-hover:text-blue-700 transition-colors duration-300"
            >
              {{ action.title }}
            </h3>
            <p class="text-sm text-slate-500 leading-relaxed font-medium">
              {{ action.description }}
            </p>
          </div>
        </button>
      </div>

      <!-- Dynamic Content Area -->
      <div
        v-if="currentAction !== 'none'"
        class="bg-white rounded-xl border border-slate-200 shadow-sm p-6 min-h-[400px]"
      >
        <!-- PAY ACTION -->
        <div v-if="currentAction === 'pay'" class="max-w-2xl mx-auto">
          <div class="mb-8">
            <label for="search-dni" class="block text-sm font-medium text-slate-700 mb-2"
              >Ingrese DNI del socio</label
            >
            <div class="flex gap-2">
              <div class="relative flex-grow">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <svg
                    class="h-5 w-5 text-slate-400"
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 20 20"
                    fill="currentColor"
                  >
                    <path
                      fill-rule="evenodd"
                      d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z"
                      clip-rule="evenodd"
                    />
                  </svg>
                </div>
                <input
                  type="text"
                  id="search-dni"
                  v-model="searchDni"
                  @keyup.enter="handleSearch"
                  class="block w-full pl-10 pr-3 py-2 border border-slate-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="Ej: 12345678"
                />
              </div>
              <button
                @click="handleSearch"
                :disabled="isSearching"
                class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none disabled:opacity-50"
              >
                {{ isSearching ? 'Buscando...' : 'Buscar' }}
              </button>
            </div>
          </div>

          <!-- Error Messages -->
          <div v-if="searchError" class="rounded-md bg-red-50 p-4 mb-6 border border-red-200">
            <div class="flex">
              <div class="flex-shrink-0">
                <svg
                  class="h-5 w-5 text-red-400"
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fill-rule="evenodd"
                    d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                    clip-rule="evenodd"
                  />
                </svg>
              </div>
              <div class="ml-3">
                <p class="text-sm font-medium text-red-800">{{ searchError }}</p>
              </div>
            </div>
          </div>

          <!-- Search Results & Register Form -->
          <div v-if="searchResult">
            <SocioFeeCard :socio="searchResult" @view="handleView" />

            <div
              v-if="searchResult.adeudaCuotas"
              class="mt-8 p-6 bg-slate-50 rounded-lg border border-slate-200"
            >
              <h4 class="text-lg font-bold text-slate-900 mb-4">Registrar Pago</h4>

              <div class="grid grid-cols-1 gap-4 mb-6">
                <div>
                  <label class="block text-sm font-medium text-slate-700 mb-2">Forma de Pago</label>
                  <select
                    v-model="formaPagoSelected"
                    class="block w-full py-2 px-3 border border-slate-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  >
                    <option value="0">Cobrador</option>
                    <option value="1">Link de Pago</option>
                    <option value="2">Sede</option>
                  </select>
                </div>
              </div>

              <button
                @click="handleRegisterPayment"
                :disabled="isProcessing"
                class="w-full inline-flex justify-center items-center px-4 py-3 border border-transparent text-sm font-bold rounded-md shadow-md text-white bg-emerald-600 hover:bg-emerald-700 focus:outline-none disabled:opacity-50 transition-all duration-200"
              >
                <svg
                  v-if="isProcessing"
                  class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
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
                {{ isProcessing ? 'Procesando...' : 'Confirmar Registro de Pago' }}
              </button>
            </div>
          </div>
        </div>

        <!-- UPDATE ACTION -->
        <div v-else-if="currentAction === 'update'" class="max-w-md mx-auto py-12">
          <div class="bg-blue-50 rounded-lg p-6 border border-blue-100 mb-8">
            <div class="flex">
              <div class="flex-shrink-0">
                <svg
                  class="h-6 w-6 text-blue-600"
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
              </div>
              <div class="ml-3">
                <h3 class="text-sm font-medium text-blue-800">Actualizar Valor de Cuota</h3>
                <p class="text-sm text-blue-700 mt-1">
                  El nuevo valor se aplicará a todos los registros de pago futuros.
                </p>
              </div>
            </div>
          </div>

          <div class="space-y-4">
            <div>
              <label for="nuevo-valor" class="block text-sm font-medium text-slate-700 mb-1"
                >Nuevo Valor ($)</label
              >
              <div class="relative rounded-md shadow-sm">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-slate-500 sm:text-sm">$</span>
                </div>
                <input
                  type="number"
                  id="nuevo-valor"
                  v-model="nuevoValorCuota"
                  step="0.01"
                  class="block w-full pl-7 pr-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
                  placeholder="0.00"
                />
              </div>
            </div>

            <button
              @click="handleUpdateValue"
              :disabled="isProcessing"
              class="w-full inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none disabled:opacity-50"
            >
              <svg
                v-if="isProcessing"
                class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
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
              {{ isProcessing ? 'Actualizando...' : 'Guardar Nuevo Valor' }}
            </button>
          </div>
        </div>
      </div>
      <!-- Dynamic Content Area end -->
    </main>

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
          :class="{
            'bg-emerald-500': toast.type === 'success',
            'bg-blue-500': toast.type === 'info',
            'bg-red-500': toast.type === 'error',
          }"
        >
          <svg
            v-if="toast.type === 'success'"
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 40 40"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM16.6667 28.3333L8.33337 20L10.6834 17.65L16.6667 23.6166L29.3167 10.9666L31.6667 13.3333L16.6667 28.3333Z"
            />
          </svg>
          <svg
            v-else-if="toast.type === 'info'"
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <svg
            v-else
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 40 40"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM21.6667 28.3333H18.3334V25H21.6667V28.3333ZM21.6667 21.6666H18.3334V11.6666H21.6667V21.6666Z"
            />
          </svg>
        </div>

        <div class="px-4 py-3 -mx-3">
          <div class="mx-3">
            <span
              class="font-semibold"
              :class="{
                'text-emerald-500': toast.type === 'success',
                'text-blue-500': toast.type === 'info',
                'text-red-500': toast.type === 'error',
              }"
            >
              {{ toast.type === 'success' ? 'Éxito' : toast.type === 'info' ? 'Info' : 'Error' }}
            </span>
            <p class="text-sm text-slate-600">
              {{ toast.message }}
            </p>
          </div>
        </div>

        <button
          @click="toast.show = false"
          class="ml-auto p-2 text-slate-400 hover:text-slate-600 focus:outline-none"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>
    </Transition>
  </div>
</template>
