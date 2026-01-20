<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-slate-100 font-sans">
    <PaymentHeader />

    <!-- Main Content -->
    <main class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 md:py-12">
      <PaymentLoading v-if="loading" />

      <PaymentValidationError
        v-else-if="validationError"
        :validation-error="validationError"
        @go-back="goBack"
      />

      <PaymentErrorState
        v-else-if="errorMessage"
        :error-message="errorMessage"
        @retry="retryPayment"
        @go-back="goBack"
      />

      <PaymentStatusScreen
        v-else-if="paymentStatus"
        :status="paymentStatus"
        :error-message="statusMessage"
        @retry="retryPayment"
        @download="downloadReceipt"
      />

      <!-- Payment Content -->
      <div v-else class="space-y-6">
        <PaymentInfoCard :payment-data="paymentData" />

        <!-- Payment Brick Container -->
        <div
          class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl border border-slate-200 overflow-hidden"
        >
          <div class="p-6 border-b border-slate-200 bg-gradient-to-r from-slate-50 to-white">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-6 w-6 text-blue-600"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z"
                  />
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">Información de pago</h3>
                <p class="text-sm text-slate-500">Complete los datos para procesar su pago</p>
              </div>
            </div>
          </div>

          <!-- Payment Brick se renderiza aquí -->
          <div class="p-6">
            <!-- Error local del Brick (sin ocultarlo) -->
            <div
              v-if="brickError"
              class="mb-6 p-4 bg-red-50 border border-red-200 rounded-xl flex items-start gap-3 animate-fadeIn"
            >
              <div
                class="w-8 h-8 bg-red-100 rounded-full flex items-center justify-center flex-shrink-0"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5 text-red-600"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
              </div>
              <div class="flex-1">
                <p class="text-sm font-semibold text-red-800">Error al procesar el pago</p>
                <p class="text-xs text-red-600 mt-1">{{ brickError }}</p>
              </div>
              <button
                @click="brickError = null"
                class="text-red-400 hover:text-red-600 transition-colors"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-4 w-4"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            </div>

            <div id="paymentBrick_container"></div>
          </div>
        </div>

        <PaymentSecurityInfo />
      </div>

      <!-- Componente oculto para exportar a PDF -->
      <div
        v-if="comprobanteData"
        style="position: absolute; left: -9999px; top: 0; background: white; color: black"
      >
        <div
          id="comprobante-pdf"
          style="background: white; color: black; font-family: Arial, sans-serif"
        >
          <ComprobanteCuota :data="comprobanteData" />
        </div>
      </div>
    </main>

    <PaymentFooter />
  </div>
</template>

<script>
import PaymentErrorState from '@/components/OnlinePayment/PaymentErrorState.vue'
import PaymentFooter from '@/components/OnlinePayment/PaymentFooter.vue'
import PaymentHeader from '@/components/OnlinePayment/PaymentHeader.vue'
import PaymentInfoCard from '@/components/OnlinePayment/PaymentInfoCard.vue'
import PaymentLoading from '@/components/OnlinePayment/PaymentLoading.vue'
import PaymentSecurityInfo from '@/components/OnlinePayment/PaymentSecurityInfo.vue'
import PaymentStatusScreen from '@/components/OnlinePayment/PaymentStatusScreen.vue'
import PaymentValidationError from '@/components/OnlinePayment/PaymentValidationError.vue'
import PaymentService from '@/services/PaymentService'
import ComprobanteCuota from '@/components/payments/comprobanteCuota.vue'
import { toPng } from 'html-to-image'
import { jsPDF } from 'jspdf'
import { nextTick } from 'vue'

export default {
  components: {
    PaymentHeader,
    PaymentFooter,
    PaymentLoading,
    PaymentValidationError,
    PaymentErrorState,
    PaymentInfoCard,
    PaymentSecurityInfo,
    PaymentStatusScreen,
    ComprobanteCuota,
  },
  data() {
    return {
      loading: true,
      paymentData: null,
      errorMessage: null,
      brickError: null,
      brickController: null,
      validationError: null,
      paymentStatus: null, // 'approved', 'rejected', 'in_process'
      statusMessage: null,
      pollingInterval: null,
      paymentToken: null,
      comprobanteData: null,
    }
  },

  async mounted() {
    await this.initializePayment()
  },

  beforeUnmount() {
    if (this.brickController) {
      this.brickController.unmount()
    }
    this.stopPolling()
  },

  methods: {
    async initializePayment() {
      try {
        // 1. Desmontar brick previo si existe (importante para reintentos)
        if (this.brickController) {
          try {
            this.brickController.unmount()
          } catch (e) {
            console.error('Error al desmontar el brick:', e)
          }
          this.brickController = null
        }

        this.loading = true
        this.errorMessage = null
        this.validationError = null

        // 🔹 Token desde URL
        const params = new URLSearchParams(window.location.search)
        this.paymentToken = params.get('token')

        if (!this.paymentToken) {
          this.errorMessage = 'Token de pago inválido o inexistente'
          this.loading = false
          return
        }

        // 🔹 Init payment (backend)
        let result
        try {
          result = await PaymentService.initPayment(this.paymentToken)
        } catch (error) {
          const backendMsg = (error.message || '').toLowerCase()

          // 2. Caso: Plazo expirado (492)
          if (error.status === 492) {
            this.validationError = {
              type: 'expired',
              title: 'Enlace expirado',
              message: 'El plazo para realizar este pago ha vencido.',
              subtext: 'Por favor, solicita un nuevo enlace de pago al Club.',
            }
            this.loading = false
            return
          }

          // 3. Caso: Token no existe o inválido (404 / 400)
          if (error.status === 404 || error.status === 400 || backendMsg.includes('no existe')) {
            this.validationError = {
              type: 'invalid',
              title: 'Enlace no válido',
              message: 'El enlace de pago es incorrecto, ha sido modificado o ya no existe.',
              subtext:
                'Verifica que el link sea el correcto y que no lo modificaste manualmente por accidente.',
            }
            this.loading = false
            return
          }

          // 4. Otros errores (Sistema)
          this.errorMessage = error.message
          this.loading = false
          return
        }

        if (!result.exit) {
          this.errorMessage = result.errormessage || 'No se pudo iniciar el pago'
          this.loading = false
          return
        }

        // 🔹 Datos válidos
        this.paymentData = result.data

        // 🔹 Si ya está pagado (backend devuelve alreadyPaid: true y el comprobante)
        if (this.paymentData.alreadyPaid && this.paymentData.comprobante) {
          this.comprobanteData = this.paymentData.comprobante
          this.paymentStatus = 'approved'
          this.loading = false
          return
        }

        // 🔹 Asegurar que el contenedor esté en el DOM
        this.loading = false
        await nextTick()

        // 🔹 SDK MP
        const mp = new window.MercadoPago(import.meta.env.VITE_MERCADOPAGO_PUBLIC_KEY, {
          locale: 'es-AR',
        })

        const bricksBuilder = mp.bricks()

        // 🔹 Render Payment Brick
        this.brickController = await bricksBuilder.create('payment', 'paymentBrick_container', {
          initialization: {
            amount: this.paymentData.monto,
            // 👉 SOLO para wallet
            preferenceId: this.paymentData.preference_Id,
            description: `Cuota ${this.paymentData.semestrePago} - ${this.paymentData.anioPago}`,
          },
          customization: {
            paymentMethods: {
              debitCard: 'all',
              mercadoPago: 'all',
            },
          },
          callbacks: {
            onReady: () => {
              console.log('Payment Brick listo')
            },

            onSubmit: ({ selectedPaymentMethod, formData }) => {
              this.brickError = null

              // 👉 SOLO Tarjeta de Débito va al backend
              if (selectedPaymentMethod === 'debit_card') {
                return PaymentService.processPayment({
                  ...formData,
                  externalReference: this.paymentData.externalReference,
                })
                  .then((result) => {
                    if (!result.exit) {
                      this.brickError =
                        result.errormessage || 'Ocurrió un error al procesar el pago'
                      return Promise.reject()
                    }

                    // ÉXITO
                    this.paymentStatus = 'in_process'
                    this.startPolling()
                    return Promise.resolve()
                  })
                  .catch((err) => {
                    this.brickError =
                      err.message ||
                      'Error de conexión con el servidor. Por favor intente nuevamente.'
                    console.error('Error en onSubmit:', err)
                    return Promise.reject()
                  })
              }

              // 👉 Billetera / Otros (Redirección o manejo automático de MP)
              this.paymentStatus = 'in_process'
              this.startPolling()
              return Promise.resolve()
            },

            onError: (error) => {
              console.error(error)
              // Usamos brickError para no romper toda la vista
              this.brickError =
                'Ocurrió un error al intentar procesar el pago. Por favor, revise los datos e intente nuevamente.'
            },
          },
        })
      } catch (err) {
        console.error(err)
        this.errorMessage = 'No se pudo inicializar el pago'
        this.loading = false
      }
    },

    startPolling() {
      this.stopPolling() // Limpiar si había uno
      // Polling cada 3 segundos
      this.pollingInterval = setInterval(async () => {
        try {
          const result = await PaymentService.getComprobante(this.paymentToken)
          // Si el backend devuelve data (comprobante), significa que el pago ya se registró
          if (result.exit && result.data) {
            this.comprobanteData = result.data
            this.paymentStatus = 'approved'
            this.stopPolling()
          }
        } catch (error) {
          if (error.status === 400 || error.status === 500) {
            this.statusMessage = error.message
            this.paymentStatus = 'rejected'
            this.stopPolling()
          }
          console.error('Error polling:', error)
        }
      }, 3000)
    },

    stopPolling() {
      if (this.pollingInterval) {
        clearInterval(this.pollingInterval)
        this.pollingInterval = null
      }
    },

    formatMonto(monto) {
      return Number(monto || 0).toLocaleString('es-AR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      })
    },

    retryPayment() {
      this.stopPolling()
      this.paymentStatus = null
      this.statusMessage = null
      this.errorMessage = null
      this.initializePayment()
    },

    goBack() {
      // Si hay historial vuelve atrás, sino cierra
      if (window.history.length > 1) {
        window.history.back()
      } else {
        window.close()
      }
    },

    async downloadReceipt() {
      if (!this.comprobanteData) return

      const element = document.getElementById('comprobante-pdf')

      try {
        // Convertimos el elemento a imagen usando html-to-image (soporta oklch mejor)
        const dataUrl = await toPng(element, {
          quality: 1.0,
          pixelRatio: 2,
          backgroundColor: '#ffffff',
          style: {
            fontFamily: 'Arial, sans-serif',
          },
          // 🔹 ESTO SOLUCIONA LOS ERRORES DE CORS Y FUENTES
          skipFonts: true,
          fontEmbedCSS: '',
        })

        const pdf = new jsPDF({
          orientation: 'portrait',
          unit: 'mm',
          format: 'a4',
        })

        const imgProps = pdf.getImageProperties(dataUrl)
        const pdfWidth = pdf.internal.pageSize.getWidth()
        const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width

        pdf.addImage(dataUrl, 'PNG', 0, 0, pdfWidth, pdfHeight)
        pdf.save(`Comprobante_${this.comprobanteData.nombreSocio.replace(/\s+/g, '_')}.pdf`)
      } catch (err) {
        console.error('Error al generar el PDF:', err)
        alert('Hubo un error al generar el comprobante. Por favor, intente nuevamente.')
      }
    },
  },
}
</script>

<style scoped>
/* Animaciones personalizadas */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }

  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.5s ease-out;
}

/* Estilos para el contenedor del brick */
#paymentBrick_container {
  min-height: 400px;
}

/* Mejoras visuales para inputs del brick */
:deep(.mp-brick-container) {
  font-family: inherit;
}

:deep(.mp-brick-container input) {
  transition: all 0.3s ease;
}

:deep(.mp-brick-container input:focus) {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}
</style>
