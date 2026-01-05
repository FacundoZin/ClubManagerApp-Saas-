<script setup>
defineProps({
  reserva: {
    type: Object,
    required: true
  }
})

defineEmits(['view', 'delete'])
</script>

<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-6 hover:shadow-md transition-shadow">
    <div class="flex justify-between items-start mb-4">
      <div>
        <h3 class="text-lg font-bold text-slate-900">{{ reserva.titulo || 'Reserva Salon' }}</h3>
        <p class="text-sm text-slate-500 flex items-center gap-1 mt-1">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            {{ reserva.fechaAlquiler }}
        </p>
      </div>
      <span v-if="reserva.pagado !== undefined" 
            class="px-2.5 py-1 rounded-full text-xs font-semibold uppercase tracking-wide"
            :class="reserva.pagado ? 'bg-emerald-100 text-emerald-800' : 'bg-yellow-100 text-yellow-800'">
        {{ reserva.pagado ? 'Pagado' : 'Pendiente' }}
      </span>
    </div>

    <div class="space-y-3 mb-6">
        <div v-if="reserva.nombreReservante" class="flex items-center text-sm text-slate-700">
             <span class="w-24 text-slate-500 font-medium shrink-0">Reservante:</span>
             <span class="font-semibold">{{ reserva.nombreReservante }}</span>
        </div>
        <div v-if="reserva.nombreSocio" class="flex items-center text-sm text-slate-700">
             <span class="w-24 text-slate-500 font-medium shrink-0">Socio:</span>
             <span class="font-semibold">{{ reserva.nombreSocio }} {{ reserva.apellidoSocio }}</span>
        </div>
        <div v-if="reserva.nombreSalon" class="flex items-center text-sm text-slate-700">
            <span class="w-24 text-slate-500 font-medium shrink-0">Salon:</span>
            <span>{{ reserva.nombreSalon }}</span>
        </div>
         <div v-if="reserva.importe !== undefined" class="flex items-center text-sm text-slate-700">
            <span class="w-24 text-slate-500 font-medium shrink-0">Importe:</span>
            <span>${{ reserva.importe }}</span>
        </div>
    </div>

    <div class="flex gap-3 mt-auto pt-4 border-t border-slate-100">
      <button @click="$emit('view', reserva.id || reserva.idReserva)" 
        class="flex-1 inline-flex justify-center items-center px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
        </svg>
        Ver detalle
      </button>
      <button @click="$emit('delete', reserva)" 
        class="inline-flex justify-center items-center px-4 py-2 text-sm font-medium text-red-600 bg-red-50 rounded-lg hover:bg-red-100 transition-colors"
        title="Cancelar Reserva">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
        </svg>
      </button>
    </div>
  </div>
</template>
