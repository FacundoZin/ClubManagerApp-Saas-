<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-slate-100 font-sans">
    <!-- Header -->
    <header
      class="bg-white/80 backdrop-blur-md border-b border-slate-200 sticky top-0 z-30 shadow-sm"
    >
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16 items-center">
          <div class="flex items-center gap-3">
            <div
              class="w-9 h-9 bg-gradient-to-br from-blue-600 to-blue-700 rounded-lg flex items-center justify-center shadow-md text-white"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
            <div>
              <h1 class="text-lg font-bold text-slate-900 tracking-tight leading-none">
                Pago de Cuota
              </h1>
              <span class="text-xs text-slate-500 font-medium">Club de Abuelos</span>
            </div>
          </div>

          <div class="flex items-center gap-2">
            <div
              class="px-3 py-1.5 bg-blue-50 text-blue-700 rounded-lg text-xs font-semibold border border-blue-200"
            >
              Pago Seguro
            </div>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 md:py-12">
      <!-- Loading State -->
      <div v-if="loading" class="flex flex-col items-center justify-center min-h-[500px]">
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

      <!-- Token Used State (Estilo "Content Not Available" / Mercado Libre) -->
      <div v-else-if="isTokenUsed" class="max-w-xl mx-auto mt-12">
        <div
          class="bg-white rounded-2xl shadow-lg border border-slate-100 overflow-hidden text-center pb-12"
        >
          <!-- Icon Header -->
          <div
            class="bg-slate-50 p-10 flex flex-col items-center justify-center border-b border-slate-100 bg-slate-50/50"
          >
            <div
              class="w-24 h-24 bg-white rounded-full flex items-center justify-center shadow-sm mb-4"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-12 w-12 text-blue-500"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
            <h3 class="text-2xl font-bold text-slate-800">¡Este pago ya fue realizado!</h3>
          </div>

          <!-- Content -->
          <div class="px-8 mt-8">
            <p class="text-slate-600 text-lg leading-relaxed">
              El enlace que utilizaste ya no está disponible porque la cuota fue abonada
              anteriormente.
            </p>
            <div class="mt-6 p-5 bg-blue-50 rounded-xl inline-block border border-blue-100">
              <p class="text-sm text-blue-800 font-medium flex items-center gap-2 justify-center">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                  />
                </svg>
                No es necesario que realices ninguna acción adicional.
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Error State -->
      <div v-else-if="errorMessage" class="max-w-2xl mx-auto">
        <div class="bg-white rounded-2xl shadow-xl border border-red-200 overflow-hidden">
          <div class="bg-gradient-to-r from-red-500 to-red-600 p-6">
            <div class="flex items-center gap-4">
              <div
                class="w-12 h-12 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-6 w-6 text-white"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
                  />
                </svg>
              </div>
              <div>
                <h3 class="text-xl font-bold text-white">Error al cargar el pago</h3>
                <p class="text-red-100 text-sm mt-1">No pudimos procesar tu solicitud</p>
              </div>
            </div>
          </div>
          <div class="p-6">
            <div class="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
              <p class="text-red-800 font-medium">{{ errorMessage }}</p>
            </div>
            <div class="flex flex-col sm:flex-row gap-3">
              <button
                @click="retryPayment"
                class="flex-1 px-6 py-3 bg-gradient-to-r from-red-500 to-red-600 text-white font-semibold rounded-lg hover:from-red-600 hover:to-red-700 transition-all duration-300 shadow-md hover:shadow-lg transform hover:-translate-y-0.5"
              >
                Reintentar
              </button>
              <button
                @click="goBack"
                class="flex-1 px-6 py-3 bg-slate-100 text-slate-700 font-semibold rounded-lg hover:bg-slate-200 transition-all duration-300"
              >
                Volver
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Payment Content -->
      <div v-else class="space-y-6">
        <!-- Welcome Card -->
        <div
          class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl border border-slate-200 overflow-hidden transform transition-all duration-500 hover:shadow-2xl"
        >
          <div class="bg-gradient-to-r from-blue-600 via-blue-700 to-indigo-700 p-8">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div
                  class="inline-flex items-center gap-2 bg-white/20 backdrop-blur-sm px-3 py-1 rounded-full mb-4"
                >
                  <div class="w-2 h-2 bg-green-400 rounded-full animate-pulse"></div>
                  <span class="text-xs font-semibold text-white">Sesión activa</span>
                </div>
                <h2 class="text-2xl md:text-3xl font-bold text-white mb-2">
                  ¡Hola, {{ paymentData?.nombreSocio || 'Socio' }}!
                </h2>
                <p class="text-blue-100 text-base md:text-lg leading-relaxed">
                  Aquí podrá realizar el pago de su cuota al Club de Abuelos correspondiente al
                  <span class="font-semibold text-white">{{ paymentData?.semestrePago }}</span>
                  del año <span class="font-semibold text-white">{{ paymentData?.anioPago }}</span
                  >.
                </p>
              </div>
              <div class="hidden md:block">
                <div
                  class="w-16 h-16 bg-white/10 backdrop-blur-sm rounded-2xl flex items-center justify-center"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-8 w-8 text-white"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                </div>
              </div>
            </div>
          </div>

          <!-- Payment Details -->
          <div class="p-6 bg-gradient-to-br from-slate-50 to-white">
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div class="bg-white rounded-xl p-4 border border-slate-200 shadow-sm">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-blue-50 rounded-lg flex items-center justify-center">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="h-5 w-5 text-blue-600"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                      />
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs text-slate-500 font-medium">Período</p>
                    <p class="text-sm font-bold text-slate-900">
                      {{ paymentData?.semestrePago }}
                    </p>
                  </div>
                </div>
              </div>

              <div class="bg-white rounded-xl p-4 border border-slate-200 shadow-sm">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-indigo-50 rounded-lg flex items-center justify-center">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="h-5 w-5 text-indigo-600"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs text-slate-500 font-medium">Año</p>
                    <p class="text-sm font-bold text-slate-900">{{ paymentData?.anioPago }}</p>
                  </div>
                </div>
              </div>

              <div
                class="bg-gradient-to-br from-emerald-500 to-emerald-600 rounded-xl p-4 shadow-md"
              >
                <div class="flex items-center gap-3">
                  <div
                    class="w-10 h-10 bg-white/20 backdrop-blur-sm rounded-lg flex items-center justify-center"
                  >
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="h-5 w-5 text-white"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        stroke-width="2"
                        d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs text-emerald-100 font-medium">Monto a pagar</p>
                    <p class="text-lg font-bold text-white">
                      ${{ formatMonto(paymentData?.monto) }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

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
            <div id="paymentBrick_container"></div>
          </div>
        </div>

        <!-- Security Info -->
        <div class="bg-blue-50/50 backdrop-blur-sm border border-blue-200 rounded-xl p-4">
          <div class="flex items-start gap-3">
            <div
              class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center flex-shrink-0 mt-0.5"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-4 w-4 text-blue-600"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
                />
              </svg>
            </div>
            <div class="flex-1">
              <h4 class="text-sm font-semibold text-blue-900 mb-1">Pago 100% seguro</h4>
              <p class="text-xs text-blue-700 leading-relaxed">
                Tus datos están protegidos. Procesado por Mercado Pago, cumpliendo con los más altos
                estándares de seguridad.
              </p>
            </div>
          </div>
        </div>
      </div>
    </main>

    <!-- Footer -->
    <footer class="border-t border-slate-200 bg-white/80 backdrop-blur-sm mt-12 py-6">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        <p class="text-slate-500 text-sm font-medium">
          © {{ new Date().getFullYear() }} Club de Abuelos - Portal de pagos online
        </p>
        <p class="text-slate-400 text-xs mt-1">Procesado de forma segura por Mercado Pago</p>
      </div>
    </footer>
  </div>
</template>

<script>
import { nextTick } from 'vue'

export default {
  data() {
    return {
      loading: true,
      paymentData: null,
      errorMessage: null,
      brickController: null,
      isTokenUsed: false,
    }
  },

  async mounted() {
    await this.initializePayment()
  },

  beforeUnmount() {
    if (this.brickController) {
      this.brickController.unmount()
    }
  },

  methods: {
    async initializePayment() {
      try {
        this.loading = true
        this.errorMessage = null
        this.isTokenUsed = false

        // 🔹 Token desde URL
        const params = new URLSearchParams(window.location.search)
        const paymentToken = params.get('token')

        if (!paymentToken) {
          this.errorMessage = 'Token de pago inválido o inexistente'
          this.loading = false
          return
        }

        // 🔹 Init payment (backend)
        const res = await fetch('http://localhost:5194/api/Payment/initPayment', {
          method: 'POST',
          headers: {
            PaymentToken: paymentToken,
          },
        })

        // Verificar si la respuesta es válida
        if (!res.ok) {
          let errorMsg = 'Error al conectar con el servidor'
          try {
            const errorData = await res.json()
            errorMsg = errorData.errormessage || errorData.message || errorMsg

            // 🔹 Detectar si el token ya fue usado (backend retorna 422 con este mensaje)
            if (errorMsg && errorMsg.toLowerCase().includes('ya fue utilizado')) {
              this.isTokenUsed = true
              this.loading = false
              return
            }
          } catch {
            errorMsg = `Error del servidor (${res.status}): ${res.statusText}`
          }
          this.errorMessage = errorMsg
          this.loading = false
          return
        }

        // Intentar parsear JSON
        let result
        try {
          result = await res.json()
        } catch (jsonError) {
          console.error('Error parsing JSON:', jsonError)
          this.errorMessage = 'El servidor devolvió una respuesta inválida'
          this.loading = false
          return
        }

        if (!result.exit) {
          // Check also here just in case backend structure changes slightly
          if (
            result.errormessage &&
            result.errormessage.toLowerCase().includes('ya fue utilizado')
          ) {
            this.isTokenUsed = true
            this.loading = false
            return
          }

          this.errorMessage = result.errormessage || 'No se pudo iniciar el pago'
          this.loading = false
          return
        }

        // 🔹 Datos válidos
        this.paymentData = result.data

        // 🔹 Asegurar que el contenedor esté en el DOM
        this.loading = false
        await nextTick()

        // 🔹 SDK MP
        const mp = new window.MercadoPago('TEST-423365d5-3d72-4d03-82c8-f85403327ce4', {
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
              // 👉 Wallet usa preference, no se procesa acá
              if (selectedPaymentMethod === 'mercadoPago') {
                return Promise.resolve()
              }

              // 👉 Tarjetas → backend
              return fetch('http://localhost:5194/api/Payment/processPayment', {
                method: 'POST',
                headers: {
                  'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                  ...formData,
                  externalReference: this.paymentData.externalReference,
                }),
              })
                .then(async (res) => {
                  const result = await res.json()

                  if (!result.exit) {
                    throw new Error(result.errormessage)
                  }

                  return Promise.resolve()
                })
                .catch((err) => {
                  this.errorMessage = err.message || 'Error al procesar el pago con tarjeta'
                  return Promise.reject()
                })
            },

            onError: (error) => {
              console.error(error)
              this.errorMessage = 'Ocurrió un error inesperado al mostrar el formulario de pago'
            },
          },
        })
      } catch (err) {
        console.error(err)
        this.errorMessage = 'No se pudo inicializar el pago'
        this.loading = false
      }
    },

    formatMonto(monto) {
      return Number(monto || 0).toLocaleString('es-AR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      })
    },

    retryPayment() {
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
