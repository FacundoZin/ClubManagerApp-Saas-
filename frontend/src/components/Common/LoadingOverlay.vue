<script setup>
defineProps({
  show: {
    type: Boolean,
    default: false,
  },
})
</script>

<template>
  <Transition
    enter-active-class="transition-all duration-200"
    enter-from-class="opacity-0 scale-95"
    enter-to-class="opacity-100 scale-100"
    leave-active-class="transition-all duration-150"
    leave-from-class="opacity-100 scale-100"
    leave-to-class="opacity-0 scale-95"
  >
    <div
      v-if="show"
      class="fixed inset-0 z-[100] flex items-center justify-center bg-slate-900/10 backdrop-blur-[1px]"
    >
      <div class="relative h-10 w-10">
        <!-- Inner Ring -->
        <div class="absolute inset-0 rounded-full border-2 border-slate-200/50"></div>
        <!-- Spinning Ring -->
        <div
          class="absolute inset-0 rounded-full border-2 border-blue-600 border-t-transparent animate-spin"
        ></div>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.animate-spin {
  animation: spin 0.6s linear infinite;
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
