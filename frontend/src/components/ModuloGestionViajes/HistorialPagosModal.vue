<script setup>
defineProps({
  isOpen: Boolean,
  inscripto: {
    type: Object,
    default: null,
  },
})

defineEmits(['close'])

const formatCurrency = (value) => {
  if (value === undefined || value === null) return '$ 0,00'
  return new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
  }).format(value)
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  return new Date(dateString + 'T00:00:00').toLocaleDateString('es-AR')
}
</script>

<template>
  <div v-if="isOpen" class="fixed inset-0 z-[70] overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
    <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity" @click="$emit('close')"></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-md border border-slate-200">
        <!-- Header -->
        <div class="bg-slate-50 px-6 py-4 border-b border-slate-200 flex items-center justify-between">
          <div>
            <h3 class="text-base font-bold text-slate-900 leading-tight">Historial de Pagos</h3>
            <p class="text-xs text-slate-500 mt-0.5">{{ inscripto?.apellido }} {{ inscripto?.nombre }} (File #{{ inscripto?.numeroFile }})</p>
          </div>
          <button @click="$emit('close')" class="text-slate-400 hover:text-slate-600 rounded-lg p-1 hover:bg-slate-100 transition-colors">
            <svg class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Body -->
        <div class="p-6">
          <div v-if="!inscripto?.historialPagos || inscripto.historialPagos.length === 0" class="text-center py-6 text-slate-500 text-sm italic">
            No hay pagos registrados para este inscripto.
          </div>
          <div v-else class="overflow-hidden border border-slate-150 rounded-xl">
            <table class="min-w-full divide-y divide-slate-100 text-xs">
              <thead class="bg-slate-50">
                <tr>
                  <th class="px-4 py-2.5 text-left font-bold text-slate-500 uppercase tracking-wider">Fecha</th>
                  <th class="px-4 py-2.5 text-left font-bold text-slate-500 uppercase tracking-wider">Recibo</th>
                  <th class="px-4 py-2.5 text-right font-bold text-slate-500 uppercase tracking-wider">Monto</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100 bg-white">
                <tr v-for="pago in inscripto.historialPagos" :key="pago.id" class="hover:bg-slate-50/80 transition-colors">
                  <td class="px-4 py-2.5 text-slate-600 font-medium">{{ formatDate(pago.fechaPago) }}</td>
                  <td class="px-4 py-2.5 text-slate-600 font-semibold">
                    <span class="px-2 py-0.5 rounded text-[10px] inline-block max-w-[120px] truncate" :class="pago.numeroRecibo.startsWith('CANCELACION') ? 'bg-red-50 text-red-600 border border-red-100' : 'bg-slate-100 text-slate-700'" :title="pago.numeroRecibo">
                      {{ pago.numeroRecibo }}
                    </span>
                  </td>
                  <td class="px-4 py-2.5 text-right font-bold text-emerald-600">{{ formatCurrency(pago.monto) }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Resumen de saldos -->
          <div class="mt-4 pt-4 border-t border-slate-100 grid grid-cols-2 gap-4 text-xs font-semibold">
            <div class="bg-emerald-50 text-emerald-700 p-2.5 rounded-xl border border-emerald-100/50 flex flex-col items-center">
              <span class="text-[9px] uppercase font-bold text-emerald-600/80 tracking-wider mb-0.5">Total Abonado</span>
              <span class="text-sm font-extrabold">{{ formatCurrency(inscripto?.montoAbonado) }}</span>
            </div>
            <div class="bg-rose-50 text-rose-700 p-2.5 rounded-xl border border-rose-100/50 flex flex-col items-center">
              <span class="text-[9px] uppercase font-bold text-rose-600/80 tracking-wider mb-0.5">Pendiente</span>
              <span class="text-sm font-extrabold">{{ formatCurrency(inscripto?.montoPendiente) }}</span>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="bg-slate-50/85 px-6 py-3 flex justify-end border-t border-slate-100">
          <button @click="$emit('close')" class="px-4 py-2 bg-white text-slate-700 font-bold rounded-xl border border-slate-200 hover:bg-slate-50 transition-all text-xs active:scale-95 shadow-sm">
            Cerrar
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
