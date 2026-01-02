<script setup>
import { reactive, ref, watch } from 'vue'

const props = defineProps({
    isOpen: Boolean,
    socioId: {
        type: Number,
        default: null
    }
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
    nombre: '',
    apellido: '',
    dni: '',
    telefono: '',
    direcccion: '', // Note: keeping typo to match DTO
    lote: '',
    localidad: ''
})

const isSubmitting = ref(false)
const isLoading = ref(false)
const errorMessage = ref('')

const resetForm = () => {
    form.nombre = ''
    form.apellido = ''
    form.dni = ''
    form.telefono = ''
    form.direcccion = ''
    form.lote = ''
    form.localidad = ''
    errorMessage.value = ''
}

const fetchSocioData = async (id) => {
    if (!id) return

    isLoading.value = true
    errorMessage.value = ''

    try {
        const response = await fetch(`http://localhost:5194/api/Socios/byId/${id}`)
        if (!response.ok) {
            throw new Error('No se pudo cargar la información del socio')
        }
        const data = await response.json()

        // Populate form
        form.nombre = data.nombre || ''
        form.apellido = data.apellido || ''
        form.dni = data.dni || ''
        form.telefono = data.telefono || ''
        form.direcccion = data.direcccion || ''
        form.lote = data.lote || ''
        form.localidad = data.localidad || ''

    } catch (error) {
        errorMessage.value = error.message
    } finally {
        isLoading.value = false
    }
}

// Watch for modal opening with a socioId
watch(() => props.isOpen, (newVal) => {
    if (newVal && props.socioId) {
        fetchSocioData(props.socioId)
    } else if (!newVal) {
        resetForm()
    }
})

const handleSubmit = async () => {
    isSubmitting.value = true
    errorMessage.value = ''

    try {
        const response = await fetch(`http://localhost:5194/api/Socios/${props.socioId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(form)
        })

        if (!response.ok) {
            let message = 'Error al actualizar el socio'

            // Si es un error del cliente (400-499), intentamos obtener el mensaje del backend
            if (response.status >= 400 && response.status < 500) {
                try {
                    const backendError = await response.text()
                    if (backendError) message = backendError
                } catch (e) {
                    // Fallback to default message
                }
            }

            throw new Error(message)
        }

        const data = await response.json()
        // The PUT endpoint in SocioController returns Ok(result.Data)
        // which in SociosManagmentService is: { Message = "...", SocioId = ... }
        // But SociosView might expect the full socio object to update the list.
        // Let's emit the updated form data as if it was the socio.
        emit('save', { ...form, id: props.socioId })
        resetForm()
    } catch (error) {
        errorMessage.value = error.message
    } finally {
        isSubmitting.value = false
    }
}
</script>

<template>
    <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto" aria-labelledby="modal-title" role="dialog"
        aria-modal="true">
        <!-- Background backdrop with blur -->
        <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity" @click="$emit('close')"></div>

        <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
            <div
                class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200">

                <!-- Loading Overlay -->
                <div v-if="isLoading"
                    class="absolute inset-0 z-10 bg-white/60 backdrop-blur-[2px] flex items-center justify-center">
                    <div class="flex flex-col items-center">
                        <svg class="animate-spin h-10 w-10 text-blue-600 mb-2" xmlns="http://www.w3.org/2000/svg"
                            fill="none" viewBox="0 0 24 24">
                            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4">
                            </circle>
                            <path class="opacity-75" fill="currentColor"
                                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                            </path>
                        </svg>
                        <p class="text-sm font-medium text-slate-600">Cargando datos...</p>
                    </div>
                </div>

                <!-- Header -->
                <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4 border-b border-slate-100">
                    <div class="sm:flex sm:items-start">
                        <div
                            class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-amber-100 sm:mx-0 sm:h-12 sm:w-12">
                            <svg class="h-6 w-6 text-amber-600" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                                stroke="currentColor" aria-hidden="true">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                    d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L10.582 16.07a4.5 4.5 0 01-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 011.13-1.897l8.932-8.931zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0115.75 21H5.25A2.25 2.25 0 013 18.75V8.25A2.25 2.25 0 015.25 6H10" />
                            </svg>
                        </div>
                        <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
                            <h3 class="text-xl font-bold leading-6 text-slate-900" id="modal-title">Actualizar Socio
                            </h3>
                            <div class="mt-2 text-slate-500">
                                <p class="text-sm font-medium">Modifique la información del socio seleccionado.</p>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Form -->
                <form @submit.prevent="handleSubmit">
                    <div class="px-4 py-5 sm:p-8 space-y-6">

                        <div v-if="errorMessage"
                            class="p-4 rounded-xl bg-red-50 border border-red-100 text-red-700 text-sm flex items-start gap-3">
                            <svg class="h-5 w-5 text-red-400 mt-0.5" fill="none" viewBox="0 0 24 24"
                                stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            {{ errorMessage }}
                        </div>

                        <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
                            <div class="space-y-1.5">
                                <label for="edit-nombre" class="block text-sm font-bold text-slate-700">Nombre</label>
                                <input type="text" id="edit-nombre" v-model="form.nombre" required
                                    class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-sm px-4 py-3 border transition-all"
                                    placeholder="Ingrese nombre">
                            </div>
                            <div class="space-y-1.5">
                                <label for="edit-apellido"
                                    class="block text-sm font-bold text-slate-700">Apellido</label>
                                <input type="text" id="edit-apellido" v-model="form.apellido" required
                                    class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-sm px-4 py-3 border transition-all"
                                    placeholder="Ingrese apellido">
                            </div>
                        </div>

                        <div class="space-y-1.5">
                            <label for="edit-dni" class="block text-sm font-bold text-slate-700">DNI</label>
                            <input type="text" id="edit-dni" v-model="form.dni" required
                                class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-sm px-4 py-3 border transition-all"
                                placeholder="Ej: 12.345.678">
                        </div>

                        <div class="space-y-1.5">
                            <label for="edit-telefono" class="block text-sm font-bold text-slate-700">Teléfono</label>
                            <input type="tel" id="edit-telefono" v-model="form.telefono"
                                class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-sm px-4 py-3 border transition-all"
                                placeholder="Ej: +54 9 11 1234 5678">
                        </div>

                        <div class="space-y-1.5">
                            <label for="edit-direccion" class="block text-sm font-bold text-slate-700">Dirección</label>
                            <input type="text" id="edit-direccion" v-model="form.direcccion"
                                class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-sm px-4 py-3 border transition-all"
                                placeholder="Calle y número">
                        </div>

                        <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
                            <div class="space-y-1.5">
                                <label for="edit-lote" class="block text-sm font-bold text-slate-700">Lote</label>
                                <input type="text" id="edit-lote" v-model="form.lote"
                                    class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-sm px-4 py-3 border transition-all"
                                    placeholder="Número de lote">
                            </div>
                            <div class="space-y-1.5">
                                <label for="edit-localidad"
                                    class="block text-sm font-bold text-slate-700">Localidad</label>
                                <input type="text" id="edit-localidad" v-model="form.localidad"
                                    class="block w-full rounded-xl border-slate-200 bg-slate-50/50 shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 focus:ring-opacity-50 sm:text-sm px-4 py-3 border transition-all"
                                    placeholder="Ciudad o Barrio">
                            </div>
                        </div>

                    </div>

                    <!-- Footer Actions -->
                    <div
                        class="bg-slate-50/80 px-4 py-4 sm:flex sm:flex-row-reverse sm:px-8 border-t border-slate-100 backdrop-blur-sm">
                        <button type="submit" :disabled="isSubmitting || isLoading"
                            class="inline-flex w-full justify-center items-center gap-2 rounded-xl bg-blue-600 px-6 py-3 text-sm font-bold text-white shadow-lg shadow-blue-200 hover:bg-blue-700 hover:shadow-blue-300 sm:ml-3 sm:w-auto disabled:opacity-50 disabled:cursor-not-allowed transition-all active:scale-95">
                            <svg v-if="isSubmitting" class="animate-spin h-4 w-4 text-white" fill="none"
                                viewBox="0 0 24 24">
                                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor"
                                    stroke-width="4"></circle>
                                <path class="opacity-75" fill="currentColor"
                                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                                </path>
                            </svg>
                            {{ isSubmitting ? 'Guardando...' : 'Actualizar Información' }}
                        </button>
                        <button type="button" @click="$emit('close')"
                            class="mt-3 inline-flex w-full justify-center rounded-xl bg-white px-6 py-3 text-sm font-bold text-slate-700 shadow-sm ring-1 ring-inset ring-slate-200 hover:bg-slate-50 sm:mt-0 sm:w-auto transition-all active:scale-95">
                            Cancelar
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>
