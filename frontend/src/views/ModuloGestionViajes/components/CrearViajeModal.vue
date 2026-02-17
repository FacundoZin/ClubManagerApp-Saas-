<script setup>
import { ref, reactive, computed } from 'vue';

const props = defineProps({
    isOpen: Boolean,
    tipo: {
        type: String,
        default: 'base', // 'base' o 'variante'
    },
    viajeBaseId: {
        type: Number,
        default: null
    }
});

const emit = defineEmits(['close', 'save']);

const form = reactive({
    nombre: '',
    destino: '',
    descripcion: '',
    fecha: '',
    precio: 0,
    capacidad: 0
});

const isSubmitting = ref(false);

const resetForm = () => {
    form.nombre = '';
    form.destino = '';
    form.descripcion = '';
    form.fecha = '';
    form.precio = 0;
    form.capacidad = 0;
};

const handleClose = () => {
    resetForm();
    emit('close');
};

const handleSubmit = async () => {
    isSubmitting.value = true;
    try {
        const payload = { ...form };
        if (props.tipo === 'variante') {
            payload.viajeBaseId = props.viajeBaseId;
        }
        emit('save', payload);
        handleClose();
    } catch (error) {
        console.error('Error submitting form:', error);
    } finally {
        isSubmitting.value = false;
    }
};

const title = computed(() => props.tipo === 'base' ? 'Nuevo Viaje Base' : 'Nueva Variante de Viaje');
</script>

<template>
    <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog" aria-modal="true">
        <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
            <!-- Background overlay -->
            <div class="fixed inset-0 bg-slate-500 bg-opacity-75 transition-opacity" aria-hidden="true" @click="handleClose"></div>

            <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>

            <div class="inline-block align-bottom bg-white rounded-xl text-left overflow-hidden shadow-2xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full border border-slate-100">
                <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
                    <div class="sm:flex sm:items-start">
                        <div class="mx-auto flex-shrink-0 flex items-center justify-center h-12 w-12 rounded-full bg-teal-100 sm:mx-0 sm:h-10 sm:w-10">
                            <svg class="h-6 w-6 text-teal-600" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v3m0 0v3m0-3h3m-3 0H9m12 0a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>
                        <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left w-full">
                            <h3 class="text-xl leading-6 font-bold text-slate-900" id="modal-title">
                                {{ title }}
                            </h3>
                            <div class="mt-4 space-y-4">
                                <div>
                                    <label class="block text-sm font-semibold text-slate-700 mb-1">Nombre</label>
                                    <input v-model="form.nombre" type="text" class="w-full px-4 py-2 rounded-lg border border-slate-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent outline-none transition-all" placeholder="Ej: Viaje a Cataratas Verano">
                                </div>
                                <div v-if="tipo === 'base'">
                                    <label class="block text-sm font-semibold text-slate-700 mb-1">Destino Principal</label>
                                    <input v-model="form.destino" type="text" class="w-full px-4 py-2 rounded-lg border border-slate-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent outline-none transition-all" placeholder="Ej: Misiones, Argentina">
                                </div>
                                <div>
                                    <label class="block text-sm font-semibold text-slate-700 mb-1">Descripción</label>
                                    <textarea v-model="form.descripcion" rows="3" class="w-full px-4 py-2 rounded-lg border border-slate-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent outline-none transition-all" placeholder="Detalles del viaje..."></textarea>
                                </div>
                                <div class="grid grid-cols-2 gap-4">
                                    <div>
                                        <label class="block text-sm font-semibold text-slate-700 mb-1">Fecha</label>
                                        <input v-model="form.fecha" type="date" class="w-full px-4 py-2 rounded-lg border border-slate-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent outline-none transition-all">
                                    </div>
                                    <div>
                                        <label class="block text-sm font-semibold text-slate-700 mb-1">Precio</label>
                                        <input v-model="form.precio" type="number" class="w-full px-4 py-2 rounded-lg border border-slate-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent outline-none transition-all">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="bg-slate-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse gap-2">
                    <button @click="handleSubmit" :disabled="isSubmitting" type="button" class="w-full inline-flex justify-center rounded-lg border border-transparent shadow-sm px-4 py-2 bg-teal-600 text-base font-medium text-white hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 sm:w-auto sm:text-sm transition-all disabled:opacity-50">
                        {{ isSubmitting ? 'Guardando...' : 'Guardar Viaje' }}
                    </button>
                    <button @click="handleClose" type="button" class="mt-3 w-full inline-flex justify-center rounded-lg border border-slate-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-slate-700 hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-teal-500 sm:mt-0 sm:w-auto sm:text-sm transition-all">
                        Cancelar
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
