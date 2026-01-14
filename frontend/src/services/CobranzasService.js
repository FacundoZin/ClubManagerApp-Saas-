const API_URL = 'http://localhost:5194/api/Cobranzas'

export default {
  async listarSociosDeudoresPorLote(lote, pageNumber = 1, pageSize = 10) {
    const response = await fetch(
      `${API_URL}/lotes/${lote}/deudores?pageNumber=${pageNumber}&pageSize=${pageSize}`,
    )

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al obtener socios deudores')
    }

    return await response.json()
  },

  async listarLotes() {
    const response = await fetch(`${API_URL}/lotes`)

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al obtener los lotes')
    }

    return await response.json()
  },

  async crearLote(loteData) {
    const response = await fetch(`${API_URL}/lotes`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(loteData),
    })

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || 'Error al crear el lote')
    }

    return await response.json()
  },
}
