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
  analiticas: {
    icon: 'M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z',
    colorClass: 'bg-indigo-600',
    defaultTitle: 'Estadísticas y Balances',
  },
  default: {
    icon: 'M4 5a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM14 5a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1h-4a1 1 0 01-1-1V5zM4 15a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1H5a1 1 0 01-1-1v-4zM14 15a1 1 0 011-1h4a1 1 0 011 1v4a1 1 0 01-1 1h-4a1 1 0 01-1-1v-4z',
    colorClass: 'bg-blue-600',
    defaultTitle: 'PANEL DE ADMINISTRACIÓN',
  },
}

// Computados
const currentModule = computed(() => {
  const moduleKey = route.meta.module
  return modulesConfig[moduleKey] || modulesConfig.default
})

const moduleName = computed(() => route.meta.headerTitle || currentModule.value.defaultTitle)

const formattedDate = computed(() => {
  return new Date()
    .toLocaleDateString('es-ES', {
      weekday: 'long',
      day: 'numeric',
      month: 'short',
    })
    .toUpperCase()
})

const userRoleName = computed(() => {
  if (!currentUser.value) return ''
  const roles = {
    0: 'Usuario',
    1: 'Administrador',
    2: 'Cobrador',
  }
  return roles[currentUser.value.rol] || 'Usuario'
})
</script>

<template>
  <header class="bg-white border-b border-slate-200 fixed top-0 w-full z-30 shadow-sm font-sans">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16 items-center">
        <!-- Logo / Título -->
        <router-link to="/" class="flex items-center gap-3 group">
          <div
            class="w-10 h-10 rounded-xl flex items-center justify-center shadow-sm text-white transition-all duration-300 group-hover:scale-105"
            :class="currentModule.colorClass"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5.5 w-5.5"
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
            <h1 class="text-[15px] font-bold text-[#1e293b] tracking-tight leading-tight">
              Asociación Civil Casa del Jubilado
            </h1>
            <p class="text-[10px] text-slate-400 font-medium uppercase tracking-widest">
              {{ moduleName }}
            </p>
          </div>
        </router-link>

        <!-- User / Actions -->
        <div class="flex items-center gap-4">
          <template v-if="currentUser">
            <div class="hidden sm:flex flex-col items-end mr-1">
              <span class="text-sm text-[#2563eb] font-semibold leading-tight">
                {{ userRoleName }}
              </span>
              <span class="text-[10px] text-slate-400 uppercase tracking-widest mt-0.5">
                {{ formattedDate }}
              </span>
            </div>

            <div
              class="h-10 w-10 rounded-full bg-[#eff6ff] border border-blue-100 flex items-center justify-center text-[#2563eb] text-xs font-bold shadow-sm"
              :title="currentUser.nombreUsuario"
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
