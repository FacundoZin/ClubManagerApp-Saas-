<script setup>
defineProps({
  alquiler: {
    type: Object,
    required: true
  }
})

defineEmits(['view-detail'])
</script>

<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm p-6 hover:shadow-md transition-shadow relative">
    <div class="flex justify-between items-start mb-4">
       <div>
           <div class="flex items-center gap-2 mb-2">
               <span class="px-2.5 py-0.5 rounded-full text-xs font-medium bg-indigo-100 text-indigo-800">
                   ID: {{ alquiler.id }}
               </span>
               <span class="text-sm text-slate-500">{{ alquiler.fechaAlquiler }}</span>
           </div>
           <h3 class="text-lg font-bold text-slate-900">{{ alquiler.nombreSocio }} {{ alquiler.apellidoSocio }}</h3>
           <p class="text-sm text-slate-500">DNI: {{ alquiler.dniSocio }}</p>
       </div>
       <div v-if="alquiler.estaAlDia !== undefined">
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
              :class="alquiler.estaAlDia ? 'bg-emerald-100 text-emerald-800' : 'bg-rose-100 text-rose-800'">
              {{ alquiler.estaAlDia ? 'Al día' : 'Deuda' }}
            </span>
       </div>
    </div>
    
    <div class="space-y-2 mb-6">
        <div v-if="alquiler.direccionSocio" class="flex items-start text-sm text-slate-600">
             <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2 mt-0.5 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
               <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
               <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
             </svg>
             {{ alquiler.direccionSocio }} {{ alquiler.localidadSocio ? `, ${alquiler.localidadSocio}` : '' }}
        </div>
         <div v-if="alquiler.telefonoSocio" class="flex items-center text-sm text-slate-600">
             <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
               <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
             </svg>
             {{ alquiler.telefonoSocio }}
         </div>
    </div>

    <div class="pt-4 border-t border-slate-100">
      <button @click="$emit('view-detail', alquiler.id)" 
        class="w-full inline-flex justify-center items-center px-4 py-2 text-sm font-medium text-indigo-600 bg-indigo-50 rounded-lg hover:bg-indigo-100 transition-colors">
        Ver Alquiler Completo
        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 ml-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14 5l7 7m0 0l-7 7m7-7H3" />
        </svg>
      </button>
    </div>
  </div>
</template>
