<script setup>
defineProps({
    isOpen: Boolean,
    title: {
        type: String,
        default: 'Confirmar Acción'
    },
    message: {
        type: String,
        default: '¿Está seguro que desea realizar esta acción?'
    },
    confirmText: {
        type: String,
        default: 'Confirmar'
    },
    cancelText: {
        type: String,
        default: 'Cancelar'
    },
    type: {
        type: String,
        default: 'danger' // 'danger', 'info', 'warning'
    }
})

defineEmits(['close', 'confirm'])
</script>

<template>
    <div v-if="isOpen" class="fixed inset-0 z-[60] overflow-y-auto" aria-labelledby="modal-title" role="dialog"
        aria-modal="true">
        <!-- Overlay -->
        <div class="fixed inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity" @click="$emit('close')"></div>

        <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
            <div
                class="relative transform overflow-hidden rounded-2xl bg-white text-left shadow-2xl transition-all sm:my-8 sm:w-full sm:max-w-md border border-slate-200">

                <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
                    <div class="sm:flex sm:items-start">
                        <!-- Icon based on type -->
                        <div class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full sm:mx-0 sm:h-10 sm:w-10"
                            :class="{
                                'bg-red-100 text-red-600': type === 'danger',
                                'bg-blue-100 text-blue-600': type === 'info',
                                'bg-amber-100 text-amber-600': type === 'warning'
                            }">
                            <svg v-if="type === 'danger'" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                                stroke-width="1.5" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                    d="M12 9v3.75m-9.303 3.376c-.866 1.5.217 3.374 1.948 3.374h14.71c1.73 0 2.813-1.874 1.948-3.374L13.949 3.378c-.866-1.5-3.032-1.5-3.898 0L2.697 16.126zM12 15.75h.007v.008H12v-.008z" />
                            </svg>
                            <svg v-else-if="type === 'info'" class="h-6 w-6" fill="none" viewBox="0 0 24 24"
                                stroke-width="1.5" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                    d="M11.25 11.25l.041-.02a.75.75 0 011.063.852l-.708 2.836a.75.75 0 001.063.853l.041-.021M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9-3.75h.008v.008H12V8.25z" />
                            </svg>
                            <svg v-else class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke-width="1.5"
                                stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round"
                                    d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z" />
                            </svg>
                        </div>

                        <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left">
                            <h3 class="text-lg font-bold leading-6 text-slate-900" id="modal-title">{{ title }}</h3>
                            <div class="mt-2">
                                <p class="text-sm text-slate-500">{{ message }}</p>
                            </div>
                        </div>
                    </div>
                </div>

                <div
                    class="bg-slate-50/80 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 border-t border-slate-100 backdrop-blur-sm">
                    <button type="button" @click="$emit('confirm')"
                        class="inline-flex w-full justify-center rounded-xl px-4 py-2.5 text-sm font-bold text-white shadow-sm sm:ml-3 sm:w-auto transition-all active:scale-95"
                        :class="{
                            'bg-red-600 hover:bg-red-700 shadow-red-200': type === 'danger',
                            'bg-blue-600 hover:bg-blue-700 shadow-blue-200': type === 'info',
                            'bg-amber-600 hover:bg-amber-700 shadow-amber-200': type === 'warning'
                        }">
                        {{ confirmText }}
                    </button>
                    <button type="button" @click="$emit('close')"
                        class="mt-3 inline-flex w-full justify-center rounded-xl bg-white px-4 py-2.5 text-sm font-bold text-slate-700 shadow-sm ring-1 ring-inset ring-slate-200 hover:bg-slate-50 sm:mt-0 sm:w-auto transition-all active:scale-95">
                        {{ cancelText }}
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
