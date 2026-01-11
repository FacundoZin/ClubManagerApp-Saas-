<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

// Configuración de módulos (Íconos y Colores coincidiendo con HomeView)
const modulesConfig = {
  socios: {
    icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0z',
    colorClass: 'bg-blue-700',
    defaultTitle: 'Gestión de Socios',
  },
  reservas: {
    icon: 'M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z',
    colorClass: 'bg-indigo-600',
    defaultTitle: 'Gestión Reservas',
  },
  articulos: {
    icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
    colorClass: 'bg-sky-600',
    defaultTitle: 'Alquiler de Artículos',
  },
  cobradores: {
    icon: 'M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z',
    colorClass: 'bg-cyan-600',
    defaultTitle: 'Módulo Cobradores',
  },
  viajes: {
    icon: 'M9 20l-5.447-2.724A1 1 0 013 16.382V5.618a1 1 0 011.447-.894L9 7m0 13l6-3m-6 3V7m6 10l4.553 2.276A1 1 0 0021 18.382V7.618a1 1 0 00-.553-.894L15 4m0 13V4m0 0L9 7',
    colorClass: 'bg-teal-600',
    defaultTitle: 'Gestión de Viajes',
  },
  pagos: {
    icon: 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z',
    colorClass: 'bg-emerald-600',
    defaultTitle: 'Registrar Pago',
  },
  default: {
    icon: 'M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6',
    colorClass: 'bg-slate-700',
    defaultTitle: 'Sistema Club Abuelos',
  },
}

// Computados
const currentModule = computed(() => {
  const moduleKey = route.meta.module
  return modulesConfig[moduleKey] || modulesConfig.default
})

// Title is now the module name (dynamic)
const moduleName = computed(() => route.meta.headerTitle || currentModule.value.defaultTitle)
</script>

<template>
  <!-- Renderizado condicional basado en meta.hideHeader -->
  <!-- Envolvemos en un div w-full para evitar conflictos con estilos globales de 'header' en App.vue -->
  <div v-if="!route.meta.hideHeader" class="w-full font-sans h-16">
    <header class="bg-white border-b border-slate-200 fixed top-0 w-full z-30 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16 items-center w-full">
          <!-- Logo / Título -->
          <div class="flex items-center gap-3">
            <div
              class="w-9 h-9 rounded-lg flex items-center justify-center shadow-md text-white transition-colors duration-300"
              :class="currentModule.colorClass"
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
                  :d="currentModule.icon"
                />
              </svg>
            </div>
            <div>
              <h1 class="text-lg font-bold text-slate-900 tracking-tight leading-none">
                Sistema Club Abuelos
              </h1>
              <span class="text-xs text-slate-500 font-medium">{{ moduleName }}</span>
            </div>
          </div>

          <!-- User / Date Info (Igual que HomeView) -->
          <div class="flex items-center gap-6">
            <div class="hidden md:flex flex-col items-end">
              <span class="text-xs font-semibold text-slate-700">Administrador</span>
              <span class="text-[10px] text-slate-400 uppercase tracking-wider">
                {{
                  new Date().toLocaleDateString('es-AR', {
                    weekday: 'long',
                    day: 'numeric',
                    month: 'short',
                  })
                }}
              </span>
            </div>
            <div
              class="h-9 w-9 rounded-full bg-slate-100 border border-slate-200 flex items-center justify-center text-slate-600 text-xs font-bold shadow-sm ring-2 ring-white"
            >
              AD
            </div>
          </div>
        </div>
      </div>
    </header>
  </div>
</template>
