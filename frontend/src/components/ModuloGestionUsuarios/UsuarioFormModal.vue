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
          <div class="relative">
            <input
              v-model="form.password"
              :type="showPassword ? 'text' : 'password'"
              id="password"
              class="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-100 focus:border-blue-500 outline-none transition-all pr-12"
              required
              minlength="6"
              placeholder="Mínimo 6 caracteres"
            />
            <button
              type="button"
              @click="showPassword = !showPassword"
              class="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 p-1"
              :title="showPassword ? 'Ocultar contraseña' : 'Ver contraseña'"
            >
              <svg
                v-if="showPassword"
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path
                  fill-rule="evenodd"
                  d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26a4 4 0 015.49 5.49L8.261 6.553z"
                  clip-rule="evenodd"
                />
                <path
                  d="M12.454 16.697L9.75 13.992a4 4 0 01-3.742-3.741L2.335 6.578A9.98 9.98 0 00.458 10c1.274 4.057 5.065 7 9.542 7 .847 0 1.669-.105 2.454-.303z"
                />
              </svg>
              <svg
                v-else
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
                <path
                  fill-rule="evenodd"
                  d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                  clip-rule="evenodd"
                />
              </svg>
            </button>
          </div>
        </div>

        <!-- Confirmar Contraseña -->
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-1" for="confirmPassword"
            >Confirmar Contraseña</label
          >
          <div class="relative">
            <input
              v-model="confirmPassword"
              :type="showPassword ? 'text' : 'password'"
              id="confirmPassword"
              class="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-100 focus:border-blue-500 outline-none transition-all pr-12"
              required
              placeholder="Repita la contraseña"
            />
          </div>
        </div>

        <!-- Rol -->
        <div>
          <label class="block text-sm font-semibold text-gray-700 mb-1">Rol</label>
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-3 mt-2">
            <label
              class="flex items-center gap-2 cursor-pointer p-3 border rounded-lg hover:bg-gray-50 transition-colors"
              :class="{ 'ring-2 ring-green-500 border-green-500 bg-green-50': form.rol === 0 }"
            >
              <input type="radio" v-model="form.rol" :value="0" class="hidden" />
              <span
                class="w-4 h-4 rounded-full border border-gray-400 flex items-center justify-center"
              >
                <span v-if="form.rol === 0" class="w-2 h-2 rounded-full bg-green-600"></span>
              </span>
              <span class="font-medium text-gray-700 text-sm">Usuario</span>
            </label>

            <label
              class="flex items-center gap-2 cursor-pointer p-3 border rounded-lg hover:bg-gray-50 transition-colors"
              :class="{ 'ring-2 ring-purple-500 border-purple-500 bg-purple-50': form.rol === 1 }"
            >
              <input type="radio" v-model="form.rol" :value="1" class="hidden" />
              <span
                class="w-4 h-4 rounded-full border border-gray-400 flex items-center justify-center"
              >
                <span v-if="form.rol === 1" class="w-2 h-2 rounded-full bg-purple-600"></span>
              </span>
              <span class="font-medium text-gray-700 text-sm">SuperAdmin</span>
            </label>

            <label
              class="flex items-center gap-2 cursor-pointer p-3 border rounded-lg hover:bg-gray-50 transition-colors"
              :class="{ 'ring-2 ring-blue-500 border-blue-500 bg-blue-50': form.rol === 2 }"
            >
              <input type="radio" v-model="form.rol" :value="2" class="hidden" />
              <span
                class="w-4 h-4 rounded-full border border-gray-400 flex items-center justify-center"
              >
                <span v-if="form.rol === 2" class="w-2 h-2 rounded-full bg-blue-600"></span>
              </span>
              <span class="font-medium text-gray-700 text-sm">Cobrador</span>
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
const showPassword = ref(false)
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
