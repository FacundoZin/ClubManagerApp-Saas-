<script setup>
import AuthService from '@/services/AuthService'
import { useRouter } from 'vue-router'

const router = useRouter()
const isSuperAdmin = AuthService.isSuperAdmin

const navigateTo = (route) => {
  router.push(route)
}

const modules = [
  {
    title: 'Gestión Socios',
    description: 'Administración completa del padrón de socios.',
    icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0z',
    route: '/socios',
    color: 'text-blue-600',
    iconBg: 'bg-blue-100',
  },
  {
    title: 'Gestión Reservas',
    description: 'Control de calendario y disponibilidad de espacios.',
    icon: 'M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z',
    route: '/reservas-salones',
    color: 'text-indigo-600',
    iconBg: 'bg-indigo-100',
  },
  {
    title: 'Alquiler de artículos',
    description: 'Préstamo y devolución de inventario.',
    icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
    route: '/alquiler-articulos',
    color: 'text-sky-600',
    iconBg: 'bg-sky-100',
  },
  {
    title: 'Módulo para cobradores',
    description: 'Menejo de cobranzas desde un solo lugar',
    icon: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z',
    route: '/cobradores',
    color: 'text-cyan-600',
    iconBg: 'bg-cyan-100',
  },
  {
    title: 'Gestión de viajes',
    description: 'Organización de excursiones y turismo.',
    icon: 'M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7',
    route: '/viajes',
    color: 'text-teal-600',
    iconBg: 'bg-teal-100',
  },
  {
    title: 'Registrar pago',
    description: 'Ingreso rápido de cuotas y movimientos.',
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    route: '/pagos',
    color: 'text-emerald-600',
    iconBg: 'bg-emerald-100',
  },
  {
    title: 'Estadísticas y Balances',
    description: 'Análisis de socios, ingresos y métricas clave.',
    icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
    route: '/analiticas',
    color: 'text-indigo-600',
    iconBg: 'bg-indigo-100',
  },
]
</script>

<template>
  <div class="min-h-screen bg-slate-50 font-sans text-slate-800 flex flex-col">
    <!-- Main Content -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 flex-grow w-full">
      <!-- Breadcrumb & Page Title -->
      <div class="mb-8">
        <nav class="flex mb-2" aria-label="Breadcrumb">
          <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li class="inline-flex items-center">
              <a href="#" class="inline-flex items-center text-sm font-medium text-slate-500 hover:text-blue-600">
                <svg class="w-3 h-3 mr-2.5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="currentColor"
                  viewBox="0 0 20 20">
                  <path
                    d="m19.707 9.293-2-2-7-7a1 1 0 0 0-1.414 0l-7 7-2 2a1 1 0 0 0 1.414 1.414L2 10.414V18a2 2 0 0 0 2 2h3a1 1 0 0 0 1-1v-4a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v4a1 1 0 0 0 1 1h3a2 2 0 0 0 2-2v-7.586l.293.293a1 1 0 0 0 1.414-1.414Z" />
                </svg>
                Inicio
              </a>
            </li>
            <li>
              <div class="flex items-center">
                <svg class="w-3 h-3 text-slate-400 mx-1" aria-hidden="true" xmlns="http://www.w3.org/2000/svg"
                  fill="none" viewBox="0 0 6 10">
                  <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="m1 9 4-4-4-4" />
                </svg>
                <span class="ml-1 text-sm font-medium text-slate-700 md:ml-2">Panel General</span>
              </div>
            </li>
          </ol>
        </nav>
        <h2 class="text-3xl font-bold text-slate-900 tracking-tight">Bienvenido de nuevo</h2>
        <p class="text-slate-500 mt-1 text-lg">
          Seleccione un módulo para gestionar las operaciones del club.
        </p>
      </div>

      <!-- Modules Grid -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <button v-for="(module, index) in modules" :key="index" @click="navigateTo(module.route)"
          class="group relative flex flex-col p-6 bg-white rounded-2xl border border-slate-200 shadow-sm hover:shadow-lg hover:-translate-y-1 transition-all duration-300 text-left overflow-hidden cursor-pointer">
          <div class="relative z-10 flex items-start justify-between mb-5">
            <div class="p-3.5 rounded-xl transition-colors duration-300 shadow-sm"
              :class="[module.iconBg, module.color]">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-7 w-7" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="module.icon" />
              </svg>
            </div>
            <div class="text-slate-300 group-hover:text-blue-500 transition-colors duration-300">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
              </svg>
            </div>
          </div>

          <div class="relative z-10 mt-auto">
            <h3 class="text-lg font-bold text-slate-900 mb-1 group-hover:text-blue-700 transition-colors duration-300">
              {{ module.title }}
            </h3>
            <p class="text-sm text-slate-500 leading-relaxed font-medium">
              {{ module.description }}
            </p>
          </div>
        </button>
      </div>
    </main>

    <!-- Footer -->
    <footer class="border-t border-slate-200 bg-white mt-auto py-8">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col md:flex-row justify-between items-center gap-4">
        <div class="text-center md:text-left">
          <p class="text-slate-500 text-sm font-medium">
            &copy; {{ new Date().getFullYear() }} Sistema Asociación Casa del Jubilado.
          </p>
          <div class="flex items-center gap-2 mt-1">
            <p class="text-slate-400 text-xs">Versión 1.0.0</p>
            <template v-if="isSuperAdmin">
              <span class="text-slate-300 text-xs">•</span>
              <router-link to="/usuarios" class="text-slate-400 text-xs hover:text-blue-600 transition-colors">
                Gestión de Usuarios
              </router-link>
            </template>
          </div>
        </div>
        <div class="flex gap-6 text-sm font-medium text-slate-500">
          <a href="#" class="hover:text-blue-600 transition-colors">Soporte</a>
          <a href="#" class="hover:text-blue-600 transition-colors">Privacidad</a>
          <a href="#" class="hover:text-blue-600 transition-colors">Términos</a>
        </div>
      </div>
    </footer>
  </div>
</template>
