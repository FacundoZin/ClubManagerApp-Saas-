<template>
  <div class="flex flex-col items-center justify-center min-h-[500px]">
    <div class="relative">
      <!-- Spinner animado -->
      <div
        class="w-20 h-20 border-4 border-blue-200 border-t-blue-600 rounded-full animate-spin"
      ></div>
      <div
        class="absolute inset-0 w-20 h-20 border-4 border-transparent border-r-blue-400 rounded-full animate-spin"
        style="animation-duration: 1.5s; animation-direction: reverse"
      ></div>
    </div>
    <div class="mt-6 text-center">
      <h3 class="text-lg font-semibold text-slate-700">Preparando tu pago...</h3>
      <p class="text-sm text-slate-500 mt-2">Estamos configurando todo de forma segura</p>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PaymentLoading',
}
</script>
