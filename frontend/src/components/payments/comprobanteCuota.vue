<template>
  <div
    class="receipt-container"
    style="
      padding: 32px;
      background-color: #ffffff;
      max-width: 700px;
      margin: 0 auto;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
      font-family: Arial, Helvetica, sans-serif !important;
      color: #1e293b;
    "
  >
    <!-- Header -->
    <div
      style="
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        border-bottom: 2px solid #f1f5f9;
        padding-bottom: 32px;
        margin-bottom: 32px;
      "
    >
      <div>
        <h1 style="font-size: 28px; font-weight: bold; color: #0f172a; margin: 0 0 4px 0">
          Asociación Civil Casa del Jubilado
        </h1>
        <p
          style="
            font-size: 14px;
            font-weight: 600;
            color: #2563eb;
            letter-spacing: 0.05em;
            text-transform: uppercase;
            margin: 0;
          "
        >
          Club de Abuelos
        </p>
        <p style="font-size: 12px; color: #64748b; margin: 16px 0 0 0; line-height: 1.5">
          Av. Juan de Garay 2223, San Francisco, Córdoba<br />
          República Argentina
        </p>
      </div>
      <div style="text-align: right">
        <div
          style="
            display: inline-block;
            padding: 8px 16px;
            background-color: #eff6ff;
            color: #1d4ed8;
            border-radius: 8px;
            font-weight: bold;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.05em;
            border: 1px solid #dbeafe;
            margin-bottom: 16px;
          "
        >
          Comprobante de Pago
        </div>
        <p style="font-size: 12px; color: #94a3b8; margin: 0">Fecha de emisión</p>
        <p style="font-size: 14px; font-weight: 600; color: #334155; margin: 0">
          {{ currentDate }}
        </p>
      </div>
    </div>

    <!-- Socio Info -->
    <div style="display: flex; gap: 16px; margin-bottom: 32px">
      <div
        style="
          flex: 1;
          padding: 16px;
          border-radius: 12px;
          background-color: #f8fafc;
          border: 1px solid #f1f5f9;
        "
      >
        <p
          style="
            font-size: 10px;
            font-weight: bold;
            color: #94a3b8;
            text-transform: uppercase;
            letter-spacing: 0.1em;
            margin: 0 0 4px 0;
          "
        >
          Nombre del Socio
        </p>
        <p style="font-size: 16px; font-weight: bold; color: #1e293b; margin: 0">
          {{ data.nombreSocio }}
        </p>
      </div>
      <div
        style="
          flex: 1;
          padding: 16px;
          border-radius: 12px;
          background-color: #f8fafc;
          border: 1px solid #f1f5f9;
        "
      >
        <p
          style="
            font-size: 10px;
            font-weight: bold;
            color: #94a3b8;
            text-transform: uppercase;
            letter-spacing: 0.1em;
            margin: 0 0 4px 0;
          "
        >
          DNI / Documento
        </p>
        <p style="font-size: 16px; font-weight: bold; color: #1e293b; margin: 0">
          {{ data.dniSocio }}
        </p>
      </div>
    </div>

    <!-- Detail Table -->
    <div
      style="margin-bottom: 40px; overflow: hidden; border-radius: 12px; border: 1px solid #e2e8f0"
    >
      <table style="width: 100%; text-align: left; border-collapse: collapse">
        <thead style="background-color: #f8fafc; border-bottom: 1px solid #e2e8f0">
          <tr>
            <th
              style="
                padding: 16px 24px;
                font-size: 10px;
                font-weight: bold;
                color: #64748b;
                text-transform: uppercase;
                letter-spacing: 0.1em;
              "
            >
              Descripción del Item
            </th>
            <th
              style="
                padding: 16px 24px;
                font-size: 10px;
                font-weight: bold;
                color: #64748b;
                text-transform: uppercase;
                letter-spacing: 0.1em;
                text-align: right;
              "
            >
              Monto
            </th>
          </tr>
        </thead>
        <tbody style="background-color: #ffffff">
          <tr style="border-bottom: 1px solid #f1f5f9">
            <td style="padding: 20px 24px">
              <p style="font-size: 14px; font-weight: bold; color: #1e293b; margin: 0">
                Pago de Cuota Social
              </p>
              <p style="font-size: 12px; color: #64748b; margin: 4px 0 0 0">
                {{ data.semestrePagoText }} - Año {{ data.anioPago }}
              </p>
            </td>
            <td style="padding: 20px 24px; text-align: right">
              <span style="font-size: 14px; font-weight: bold; color: #0f172a"
                >$ {{ formatMonto(data.monto) }}</span
              >
            </td>
          </tr>
        </tbody>
        <tfoot style="background-color: #f8fafc">
          <tr>
            <td style="padding: 16px 24px; text-align: right">
              <span
                style="
                  font-size: 12px;
                  font-weight: bold;
                  color: #64748b;
                  text-transform: uppercase;
                  letter-spacing: 0.05em;
                "
                >Total Abonado</span
              >
            </td>
            <td style="padding: 16px 24px; text-align: right">
              <span style="font-size: 20px; font-weight: bold; color: #2563eb"
                >$ {{ formatMonto(data.monto) }}</span
              >
            </td>
          </tr>
        </tfoot>
      </table>
    </div>

    <!-- Footer Security / Legal -->
    <div style="position: relative; padding-top: 32px; border-top: 1px solid #f1f5f9">
      <div style="display: flex; justify-content: space-between; align-items: center">
        <div>
          <p style="font-size: 10px; color: #94a3b8; max-width: 320px; line-height: 1.5; margin: 0">
            Este documento es un comprobante de pago válido generado de forma electrónica. Conserve
            este recibo para cualquier trámite administrativo.
          </p>
        </div>

        <!-- Stamp Emulation -->
        <div
          style="
            display: flex;
            flex-direction: column;
            align-items: center;
            opacity: 0.7;
            transform: rotate(-5deg);
          "
        >
          <div
            style="
              width: 80px;
              height: 80px;
              border-radius: 50%;
              border: 4px double #10b981;
              display: flex;
              align-items: center;
              justify-content: center;
            "
          >
            <div
              style="
                text-align: center;
                font-weight: bold;
                text-transform: uppercase;
                font-size: 10px;
                color: #059669;
              "
            >
              Pago<br />Aprobado
            </div>
          </div>
          <p
            style="
              font-size: 8px;
              font-weight: bold;
              color: #10b981;
              margin: 4px 0 0 0;
              letter-spacing: -0.05em;
            "
          >
            SISTEMA ONLINE C.A.S.A
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ComprobanteCuota',
  props: {
    data: {
      type: Object,
      required: true,
      // Espera InfoComprobanteDto: { nombreSocio, dniSocio, anioPago, semestrePagoText, monto }
    },
  },
  computed: {
    currentDate() {
      return new Date().toLocaleDateString('es-AR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      })
    },
  },
  methods: {
    formatMonto(monto) {
      if (!monto) return '0,00'
      // Si el backend envía "1500,50", reemplazamos la coma por punto para Number()
      const cleanMonto = typeof monto === 'string' ? monto.replace(',', '.') : monto
      return Number(cleanMonto).toLocaleString('es-AR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      })
    },
  },
}
</script>

<style scoped>
/* Sin Tailwind para evitar inyección de oklch */
</style>
