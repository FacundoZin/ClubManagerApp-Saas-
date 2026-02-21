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
      <!-- Breadcrumb & Page Title -->
      <div class="mb-8">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a
                href="#"
                @click.prevent="goHome"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-blue-600"
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
        <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Ortopedia y Alquileres</h2>
        <p class="text-slate-500 mt-1 text-lg">
          Gestión de artículos ortopédicos y seguimiento de alquileres.
        </p>
      </div>

      <!-- Navigation Pill-Navbar -->
      <div class="flex flex-col sm:flex-row items-center gap-4 mb-8">
        <div
          class="flex flex-wrap items-center gap-2 p-1.5 bg-slate-200/50 rounded-[2rem] w-full sm:w-auto border border-slate-200/50 backdrop-blur-sm shadow-inner"
        >
          <button
            v-for="tab in [
              {
                id: 'articulos',
                label: 'Gestión de Artículos',
                icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
              },
              {
                id: 'gestionar-alquileres',
                label: 'Gestión de Alquileres',
                icon: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01',
              },
              { id: 'nuevo-alquiler', label: 'Registrar nuevo alquiler', icon: 'M12 4v16m8-8H4' },
            ]"
            :key="tab.id"
            @click="handleTabChange(tab.id)"
            class="flex-1 sm:flex-none flex items-center justify-center px-5 py-2.5 rounded-[1.5rem] transition-all duration-300 font-bold text-sm"
            :class="[
              activeTab === tab.id
                ? 'bg-white text-blue-600 shadow-md border border-slate-200 translate-y-[-1px] scale-[1.02]'
                : 'text-slate-500 hover:text-slate-700 hover:bg-white/40',
            ]"
          >
            <svg class="w-4 h-4 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="tab.icon" />
            </svg>
            {{ tab.label }}
          </button>
        </div>

        <button
          @click="goHome"
          class="hidden sm:flex items-center px-4 py-2 text-slate-400 hover:text-red-500 font-bold text-sm transition-colors group"
        >
          <svg
            class="w-4 h-4 mr-2 group-hover:-translate-x-1 transition-transform"
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
          Volver
        </button>
      </div>

      <!-- Tab Content Area -->
      <div
        class="bg-white rounded-3xl border border-slate-200 shadow-sm p-6 min-h-[400px] overflow-hidden"
      >
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
        class="fixed bottom-5 right-5 z-50 flex w-full max-w-sm overflow-hidden bg-white rounded-lg shadow-2xl border border-slate-200 pointer-events-auto ring-1 ring-black ring-opacity-5"
      >
        <div
          class="flex items-center justify-center w-12"
          :class="{
            'bg-emerald-500': toast.type === 'success',
            'bg-red-500': toast.type === 'error',
          }"
        >
          <svg
            v-if="toast.type === 'success'"
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 40 40"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM16.6667 28.3333L8.33337 20L10.6834 17.65L16.6667 23.6166L29.3167 10.9666L31.6667 13.3333L16.6667 28.3333Z"
            />
          </svg>
          <svg
            v-else
            class="w-6 h-6 text-white fill-current"
            viewBox="0 0 40 40"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M20 3.33331C10.8 3.33331 3.33337 10.8 3.33337 20C3.33337 29.2 10.8 36.6666 20 36.6666C29.2 36.6666 36.6667 29.2 36.6667 20C36.6667 10.8 29.2 3.33331 20 3.33331ZM21.6667 28.3333H18.3334V25H21.6667V28.3333ZM21.6667 21.6666H18.3334V11.6666H21.6667V21.6666Z"
            />
          </svg>
        </div>

        <div class="px-4 py-3 -mx-3">
          <div class="mx-3">
            <span
              class="font-semibold"
              :class="{
                'text-emerald-500': toast.type === 'success',
                'text-red-500': toast.type === 'error',
              }"
            >
              {{ toast.type === 'success' ? 'Éxito' : 'Error' }}
            </span>
            <p class="text-sm text-slate-600">
              {{ toast.message }}
            </p>
          </div>
        </div>

        <button
          @click="toast.show = false"
          class="ml-auto p-2 text-slate-400 hover:text-slate-600 focus:outline-none"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.font-sans {
  font-family:
    'Outfit',
    'Inter',
    system-ui,
    -apple-system,
    sans-serif;
}

/* Custom scrollbar for horizontal tabs if needed */
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
