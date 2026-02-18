<script setup>
import { ref, onMounted, watch } from 'vue'
import ViajesService from '../../services/viajesService'
import SociosService from '../../services/SociosService'

const props = defineProps({
  isOpen: Boolean,
  socio: Object,
  idViaje: Number,
})

const emit = defineEmits(['close', 'save'])

const localSocio = ref(props.socio)
const searchDni = ref('')
const isSearchingSocio = ref(false)
const variantes = ref([])
const isLoading = ref(false)
const selectedVarianteId = ref(null)
const selectedVariante = ref(null)
const montoAbonado = ref(null)
const isSubmitting = ref(false)
const errorMessage = ref('')

const fetchVariantes = async () => {
  if (!props.idViaje) return
  isLoading.value = true
  try {
    variantes.value = await ViajesService.listarVariantesDeViaje(props.idViaje)
  } catch (error) {
    errorMessage.value = 'Error al cargar las variantes del viaje.'
  } finally {
    isLoading.value = false
  }
}

const handleSearchSocio = async () => {
  if (!searchDni.value) return
  isSearchingSocio.value = true
  errorMessage.value = ''
  try {
    const s = await SociosService.getByDni(searchDni.value)
    localSocio.value = s
  } catch (err) {
    errorMessage.value = 'Socio no encontrado.'
  } finally {
    isSearchingSocio.value = false
  }
}

onMounted(() => {
  fetchVariantes()
})

watch(selectedVarianteId, (newId) => {
  selectedVariante.value = variantes.value.find((v) => v.id === newId)
})

const formatCurrency = (value) => {
  if (value === null || value === undefined) return '$0,00'
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const selectVariante = (variante) => {
  selectedVarianteId.value = variante.id
}

const handleSubmit = async () => {
  if (!selectedVarianteId.value) {
    errorMessage.value = 'Debe seleccionar una variante.'
    return
  }
  if (!localSocio.value) {
    errorMessage.value = 'Debe seleccionar un socio.'
    return
  }
  if (!montoAbonado.value) {
    errorMessage.value = 'Debe ingresar un monto a abonar.'
    return
  }

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    await ViajesService.inscribirSocio({
      viajeVarianteId: selectedVarianteId.value,
      socioId: localSocio.value.id,
      montoAbonado: montoAbonado.value,
    })
    emit('save')
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-[70] overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <div
      class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-2xl border border-slate-200"
      >
        <!-- Header -->
        <div class="bg-white px-6 py-5 border-b border-slate-100 flex items-center justify-between">
          <div>
            <h3 class="text-xl font-bold text-slate-900">Confirmar Inscripción</h3>
            <p v-if="localSocio" class="text-sm text-slate-500">
              Socio:
              <span class="font-bold text-slate-700"
                >{{ localSocio.nombre }} {{ localSocio.apellido }}</span
              >
              (DNI: {{ localSocio.dni }})
            </p>
            <p v-else class="text-sm text-slate-500">
              Seleccione el socio para inscribir al viaje.
            </p>
          </div>
          <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 p-2">
            <svg class="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </button>
        </div>

        <div class="px-6 py-6 overflow-y-auto max-h-[70vh]">
          <div
            v-if="errorMessage"
            class="mb-6 p-4 bg-red-50 border border-red-100 rounded-xl text-red-700 text-sm"
          >
            {{ errorMessage }}
          </div>

          <!-- Socio Search (if not provided) -->
          <div v-if="!props.socio && !localSocio" class="mb-8">
            <p class="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4">
              1. BUSCAR SOCIO POR DNI
            </p>
            <div class="flex gap-2">
              <input
                type="text"
                v-model="searchDni"
                placeholder="Ej: 12345678"
                @keyup.enter="handleSearchSocio"
                class="block w-full px-4 py-2 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
              />
              <button
                @click="handleSearchSocio"
                :disabled="isSearchingSocio"
                class="px-4 py-2 bg-slate-900 text-white font-bold rounded-xl disabled:opacity-50"
              >
                {{ isSearchingSocio ? '...' : 'Buscar' }}
              </button>
            </div>
          </div>

          <p
            v-if="localSocio"
            class="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4"
          >
            {{ props.socio ? '1.' : '2.' }} ELIJA LA VARIANTE
          </p>

          <div v-if="isLoading" class="flex justify-center py-10">
            <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600"></div>
          </div>

          <div v-else-if="localSocio && variantes.length === 0" class="text-center py-10">
            <p class="text-slate-500 italic">No hay variantes disponibles para este viaje.</p>
          </div>

          <div v-else-if="localSocio" class="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8">
            <button
              v-for="v in variantes"
              :key="v.id"
              @click="selectVariante(v)"
              class="relative p-4 text-left border rounded-2xl transition-all group"
              :class="
                selectedVarianteId === v.id
                  ? 'border-blue-500 bg-blue-50 ring-1 ring-blue-500'
                  : 'border-slate-200 hover:border-blue-300 hover:bg-slate-50'
              "
            >
              <div class="flex justify-between items-start mb-2">
                <span class="font-bold text-slate-900 group-hover:text-blue-700">{{
                  v.nombreVariante
                }}</span>
                <div
                  v-if="selectedVarianteId === v.id"
                  class="w-5 h-5 bg-blue-600 rounded-full flex items-center justify-center"
                >
                  <svg
                    class="w-3 h-3 text-white"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="3"
                      d="M5 13l4 4L19 7"
                    />
                  </svg>
                </div>
              </div>
              <div class="space-y-1">
                <p class="text-sm text-slate-600 font-medium">
                  Total: {{ formatCurrency(v.valorViaje) }}
                </p>
                <p class="text-[11px] text-slate-400">
                  Seña Mínima: {{ formatCurrency(v.valorSeña) }}
                </p>
                <div class="flex gap-2 mt-2">
                  <span class="text-[10px] bg-slate-200 px-1.5 py-0.5 rounded text-slate-600">{{
                    v.regimen === 0 ? 'M. Pensión' : 'P. Completa'
                  }}</span>
                  <span class="text-[10px] bg-slate-200 px-1.5 py-0.5 rounded text-slate-600">{{
                    v.tipoDeButaca
                  }}</span>
                </div>
              </div>
            </button>
          </div>

          <div
            v-if="selectedVarianteId"
            class="animate-in fade-in slide-in-from-bottom-4 duration-500"
          >
            <p class="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4">
              {{ props.socio ? '2.' : '3.' }} MONTO A ABONAR
            </p>
            <div class="bg-slate-50 p-4 rounded-xl border border-slate-200">
              <label for="montoInscripcion" class="block text-sm font-medium text-slate-700 mb-2"
                >Monto de entrega inicial ($)</label
              >
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="text-slate-500 sm:text-sm">$</span>
                </div>
                <input
                  type="text"
                  id="montoInscripcion"
                  v-model="montoAbonado"
                  required
                  :placeholder="
                    selectedVariante ? `Ej: ${selectedVariante.valorSeña}` : 'Ingrese el monto'
                  "
                  class="block w-full pl-7 pr-12 py-3 border border-slate-300 rounded-xl leading-5 bg-white shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-slate-900"
                  @input="montoAbonado = $event.target.value.replace(/[^0-9.]/g, '')"
                />
              </div>
              <p class="mt-2 text-[11px] text-slate-500 italic">
                Debe ser al menos {{ formatCurrency(selectedVariante?.valorSeña) }} y no superar
                {{ formatCurrency(selectedVariante?.valorViaje) }}.
              </p>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div
          class="bg-slate-50 px-6 py-4 flex flex-col sm:flex-row-reverse gap-3 border-t border-slate-200"
        >
          <button
            @click="handleSubmit"
            :disabled="isSubmitting || !selectedVarianteId || !localSocio"
            class="inline-flex justify-center px-6 py-3 bg-blue-600 text-white font-bold rounded-xl shadow-lg hover:bg-blue-700 transition-all disabled:opacity-50"
          >
            {{ isSubmitting ? 'Inscribiendo...' : 'Confirmar Inscripción' }}
          </button>
          <button
            @click="$emit('close')"
            class="inline-flex justify-center px-6 py-3 bg-white text-slate-900 font-bold rounded-xl border border-slate-200 hover:bg-slate-50 transition-all"
          >
            Cancelar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Estilos preventivos, aunque el type="text" ya elimina las flechas */
.no-spinner::-webkit-inner-spin-button,
.no-spinner::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.no-spinner {
  -moz-appearance: textfield;
  appearance: none;
}
</style>
