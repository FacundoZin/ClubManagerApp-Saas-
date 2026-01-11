<script setup>
defineProps({
    show: {
        type: Boolean,
        default: false
    },
    message: {
        type: String,
        default: 'Procesando...'
    },
    isFullPage: {
        type: Boolean,
        default: true
    }
})
</script>

<template>
    <Transition enter-active-class="transition ease-out duration-300" enter-from-class="opacity-0"
        enter-to-class="opacity-100" leave-active-class="transition ease-in duration-200" leave-from-class="opacity-100"
        leave-to-class="opacity-0">
        <div v-if="show" :class="[
            isFullPage ? 'fixed inset-0 z-[100]' : 'absolute inset-0 z-10',
            'flex items-center justify-center bg-white/70 backdrop-blur-[2px]'
        ]">
            <div class="flex flex-col items-center bg-white p-6 rounded-2xl shadow-xl border border-slate-100">
                <div class="relative h-12 w-12">
                    <!-- Outer Ring -->
                    <div class="absolute inset-0 rounded-full border-4 border-slate-100"></div>
                    <!-- Spinning Ring -->
                    <div
                        class="absolute inset-0 rounded-full border-4 border-teal-600 border-t-transparent animate-spin">
                    </div>
                </div>
                <p v-if="message"
                    class="mt-4 text-sm font-bold text-slate-700 tracking-tight animate-pulse text-center">
                    {{ message }}
                </p>
            </div>
        </div>
    </Transition>
</template>

<style scoped>
.animate-spin {
    animation: spin 0.8s linear infinite;
}

@keyframes spin {
    from {
        transform: rotate(0deg);
    }

    to {
        transform: rotate(360deg);
    }
}
</style>
