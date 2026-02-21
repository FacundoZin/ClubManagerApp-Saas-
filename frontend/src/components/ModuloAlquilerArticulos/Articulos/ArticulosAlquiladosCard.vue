<script setup>
import { ref } from 'vue'
import AlquilerService from '../../../services/AlquilerService'
import ConfirmModal from '../../Common/ConfirmModal.vue'

const props = defineProps({
  alquilerId: {
    type: [Number, String],
    required: true,
  },
  items: {
    type: Array,
    required: true,
  },
})

const emit = defineEmits(['updated', 'add-item'])

const isComponentLoading = ref(false)
const isConfirmDeleteOpen = ref(false)
const itemToDeleteId = ref(null)

const handleUpdateQuantity = async (item, newQty) => {
  if (newQty < 1) return
  isComponentLoading.value = true
  try {
    await AlquilerService.updateItemQuantity(props.alquilerId, {
      ItemId: item.id,
      NuevaCantidad: newQty,
    })
    emit('updated', 'Cantidad actualizada')
  } catch (e) {
    emit('updated', e.message, 'error')
  } finally {
    isComponentLoading.value = false
  }
}

const handleRemoveItem = (itemId) => {
  itemToDeleteId.value = itemId
  isConfirmDeleteOpen.value = true
}

const confirmRemoveItem = async () => {
  if (!itemToDeleteId.value) return

  isConfirmDeleteOpen.value = false
  isComponentLoading.value = true
  try {
    await AlquilerService.removeItem(props.alquilerId, itemToDeleteId.value)
    emit('updated', 'Artículo eliminado')
  } catch (e) {
    emit('updated', e.message, 'error')
  } finally {
    isComponentLoading.value = false
    itemToDeleteId.value = null
  }
}
</script>

<template>
  <div class="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden relative">
    <!-- Overlay interno para no refrescar toda la vista -->
    <div
      v-if="isComponentLoading"
      class="absolute inset-0 bg-white/50 backdrop-blur-[1px] z-10 flex items-center justify-center"
    >
      <div
        class="flex items-center gap-2 bg-white px-4 py-2 rounded-full shadow-md border border-slate-100"
      >
        <svg
          class="animate-spin h-4 w-4 text-teal-600"
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
        <span class="text-xs font-bold text-slate-600">Actualizando...</span>
      </div>
    </div>

    <div class="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50">
      <h2 class="text-base font-bold text-slate-900">Artículos Alquilados</h2>
      <button
        @click="emit('add-item')"
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

    <!-- Mobile View (Cards) -->
    <div class="sm:hidden divide-y divide-slate-100">
      <div v-for="item in items" :key="item.id" class="p-4 space-y-3">
        <div class="flex justify-between items-start">
          <h3 class="font-bold text-slate-900">{{ item.nombreArticulo }}</h3>
          <button @click="handleRemoveItem(item.id)" class="text-rose-500 hover:text-rose-700 p-1">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
              />
            </svg>
          </button>
        </div>
        <div class="grid grid-cols-2 gap-2 text-sm">
          <div>
            <span class="text-slate-400 block text-xs uppercase font-bold tracking-wider"
              >Precio Unit.</span
            >
            <span class="text-slate-700 font-medium"
              >${{ item.precioAlquiler.toLocaleString() }}</span
            >
          </div>
          <div class="text-right">
            <span class="text-slate-400 block text-xs uppercase font-bold tracking-wider"
              >Subtotal</span
            >
            <span class="text-slate-900 font-bold">${{ item.subtotal.toLocaleString() }}</span>
          </div>
        </div>
        <div class="flex items-center justify-between pt-2">
          <span class="text-slate-500 text-sm">Cantidad:</span>
          <div
            class="flex items-center border border-slate-200 rounded-lg overflow-hidden bg-white"
          >
            <button
              @click="handleUpdateQuantity(item, item.cantidad - 1)"
              class="px-3 py-1.5 hover:bg-slate-50 text-slate-600 transition-colors border-r border-slate-200"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-3 w-3"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                stroke-width="4"
              >
                <path stroke-linecap="round" stroke-linejoin="round" d="M20 12H4" />
              </svg>
            </button>
            <span class="px-4 text-sm font-bold text-slate-700 min-w-[40px] text-center">
              {{ item.cantidad }}
            </span>
            <button
              @click="handleUpdateQuantity(item, item.cantidad + 1)"
              class="px-3 py-1.5 hover:bg-slate-50 text-slate-600 transition-colors border-l border-slate-200"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-3 w-3"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                stroke-width="4"
              >
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
              </svg>
            </button>
          </div>
        </div>
      </div>
      <div v-if="!items || items.length === 0" class="p-8 text-center text-slate-400 italic">
        No hay artículos en este alquiler.
      </div>
    </div>

    <!-- Desktop View (Table) -->
    <div class="hidden sm:block overflow-x-auto no-scrollbar">
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
          <tr v-for="item in items" :key="item.id">
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
              {{ item.nombreArticulo }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
              ${{ item.precioAlquiler.toLocaleString() }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
              <div
                class="flex items-center border border-slate-200 rounded-lg overflow-hidden bg-slate-50 w-fit"
              >
                <button
                  @click="handleUpdateQuantity(item, item.cantidad - 1)"
                  class="px-2 py-1.5 hover:bg-slate-200 text-slate-600 transition-colors border-r border-slate-200"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-3 w-3"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    stroke-width="4"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" d="M20 12H4" />
                  </svg>
                </button>
                <span class="px-3 text-sm font-bold text-slate-700 min-w-[32px] text-center">
                  {{ item.cantidad }}
                </span>
                <button
                  @click="handleUpdateQuantity(item, item.cantidad + 1)"
                  class="px-2 py-1.5 hover:bg-slate-200 text-slate-600 transition-colors border-l border-slate-200"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-3 w-3"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                    stroke-width="4"
                  >
                    <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
                  </svg>
                </button>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
              ${{ item.subtotal.toLocaleString() }}
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
          <tr v-if="!items || items.length === 0">
            <td colspan="5" class="px-6 py-8 text-center text-slate-400 italic">
              No hay artículos en este alquiler.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>

  <ConfirmModal
    :is-open="isConfirmDeleteOpen"
    title="Eliminar Artículo"
    message="¿Está seguro que desea eliminar este artículo del alquiler?"
    confirm-text="Eliminar"
    type="danger"
    @close="isConfirmDeleteOpen = false"
    @confirm="confirmRemoveItem"
  />
</template>
