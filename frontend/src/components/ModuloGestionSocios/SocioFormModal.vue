<script setup>
import { reactive, ref, onMounted, watch } from 'vue'
import ConfirmModal from '../Common/ConfirmModal.vue'
import CobranzasService from '../../services/CobranzasService'
import SociosService from '../../services/SociosService'

const props = defineProps({
  isOpen: Boolean,
  socio: {
    type: Object,
    default: null,
  },
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  nombre: null,
  apellido: null,
  dni: null,
  telefono: null,
  direcccion: null, // Note: keeping typo to match DTO
  idLote: null,
  localidad: null,
  preferenciaDePago: null,
})

const paymentOptions = [
  { value: 0, label: 'Cobrador' },
  { value: 1, label: 'Link de Pago' },
  { value: 2, label: 'Sede' },
]

const lotes = ref([])
const isLoadingLotes = ref(false)

const fetchLotes = async () => {
  try {
    isLoadingLotes.value = true
    lotes.value = await CobranzasService.listarLotes()
  } catch (error) {
    console.error('Error fetching lotes:', error)
  } finally {
    isLoadingLotes.value = false
  }
}

onMounted(() => {
  fetchLotes()
})

// Watch for modal opening to ensure lotes are refreshed if needed
watch(
  () => props.isOpen,
  (newVal) => {
    if (newVal) {
      if (lotes.value.length === 0) fetchLotes()
    } else {
      resetForm()
    }
  },
)

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.nombre = null
  form.apellido = null
  form.dni = null
  form.telefono = null
  form.direcccion = null
  form.idLote = null
  form.localidad = null
  form.preferenciaDePago = null
  errorMessage.value = ''
}

const showConfirmModal = ref(false)
const confirmModalConfig = reactive({
  title: '',
  message: '',
  confirmText: 'Reactivar',
  socioId: null,
})

const handleReactivationConfirm = async () => {
  try {
    const socioId = confirmModalConfig.socioId
    const data = await SociosService.reactivar(socioId)
    emit('save', data)
    resetForm()
    showConfirmModal.value = false
  } catch (error) {
    errorMessage.value = error.message
    showConfirmModal.value = false
  }
}

const handleSubmit = async () => {
  isSubmitting.value = true
  errorMessage.value = ''

  try {
    // Sanitize data: convert empty values to null or appropriate types
    const sanitizedForm = Object.fromEntries(
      Object.entries(form).map(([key, value]) => [
        key,
        value === '' || value === null ? null : key === 'idLote' ? Number(value) : value,
      ]),
    )

    const data = await SociosService.create(sanitizedForm)
    emit('save', data)
    resetForm()
  } catch (error) {
    if (error.status === 409) {
      const { data } = error.data
      if (data && data.id) {
        confirmModalConfig.title = 'Socio Inactivo Detectado'
        confirmModalConfig.message = `El socio ${data.nombre} ya existe en el sistema pero está dado de baja. ¿Desea reactivarlo?`
        confirmModalConfig.socioId = data.id
        showConfirmModal.value = true
        isSubmitting.value = false
        return
      }
      errorMessage.value = 'El socio existe pero está inactivo.'
    } else {
      errorMessage.value = error.message
    }
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-50 overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <!-- Background backdrop with blur -->
    <div
      class="fixed inset-0 bg-slate-900/30 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <!-- Header -->
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4 border-b border-slate-100">
          <div class="sm:flex sm:items-start">
            <div
              class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-blue-100 sm:mx-0 sm:h-12 sm:w-12"
            >
              <svg
                class="h-6 w-6 text-blue-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
                aria-hidden="true"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M19 7.5v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z"
                />
              </svg>
            </div>
            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
              <h3 class="text-xl font-bold leading-6 text-slate-900" id="modal-title">
                Registrar Nuevo Socio
              </h3>
              <div class="mt-1">
                <p class="text-sm font-medium text-slate-500">
                  Complete la información para dar de alta un nuevo socio.
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-5 sm:p-6 space-y-4">
            <div
              v-if="errorMessage"
              class="p-3 rounded-xl bg-red-50 border border-red-100 text-red-700 text-sm flex items-start gap-2"
            >
              <svg
                class="h-5 w-5 text-red-400 shrink-0"
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
              {{ errorMessage }}
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label for="nombre" class="block text-sm font-bold text-slate-700">Nombre</label>
                <input
                  type="text"
                  id="nombre"
                  v-model="form.nombre"
                  required
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-3 py-2 border transition-all"
                  placeholder="Juan"
                />
              </div>
              <div class="space-y-1">
                <label for="apellido" class="block text-sm font-bold text-slate-700"
                  >Apellido</label
                >
                <input
                  type="text"
                  id="apellido"
                  v-model="form.apellido"
                  required
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-3 py-2 border transition-all"
                  placeholder="Pérez"
                />
              </div>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label for="dni" class="block text-sm font-bold text-slate-700">DNI</label>
                <input
                  type="text"
                  id="dni"
                  v-model="form.dni"
                  required
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-3 py-2 border transition-all"
                  placeholder="12345678"
                />
              </div>
              <div class="space-y-1">
                <label for="telefono" class="block text-sm font-bold text-slate-700"
                  >Teléfono</label
                >
                <input
                  type="tel"
                  id="telefono"
                  v-model="form.telefono"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-3 py-2 border transition-all"
                  placeholder="11 1234-5678"
                />
              </div>
            </div>

            <div class="space-y-1">
              <label for="direccion" class="block text-sm font-bold text-slate-700"
                >Dirección</label
              >
              <input
                type="text"
                id="direccion"
                v-model="form.direcccion"
                class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-4 py-2 border transition-all"
                placeholder="Calle y número (p. ej. Av. Siempreviva 742)"
              />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1">
                <label for="lote" class="block text-sm font-bold text-slate-700">Lote / Zona</label>
                <select
                  id="lote"
                  v-model="form.idLote"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-3 py-2 border transition-all bg-white"
                >
                  <option :value="null" disabled>Seleccionar Lote / Zona</option>
                  <option v-for="lote in lotes" :key="lote.id" :value="lote.id">
                    {{ lote.nombreLote }}
                  </option>
                </select>
              </div>
              <div class="space-y-1">
                <label for="localidad" class="block text-sm font-bold text-slate-700"
                  >Localidad</label
                >
                <input
                  type="text"
                  id="localidad"
                  v-model="form.localidad"
                  class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-3 py-2 border transition-all"
                  placeholder="Morón"
                />
              </div>
            </div>

            <div class="space-y-1">
              <label for="preferenciaDePago" class="block text-sm font-bold text-slate-700"
                >Preferencia de Pago</label
              >
              <select
                id="preferenciaDePago"
                v-model="form.preferenciaDePago"
                required
                class="block w-full rounded-xl border-slate-200 bg-slate-50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 sm:text-sm px-3 py-2 border transition-all bg-white"
              >
                <option :value="null" disabled>Seleccionar forma de pago</option>
                <option v-for="option in paymentOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </option>
              </select>
            </div>
          </div>

          <!-- Footer Actions -->
          <div
            class="bg-slate-50 px-4 py-3 flex gap-3 sm:flex-row-reverse sm:px-6 border-t border-slate-100"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex flex-1 justify-center items-center gap-2 rounded-xl bg-blue-600 px-4 py-2.5 text-sm font-bold text-white shadow-lg shadow-blue-100 hover:bg-blue-700 sm:ml-3 sm:w-auto sm:flex-none disabled:opacity-50 transition-all active:scale-95"
            >
              <svg
                v-if="isSubmitting"
                class="animate-spin h-4 w-4 text-white"
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
              {{ isSubmitting ? 'Guardando...' : 'Guardar Socio' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="inline-flex flex-1 justify-center rounded-xl bg-white px-4 py-2.5 text-sm font-bold text-slate-700 shadow-sm ring-1 ring-inset ring-slate-200 hover:bg-slate-50 sm:w-auto sm:flex-none transition-all active:scale-95"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>

  <ConfirmModal
    :is-open="showConfirmModal"
    :title="confirmModalConfig.title"
    :message="confirmModalConfig.message"
    :confirm-text="confirmModalConfig.confirmText"
    type="info"
    @close="showConfirmModal = false"
    @confirm="handleReactivationConfirm"
  />
</template>
