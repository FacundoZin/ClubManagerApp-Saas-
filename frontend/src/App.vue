<template>
  <LoadingOverlay :show="isLoading" message="Cargando..." />
  <AppHeader v-if="showContent && !route.meta.hideHeader" />
  <main :class="{ 'pt-16': !route.meta.hideHeader }">
    <router-view v-slot="{ Component }">
      <transition name="fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
  </main>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { RouterView, useRoute, useRouter } from 'vue-router'
import AppHeader from '@/components/Common/AppHeader.vue'
import LoadingOverlay from '@/components/Common/LoadingOverlay.vue'
import AuthService from '@/services/AuthService'

const route = useRoute()
const router = useRouter()
const isLoading = ref(false)
const showContent = ref(false)

// Manejo de carga global
router.beforeEach((to, from, next) => {
  isLoading.value = true
  next()
})

router.afterEach(() => {
  // Delay mínimo para evitar parpadeos, ahora mucho más corto
  setTimeout(() => {
    isLoading.value = false
  }, 100)
})

onMounted(async () => {
  isLoading.value = true
  // Verificar sesión al montar el app
  await AuthService.checkAuth()
  showContent.value = true
  isLoading.value = false
})
</script>

<style>
/* Estilos globales para transiciones */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

header {
  line-height: 1.5;
  max-height: 100vh;
}
</style>
