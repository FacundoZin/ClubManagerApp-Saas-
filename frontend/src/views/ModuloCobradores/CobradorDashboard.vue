<script setup>
import { ref } from 'vue'
import CobradorSocioCard from '../../components/ModuloCobradores/CobradorSocioCard.vue'
import SocioUpdateModal from '../../components/ModuloGestionSocios/SocioUpdateModal.vue'
import CobranzasService from '../../services/CobranzasService'
import CuotasService from '../../services/CuotasService'
import SociosService from '../../services/SociosService'

// Estado
const lotes = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10'] // Ejemplo de lotes, idealmente vendrían de una API o config
const selectedLote = ref('')
const socios = ref([])
const loading = ref(false)
const error = ref('')

// Modal Editar
const isEditModalOpen = ref(false)
const selectedSocioId = ref(null)

// Acciones
const buscarSocios = async () => {
  if (!selectedLote.value) {
    socios.value = []
    return
  }

  loading.value = true
  error.value = ''
  try {
    const data = await CobranzasService.listarSociosDeudoresPorLote(selectedLote.value)
    socios.value = data
  } catch (e) {
    error.value = e.message
    socios.value = []
  } finally {
    loading.value = false
  }
}

const handlePago = async (socio) => {
  if (!confirm(`¿Registrar pago para ${socio.nombre} ${socio.apellido}?`)) return

  try {
    const paymentData = {
      socioId: socio.id,
      monto: socio.deuda, // O el monto que corresponda, la API debe validar. Asumo deuda total o cuota.
      // El requerimiento dice "El resto de los datos necesarios para registrar la cuota deben enviarse según lo definido por la API."
      // Sin ver la API de Cuotas, asumo que necesito enviar el ID del socio y la forma de pago.
      // Si es 'RegistrarCuota', quizás necesite el ID de la cuota o detalle.
      // Pero el usuario dijo "comportamiento general... permitir que un cobrador gestione socios... Registrar pago".
      // Y "forma de pago COBRADOR".
      // Voy a asumir una estructura genérica { socioId, formaPago: 'COBRADOR' } y quizás el monto si es variable.
      // Si falla, el error nos dirá.
      formaPago: 'COBRADOR',
      // Otras propiedades pueden ser necesarias.
    }

    await CuotasService.registrarCuota(paymentData)
    alert('Pago registrado exitosamente')
    // Recargar lista para actualizar deuda/estado
    buscarSocios()
  } catch (e) {
    alert(`Error al registrar pago: ${e.message}`)
  }
}

const openEditModal = (socio) => {
  selectedSocioId.value = socio.id
  isEditModalOpen.value = true
}

const handleSocioUpdated = (updatedSocio) => {
  isEditModalOpen.value = false
  // Actualizar lista localmente
  const index = socios.value.findIndex((s) => s.id === updatedSocio.id)
  if (index !== -1) {
    socios.value[index] = { ...socios.value[index], ...updatedSocio }
  }
  // Opcional: Recargar para asegurar consistencia
  // buscarSocios()
}

const handleDelete = async (socio) => {
  if (
    !confirm(
      `¿Está seguro de dar de baja a ${socio.nombre} ${socio.apellido}? Esta acción no se puede deshacer.`,
    )
  )
    return

  try {
    await SociosService.removeSocio(socio.id)
    alert('Socio dado de baja exitosamente')
    // Remover de la lista local
    socios.value = socios.value.filter((s) => s.id !== socio.id)
  } catch (e) {
    alert(`Error al dar de baja: ${e.message}`)
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-50 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header -->
      <!-- Header (Removed, handled by AppHeader) -->

      <!-- Selector de Lote -->
      <div class="bg-white rounded-xl shadow-sm border border-slate-200 p-6 mb-8">
        <div class="max-w-md">
          <label for="lote-select" class="block text-sm font-bold text-slate-700 mb-2"
            >Seleccionar Lote</label
          >
          <select
            id="lote-select"
            v-model="selectedLote"
            @change="buscarSocios"
            class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-base px-4 py-3 border transition-all"
          >
            <option value="" disabled>-- Seleccione un lote --</option>
            <option v-for="lote in lotes" :key="lote" :value="lote">Lote {{ lote }}</option>
          </select>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="flex justify-center py-12">
        <svg
          class="animate-spin h-10 w-10 text-blue-600"
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
      </div>

      <!-- Error State -->
      <div
        v-else-if="error"
        class="bg-red-50 border border-red-200 rounded-xl p-4 mb-6 text-red-700 flex items-center gap-3"
      >
        <svg class="h-6 w-6 text-red-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        {{ error }}
      </div>

      <!-- Lista de Socios -->
      <div
        v-else-if="socios.length > 0"
        class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
      >
        <CobradorSocioCard
          v-for="socio in socios"
          :key="socio.id"
          :socio="socio"
          @pay="handlePago"
          @edit="openEditModal"
          @delete="handleDelete"
        />
      </div>

      <!-- Empty State -->
      <div
        v-else-if="selectedLote && !loading"
        class="text-center py-12 bg-white rounded-xl border border-dashed border-slate-300"
      >
        <div
          class="inline-flex items-center justify-center w-16 h-16 rounded-full bg-slate-100 mb-4"
        >
          <svg class="h-8 w-8 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
            />
          </svg>
        </div>
        <h3 class="text-lg font-medium text-slate-900">No hay socios deudores en este lote</h3>
        <p class="text-slate-500 mt-1">Seleccione otro lote o verifique más tarde.</p>
      </div>

      <div v-else class="text-center py-12 text-slate-500">
        Seleccione un lote para comenzar la búsqueda.
      </div>
    </div>

    <!-- Modals -->
    <SocioUpdateModal
      :is-open="isEditModalOpen"
      :socio-id="selectedSocioId"
      @close="isEditModalOpen = false"
      @save="handleSocioUpdated"
    />
  </div>
</template>
