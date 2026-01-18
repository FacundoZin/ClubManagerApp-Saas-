<template>
  <div
    class="min-h-screen flex items-center justify-center bg-gray-100 p-4 bg-gradient-to-br from-blue-50 to-gray-200"
  >
    <div
      class="bg-white p-8 sm:p-10 rounded-2xl shadow-xl w-full max-w-md border border-white/50 backdrop-blur-sm"
    >
      <div class="text-center mb-8">
        <div
          class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-blue-100 mb-4 text-blue-600"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-8 w-8"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
            />
          </svg>
        </div>
        <h1 class="text-3xl font-bold text-gray-800">Sistema Casa del Jubilado</h1>
        <p class="text-gray-500 mt-2">Por favor ingrese sus credenciales para acceder</p>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-6">
        <div>
          <label class="block text-gray-700 text-sm font-semibold mb-2" for="username">
            Nombre de Usuario
          </label>
          <input
            v-model="username"
            type="text"
            id="username"
            class="w-full px-4 py-3 rounded-xl bg-gray-50 border border-gray-200 focus:border-blue-500 focus:bg-white focus:ring-2 focus:ring-blue-200 focus:outline-none transition-all duration-200"
            required
            placeholder="Ej. admin"
            autofocus
          />
        </div>

        <div>
          <label class="block text-gray-700 text-sm font-semibold mb-2" for="password">
            Contraseña
          </label>
          <input
            v-model="password"
            type="password"
            id="password"
            class="w-full px-4 py-3 rounded-xl bg-gray-50 border border-gray-200 focus:border-blue-500 focus:bg-white focus:ring-2 focus:ring-blue-200 focus:outline-none transition-all duration-200"
            required
            placeholder="••••••••"
          />
        </div>

        <div
          v-if="error"
          class="p-3 bg-red-50 border border-red-100 text-red-600 text-sm rounded-lg flex items-center"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5 mr-2 flex-shrink-0"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
              clip-rule="evenodd"
            />
          </svg>
          {{ error }}
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3.5 px-4 rounded-xl shadow-lg shadow-blue-500/30 transform transition duration-200 hover:-translate-y-0.5 disabled:opacity-70 disabled:cursor-not-allowed disabled:transform-none flex justify-center items-center"
        >
          <svg
            v-if="loading"
            class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
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
          <span v-else>Iniciar Sesión</span>
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import AuthService from '../services/AuthService'

const router = useRouter()
const username = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const handleLogin = async () => {
  loading.value = true
  error.value = ''

  const result = await AuthService.login(username.value, password.value)

  if (result.success) {
    router.push('/')
  } else {
    error.value = result.message
  }
  loading.value = false
}
</script>
