# Plan de Implementación — Actualizar Formulario de Inscripción en el Dashboard de Viajes

## Contexto

El sistema tiene **2 formas** de inscribir personas a un viaje:

1. **Desde la vista de detalle del viaje** (`ViajeDetailView.vue`) → abre el `InscripcionConfirmModal.vue` directamente con el `idViaje` ya conocido. **Este flujo YA fue actualizado correctamente** en la refactorización anterior.

2. **Desde el dashboard del módulo de viajes** (`ViajesDashboard.vue`) → el usuario navega al módulo, elige la opción "Inscribir socio a viaje", y se muestra un formulario inline que primero busca un socio por DNI, luego selecciona un viaje, y finalmente abre el `InscripcionConfirmModal.vue` pasándole el socio y el viaje. **Este flujo quedó desactualizado** y sigue la lógica vieja.

### Problema Actual

El flujo de inscripción en `ViajesDashboard.vue` (líneas 345-432) tiene los siguientes problemas:

- **Paso 1** busca un socio por DNI usando `SociosService.getByDni()` — esto ya no corresponde porque los inscriptos no son socios.
- **Paso 2** selecciona un viaje desde un combo box y luego abre el `InscripcionConfirmModal` pasándole `socio` y `idViaje`.
- El botón de la card de acción dice "Inscribir socio a viaje" y la descripción dice "Registrar a un socio en una variante de viaje" — deben actualizarse.
- El import de `SociosService` en este archivo solo se usa para este flujo y debe eliminarse.

Adicionalmente, en `ViajeDetailView.vue` (línea 229) el botón dice **"Inscribir Socio"** y debería decir **"Inscribir Persona"**.

---

## Archivos Afectados

| Archivo | Acción |
|---------|--------|
| `frontend/src/views/ModuloGestionViajes/ViajesDashboard.vue` | **MODIFICAR** |
| `frontend/src/views/ModuloGestionViajes/ViajeDetailView.vue` | **MODIFICAR** (texto del botón) |

> **IMPORTANTE**: No se necesitan cambios en el backend ni en `InscripcionConfirmModal.vue`. El modal ya funciona correctamente y se reutiliza tal cual.

---

## Cambios Detallados

### 1. `ViajesDashboard.vue` — Sección `<script setup>`

#### 1.1 Eliminar import de SociosService

```diff
- import SociosService from '../../services/SociosService'
```

#### 1.2 Eliminar todas las variables del flujo de inscripción viejo

Eliminar las siguientes variables (líneas 22-27):

```diff
- // Inscription Flow State
- const searchDni = ref('')
- const selectedSocio = ref(null)
- const isSearchingSocio = ref(false)
- const searchError = ref('')
- const comboViajes = ref([])
- const selectedViajeId = ref(null)
```

Y reemplazar por las nuevas variables necesarias:
```javascript
// Inscription Flow State (nuevo)
const comboViajes = ref([])
const selectedViajeId = ref(null)
```

#### 1.3 Actualizar los textos del array `actions`

Buscar el objeto con `id: 'inscribir'` (líneas 70-77) y cambiar:

```diff
  {
    id: 'inscribir',
-   title: 'Inscribir socio a viaje',
+   title: 'Inscribir persona a viaje',
-   description: 'Registrar a un socio en una variante de viaje.',
+   description: 'Registrar una o más personas en un viaje.',
    icon: 'M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z',
    color: 'text-blue-600',
    bg: 'bg-blue-50',
    hoverBorder: 'group-hover:border-blue-200',
  },
```

#### 1.4 Actualizar `selectAction` para la acción 'inscribir'

Actualmente (líneas 86-88) al seleccionar 'inscribir' llama a `resetInscriptionFlow()`. Debe cambiarse para que cargue el combo de viajes:

```diff
  } else if (actionId === 'inscribir') {
-   resetInscriptionFlow()
+   fetchComboViajes()
  }
```

#### 1.5 Eliminar las funciones del flujo viejo

Eliminar completamente estas funciones:
- `resetInscriptionFlow` (líneas 146-153)
- `handleSearchSocio` (líneas 155-171)
- `confirmInscripcion` (líneas 173-176)

#### 1.6 Agregar las nuevas funciones

```javascript
const fetchComboViajes = async () => {
  try {
    comboViajes.value = await ViajesService.getComboBoxViajes()
  } catch (error) {
    showToast(error.message, 'error')
  }
}

const confirmInscripcion = () => {
  if (!selectedViajeId.value) return
  isInscripcionModalOpen.value = true
}

const resetInscriptionFlow = () => {
  selectedViajeId.value = null
  comboViajes.value = []
}
```

#### 1.7 Actualizar `handleFinishInscripcion`

Actualmente (líneas 178-183):
```javascript
const handleFinishInscripcion = () => {
  isInscripcionModalOpen.value = false
  showToast('Inscripción realizada con éxito')
  resetInscriptionFlow()
  currentAction.value = 'none'
}
```

Cambiar a:
```javascript
const handleFinishInscripcion = () => {
  isInscripcionModalOpen.value = false
  showToast('Inscripción realizada con éxito')
  resetInscriptionFlow()
  // Mantener al usuario en la sección de inscripción por si quiere inscribir más
  fetchComboViajes()
}
```

---

### 2. `ViajesDashboard.vue` — Sección `<template>`

#### 2.1 Reemplazar toda la sección "INSCRIBIR ACTION" (líneas 345-432)

Reemplazar el bloque completo `v-else-if="currentAction === 'inscribir'"` con el siguiente contenido. El nuevo flujo es simplemente:
1. Seleccionar un viaje desde un combo box.
2. Hacer clic en "Continuar" → se abre el `InscripcionConfirmModal` con ese `idViaje`.
3. El modal ya tiene todo el wizard de 3 pasos (file, personas, variantes, montos).

```html
<!-- INSCRIBIR ACTION -->
<div v-else-if="currentAction === 'inscribir'"
  class="max-w-3xl mx-auto py-4 animate-in fade-in slide-in-from-right-4 duration-500">
  <h3 class="text-xl font-bold text-slate-900 mb-8 flex items-center">
    <span
      class="flex items-center justify-center w-8 h-8 rounded-full bg-blue-100 text-blue-600 text-sm mr-3">
      <svg class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
          d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
      </svg>
    </span>
    Inscribir Persona a Viaje
  </h3>

  <!-- Paso único: Seleccionar Viaje -->
  <div class="mb-10 p-6 bg-slate-50 rounded-2xl border border-slate-200">
    <h4 class="text-sm font-bold text-slate-400 uppercase tracking-wider mb-4">
      Seleccione el viaje
    </h4>
    <p class="text-sm text-slate-500 mb-4">
      Elija el destino al que desea inscribir personas. Luego se abrirá el formulario
      para cargar los datos de los inscriptos, seleccionar la variante y registrar la entrega inicial.
    </p>
    <div class="space-y-4">
      <label class="block text-sm font-medium text-slate-700">Viaje disponible</label>
      <select v-model="selectedViajeId"
        class="block w-full px-4 py-3 border border-slate-300 rounded-xl bg-white shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm">
        <option :value="null" disabled>Seleccione un viaje...</option>
        <option v-for="v in comboViajes" :key="v.idViaje" :value="v.idViaje">
          {{ v.nombreViaje }}
        </option>
      </select>

      <div class="flex justify-end mt-6">
        <button @click="confirmInscripcion" :disabled="!selectedViajeId"
          class="px-8 py-3 bg-blue-600 text-white font-bold rounded-xl shadow-lg hover:bg-blue-700 transition-all disabled:opacity-50 flex items-center">
          Continuar a Inscripción
          <svg class="w-4 h-4 ml-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
              d="M14 5l7 7m0 0l-7 7m7-7H3" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</div>
```

> **Nota**: Se eliminan por completo el input de búsqueda por DNI, la card de "Socio encontrado", y toda la lógica del Paso 1 viejo. El flujo simplificado es: elegir viaje → abrir modal (que ya contiene todo el wizard actualizado).

#### 2.2 Actualizar la invocación del modal `InscripcionConfirmModal`

Actualmente (líneas 444-445):
```html
<InscripcionConfirmModal v-if="isInscripcionModalOpen" :is-open="isInscripcionModalOpen" :socio="selectedSocio"
  :id-viaje="selectedViajeId" @close="isInscripcionModalOpen = false" @save="handleFinishInscripcion" />
```

Cambiar a (eliminar la prop `:socio` que ya no existe en el modal actualizado):
```html
<InscripcionConfirmModal v-if="isInscripcionModalOpen" :is-open="isInscripcionModalOpen"
  :id-viaje="selectedViajeId" @close="isInscripcionModalOpen = false" @save="handleFinishInscripcion" />
```

---

### 3. `ViajeDetailView.vue` — Texto del botón

Buscar el botón "Inscribir Socio" (línea 229) y cambiar el texto:

```diff
- Inscribir Socio
+ Inscribir Persona
```

---

## Resumen Visual del Nuevo Flujo

```
Dashboard de Viajes → Opción "Inscribir persona a viaje"
    ↓
[Seleccionar viaje desde combo box] → Botón "Continuar a Inscripción"
    ↓
Se abre InscripcionConfirmModal (wizard de 3 pasos, ya actualizado):
    ├─ Paso 1: Ingresar File + agregar personas (nombre, apellido, teléfono)
    ├─ Paso 2: Seleccionar variante para cada persona (o aplicar a todos)
    └─ Paso 3: Ingresar monto de entrega + número de recibo por persona
    ↓
Confirmación → POST a /api/Viajes/inscribir
```

---

## Checklist de Verificación

- [ ] La opción del dashboard dice "Inscribir persona a viaje" (no "socio")
- [ ] La descripción dice "Registrar una o más personas en un viaje" (no "socio" ni "variante")
- [ ] Al seleccionar la opción se carga el combo de viajes disponibles (sin buscar socio por DNI)
- [ ] No hay input de búsqueda por DNI en la sección de inscripción del dashboard
- [ ] No se importa ni usa `SociosService` en `ViajesDashboard.vue`
- [ ] Al seleccionar un viaje y hacer clic en "Continuar", se abre el `InscripcionConfirmModal` con el `idViaje` correcto
- [ ] El modal NO recibe la prop `socio` (ya no existe esa prop en el componente actualizado)
- [ ] El wizard de 3 pasos dentro del modal funciona correctamente (file → variantes → montos)
- [ ] En `ViajeDetailView.vue`, el botón dice "Inscribir Persona" en lugar de "Inscribir Socio"
- [ ] Después de inscribir exitosamente desde el dashboard, el combo se recarga para permitir otra inscripción
