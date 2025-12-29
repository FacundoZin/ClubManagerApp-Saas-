<template>
  <footer class="border-t border-slate-200 bg-white/80 backdrop-blur-sm mt-12 py-6">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
      <p class="text-slate-500 text-sm font-medium">
        © {{ new Date().getFullYear() }} Club de Abuelos - Portal de pagos online
      </p>
      <p class="text-slate-400 text-xs mt-1">Procesado de forma segura por Mercado Pago</p>
    </div>
  </footer>
</template>

<script>
export default {
  name: 'PaymentFooter',
}
</script>
