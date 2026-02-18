<script setup>
defineProps({
  reservas: {
    type: Array,
    default: () => []
  }
})

defineEmits(['view-details', 'cancel'])
</script>

<template>
  <div class="overflow-x-auto rounded-xl border border-slate-200 shadow-sm">
    <table class="min-w-full divide-y divide-slate-200">
      <thead class="bg-slate-50">
        <tr>
          <th scope="col" class="px-6 py-4 text-left text-xs font-bold text-slate-500 uppercase tracking-wider">
            Título
          </th>
          <th scope="col" class="px-6 py-4 text-left text-xs font-bold text-slate-500 uppercase tracking-wider">
            Fecha
          </th>
          <th scope="col" class="px-6 py-4 text-left text-xs font-bold text-slate-500 uppercase tracking-wider">
            Reservante
          </th>
          <th scope="col" class="px-6 py-4 text-left text-xs font-bold text-slate-500 uppercase tracking-wider">
            Estado
          </th>
          <th scope="col" class="px-6 py-4 text-right text-xs font-bold text-slate-500 uppercase tracking-wider">
            Acciones
          </th>
        </tr>
      </thead>
      <tbody class="bg-white divide-y divide-slate-200">
        <tr v-for="reserva in reservas" :key="reserva.id" class="hover:bg-slate-50/80 transition-colors duration-200">
          <td class="px-6 py-4 whitespace-nowrap text-sm font-bold text-slate-900">
            {{ reserva.titulo || 'Sin título' }}
          </td>
          <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500 font-medium">
            {{ reserva.fechaAlquiler }}
          </td>
          <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 font-medium">
            {{ reserva.nombreReservante }}
          </td>
          <td class="px-6 py-4 whitespace-nowrap text-sm">
            <span
              class="inline-flex items-center px-2.5 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider border"
              :class="reserva.pagado ? 'bg-emerald-50 text-emerald-700 border-emerald-200' : 'bg-amber-50 text-amber-700 border-amber-200'">
              {{ reserva.pagado ? 'Pagado' : 'Pendiente' }}
            </span>
          </td>
          <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
            <button @click="$emit('view-details', reserva.id)"
              class="text-indigo-600 hover:text-indigo-900 bg-indigo-50 hover:bg-indigo-100 p-2.5 rounded-xl transition-all duration-200"
              title="Ver detalles">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
            </button>
            <button @click="$emit('cancel', reserva)"
              class="text-red-600 hover:text-red-900 bg-red-50 hover:bg-red-100 p-2.5 rounded-xl transition-all duration-200"
              title="Cancelar reserva">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24"
                stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          </td>
        </tr>
        <tr v-if="reservas.length === 0">
          <td colspan="5" class="px-6 py-12 text-center text-slate-500">
            No se encontraron reservas.
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
