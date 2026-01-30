<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800">
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Breadcrumb -->
      <div class="mb-6">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <router-link
                to="/"
                class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-teal-600"
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
              </router-link>
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
                <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2">Analíticas</span>
              </div>
            </li>
          </ol>
        </nav>
        <div>
          <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Panel de Analíticas</h2>
          <p class="text-slate-500 mt-1">
            Visualice el rendimiento, crecimiento y estadísticas del club.
          </p>
        </div>
      </div>

      <!-- Tabs Navigation -->
      <div class="border-b border-slate-200 mb-8">
        <nav class="-mb-px flex space-x-8" aria-label="Tabs">
          <button
            @click="activeView = 'socios'"
            :class="[
              activeView === 'socios'
                ? 'border-teal-500 text-teal-600 font-bold'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 text-sm transition-all flex items-center gap-2',
            ]"
          >
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
                d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"
              />
            </svg>
            Estadísticas de Socios
          </button>
          <button
            @click="activeView = 'ingresos'"
            :class="[
              activeView === 'ingresos'
                ? 'border-teal-500 text-teal-600 font-bold'
                : 'border-transparent text-slate-500 hover:text-slate-700 hover:border-slate-300',
              'whitespace-nowrap py-4 px-1 border-b-2 text-sm transition-all flex items-center gap-2',
            ]"
          >
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
                d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
            Reporte de Ingresos
          </button>
        </nav>
      </div>

      <!-- Active Dashboard Component -->
      <Transition name="fade" mode="out-in">
        <DashboardAnaliticasSocios v-if="activeView === 'socios'" />
        <DashboardAnaliticasIngresos v-else-if="activeView === 'ingresos'" />
      </Transition>
    </main>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import DashboardAnaliticasSocios from './DashboardAnaliticasSocios.vue'
import DashboardAnaliticasIngresos from './DashboardAnaliticasIngresos.vue'

const activeView = ref('socios')
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition:
    opacity 0.2s ease,
    transform 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(4px);
}
</style>
