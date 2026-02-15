<script setup>
import { ref, onMounted } from 'vue'
import AlquilerService from '../../../services/AlquilerService'

const props = defineProps({
  isOpen: Boolean,
  alquilerId: Number,
})

const emit = defineEmits(['close', 'saved'])

const mes = ref(new Date().getMonth() + 1)
const anio = ref(new Date().getFullYear())
const loading = ref(false)
const error = ref('')

const isMesOpen = ref(false)
const isAnioOpen = ref(false)

const meses = [
  { value: 1, label: 'Enero' },
  { value: 2, label: 'Febrero' },
  { value: 3, label: 'Marzo' },
  { value: 4, label: 'Abril' },
  { value: 5, label: 'Mayo' },
  { value: 6, label: 'Junio' },
  { value: 7, label: 'Julio' },
  { value: 8, label: 'Agosto' },
  { value: 9, label: 'Septiembre' },
  { value: 10, label: 'Octubre' },
  { value: 11, label: 'Noviembre' },
  { value: 12, label: 'Diciembre' },
]

const years = ref([])

onMounted(() => {
  const currentYear = new Date().getFullYear()
  // Años desde hace 5 hasta el actual estrictamente
  for (let i = currentYear - 5; i <= currentYear; i++) {
    years.value.push(i)
  }
})

const selectMes = (m) => {
  mes.value = m
  isMesOpen.value = false
}

const selectAnio = (y) => {
  anio.value = y
  isAnioOpen.value = false
}

const toggleMesDropdown = () => {
  isMesOpen.value = !isMesOpen.value
  isAnioOpen.value = false
}

const toggleAnioDropdown = () => {
  isAnioOpen.value = !isAnioOpen.value
  isMesOpen.value = false
}

const handleClose = () => {
  error.value = ''
  isMesOpen.value = false
  isAnioOpen.value = false
  emit('close')
}

const handleSubmit = async () => {
  loading.value = true
  error.value = ''
  try {
    await AlquilerService.registerPayment(props.alquilerId, mes.value, anio.value)
    emit('saved', 'Pago registrado correctamente')
    handleClose()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <Transition
    enter-active-class="ease-out duration-300"
    enter-from-class="opacity-0"
    enter-to-class="opacity-100"
    leave-active-class="ease-in duration-200"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto">
      <div
        class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0"
      >
        <!-- Backdrop Backdrop -->
        <div
          class="fixed inset-0 bg-slate-900/40 backdrop-blur-[2px] transition-opacity"
          aria-hidden="true"
          @click="handleClose"
        ></div>

        <!-- Centering trick -->
        <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true"
          >&#8203;</span
        >

        <!-- Modal Card -->
        <div
          class="relative inline-block align-bottom bg-white rounded-3xl text-left shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-md sm:w-full border border-slate-100 z-10 overflow-hidden"
        >
          <div class="bg-white px-6 pt-6 pb-4 sm:p-8 sm:pb-6">
            <div class="sm:flex sm:items-start">
              <div
                class="mx-auto flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-full bg-teal-50 sm:mx-0 sm:h-10 sm:w-10 border border-teal-100"
              >
                <svg
                  class="h-6 w-6 text-teal-600"
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
              </div>
              <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left w-full">
                <h3 class="text-xl leading-6 font-bold text-slate-900">Registrar Pago</h3>
                <p class="mt-2 text-sm text-slate-500">
                  Seleccione el período correspondiente al pago.
                </p>

                <div class="mt-6 space-y-4">
                  <div class="grid grid-cols-2 gap-4">
                    <!-- Custom Mes Dropdown -->
                    <div class="relative">
                      <label
                        class="block text-xs font-bold text-slate-400 uppercase tracking-wider mb-2"
                        >Mes</label
                      >
                      <button
                        @click="toggleMesDropdown"
                        type="button"
                        class="w-full flex items-center justify-between bg-white border border-slate-200 text-slate-900 text-sm rounded-2xl p-2.5 hover:border-teal-500 transition-all focus:ring-2 focus:ring-teal-500/20 shadow-sm"
                      >
                        <span class="font-medium">{{
                          meses.find((m) => m.value === mes)?.label
                        }}</span>
                        <svg
                          class="w-4 h-4 text-slate-400 transition-transform"
                          :class="{ 'rotate-180': isMesOpen }"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M19 9l-7 7-7-7"
                          />
                        </svg>
                      </button>

                      <div
                        v-if="isMesOpen"
                        class="absolute z-20 mt-1 w-full bg-white border border-slate-200 rounded-2xl shadow-lg ring-1 ring-black/5 overflow-hidden animate-in fade-in slide-in-from-top-1 duration-200"
                      >
                        <div
                          class="overflow-y-auto py-1 custom-scrollbar"
                          style="max-height: 144px"
                        >
                          <div
                            v-for="m in meses"
                            :key="m.value"
                            @click="selectMes(m.value)"
                            class="px-4 py-2.5 text-sm text-slate-600 hover:bg-slate-50 hover:text-teal-600 cursor-pointer transition-colors border-b border-slate-50 last:border-0"
                            :class="{ 'bg-teal-50 text-teal-700 font-bold': mes === m.value }"
                          >
                            {{ m.label }}
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Custom Año Dropdown -->
                    <div class="relative">
                      <label
                        class="block text-xs font-bold text-slate-400 uppercase tracking-wider mb-2"
                        >Año</label
                      >
                      <button
                        @click="toggleAnioDropdown"
                        type="button"
                        class="w-full flex items-center justify-between bg-white border border-slate-200 text-slate-900 text-sm rounded-2xl p-2.5 hover:border-teal-500 transition-all focus:ring-2 focus:ring-teal-500/20 shadow-sm"
                      >
                        <span class="font-medium">{{ anio }}</span>
                        <svg
                          class="w-4 h-4 text-slate-400 transition-transform"
                          :class="{ 'rotate-180': isAnioOpen }"
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                        >
                          <path
                            stroke-linecap="round"
                            stroke-linejoin="round"
                            stroke-width="2"
                            d="M19 9l-7 7-7-7"
                          />
                        </svg>
                      </button>

                      <div
                        v-if="isAnioOpen"
                        class="absolute z-20 mt-1 w-full bg-white border border-slate-200 rounded-2xl shadow-lg ring-1 ring-black/5 overflow-hidden animate-in fade-in slide-in-from-top-1 duration-200"
                      >
                        <div class="py-1">
                          <div
                            v-for="y in years.slice().reverse()"
                            :key="y"
                            @click="selectAnio(y)"
                            class="px-4 py-2.5 text-sm text-slate-600 hover:bg-slate-50 hover:text-teal-600 cursor-pointer transition-colors border-b border-slate-50 last:border-0"
                            :class="{ 'bg-teal-50 text-teal-700 font-bold': anio === y }"
                          >
                            {{ y }}
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <div
                    v-if="error"
                    class="p-3 bg-rose-50 border border-rose-100 rounded-2xl flex items-start gap-3 animate-head-shake"
                  >
                    <svg
                      class="h-5 w-5 text-rose-500 shrink-0 mt-0.5"
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
                    <p class="text-sm text-rose-600 font-medium">{{ error }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div
            class="bg-slate-50 px-6 py-4 sm:px-8 sm:flex sm:flex-row-reverse gap-3 border-t border-slate-100 rounded-b-3xl"
          >
            <button
              @click="handleSubmit"
              :disabled="loading"
              type="button"
              class="w-full inline-flex justify-center rounded-2xl border border-transparent shadow-sm px-6 py-2.5 bg-teal-600 text-base font-semibold text-white hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 sm:w-auto transition-all disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <svg
                v-if="loading"
                class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
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
              {{ loading ? 'Registrando...' : 'Confirmar Pago' }}
            </button>
            <button
              @click="handleClose"
              type="button"
              class="mt-3 w-full inline-flex justify-center rounded-2xl border border-slate-200 shadow-sm px-6 py-2.5 bg-white text-base font-semibold text-slate-700 hover:bg-slate-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 sm:mt-0 sm:w-auto transition-all"
            >
              Cancelar
            </button>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
@keyframes head-shake {
  0% {
    transform: translateX(0);
  }
  6.5% {
    transform: translateX(-6px) rotateY(-9deg);
  }
  18.5% {
    transform: translateX(5px) rotateY(7deg);
  }
  31.5% {
    transform: translateX(-3px) rotateY(-5deg);
  }
  43.5% {
    transform: translateX(2px) rotateY(3deg);
  }
  50% {
    transform: translateX(0);
  }
}
.animate-head-shake {
  animation: head-shake 0.8s ease-in-out;
}

.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: #e2e8f0;
  border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #cbd5e1;
}
</style>
