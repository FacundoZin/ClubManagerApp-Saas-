const API_URL = `${import.meta.env.VITE_API_URL}/Viajes`;

const handleError = async (response, defaultMessage) => {
  if (response.status >= 500) {
    throw new Error(
      "Algo salió mal en el servidor. Por favor intente más tarde.",
    );
  }
  const errorText = await response.text();
  if (!errorText) return defaultMessage;
  try {
    const errorObj = JSON.parse(errorText);
    if (errorObj.errors) {
      const firstErrorKey = Object.keys(errorObj.errors)[0];
      return errorObj.errors[firstErrorKey][0];
    }
    return errorObj.mensaje || errorObj.message || errorText || defaultMessage;
  } catch (e) {
    return errorText || defaultMessage;
  }
};

export default {
  async listarViajesDisponibles() {
    const response = await fetch(`${API_URL}`, { credentials: 'include' });
    if (!response.ok) {
      const msg = await handleError(response, "Error al listar viajes disponibles");
      throw new Error(msg);
    }
    return await response.json();
  },

  async listarVariantesDeViaje(idViajeBase) {
    const response = await fetch(`${API_URL}/variantes/${idViajeBase}`, { credentials: 'include' });
    if (!response.ok) {
      const msg = await handleError(response, "Error al listar variantes de viaje");
      throw new Error(msg);
    }
    return await response.json();
  },

  async verViajeCompleto(idViajeBase) {
    const response = await fetch(`${API_URL}/completo/${idViajeBase}`, { credentials: 'include' });
    if (!response.ok) {
      const msg = await handleError(response, "Error al obtener detalle completo del viaje");
      throw new Error(msg);
    }
    return await response.json();
  },

  async createViaje(dto) {
    const response = await fetch(`${API_URL}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(dto),
    });
    if (!response.ok) {
      const msg = await handleError(response, "Error al crear el viaje");
      throw new Error(msg);
    }
  },

  async createVarianteViaje(dto) {
    const response = await fetch(`${API_URL}/variante`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(dto),
    });
    if (!response.ok) {
      const msg = await handleError(response, "Error al crear la variante de viaje");
      throw new Error(msg);
    }
  },

  async inscribirSocio(dto) {
    const response = await fetch(`${API_URL}/inscribir`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(dto),
    });
    if (!response.ok) {
      const msg = await handleError(response, "Error al inscribir al socio");
      throw new Error(msg);
    }
  },

  async actualizarPago(dto) {
    const response = await fetch(`${API_URL}/pago`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(dto),
    });
    if (!response.ok) {
      const msg = await handleError(response, "Error al actualizar el pago");
      throw new Error(msg);
    }
  },

  async cancelarInscripcion(idInscripto) {
    const response = await fetch(`${API_URL}/inscripcion/${idInscripto}`, {
      method: 'DELETE',
      credentials: 'include',
    });
    if (!response.ok) {
      const msg = await handleError(response, "Error al cancelar la inscripción");
      throw new Error(msg);
    }
  },

  async getComboBoxViajes() {
    const response = await fetch(`${API_URL}/combobox`, { credentials: 'include' });
    if (!response.ok) {
      const msg = await handleError(response, "Error al obtener combobox de viajes");
      throw new Error(msg);
    }
    return await response.json();
  },

  async getComboBoxVariantes(idViaje) {
    const response = await fetch(`${API_URL}/${idViaje}/variantes/combobox`, { credentials: 'include' });
    if (!response.ok) {
      const msg = await handleError(response, "Error al obtener combobox de variantes");
      throw new Error(msg);
    }
    return await response.json();
  }
};
