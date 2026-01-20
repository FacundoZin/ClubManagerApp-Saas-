<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AuthService from '../../services/AuthService'

const route = useRoute()
const router = useRouter()
const currentUser = AuthService.currentUser

// Calcular iniciales del usuario
const userInitials = computed(() => {
  if (!currentUser.value || !currentUser.value.nombreUsuario) return '??'
  const name = currentUser.value.nombreUsuario.trim()
  if (name.length === 0) return '??'

  // Si tiene espacios, tomamos la primera letra de las dos primeras palabras
  const parts = name.split(/\s+/)
  if (parts.length > 1) {
    return (parts[0][0] + parts[1][0]).toUpperCase()
  }

  // Si es una sola palabra, tomamos las dos primeras letras si existen
  return name.substring(0, Math.min(2, name.length)).toUpperCase()
})

const logout = async () => {
  await AuthService.logout()
  router.push('/login')
}

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
  usuarios: {
    icon: 'M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0z',
    colorClass: 'bg-purple-600',
    defaultTitle: 'Gestión de Usuarios',
  },
  default: {
    icon: 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4',
    colorClass: 'bg-blue-700',
    defaultTitle: 'Panel de Administración',
  },
}

// Computados
const currentModule = computed(() => {
  const moduleKey = route.meta.module
  return modulesConfig[moduleKey] || modulesConfig.default
})

const moduleName = computed(() => route.meta.headerTitle || currentModule.value.defaultTitle)
</script>

<template>
  <header class="bg-white border-b border-slate-200 fixed top-0 w-full z-30 shadow-sm font-sans">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16 items-center">
        <!-- Logo / Título -->
        <router-link to="/" class="flex items-center gap-3 group">
          <div
            class="w-9 h-9 rounded-lg flex items-center justify-center shadow-md text-white transition-all duration-300 group-hover:scale-105"
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
            <span class="text-xs text-slate-500 font-medium transition-all duration-300">{{
              moduleName
            }}</span>
          </div>
        </router-link>

        <!-- User / Actions -->
        <div class="flex items-center gap-4">
          <template v-if="currentUser">
            <div class="hidden sm:flex flex-col items-end">
              <span class="text-xs font-semibold text-slate-700 leading-tight">
                {{ currentUser.nombreUsuario }}
              </span>
              <span class="text-[10px] text-slate-400 uppercase tracking-wider font-medium">
                {{ currentUser.rol === 1 ? 'Administrador' : 'Usuario' }}
              </span>
            </div>

            <div
              class="h-9 w-9 rounded-full bg-slate-100 border border-slate-200 flex items-center justify-center text-slate-600 text-xs font-bold shadow-sm ring-2 ring-white"
              :title="currentUser.rol === 1 ? 'SuperAdmin' : 'Usuario'"
            >
              {{ userInitials }}
            </div>

            <div class="h-6 w-px bg-slate-200 mx-1"></div>

            <button
              @click="logout"
              class="p-2 text-slate-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all duration-200 group"
              title="Cerrar Sesión"
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
                  d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
                />
              </svg>
            </button>
          </template>
        </div>
      </div>
    </div>
  </header>
</template>
