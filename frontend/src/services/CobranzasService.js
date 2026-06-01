const API_URL = `${import.meta.env.VITE_API_URL}/Cobranzas`

export default {
  async listarSociosDeudoresPorLote(lote, pageNumber = 1, pageSize = 10) {
    const response = await fetch(
      `${API_URL}/lotes/${lote}/deudores?pageNumber=${pageNumber}&pageSize=${pageSize}`,
      { credentials: 'include' },
    )

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al obtener socios deudores')
    }

    return await response.json()
  },

  async listarLotes() {
    const response = await fetch(`${API_URL}/lotes`, { credentials: 'include' })

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al obtener los lotes')
    }


    const data = await response.json()
    return data
  },

  async crearLote(loteData) {
    const response = await fetch(`${API_URL}/lotes`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(loteData),
    })

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al crear el lote')
    }

    return await response.json()
  },

  async verListadoDeCobradores() {
    const response = await fetch(`${API_URL}/cobradores`, { credentials: 'include' })

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al obtener el listado de cobradores')
    }

    return await response.json()
  },

  async verHistorialDeCobradorByMes(idCobrador, mes, anio) {
    const response = await fetch(
      `${API_URL}/cobrador/historial?idCobrador=${idCobrador}&mes=${mes}&anio=${anio}`,
      { credentials: 'include' },
    )

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al obtener el historial del cobrador')
    }

    return await response.json()
  },

  async descargarPlanillaDeudores(idLote) {
    const response = await fetch(
      `${API_URL}/lotes/${idLote}/planilla-deudores`,
      { credentials: 'include' }
    )

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al descargar la planilla')
    }

    const blob = await response.blob()
    const url = window.URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.style.display = 'none'
    a.href = url
    a.download = `planilla_deudores_lote_${idLote}.pdf`
    document.body.appendChild(a)
    a.click()
    a.remove()
    window.URL.revokeObjectURL(url)
  },
}
