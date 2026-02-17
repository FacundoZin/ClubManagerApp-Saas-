<script setup>
import { ref } from 'vue';

const dni = ref('');
const isSearching = ref(false);

const emit = defineEmits(['buscar']);

const handleSearch = () => {
    if (!dni.value) return;
    emit('buscar', dni.value);
};
</script>

<template>
    <div class="bg-white rounded-2xl shadow-sm border border-slate-200 p-6 sm:p-8">
        <div class="flex items-center gap-3 mb-6">
            <div class="w-10 h-10 rounded-xl bg-teal-50 text-teal-600 flex items-center justify-center">
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                </svg>
            </div>
            <div>
                <h3 class="text-xl font-black text-slate-900 tracking-tight">Nueva Inscripción</h3>
                <p class="text-sm text-slate-500 font-medium leading-none mt-1">Busque al socio para inscribirlo en un viaje.</p>
            </div>
        </div>

        <div class="flex flex-col sm:flex-row gap-4">
            <div class="flex-grow">
                <div class="relative">
                    <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                        <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
                        </svg>
                    </div>
                    <input 
                        v-model="dni" 
                        type="text" 
                        @keyup.enter="handleSearch"
                        class="w-full pl-11 pr-4 py-3.5 rounded-xl border border-slate-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent outline-none transition-all font-bold text-slate-700"
                        placeholder="Ingrese DNI del socio..."
                    >
                </div>
            </div>
            <button 
                @click="handleSearch"
                :disabled="!dni || isSearching"
                class="px-8 py-3.5 bg-slate-900 text-white rounded-xl font-black text-sm uppercase tracking-widest hover:bg-slate-800 transition-all active:scale-95 disabled:opacity-50"
            >
                {{ isSearching ? 'Buscando...' : 'Buscar Socio' }}
            </button>
        </div>
    </div>
</template>
