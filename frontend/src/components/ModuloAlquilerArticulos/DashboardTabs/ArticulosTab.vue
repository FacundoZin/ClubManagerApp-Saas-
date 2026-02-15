<script setup>
import { ref, onMounted } from 'vue'
import ArticuloService from '../../../services/ArticuloService'
import ArticuloCard from '../Articulos/ArticuloCard.vue'
import ArticuloFormModal from '../Articulos/ArticuloFormModal.vue'
import UpdatePrecioModal from '../Articulos/UpdatePrecioModal.vue'

const emit = defineEmits(['show-toast'])

// State
const articulos = ref([])
const isArticuloModalOpen = ref(false)
const isUpdatePrecioModalOpen = ref(false)
const selectedArticulo = ref(null)
const loadingArticulos = ref(false)

// Methods
const loadArticulos = async () => {
  loadingArticulos.value = true
  try {
    const result = await ArticuloService.getAll()
    articulos.value = result
  } catch (e) {
    emit('show-toast', { message: 'Error cargando artículos: ' + e.message, type: 'error' })
  } finally {
    loadingArticulos.value = false
  }
}

const handleCreateArticulo = async () => {
  isArticuloModalOpen.value = false
  emit('show-toast', { message: 'Artículo creado correctamente', type: 'success' })
  loadArticulos()
}

const openUpdatePrecio = (articulo) => {
  selectedArticulo.value = articulo
  isUpdatePrecioModalOpen.value = true
}

const handleUpdatePrecio = () => {
  isUpdatePrecioModalOpen.value = false
  selectedArticulo.value = null
  emit('show-toast', { message: 'Precio actualizado correctamente', type: 'success' })
  loadArticulos()
}

onMounted(() => {
  loadArticulos()
})
</script>

<template>
  <div class="h-full">
    <!-- Header Actions (Mobile) -->
    <div class="mb-6 sm:hidden">
      <button
        @click="isArticuloModalOpen = true"
        class="w-full inline-flex justify-center items-center px-4 py-3 border border-transparent text-sm font-semibold rounded-lg shadow-sm text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 transition-colors"
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
            d="M12 4v16m8-8H4"
          />
        </svg>
        Nuevo Artículo
      </button>
    </div>

    <!-- Header Actions (Desktop) - Optional: Can be placed here or in parent if we want it in the main header -->
    <div class="hidden sm:flex justify-end mb-6">
      <button
        @click="isArticuloModalOpen = true"
        class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 transition-colors"
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
            d="M12 4v16m8-8H4"
          />
        </svg>
        Nuevo Artículo
      </button>
    </div>

    <!-- Content -->
    <div v-if="loadingArticulos" class="flex justify-center py-12">
      <svg
        class="animate-spin h-8 w-8 text-teal-600"
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
      v-else-if="articulos.length > 0"
      class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6"
    >
      <ArticuloCard
        v-for="art in articulos"
        :key="art.id"
        :articulo="art"
        @update-price="openUpdatePrecio"
      />
    </div>

    <div
      v-else
      class="text-center py-12 text-slate-500 bg-white rounded-lg border border-dashed border-slate-200"
    >
      <svg
        class="mx-auto h-12 w-12 text-slate-400"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
        aria-hidden="true"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"
        />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-slate-900">No hay artículos</h3>
      <p class="mt-1 text-sm text-slate-500">Comience creando un nuevo artículo.</p>
      <div class="mt-6">
        <button
          @click="isArticuloModalOpen = true"
          class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-teal-600 hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500"
        >
          <svg
            class="-ml-1 mr-2 h-5 w-5"
            xmlns="http://www.w3.org/2000/svg"
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
          Nuevo Artículo
        </button>
      </div>
    </div>

    <!-- Modals -->
    <ArticuloFormModal
      :is-open="isArticuloModalOpen"
      @close="isArticuloModalOpen = false"
      @save="handleCreateArticulo"
    />

    <UpdatePrecioModal
      v-if="selectedArticulo"
      :is-open="isUpdatePrecioModalOpen"
      :articulo="selectedArticulo"
      @close="isUpdatePrecioModalOpen = false"
      @save="handleUpdatePrecio"
    />
  </div>
</template>
