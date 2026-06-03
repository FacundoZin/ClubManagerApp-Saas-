<script setup>
import { ref, onMounted } from 'vue'
import ViajesService from '../../services/viajesService'

const props = defineProps({
  isOpen: Boolean,
  idViaje: Number,
})

const emit = defineEmits(['close', 'save'])

const step = ref(1) // 1: File & Personas, 2: Variantes, 3: Montos
const numeroFile = ref('')
const inscriptos = ref([]) // Array of { nombre, apellido, telefono, varianteViajeId, montoAbonado }

// Form for current person being added
const currentNombre = ref('')
const currentApellido = ref('')
const currentTelefono = ref('')

const variantes = ref([])
const isLoadingVariantes = ref(false)
const isSubmitting = ref(false)
const errorMessage = ref('')

const fetchVariantes = async () => {
  if (!props.idViaje) return
  isLoadingVariantes.value = true
  try {
    variantes.value = await ViajesService.listarVariantesDeViaje(props.idViaje)
  } catch (error) {
    errorMessage.value = 'Error al cargar las variantes del viaje.'
  } finally {
    isLoadingVariantes.value = false
  }
}

onMounted(() => {
  fetchVariantes()
})

const formatCurrency = (value) => {
  if (value === null || value === undefined) return '$0,00'
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const addPerson = () => {
  if (!currentNombre.value.trim() || !currentApellido.value.trim()) {
    errorMessage.value = 'Nombre y Apellido son campos obligatorios.'
    return
  }
  errorMessage.value = ''
  inscriptos.value.push({
    nombre: currentNombre.value.trim(),
    apellido: currentApellido.value.trim(),
    telefono: currentTelefono.value.trim(),
    varianteViajeId: null,
    montoAbonado: '',
    numeroRecibo: '',
  })
  // Clear inputs
  currentNombre.value = ''
  currentApellido.value = ''
  currentTelefono.value = ''
}

const removePerson = (index) => {
  inscriptos.value.splice(index, 1)
}

const aplicarVarianteATodos = (varianteId) => {
  inscriptos.value.forEach((ins) => {
    ins.varianteViajeId = varianteId
  })
}

// Navigation validators
const canGoToStep2 = () => {
  return (
    numeroFile.value.trim().length > 0 &&
    (inscriptos.value.length > 0 || (currentNombre.value.trim() && currentApellido.value.trim()))
  )
}

const handleNextStep1 = () => {
  // If there's values in current inputs, add them automatically
  if (currentNombre.value.trim() && currentApellido.value.trim()) {
    addPerson()
  }

  if (inscriptos.value.length === 0) {
    errorMessage.value = 'Debe agregar al menos un inscripto.'
    return
  }
  if (!numeroFile.value.trim()) {
    errorMessage.value = 'El número de file es obligatorio.'
    return
  }
  errorMessage.value = ''
  step.value = 2
}

const handleNextStep2 = () => {
  const missingVariante = inscriptos.value.some((ins) => !ins.varianteViajeId)
  if (missingVariante) {
    errorMessage.value = 'Debe seleccionar una variante para todos los inscriptos.'
    return
  }

  // El usuario solicitó no autocompletar el monto de seña,
  // se debe ingresar a mano en el paso 3.

  errorMessage.value = ''
  step.value = 3
}

const handleSubmit = async () => {
  // Validations for step 3
  for (let i = 0; i < inscriptos.value.length; i++) {
    const ins = inscriptos.value[i]
    const v = variantes.value.find((varItem) => varItem.id === ins.varianteViajeId)
    if (!v) continue

    const monto = parseFloat(ins.montoAbonado)
    if (isNaN(monto) || monto < v.valorSeña) {
      errorMessage.value = `El monto de ${ins.nombre} ${ins.apellido} debe ser mayor o igual a la seña mínima (${formatCurrency(v.valorSeña)}).`
      return
    }
    if (monto > v.valorViaje) {
      errorMessage.value = `El monto de ${ins.nombre} ${ins.apellido} no puede superar el valor total del viaje (${formatCurrency(v.valorViaje)}).`
      return
    }
    if (!ins.numeroRecibo || ins.numeroRecibo.trim() === '') {
      errorMessage.value = `Debe ingresar el número de recibo de entrega para ${ins.nombre} ${ins.apellido}.`
      return
    }
  }

  isSubmitting.value = true
  errorMessage.value = ''

  const payload = {
    numeroFile: numeroFile.value.trim(),
    inscriptos: inscriptos.value.map((ins) => ({
      nombre: ins.nombre,
      apellido: ins.apellido,
      telefono: ins.telefono,
      varianteViajeId: ins.varianteViajeId,
      montoAbonado: parseFloat(ins.montoAbonado),
      numeroRecibo: ins.numeroRecibo.trim(),
    })),
  }

  try {
    await ViajesService.inscribirPersonas(payload)
    emit('save')
  } catch (error) {
    errorMessage.value = error.message
  } finally {
    isSubmitting.value = false
  }
}

const close = () => {
  // Reset modal state
  step.value = 1
  numeroFile.value = ''
  inscriptos.value = []
  currentNombre.value = ''
  currentApellido.value = ''
  currentTelefono.value = ''
  errorMessage.value = ''
  emit('close')
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
      @click="close"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-2xl border border-slate-200"
      >
        <!-- Header -->
        <div class="bg-white px-6 py-5 border-b border-slate-100 flex items-center justify-between">
          <div>
            <h3 class="text-xl font-bold text-slate-900">Inscripción al Viaje</h3>
            <p class="text-sm text-slate-500">Formulario por pasos para la carga de inscriptos</p>
          </div>
          <button @click="close" class="text-slate-400 hover:text-slate-600 p-2">
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

        <!-- Wizard Progress Bar -->
        <div class="px-6 pt-4 bg-slate-50 border-b border-slate-100">
          <div
            class="flex items-center justify-between max-w-md mx-auto mb-4 text-xs font-bold text-slate-400"
          >
            <div class="flex flex-col items-center gap-1.5" :class="{ 'text-blue-600': step >= 1 }">
              <span
                class="w-6 h-6 rounded-full flex items-center justify-center border text-[11px]"
                :class="step >= 1 ? 'border-blue-600 bg-blue-50 text-blue-600' : 'border-slate-300'"
                >1</span
              >
              <span>Datos & File</span>
            </div>
            <div class="flex-1 h-0.5 bg-slate-200 mx-2" :class="{ 'bg-blue-600': step >= 2 }"></div>
            <div class="flex flex-col items-center gap-1.5" :class="{ 'text-blue-600': step >= 2 }">
              <span
                class="w-6 h-6 rounded-full flex items-center justify-center border text-[11px]"
                :class="step >= 2 ? 'border-blue-600 bg-blue-50 text-blue-600' : 'border-slate-300'"
                >2</span
              >
              <span>Variantes</span>
            </div>
            <div class="flex-1 h-0.5 bg-slate-200 mx-2" :class="{ 'bg-blue-600': step >= 3 }"></div>
            <div class="flex flex-col items-center gap-1.5" :class="{ 'text-blue-600': step >= 3 }">
              <span
                class="w-6 h-6 rounded-full flex items-center justify-center border text-[11px]"
                :class="step >= 3 ? 'border-blue-600 bg-blue-50 text-blue-600' : 'border-slate-300'"
                >3</span
              >
              <span>Entregas</span>
            </div>
          </div>
        </div>

        <div class="px-6 py-6 overflow-y-auto max-h-[60vh]">
          <div
            v-if="errorMessage"
            class="mb-6 p-4 bg-red-50 border border-red-100 rounded-xl text-red-700 text-sm"
          >
            {{ errorMessage }}
          </div>

          <!-- STEP 1: FILE & PERSONAS -->
          <div v-if="step === 1" class="space-y-6">
            <div>
              <label
                for="fileNumber"
                class="block text-sm font-bold text-slate-700 uppercase tracking-wider mb-2"
              >
                Número de File / Grupo
              </label>
              <input
                type="text"
                id="fileNumber"
                v-model="numeroFile"
                placeholder="Ej: FILE-452A"
                class="block w-full px-4 py-2.5 border border-slate-300 rounded-xl focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm text-slate-900 font-medium"
              />
              <p class="mt-1 text-xs text-slate-400">
                Identificador para agrupar inscriptos que viajan juntos.
              </p>
            </div>

            <div class="border-t border-slate-100 pt-4">
              <span class="block text-sm font-bold text-slate-700 uppercase tracking-wider mb-3">
                Inscriptos Agregados ({{ inscriptos.length }})
              </span>

              <div
                v-if="inscriptos.length === 0"
                class="p-6 bg-slate-50 border border-dashed border-slate-200 rounded-2xl text-center"
              >
                <p class="text-sm text-slate-500 italic">
                  No ha agregado ningún inscripto. Use el formulario inferior.
                </p>
              </div>
              <div v-else class="space-y-2">
                <div
                  v-for="(person, idx) in inscriptos"
                  :key="idx"
                  class="flex items-center justify-between p-3.5 bg-slate-50 border border-slate-200 rounded-xl"
                >
                  <div>
                    <span class="font-semibold text-slate-800"
                      >{{ person.nombre }} {{ person.apellido }}</span
                    >
                    <span v-if="person.telefono" class="block text-xs text-slate-500"
                      >Tel: {{ person.telefono }}</span
                    >
                  </div>
                  <button
                    @click="removePerson(idx)"
                    class="text-red-500 hover:text-red-700 p-1.5 hover:bg-red-50 rounded-lg transition-colors"
                  >
                    <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                      />
                    </svg>
                  </button>
                </div>
              </div>
            </div>

            <!-- Form to add travelers -->
            <div class="bg-blue-50/50 border border-blue-100 p-5 rounded-2xl space-y-4">
              <span class="block text-xs font-bold text-blue-700 uppercase tracking-wider"
                >Cargar Inscripto</span
              >
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                <div>
                  <input
                    type="text"
                    v-model="currentNombre"
                    placeholder="Nombre"
                    class="block w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-1 focus:ring-blue-500 focus:border-blue-500 text-sm bg-white"
                  />
                </div>
                <div>
                  <input
                    type="text"
                    v-model="currentApellido"
                    placeholder="Apellido"
                    class="block w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-1 focus:ring-blue-500 focus:border-blue-500 text-sm bg-white"
                  />
                </div>
              </div>
              <div class="flex gap-2">
                <input
                  type="text"
                  v-model="currentTelefono"
                  placeholder="Teléfono (Opcional)"
                  class="block w-full px-3 py-2 border border-slate-300 rounded-lg focus:ring-1 focus:ring-blue-500 focus:border-blue-500 text-sm bg-white"
                />
                <button
                  @click="addPerson"
                  class="px-4 py-2 bg-blue-600 text-white text-sm font-semibold rounded-lg hover:bg-blue-700 transition-colors whitespace-nowrap"
                >
                  Agregar Persona
                </button>
              </div>
            </div>
          </div>

          <!-- STEP 2: SELECCIÓN DE VARIANTES -->
          <div v-if="step === 2" class="space-y-6">
            <div v-if="isLoadingVariantes" class="flex justify-center py-10">
              <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600"></div>
            </div>
            <div v-else-if="variantes.length === 0" class="text-center py-10">
              <p class="text-slate-500 italic">No hay variantes disponibles para este viaje.</p>
            </div>
            <div v-else class="space-y-6">
              <div
                class="p-4 bg-slate-50 border border-slate-200 rounded-2xl flex flex-wrap gap-2 items-center justify-between"
              >
                <div>
                  <span class="block font-bold text-slate-800 text-sm"
                    >¿Todos viajan en la misma variante?</span
                  >
                  <span class="text-xs text-slate-500"
                    >Seleccione una para aplicar a todos rápidamente.</span
                  >
                </div>
                <div class="flex flex-wrap gap-1.5">
                  <button
                    v-for="v in variantes"
                    :key="'apply-' + v.id"
                    @click="aplicarVarianteATodos(v.id)"
                    class="px-3 py-1.5 bg-white border border-slate-200 hover:border-blue-500 hover:text-blue-600 text-xs font-semibold rounded-lg transition-colors"
                  >
                    {{ v.nombreVariante }}
                  </button>
                </div>
              </div>

              <div class="space-y-4">
                <div
                  v-for="(person, idx) in inscriptos"
                  :key="idx"
                  class="border border-slate-100 rounded-2xl p-4 bg-white shadow-sm space-y-3"
                >
                  <div class="flex items-center justify-between">
                    <span class="font-bold text-slate-800"
                      >{{ person.nombre }} {{ person.apellido }}</span
                    >
                    <span class="text-xs text-slate-400 uppercase font-semibold">Variante</span>
                  </div>

                  <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                    <button
                      v-for="v in variantes"
                      :key="v.id"
                      @click="person.varianteViajeId = v.id"
                      class="p-3 text-left border rounded-xl transition-all group flex flex-col justify-between"
                      :class="
                        person.varianteViajeId === v.id
                          ? 'border-blue-500 bg-blue-50/50 ring-1 ring-blue-500'
                          : 'border-slate-200 hover:border-blue-300 hover:bg-slate-50'
                      "
                    >
                      <div class="flex justify-between items-start w-full">
                        <span class="text-xs font-bold text-slate-800 group-hover:text-blue-700">{{
                          v.nombreVariante
                        }}</span>
                        <div
                          v-if="person.varianteViajeId === v.id"
                          class="w-4 h-4 bg-blue-600 rounded-full flex items-center justify-center"
                        >
                          <svg
                            class="w-2.5 h-2.5 text-white"
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
                      <div class="mt-2 text-[11px] text-slate-500 space-y-0.5">
                        <p class="font-semibold text-slate-700">
                          Total: {{ formatCurrency(v.valorViaje) }}
                        </p>
                        <p>Seña Mínima: {{ formatCurrency(v.valorSeña) }}</p>
                      </div>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- STEP 3: MONTOS A ABONAR -->
          <div v-if="step === 3" class="space-y-6">
            <div class="space-y-4">
              <div
                v-for="(person, idx) in inscriptos"
                :key="idx"
                class="bg-slate-50 border border-slate-200 p-4 rounded-xl space-y-3"
              >
                <div class="flex justify-between items-start">
                  <div>
                    <span class="font-bold text-slate-800 block"
                      >{{ person.nombre }} {{ person.apellido }}</span
                    >
                    <span
                      class="text-xs text-blue-600 font-semibold bg-blue-50 px-2 py-0.5 rounded-full border border-blue-100"
                    >
                      {{ variantes.find((v) => v.id === person.varianteViajeId)?.nombreVariante }}
                    </span>
                  </div>
                  <div class="text-right text-xs text-slate-500 space-y-0.5">
                    <p>
                      Total:
                      <span class="font-bold text-slate-700">{{
                        formatCurrency(
                          variantes.find((v) => v.id === person.varianteViajeId)?.valorViaje,
                        )
                      }}</span>
                    </p>
                    <p>
                      Seña:
                      <span class="font-bold text-slate-700">{{
                        formatCurrency(
                          variantes.find((v) => v.id === person.varianteViajeId)?.valorSeña,
                        )
                      }}</span>
                    </p>
                  </div>
                </div>

                <div class="pt-2 border-t border-slate-200 grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div>
                    <label
                      :for="'monto-' + idx"
                      class="block text-xs font-semibold text-slate-600 mb-1"
                      >Monto de entrega inicial ($)</label
                    >
                    <div class="relative">
                      <div
                        class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none"
                      >
                        <span class="text-slate-500 sm:text-sm">$</span>
                      </div>
                      <input
                        type="text"
                        :id="'monto-' + idx"
                        v-model="person.montoAbonado"
                        required
                        placeholder="Ingrese el monto"
                        class="block w-full pl-7 pr-4 py-2 border border-slate-300 rounded-xl leading-5 bg-white shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm text-slate-900"
                        @input="person.montoAbonado = $event.target.value.replace(/[^0-9.]/g, '')"
                      />
                    </div>
                  </div>
                  <div>
                    <label
                      :for="'recibo-' + idx"
                      class="block text-xs font-semibold text-slate-600 mb-1"
                      >Número de Recibo</label
                    >
                    <input
                      type="text"
                      :id="'recibo-' + idx"
                      v-model="person.numeroRecibo"
                      required
                      placeholder="Ej: 0001-00001234"
                      class="block w-full px-4 py-2 border border-slate-300 rounded-xl leading-5 bg-white shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm text-slate-900"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div
          class="bg-slate-50 px-6 py-4 flex flex-col sm:flex-row-reverse gap-3 border-t border-slate-200"
        >
          <!-- STEP 1 CONTROLS -->
          <template v-if="step === 1">
            <button
              @click="handleNextStep1"
              :disabled="!canGoToStep2()"
              class="inline-flex justify-center px-6 py-2.5 bg-blue-600 text-white font-bold rounded-xl shadow-lg hover:bg-blue-700 transition-all disabled:opacity-50 text-sm"
            >
              Siguiente
            </button>
            <button
              @click="close"
              class="inline-flex justify-center px-6 py-2.5 bg-white text-slate-900 font-bold rounded-xl border border-slate-200 hover:bg-slate-50 transition-all text-sm"
            >
              Cancelar
            </button>
          </template>

          <!-- STEP 2 CONTROLS -->
          <template v-if="step === 2">
            <button
              @click="handleNextStep2"
              class="inline-flex justify-center px-6 py-2.5 bg-blue-600 text-white font-bold rounded-xl shadow-lg hover:bg-blue-700 transition-all text-sm"
            >
              Siguiente
            </button>
            <button
              @click="step = 1"
              class="inline-flex justify-center px-6 py-2.5 bg-white text-slate-900 font-bold rounded-xl border border-slate-200 hover:bg-slate-50 transition-all text-sm"
            >
              Anterior
            </button>
          </template>

          <!-- STEP 3 CONTROLS -->
          <template v-if="step === 3">
            <button
              @click="handleSubmit"
              :disabled="isSubmitting"
              class="inline-flex justify-center px-6 py-2.5 bg-blue-600 text-white font-bold rounded-xl shadow-lg hover:bg-blue-700 transition-all disabled:opacity-50 text-sm"
            >
              {{ isSubmitting ? 'Inscribiendo...' : 'Confirmar Inscripción' }}
            </button>
            <button
              @click="step = 2"
              class="inline-flex justify-center px-6 py-2.5 bg-white text-slate-900 font-bold rounded-xl border border-slate-200 hover:bg-slate-50 transition-all text-sm"
            >
              Anterior
            </button>
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped></style>
