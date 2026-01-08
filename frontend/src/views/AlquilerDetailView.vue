<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AlquilerService from '../services/AlquilerService'
import AddItemModal from '../components/Alquileres/AddItemModal.vue'
import ConfirmModal from '../components/ConfirmModal.vue'

const route = useRoute()
const router = useRouter()
const alquilerId = route.params.id

const alquiler = ref(null)
const loading = ref(true)
const error = ref('')

// Modals
const isAddItemModalOpen = ref(false)
const isConfirmFinalizeOpen = ref(false)

// Toast
const toast = ref({ show: false, message: '', type: 'success' })
const showToast = (message, type = 'success') => {
  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

onMounted(() => {
  loadAlquiler()
})

const loadAlquiler = async () => {
  loading.value = true
  error.value = ''
  try {
    const result = await AlquilerService.getById(alquilerId)
    alquiler.value = result
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

const handleAddItem = async () => {
  isAddItemModalOpen.value = false
  showToast('Item agregado correctamente')
  loadAlquiler()
}

const handleRemoveItem = async (itemId) => {
  if (!confirm('¿Está seguro de eliminar este artículo del alquiler?')) return

  try {
    await AlquilerService.removeItem(alquilerId, itemId)
    showToast('Artículo eliminado')
    loadAlquiler()
  } catch (e) {
    showToast(e.message, 'error')
  }
}

const handleUpdateQuantity = async (item, newQty) => {
  if (newQty < 1) return
  try {
    await AlquilerService.updateItemQuantity(alquilerId, {
      ItemId: item.id, // DTO says ItemId. Let's assume passed Item has Id from DetailsItemAlquilerDto
      NuevaCantidad: newQty,
    })
    showToast('Cantidad actualizada')
    loadAlquiler()
  } catch (e) {
    showToast(e.message, 'error')
  }
}

const handleRegisterPayment = async () => {
  try {
    await AlquilerService.registerPayment(alquilerId)
    showToast('Pago registrado correctamente')
    loadAlquiler()
  } catch (e) {
    showToast(e.message, 'error')
  }
}

const handleFinalize = async () => {
  try {
    await AlquilerService.finalize(alquilerId)
    showToast('Alquiler finalizado correctamente')
    isConfirmFinalizeOpen.value = false
    // Maybe redirect or reload
    loadAlquiler()
  } catch (e) {
    showToast(e.message, 'error')
    isConfirmFinalizeOpen.value = false
  }
}

const goBack = () => router.push('/alquiler-articulos')
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800 pb-12">
    <!-- Header Simple -->
    <div class="bg-white border-b border-slate-200 shadow-sm sticky top-0 z-30">
      <div class="max-w-5xl mx-auto px-4 sm:px-6 py-4 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <button
            @click="goBack"
            class="p-2 -ml-2 text-slate-400 hover:text-slate-600 rounded-full hover:bg-slate-100 transition-colors"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-6 w-6"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M10 19l-7-7m0 0l7-7m-7 7h18"
              />
            </svg>
          </button>
          <h1 class="text-xl font-bold text-slate-900">Detalle de Alquiler</h1>
        </div>
        <div v-if="alquiler">
          <span
            class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
            :class="
              alquiler.estaAlDia ? 'bg-emerald-100 text-emerald-800' : 'bg-rose-100 text-rose-800'
            "
          >
            {{ alquiler.estaAlDia ? 'Al día' : 'Con Deuda' }}
          </span>
        </div>
      </div>
    </div>

    <div v-if="loading" class="flex justify-center py-20">
      <svg
        class="animate-spin h-10 w-10 text-teal-600"
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
    </div>

    <div
      v-else-if="error"
      class="max-w-3xl mx-auto mt-12 p-6 bg-red-50 text-red-700 rounded-lg text-center"
    >
      {{ error }}
    </div>

    <div v-else class="max-w-5xl mx-auto px-4 sm:px-6 mt-8 space-y-6">
      <!-- Info Cards Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Socio Info -->
        <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
          <h2
            class="text-sm font-medium text-slate-500 uppercase tracking-wider mb-4 border-b border-slate-100 pb-2"
          >
            Información del Socio
          </h2>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-slate-500">Nombre:</span>
              <span class="font-medium text-slate-900"
                >{{ alquiler.nombreSocio }} {{ alquiler.apellidoSocio }}</span
              >
            </div>
            <div class="flex justify-between">
              <span class="text-slate-500">DNI:</span>
              <span class="font-medium text-slate-900">{{ alquiler.dniSocio }}</span>
            </div>
            <div class="flex justify-between" v-if="alquiler.telefonoSocio">
              <span class="text-slate-500">Teléfono:</span>
              <span class="font-medium text-slate-900">{{ alquiler.telefonoSocio }}</span>
            </div>
            <div class="flex justify-between" v-if="alquiler.direccionSocio">
              <span class="text-slate-500">Dirección:</span>
              <span class="font-medium text-slate-900 text-right"
                >{{ alquiler.direccionSocio }} <br /><span class="text-xs text-slate-400">{{
                  alquiler.localidadSocio
                }}</span></span
              >
            </div>
          </div>
        </div>

        <!-- Rental Info & Actions -->
        <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6 flex flex-col">
          <h2
            class="text-sm font-medium text-slate-500 uppercase tracking-wider mb-4 border-b border-slate-100 pb-2"
          >
            Estado y Acciones
          </h2>
          <div class="flex-1 space-y-3">
            <div class="flex justify-between">
              <span class="text-slate-500">Fecha Inicio:</span>
              <span class="font-medium text-slate-900">{{ alquiler.fechaAlquiler }}</span>
            </div>
            <div
              v-if="alquiler.observaciones"
              class="text-sm text-slate-600 italic bg-slate-50 p-2 rounded"
            >
              "{{ alquiler.observaciones }}"
            </div>
          </div>

          <div class="mt-6 flex flex-col gap-3">
            <button
              @click="handleRegisterPayment"
              class="w-full inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-teal-700 bg-teal-100 hover:bg-teal-200 transition-colors"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-2"
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
              Registrar Pago
            </button>
            <button
              @click="isConfirmFinalizeOpen = true"
              class="w-full inline-flex justify-center items-center px-4 py-2 border border-slate-300 text-sm font-medium rounded-md text-slate-700 bg-white hover:bg-slate-50 transition-colors"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 mr-2 text-slate-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M5 13l4 4L19 7"
                />
              </svg>
              Finalizar Alquiler
            </button>
          </div>
        </div>
      </div>

      <!-- Articulos (Items) -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
        <div
          class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50"
        >
          <h2 class="text-base font-bold text-slate-900">Artículos Alquilados</h2>
          <button
            @click="isAddItemModalOpen = true"
            class="text-sm font-medium text-teal-600 hover:text-teal-800 flex items-center"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-4 w-4 mr-1"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M12 4v16m8-8H4"
              />
            </svg>
            Agregar Artículo
          </button>
        </div>

        <table class="min-w-full divide-y divide-slate-200">
          <thead class="bg-white">
            <tr>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider"
              >
                Artículo
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider"
              >
                Precio Unit.
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider"
              >
                Cantidad
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider"
              >
                Subtotal
              </th>
              <th
                scope="col"
                class="px-6 py-3 text-right text-xs font-medium text-slate-500 uppercase tracking-wider"
              >
                Acciones
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-200 bg-white">
            <tr v-for="item in alquiler.items" :key="item.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                {{ item.nombreArticulo }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                ${{ item.precioAlquiler }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                <div class="flex items-center space-x-2">
                  <button
                    @click="handleUpdateQuantity(item, item.cantidad - 1)"
                    class="p-1 text-slate-400 hover:text-slate-600 border rounded"
                  >
                    -
                  </button>
                  <span>{{ item.cantidad }}</span>
                  <button
                    @click="handleUpdateQuantity(item, item.cantidad + 1)"
                    class="p-1 text-slate-400 hover:text-slate-600 border rounded"
                  >
                    +
                  </button>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                ${{ item.subtotal }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
                <button
                  @click="handleRemoveItem(item.id)"
                  class="text-rose-600 hover:text-rose-900 font-medium"
                >
                  Eliminar
                </button>
              </td>
            </tr>
            <tr v-if="!alquiler.items || alquiler.items.length === 0">
              <td colspan="5" class="px-6 py-8 text-center text-slate-400 italic">
                No hay artículos en este alquiler.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Historial Pagos -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden mb-12">
        <div class="px-6 py-4 border-b border-slate-200 bg-slate-50">
          <h2 class="text-base font-bold text-slate-900">Historial de Pagos</h2>
        </div>
        <div
          class="p-6 grid grid-cols-2 sm:grid-cols-4 md:grid-cols-6 gap-4"
          v-if="alquiler.historialDePagos && alquiler.historialDePagos.length > 0"
        >
          <div
            v-for="pago in alquiler.historialDePagos"
            :key="pago.id"
            class="bg-emerald-50 border border-emerald-100 rounded-lg p-3 text-center"
          >
            <div class="text-xs text-emerald-600 uppercase font-semibold">
              {{ pago.mes }}/{{ pago.anio }}
            </div>
            <div class="text-lg font-bold text-emerald-700">${{ pago.monto }}</div>
          </div>
        </div>
        <div v-else class="p-8 text-center text-slate-400">
          No se han registrado pagos para este alquiler.
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

    <ConfirmModal
      :is-open="isConfirmFinalizeOpen"
      title="Finalizar Alquiler"
      message="¿Está seguro que desea finalizar este alquiler? Se registrará la devolución de todos los artículos."
      confirm-text="Finalizar Alquiler"
      type="info"
      @close="isConfirmFinalizeOpen = false"
      @confirm="handleFinalize"
    />

    <!-- Toast -->
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
          :class="{ 'bg-teal-500': toast.type === 'success', 'bg-red-500': toast.type === 'error' }"
        >
          <svg
            v-if="toast.type === 'success'"
            class="w-6 h-6 text-white"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M5 13l4 4L19 7"
            />
          </svg>
          <svg
            v-else
            class="w-6 h-6 text-white"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </div>
        <div class="px-4 py-3">
          <span
            class="font-semibold"
            :class="{
              'text-teal-500': toast.type === 'success',
              'text-red-500': toast.type === 'error',
            }"
          >
            {{ toast.type === 'success' ? 'Éxito' : 'Error' }}
          </span>
          <p class="text-sm text-slate-600">{{ toast.message }}</p>
        </div>
      </div>
    </Transition>
  </div>
</template>
