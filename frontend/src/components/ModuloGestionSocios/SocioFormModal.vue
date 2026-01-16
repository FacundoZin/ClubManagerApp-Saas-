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
  nombre: '',
  apellido: '',
  dni: '',
  telefono: '',
  direcccion: '', // Note: keeping typo to match DTO
  idLote: '',
  localidad: '',
  idLote: '',
  localidad: '',
  preferenciaDePago: '',
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
    if (newVal && lotes.value.length === 0) {
      fetchLotes()
    }
  },
)

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.nombre = ''
  form.apellido = ''
  form.dni = ''
  form.telefono = ''
  form.direcccion = ''
  form.idLote = ''
  form.localidad = ''
  form.preferenciaDePago = ''
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
    const data = await SociosService.create(form)
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
        class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <!-- Header -->
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4 border-b border-slate-100">
          <div class="sm:flex sm:items-start">
            <div
              class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-blue-100 sm:mx-0 sm:h-10 sm:w-10"
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
              <h3 class="text-lg font-semibold leading-6 text-slate-900" id="modal-title">
                Registrar Nuevo Socio
              </h3>
              <div class="mt-2">
                <p class="text-sm text-slate-500">
                  Complete la información para dar de alta un nuevo socio en el sistema.
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-5 sm:p-6 space-y-4">
            <div v-if="errorMessage" class="p-3 rounded-md bg-red-50 text-red-700 text-sm mb-4">
              {{ errorMessage }}
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label for="nombre" class="block text-sm font-medium text-slate-700">Nombre</label>
                <input
                  type="text"
                  id="nombre"
                  v-model="form.nombre"
                  required
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                />
              </div>
              <div>
                <label for="apellido" class="block text-sm font-medium text-slate-700"
                  >Apellido</label
                >
                <input
                  type="text"
                  id="apellido"
                  v-model="form.apellido"
                  required
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                />
              </div>
            </div>

            <div>
              <label for="dni" class="block text-sm font-medium text-slate-700">DNI</label>
              <input
                type="text"
                id="dni"
                v-model="form.dni"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
              />
            </div>

            <div>
              <label for="telefono" class="block text-sm font-medium text-slate-700"
                >Teléfono</label
              >
              <input
                type="tel"
                id="telefono"
                v-model="form.telefono"
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
              />
            </div>

            <div>
              <label for="direccion" class="block text-sm font-medium text-slate-700"
                >Dirección</label
              >
              <input
                type="text"
                id="direccion"
                v-model="form.direcccion"
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
              />
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label for="lote" class="block text-sm font-medium text-slate-700"
                  >Lote / Zona</label
                >
                <select
                  id="lote"
                  v-model="form.idLote"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border bg-white"
                >
                  <option value="">Seleccione un lote</option>
                  <option v-for="lote in lotes" :key="lote.id" :value="lote.id">
                    {{ lote.nombreLote }}
                  </option>
                </select>
                <p v-if="isLoadingLotes" class="mt-1 text-xs text-slate-500 italic">
                  Cargando lotes...
                </p>
              </div>
              <div>
                <label for="localidad" class="block text-sm font-medium text-slate-700"
                  >Localidad</label
                >
                <input
                  type="text"
                  id="localidad"
                  v-model="form.localidad"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                />
              </div>
            </div>

            <div>
              <label for="preferenciaDePago" class="block text-sm font-medium text-slate-700"
                >Preferencia de Pago</label
              >
              <select
                id="preferenciaDePago"
                v-model="form.preferenciaDePago"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border bg-white"
              >
                <option value="" disabled>Seleccione una forma de pago</option>
                <option v-for="option in paymentOptions" :key="option.value" :value="option.value">
                  {{ option.label }}
                </option>
              </select>
            </div>
          </div>

          <!-- Footer Actions -->
          <div
            class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 border-t border-slate-200"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex w-full justify-center rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 sm:ml-3 sm:w-auto disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {{ isSubmitting ? 'Guardando...' : 'Guardar Socio' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-slate-900 shadow-sm ring-1 ring-inset ring-slate-300 hover:bg-slate-50 sm:mt-0 sm:w-auto transition-colors"
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
