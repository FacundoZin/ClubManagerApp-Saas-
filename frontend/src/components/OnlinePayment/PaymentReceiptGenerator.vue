<template>
    <!-- Contenedor oculto para el renderizado del comprobante -->
    <div class="fixed -left-[9999px] top-0">
        <div ref="receiptRef"
            class="w-[600px] bg-white p-8 border-[12px] border-slate-100 shadow-2xl font-sans text-slate-800">
            <!-- Header del Club -->
            <div class="flex items-center justify-between border-b-2 border-blue-600 pb-6 mb-8">
                <div class="flex items-center gap-4">
                    <div class="w-16 h-16 bg-blue-600 rounded-full flex items-center justify-center text-white">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-7h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                        </svg>
                    </div>
                    <div>
                        <h1 class="text-2xl font-black text-blue-900 uppercase tracking-tight">Club de los Abuelos</h1>
                        <p class="text-sm font-bold text-blue-600 uppercase tracking-widest">Comprobante de Pago</p>
                    </div>
                </div>
                <div class="text-right">
                    <p class="text-xs font-bold text-slate-400 uppercase">Fecha de Emisión</p>
                    <p class="text-sm font-black text-slate-700">{{ currentDate }}</p>
                </div>
            </div>

            <!-- Cuerpo del Comprobante -->
            <div class="space-y-6">
                <div class="grid grid-cols-2 gap-8">
                    <div class="space-y-1">
                        <p class="text-[10px] font-bold text-slate-400 uppercase tracking-wider">Socio</p>
                        <p class="text-lg font-black text-slate-900">{{ receiptData?.nombreSocio }}</p>
                    </div>
                    <div class="space-y-1">
                        <p class="text-[10px] font-bold text-slate-400 uppercase tracking-wider">DNI</p>
                        <p class="text-lg font-black text-slate-900">{{ receiptData?.dniSocio }}</p>
                    </div>
                </div>

                <div class="bg-slate-50 rounded-2xl p-6 border border-slate-100">
                    <div class="flex justify-between items-center mb-4">
                        <h2 class="text-sm font-bold text-slate-500 uppercase tracking-widest">Concepto Detallado</h2>
                        <span
                            class="px-3 py-1 bg-blue-100 text-blue-700 text-[10px] font-black rounded-full uppercase">Pago
                            Online</span>
                    </div>
                    <div class="flex justify-between items-end">
                        <div>
                            <p class="text-xl font-black text-slate-900">Cuota Social</p>
                            <p class="text-sm font-bold text-blue-600">{{ receiptData?.semestrePagoText }} - {{
                                receiptData?.anioPago }}</p>
                        </div>
                        <div class="text-right">
                            <p class="text-3xl font-black text-blue-600">${{ receiptData?.monto }}</p>
                        </div>
                    </div>
                </div>

                <!-- Footer / Seguridad -->
                <div class="pt-6 border-t border-dashed border-slate-200">
                    <div class="flex items-center gap-3 mb-4">
                        <div class="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center text-green-600">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20"
                                fill="currentColor">
                                <path fill-rule="evenodd"
                                    d="M2.166 4.999A11.954 11.954 0 0010 1.944 11.954 11.954 0 0017.834 5c.11.65.166 1.32.166 2.001 0 5.225-3.34 9.67-8 11.317C5.34 16.67 2 12.225 2 7c0-.682.057-1.35.166-2.001zm11.541 3.708a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                                    clip-rule="evenodd" />
                            </svg>
                        </div>
                        <p class="text-[10px] font-bold text-slate-500 leading-tight">
                            Este comprobante es válido como constancia de pago oficial.<br>
                            Transacción procesada de forma segura por el sistema del Club.
                        </p>
                    </div>

                    <div class="flex justify-between items-center bg-slate-900 text-white p-4 rounded-xl">
                        <div>
                            <p class="text-[8px] font-bold text-slate-400 uppercase">Código de Verificación</p>
                            <p class="text-xs font-mono font-bold tracking-tighter">{{ verificationCode }}</p>
                        </div>
                        <div class="text-right">
                            <p class="text-[14px] font-black italic opacity-50">SISTEMA CLUB ABUELOS</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
import { toJpeg } from 'html-to-image'
import { computed, ref } from 'vue'

const props = defineProps({
    receiptData: {
        type: Object,
        required: true
    }
})

const receiptRef = ref(null)

const currentDate = computed(() => {
    return new Date().toLocaleDateString('es-AR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    })
})

const verificationCode = computed(() => {
    // Generar un código determinístico basado en los datos para que parezca real
    const base = `${props.receiptData?.dniSocio}-${props.receiptData?.anioPago}-${props.receiptData?.monto}`
    return btoa(base).substring(0, 16).toUpperCase()
})

const downloadReceipt = async () => {
    if (!receiptRef.value) return

    try {
        const dataUrl = await toJpeg(receiptRef.value, {
            quality: 0.95,
            backgroundColor: '#ffffff',
            pixelRatio: 2 // Mayor calidad
        })

        const link = document.createElement('a')
        link.download = `comprobante-pago-${props.receiptData?.dniSocio}-${props.receiptData?.anioPago}.jpg`
        link.href = dataUrl
        link.click()
    } catch (error) {
        console.error('Error generating receipt image:', error)
    }
}

// Exponer el método para que el padre pueda llamarlo
defineExpose({
    downloadReceipt
})
</script>

<style scoped>
/* Estilos específicos para asegurar que el renderizado sea perfecto */
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;700;900&display=swap');

.font-sans {
    font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
}
</style>
