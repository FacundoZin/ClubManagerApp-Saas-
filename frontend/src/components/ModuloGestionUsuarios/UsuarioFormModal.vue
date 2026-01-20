<template>
  <div
    class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm"
    @click.self="$emit('close')"
  >
    <div
      class="bg-white rounded-2xl shadow-xl w-full max-w-md overflow-hidden transform transition-all"
    >
      <div
        class="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50/50"
      >
        <h3 class="text-lg font-bold text-gray-800">Crear Nuevo Usuario</h3>
        <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 transition-colors">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-6 w-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>

      <form @submit.prevent="submitForm" class="p-6 space-y-5">
        <!-- Nombre de Usuario -->
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-1" for="nombreUsuario"
            >Usuario</label
          >
          <input
            v-model="form.nombreUsuario"
            type="text"
            id="nombreUsuario"
            class="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-100 focus:border-blue-500 outline-none transition-all placeholder-gray-400"
            placeholder="Ej: nuevoAdmin"
            required
            minlength="3"
          />
        </div>

        <!-- Contraseña -->
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-1" for="password"
            >Contraseña</label
          >
          <input
            v-model="form.password"
            type="password"
            id="password"
            class="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-100 focus:border-blue-500 outline-none transition-all"
            required
            minlength="6"
            placeholder="Mínimo 6 caracteres"
          />
        </div>

        <!-- Confirmar Contraseña -->
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-1" for="confirmPassword"
            >Confirmar Contraseña</label
          >
          <input
            v-model="confirmPassword"
            type="password"
            id="confirmPassword"
            class="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-100 focus:border-blue-500 outline-none transition-all"
            required
            placeholder="Repita la contraseña"
          />
        </div>

        <!-- Rol -->
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-1">Rol</label>
          <div class="flex gap-4 mt-2">
            <label
              class="flex items-center gap-2 cursor-pointer p-3 border rounded-lg flex-1 hover:bg-gray-50 transition-colors"
              :class="{ 'ring-2 ring-blue-500 border-blue-500 bg-blue-50': form.rol === 0 }"
            >
              <input type="radio" v-model="form.rol" :value="0" class="hidden" />
              <span
                class="w-4 h-4 rounded-full border border-gray-400 flex items-center justify-center"
              >
                <span v-if="form.rol === 0" class="w-2 h-2 rounded-full bg-blue-600"></span>
              </span>
              <span class="font-medium text-gray-700">Usuario</span>
            </label>

            <label
              class="flex items-center gap-2 cursor-pointer p-3 border rounded-lg flex-1 hover:bg-gray-50 transition-colors"
              :class="{ 'ring-2 ring-purple-500 border-purple-500 bg-purple-50': form.rol === 1 }"
            >
              <input type="radio" v-model="form.rol" :value="1" class="hidden" />
              <span
                class="w-4 h-4 rounded-full border border-gray-400 flex items-center justify-center"
              >
                <span v-if="form.rol === 1" class="w-2 h-2 rounded-full bg-purple-600"></span>
              </span>
              <span class="font-medium text-gray-700">SuperAdmin</span>
            </label>
          </div>
        </div>

        <div
          v-if="error"
          class="text-sm text-red-600 bg-red-50 p-3 rounded-lg border border-red-100"
        >
          {{ error }}
        </div>

        <div class="flex justify-end gap-3 pt-4">
          <button
            type="button"
            @click="$emit('close')"
            class="px-5 py-2.5 text-gray-700 font-medium hover:bg-gray-100 rounded-xl transition-colors"
          >
            Cancelar
          </button>
          <button
            type="submit"
            :disabled="loading"
            class="px-6 py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-semibold rounded-xl shadow-lg shadow-blue-500/20 transition-all transform hover:-translate-y-0.5 disabled:opacity-70 disabled:transform-none flex items-center"
          >
            <svg
              v-if="loading"
              class="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
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
            Crear Usuario
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import UsuariosService from '../../services/UsuariosService'

const emit = defineEmits(['close', 'created'])

const form = ref({
  nombreUsuario: '',
  password: '',
  rol: 0, // 0 = Usuario
})
const confirmPassword = ref('')
const loading = ref(false)
const error = ref('')

const submitForm = async () => {
  if (form.value.password !== confirmPassword.value) {
    error.value = 'Las contraseñas no coinciden'
    return
  }

  loading.value = true
  error.value = ''

  try {
    await UsuariosService.create(form.value)
    emit('created')
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>
