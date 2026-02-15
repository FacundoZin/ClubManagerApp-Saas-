<script setup>
import { computed } from 'vue'

const props = defineProps({
  statusData: {
    type: Object,
    required: true,
  },
})

const emit = defineEmits(['create-new', 'view-detail', 'cancel'])

const hasActiveRental = computed(() => props.statusData.idAlquiler !== null)

const handlePrimaryAction = () => {
  if (hasActiveRental.value) {
    emit('view-detail', props.statusData.idAlquiler)
  } else {
    emit('create-new', props.statusData)
  }
}
</script>

<template>
  <div
    class="max-w-md mx-auto bg-white rounded-2xl shadow-xl border border-slate-100 overflow-hidden"
  >
    <div class="p-6 sm:p-8">
      <div class="flex items-center justify-center mb-6">
        <div
          class="h-16 w-16 rounded-full flex items-center justify-center border"
          :class="
            hasActiveRental
              ? 'bg-blue-50 border-blue-100 text-blue-600'
              : 'bg-teal-50 border-teal-100 text-teal-600'
          "
        >
          <!-- Icon for Active Rental (Warning/Edit) -->
          <svg
            v-if="hasActiveRental"
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
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
            />
          </svg>
          <!-- Icon for No Rental (Success/Add) -->
          <svg
            v-else
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
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        </div>
      </div>

      <div class="text-center">
        <h3 class="text-xl font-bold text-slate-900 mb-2">
          {{ statusData.apellidoSocio }}, {{ statusData.nombreSocio }}
        </h3>
        <p class="text-sm text-slate-500 mb-6 px-4">
          {{ statusData.mensaje }}
        </p>

        <div class="space-y-3">
          <button
            @click="handlePrimaryAction"
            class="w-full inline-flex justify-center items-center px-4 py-3 border border-transparent text-sm font-semibold rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-offset-2 transition-all shadow-md group"
            :class="
              hasActiveRental
                ? 'bg-blue-600 hover:bg-blue-700 focus:ring-blue-500'
                : 'bg-teal-600 hover:bg-teal-700 focus:ring-teal-500'
            "
          >
            <span v-if="hasActiveRental">Ver detalle del alquiler</span>
            <span v-else>Crear nuevo alquiler</span>

            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5 ml-2 group-hover:translate-x-1 transition-transform"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M17 8l4 4m0 0l-4 4m4-4H3"
              />
            </svg>
          </button>

          <button
            @click="emit('cancel')"
            class="w-full inline-flex justify-center items-center px-4 py-3 border border-slate-200 text-sm font-medium rounded-xl text-slate-700 bg-white hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-slate-500 transition-colors"
          >
            Buscar otro socio
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
