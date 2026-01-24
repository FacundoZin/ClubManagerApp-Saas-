<template>
  <div class="px-6 py-6 max-w-7xl mx-auto">
    <div class="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
      <!-- Header de la sección -->
      <div class="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
        <div>
          <h2 class="text-xl font-bold text-gray-800">Gestión de Usuarios</h2>
          <p class="text-sm text-gray-500 mt-1">Administra los accesos al sistema</p>
        </div>
        <button
          @click="openCreateModal"
          class="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2.5 rounded-xl text-sm font-semibold shadow-sm transition-all duration-200 flex items-center gap-2"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="h-5 w-5"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fill-rule="evenodd"
              d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
              clip-rule="evenodd"
            />
          </svg>
          Nuevo Usuario
        </button>
      </div>

      <!-- Tabla de Usuarios -->
      <div v-if="loading" class="p-12 flex justify-center">
        <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600"></div>
      </div>

      <div v-else-if="error" class="p-8 text-center text-red-500">
        {{ error }}
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-gray-50/50 border-b border-gray-100">
              <th class="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                Usuario
              </th>
              <th class="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                Rol
              </th>
              <th class="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                Fecha Creación
              </th>
              <th class="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">
                Último Acceso
              </th>
              <th
                class="px-6 py-4 text-xs font-semibold text-gray-500 uppercase tracking-wider text-right"
              >
                Estado
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-50">
            <tr
              v-for="user in usuarios"
              :key="user.id"
              class="hover:bg-gray-50/50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div
                    class="h-9 w-9 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center font-bold text-sm"
                  >
                    {{ user.nombreUsuario.charAt(0).toUpperCase() }}
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-semibold text-gray-900">{{ user.nombreUsuario }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  class="px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full"
                  :class="getRoleClass(user.rol)"
                >
                  {{ getRoleLabel(user.rol) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                {{ formatDate(user.fechaCreacion) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <span v-if="user.ultimoAcceso">{{ formatDate(user.ultimoAcceso) }}</span>
                <span v-else class="text-gray-400 italic">Nunca</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right">
                <div class="flex justify-end gap-2">
                  <span
                    class="inline-block w-2.5 h-2.5 rounded-full bg-green-500"
                    title="Activo"
                  ></span>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-if="usuarios.length === 0" class="p-12 text-center text-gray-500">
          No hay usuarios registrados.
        </div>
      </div>
    </div>

    <!-- Modal para crear usuario -->
    <UsuarioFormModal v-if="showModal" @close="closeModal" @created="onUserCreated" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import UsuariosService from '../../services/UsuariosService'
import UsuarioFormModal from '../../components/ModuloGestionUsuarios/UsuarioFormModal.vue'
import { format } from 'date-fns'
import { es } from 'date-fns/locale'

const usuarios = ref([])
const loading = ref(true)
const error = ref('')
const showModal = ref(false)

const loadUsuarios = async () => {
  loading.value = true
  error.value = ''
  try {
    const response = await UsuariosService.getAll()
    if (response) {
      usuarios.value = Array.isArray(response) ? response : response.data || []
      // Si response.data viene wrapper en Result<T>, ajustar según estructura
      if (response.data && Array.isArray(response.data)) {
        usuarios.value = response.data
      }
    }
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

const openCreateModal = () => {
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const onUserCreated = () => {
  loadUsuarios()
  showModal.value = false
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  return format(new Date(dateString), 'dd MMM yyyy HH:mm', { locale: es })
}

const getRoleLabel = (role) => {
  switch (role) {
    case 1:
      return 'SuperAdmin'
    case 2:
      return 'Cobrador'
    default:
      return 'Usuario'
  }
}

const getRoleClass = (role) => {
  switch (role) {
    case 1:
      return 'bg-purple-100 text-purple-800'
    case 2:
      return 'bg-blue-100 text-blue-800'
    default:
      return 'bg-green-100 text-green-800'
  }
}

onMounted(() => {
  loadUsuarios()
})
</script>
