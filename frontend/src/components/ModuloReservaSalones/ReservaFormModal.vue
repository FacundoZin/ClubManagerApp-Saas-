<script setup>
import { reactive, ref, computed, watch } from 'vue'
import SalonService from '../../services/SalonService'

const props = defineProps({
  isOpen: Boolean,
  salones: {
    type: Array,
    default: () => [],
  },
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  titulo: '',
  fecha: '',
  salonId: '',
  dniSocio: '',
  importe: 0,
  totalPagado: 0,
})

const isSubmitting = ref(false)
const errorMessage = ref('')

const resetForm = () => {
  form.titulo = ''
  form.fecha = ''
  form.salonId = ''
  form.dniSocio = ''
  form.importe = 0
  form.totalPagado = 0
  errorMessage.value = ''
}

// Limpiar el mensaje de error cuando el usuario modifique cualquier campo del formulario
watch(
  () => ({ ...form }),
  () => {
    if (errorMessage.value) errorMessage.value = ''
  },
  { deep: true },
)

// Limpiar el formulario cuando se cierra el modal
watch(
  () => props.isOpen,
  (val) => {
    if (!val) {
      resetForm()
    }
  },
)

const handleSubmit = async () => {
  // Validate
  if (!form.fecha || !form.salonId || !form.dniSocio) {
    errorMessage.value = 'Por favor complete los campos requeridos (*)'
    return
  }

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const dto = {
      Titulo: form.titulo,
      Fecha: form.fecha,
      SalonId: parseInt(form.salonId),
      DniSocio: form.dniSocio,
      Importe: parseFloat(form.importe),
      TotalPagado: parseFloat(form.totalPagado),
    }

    const result = await SalonService.createReserva(dto)

    emit('save', result)
    resetForm()
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
    class="fixed inset-0 z-50 overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <div
      class="fixed inset-0 bg-slate-900/30 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4 border-b border-slate-100">
          <div class="sm:flex sm:items-start">
            <div
              class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-blue-100 sm:mx-0 sm:h-10 sm:w-10"
            >
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
                  d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                />
              </svg>
            </div>
            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
              <h3 class="text-lg font-semibold leading-6 text-slate-900" id="modal-title">
                Registrar Nueva Reserva
              </h3>
              <div class="mt-2">
                <p class="text-sm text-slate-500">
                  Complete los datos para agendar una nueva reserva de salón.
                </p>
              </div>
            </div>
          </div>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-5 sm:p-6 space-y-4">
            <div v-if="errorMessage" class="p-3 rounded-md bg-red-50 text-red-700 text-sm mb-4">
              {{ errorMessage }}
            </div>

            <div>
              <label for="titulo" class="block text-sm font-medium text-slate-700"
                >Título / Motivo</label
              >
              <input
                type="text"
                id="titulo"
                v-model="form.titulo"
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                placeholder="Ej: Cumpleaños de 15"
              />
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label for="salon" class="block text-sm font-medium text-slate-700">Salón *</label>
                <select
                  id="salon"
                  v-model="form.salonId"
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                >
                  <option value="" disabled>Seleccione un salón</option>
                  <option v-for="salon in salones" :key="salon.id" :value="salon.id">
                    {{ salon.nombre }}
                  </option>
                </select>
              </div>
              <div>
                <label for="fecha" class="block text-sm font-medium text-slate-700">Fecha *</label>
                <input
                  type="date"
                  id="fecha"
                  v-model="form.fecha"
                  required
                  class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                />
              </div>
            </div>

            <div>
              <label for="dniSocio" class="block text-sm font-medium text-slate-700"
                >DNI Socio *</label
              >
              <input
                type="text"
                id="dniSocio"
                v-model="form.dniSocio"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                placeholder="Ingrese DNI del socio titular"
              />
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label for="importe" class="block text-sm font-medium text-slate-700"
                  >Importe Total</label
                >
                <div class="relative mt-1 rounded-md shadow-sm">
                  <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                    <span class="text-slate-500 sm:text-sm">$</span>
                  </div>
                  <input
                    type="number"
                    id="importe"
                    v-model.number="form.importe"
                    step="0.01"
                    min="0"
                    class="block w-full rounded-md border-slate-300 pl-7 pr-3 focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                  />
                </div>
              </div>
              <div>
                <label for="totalPagado" class="block text-sm font-medium text-slate-700"
                  >Monto Abonado</label
                >
                <div class="relative mt-1 rounded-md shadow-sm">
                  <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
                    <span class="text-slate-500 sm:text-sm">$</span>
                  </div>
                  <input
                    type="number"
                    id="totalPagado"
                    v-model.number="form.totalPagado"
                    step="0.01"
                    min="0"
                    class="block w-full rounded-md border-slate-300 pl-7 pr-3 focus:border-blue-500 focus:ring-blue-500 sm:text-sm px-3 py-2 border"
                  />
                </div>
              </div>
            </div>
          </div>

          <div
            class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 border-t border-slate-200"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex w-full justify-center rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 sm:ml-3 sm:w-auto disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {{ isSubmitting ? 'Guardando...' : 'Crear Reserva' }}
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
</template>
