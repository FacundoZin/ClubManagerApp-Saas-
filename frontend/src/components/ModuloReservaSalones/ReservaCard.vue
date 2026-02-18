<script setup>
defineProps({
  reserva: {
    type: Object,
    required: true,
  },
})

defineEmits(['view', 'delete'])

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(amount)
}
</script>

<template>
  <div
    class="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 hover:shadow-md transition-all duration-300 flex flex-col h-full relative group">
    <!-- Header: Título, Fecha y Estado -->
    <div class="flex justify-between items-start mb-5">
      <div class="flex flex-col sm:flex-row sm:items-center gap-2 pr-12">
        <h3 class="text-lg font-bold text-slate-900 leading-tight">
          {{ reserva.titulo || 'Reserva del Salón' }}
        </h3>
        <span
          class="inline-flex items-center px-2.5 py-0.5 rounded-lg text-xs font-semibold bg-slate-100 text-slate-600 border border-slate-200 whitespace-nowrap">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5 mr-1" fill="none" viewBox="0 0 24 24"
            stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          {{ reserva.fechaAlquiler }}
        </span>
      </div>

      <!-- Badge de Pago (Absoluto para mantener alineación del título) -->
      <div class="absolute top-6 right-6">
        <span v-if="reserva.pagado !== undefined"
          class="px-2.5 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider border transition-colors duration-300"
          :class="reserva.pagado
              ? 'bg-emerald-50 text-emerald-700 border-emerald-200'
              : 'bg-amber-50 text-amber-700 border-amber-200'
            ">
          {{ reserva.pagado ? 'Pagado' : 'Pendiente' }}
        </span>
      </div>
    </div>

    <!-- Bloque de Información Unificado -->
    <div class="space-y-3 mb-4">
      <!-- Reservante / Socio -->
      <div v-if="reserva.nombreReservante || reserva.nombreSocio" class="flex items-center text-sm text-slate-600">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-3 text-slate-400 shrink-0" fill="none"
          viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
        </svg>
        <span class="truncate">
          <span class="font-medium text-slate-500 mr-1">Titular:</span>
          <span class="font-semibold text-slate-800">
            {{ reserva.nombreReservante || `${reserva.nombreSocio} ${reserva.apellidoSocio}` }}
          </span>
        </span>
      </div>

      <!-- Salon -->
      <div v-if="reserva.nombreSalon" class="flex items-center text-sm text-slate-600">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-3 text-slate-400 shrink-0" fill="none"
          viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
        </svg>
        <span class="truncate">
          <span class="font-medium text-slate-500 mr-1">Lugar:</span>
          <span class="text-slate-800 font-medium">{{ reserva.nombreSalon }}</span>
        </span>
      </div>
    </div>

    <!-- Bloque de Datos Estilizados (Importe y Estado) -->
    <div v-if="reserva.importe !== undefined" class="grid grid-cols-2 gap-3 mb-6 mt-2">
      <div class="p-3 bg-slate-50 rounded-xl border border-slate-100 group-hover:bg-slate-100/50 transition-colors">
        <p class="text-[10px] uppercase tracking-wider font-bold text-slate-400 mb-0.5">Importe Total</p>
        <p class="text-sm font-bold text-slate-700">{{ formatCurrency(reserva.importe) }}</p>
      </div>
      <div class="p-3 bg-slate-50 rounded-xl border border-slate-100 group-hover:bg-slate-100/50 transition-colors">
        <p class="text-[10px] uppercase tracking-wider font-bold text-slate-400 mb-0.5">Estado Pago</p>
        <p class="text-sm font-bold" :class="reserva.pagado ? 'text-emerald-600' : 'text-amber-600'">
          {{ reserva.pagado ? 'Pagado' : 'Pendiente' }}
        </p>
      </div>
    </div>

    <!-- Botones de Acción -->
    <div class="flex gap-3 pt-5 border-t border-slate-100 mt-auto">
      <button @click="$emit('view', reserva.id || reserva.idReserva)"
        class="flex-1 inline-flex justify-center items-center px-4 py-2.5 text-sm font-bold text-indigo-700 bg-indigo-50 rounded-xl hover:bg-indigo-100 active:bg-indigo-200 transition-all duration-200">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
          stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
        </svg>
        Ver detalle
      </button>

      <button @click="$emit('delete', reserva)"
        class="inline-flex justify-center items-center p-2.5 text-sm font-medium text-slate-400 bg-slate-50 rounded-xl hover:bg-red-50 hover:text-red-600 active:bg-red-100 transition-all duration-200 group-hover:border-red-100 border border-transparent"
        title="Cancelar Reserva">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
        </svg>
      </button>
    </div>
  </div>
</template>
