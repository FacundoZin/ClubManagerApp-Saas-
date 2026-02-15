<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import ArticulosTab from '../../components/ModuloAlquilerArticulos/DashboardTabs/ArticulosTab.vue'
import GestionAlquileresTab from '../../components/ModuloAlquilerArticulos/DashboardTabs/GestionAlquileresTab.vue'
import NuevoAlquilerTab from '../../components/ModuloAlquilerArticulos/DashboardTabs/NuevoAlquilerTab.vue'

const router = useRouter()
const route = useRoute()

// Tabs
const activeTab = ref('articulos')

const handleTabChange = (tab) => {
  activeTab.value = tab
}

// Toast
const toast = ref({ show: false, message: '', type: 'success' })
const showToast = (message, type = 'success') => {
  // If message is an object (emitted event), extract props
  if (typeof message === 'object' && message.message) {
    type = message.type || 'success'
    message = message.message
  }

  toast.value = { show: true, message, type }
  setTimeout(() => (toast.value.show = false), 3000)
}

// Event Handlers
const handleAlquilerCreated = (result) => {
  activeTab.value = 'gestionar-alquileres'
  // Optional: Navigate to detail if desired, or just show the list
  if (result && result.idAlquiler) {
    router.push(`/ortopedia/alquileres/${result.idAlquiler}`)
  }
}

// Initial Load
onMounted(() => {
  if (route.query.success === 'finalizado') {
    showToast('Alquiler finalizado correctamente')
    activeTab.value = 'gestionar-alquileres'
    router.replace({ query: { ...route.query, success: undefined } })
  }
})

const goHome = () => router.push('/')
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb -->
      <div class="mb-6 overflow-x-auto -mx-4 px-4 sm:mx-0 sm:px-0 no-scrollbar">
        <nav class="flex mb-2 min-w-max" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a
                href="#"
                @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-green-600"
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
                  >Alquiler Artículos</span
                >
              </div>
            </li>
          </ol>
        </nav>
        <div class="flex flex-col sm:flex-row justify-between items-start sm:items-end gap-4">
          <div>
            <h2 class="text-2xl sm:text-3xl font-bold text-slate-900 tracking-tight">
              Ortopedia y Alquileres
            </h2>
            <p class="text-slate-500 mt-1 text-sm sm:text-base">
              Gestión de artículos ortopédicos y seguimiento de alquileres.
            </p>
          </div>
        </div>
      </div>

      <!-- Tabs -->
      <div
        class="border-b border-slate-200 mb-6 -mx-4 px-4 sm:mx-0 sm:px-0 overflow-x-auto no-scrollbar snap-x snap-mandatory touch-pan-x overscroll-behavior-x-contain"
      >
        <nav class="-mb-px flex space-x-6 sm:space-x-8 min-w-max pb-px" aria-label="Tabs">
          <button
            v-for="tab in [
              { id: 'articulos', label: 'Gestión de Artículos' },
              { id: 'gestionar-alquileres', label: 'Gestión de Alquileres' },
              { id: 'nuevo-alquiler', label: 'Registrar nuevo alquiler' },
            ]"
            :key="tab.id"
            @click="handleTabChange(tab.id)"
            :class="[
              activeTab === tab.id
                ? 'border-green-500 text-green-600'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm transition-all snap-start flex-shrink-0 active:scale-95',
            ]"
          >
            {{ tab.label }}
          </button>
        </nav>
      </div>

      <!-- Tab Content -->
      <div class="min-h-[400px]">
        <ArticulosTab v-if="activeTab === 'articulos'" @show-toast="showToast" />

        <GestionAlquileresTab v-if="activeTab === 'gestionar-alquileres'" @show-toast="showToast" />

        <NuevoAlquilerTab
          v-if="activeTab === 'nuevo-alquiler'"
          @show-toast="showToast"
          @alquiler-created="handleAlquilerCreated"
        />
      </div>
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
        class="fixed bottom-4 right-4 z-50 max-w-sm w-full bg-white shadow-lg rounded-lg pointer-events-auto ring-1 ring-black ring-opacity-5 overflow-hidden"
      >
        <div class="p-4">
          <div class="flex items-start">
            <div class="flex-shrink-0">
              <svg
                v-if="toast.type === 'success'"
                class="h-6 w-6 text-green-400"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              <svg
                v-else
                class="h-6 w-6 text-red-400"
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
            </div>
            <div class="ml-3 w-0 flex-1 pt-0.5">
              <p class="text-sm font-medium text-slate-900">{{ toast.message }}</p>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>
