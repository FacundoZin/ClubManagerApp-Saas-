---
description: Plan completo para implementar el frontend del módulo de gestión de viajes, consumiendo el backend existente.
---

# Implementación Frontend — Módulo de Gestión de Viajes

## Contexto del Proyecto

Este sistema es una aplicación interna de gestión para un club de jubilados. El frontend usa **Vue 3 (Composition API `<script setup>`)**, **TailwindCSS** y **Vue Router**. El backend es una API .NET que ya está 100 % implementada.

> [!IMPORTANT]
>
> - **No crear** componentes comunes nuevos ni modificar los existentes en `frontend/src/components/Common/`.
> - La estética debe seguir **exactamente** el patrón visual de `frontend/src/views/ModuloGestionSocios/SociosDashboard.vue` (fondo `bg-slate-50`, cards blancas con borde `border-slate-200`, tipografía `text-slate-900/500/700`, botones con colores `teal`/`emerald`/`blue`, etc.).
> - Todas las peticiones HTTP usan `fetch` con `credentials: 'include'` (autenticación por cookies).
> - El archivo de servicio ya existe vacío en `frontend/src/services/viajesService.js`.

---

## 1. Archivo de Servicio — `frontend/src/services/viajesService.js`

Implementar **todos** los métodos para consumir los 8 endpoints del backend. Usar como referencia el patrón de `frontend/src/services/SociosService.js`.

### Base URL

```js
const API_URL = `${import.meta.env.VITE_API_URL}/Viajes`;
```

### Handler de errores (copiar el patrón exacto de SociosService)

```js
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
```

### Métodos requeridos (export default object)

| Método JS                             | HTTP     | URL                                        | Body / Params | Response esperada           |
| ------------------------------------- | -------- | ------------------------------------------ | ------------- | --------------------------- |
| `listarViajesDisponibles()`           | `GET`    | `/api/Viajes`                              | —             | `PreviewViajeDto[]`         |
| `listarVariantesDeViaje(idViajeBase)` | `GET`    | `/api/Viajes/variantes/{idViajeBase}`      | —             | `PreviewVarianteViajeDto[]` |
| `verViajeCompleto(idViajeBase)`       | `GET`    | `/api/Viajes/completo/{idViajeBase}`       | —             | `FullViewViajeDto`          |
| `createViaje(dto)`                    | `POST`   | `/api/Viajes`                              | JSON body     | `object`                    |
| `createVarianteViaje(dto)`            | `POST`   | `/api/Viajes/variante`                     | JSON body     | `object`                    |
| `inscribirSocio(dto)`                 | `POST`   | `/api/Viajes/inscribir`                    | JSON body     | `object`                    |
| `actualizarPago(dto)`                 | `POST`   | `/api/Viajes/pago`                         | JSON body     | void (200 OK)               |
| `cancelarInscripcion(idInscripto)`    | `DELETE` | `/api/Viajes/inscripcion/{idInscripto}`    | —             | `object`                    |
| `getComboBoxViajes()`                 | `GET`    | `/api/Viajes/combobox`                     | —             | `ComboBoxViajes[]`          |
| `getComboBoxVariantes(idViaje)`       | `GET`    | `/api/Viajes/{idViaje}/variantes/combobox` | —             | `ComboBoxVariantesViaje[]`  |

### DTOs de entrada (JSON body shapes)

#### `CreateViajeDto`

```json
{
  "titulo": "string (requerido)",
  "dias": "int (1-365, requerido)",
  "noches": "int (1-365, requerido)",
  "fechaSalida": "string ISO date 'YYYY-MM-DD' (requerido, debe ser futura)",
  "ventasParaLiberado": "int? (opcional)",
  "valorBase": "decimal > 0 (requerido)",
  "porcentajeComision": "decimal > 0 (requerido)"
}
```

#### `CreateVarianteViajeDto`

```json
{
  "idViaje": "int (requerido)",
  "nombreVariante": "string (requerido)",
  "valorViaje": "decimal (requerido)",
  "valorSeña": "decimal > 0 (requerido, debe ser <= valorViaje)",
  "regimen": "int (0 = MediaPension, 1 = PensionCompleta)",
  "tipoDeButaca": "string (requerido)"
}
```

#### `InsertInscriptoViajeDto`

```json
{
  "viajeVarianteId": "int (requerido)",
  "socioId": "int (requerido)",
  "montoAbonado": "decimal (requerido, debe ser >= seña y <= valorViaje)"
}
```

#### `UpdatePagoViajeDto`

```json
{
  "idInscripto": "int (requerido)",
  "montoAbonado": "decimal (requerido, debe ser <= montoPendiente)"
}
```

### DTOs de salida (response shapes)

#### `PreviewViajeDto`

```json
{
  "id": "int",
  "titulo": "string",
  "dias": "int",
  "noches": "int",
  "fechasalida": "string date",
  "ventasParaLiberado": "int?",
  "valorBase": "decimal",
  "porcentajeComision": "decimal"
}
```

#### `PreviewVarianteViajeDto`

```json
{
  "id": "int",
  "nombreVariante": "string",
  "valorViaje": "decimal",
  "valorSeña": "decimal",
  "regimen": "int (0=MediaPension, 1=PensionCompleta)",
  "tipoDeButaca": "string"
}
```

#### `FullViewViajeDto`

```json
{
  "id": "int",
  "titulo": "string",
  "dias": "int",
  "noches": "int",
  "fechasalida": "string date",
  "ventasParaLiberado": "int?",
  "valorBase": "decimal",
  "totalInscriptos": "int",
  "totalCancelados": "int",
  "totalRecaudado": "decimal",
  "totalPendiente": "decimal",
  "montoComision": "decimal",
  "montoParaAgencia": "decimal",
  "totalLiberados": "int",
  "variantes": "VarianteViajeWithInscriptosDto[]"
}
```

#### `VarianteViajeWithInscriptosDto`

```json
{
  "id": "int",
  "nombreVariante": "string",
  "valorViaje": "decimal",
  "valorSeña": "decimal",
  "regimen": "int",
  "tipoDeButaca": "string",
  "inscriptos": "InscriptosDto[]"
}
```

#### `InscriptosDto`

```json
{
  "id": "int",
  "nombreSocio": "string",
  "dniSocio": "string",
  "telefonoSocio": "string",
  "montoAbonado": "decimal",
  "montoPendiente": "decimal",
  "cancelado": "bool"
}
```

#### `ComboBoxViajes`

```json
{ "idViaje": "int", "nombreViaje": "string" }
```

#### `ComboBoxVariantesViaje`

```json
{ "idVariante": "int", "nombreVariante": "string" }
```

---

## 2. Router — `frontend/src/router/index.js`

Agregar 2 rutas nuevas después de la ruta de `/analiticas` (línea 109 aprox):

```js
import ViajesDashboard from "@/views/ModuloGestionViajes/ViajesDashboard.vue";
import ViajeDetailView from "@/views/ModuloGestionViajes/ViajeDetailView.vue";
```

```js
{
  path: '/viajes',
  name: 'viajes',
  component: ViajesDashboard,
  meta: { module: 'viajes', headerTitle: 'Gestión de Viajes', headerDescription: 'Organización de excursiones y turismo', requiresAuth: true }
},
{
  path: '/viajes/:id',
  name: 'viaje-detail',
  component: ViajeDetailView,
  meta: { module: 'viajes', headerTitle: 'Detalle de Viaje', requiresAuth: true }
},
```

---

## 3. Vistas — `frontend/src/views/ModuloGestionViajes/`

### 3.1 `ViajesDashboard.vue` (vista principal del módulo)

**Patrón visual:** Idéntico a `SociosDashboard.vue`. Pantalla con breadcrumb + título + tarjetas de acción + área de contenido dinámico.

**Tarjetas de acción (3):**

1. **"Crear viaje"** — Abre un modal (`ViajeFormModal`) para crear un viaje base nuevo.
2. **"Ver viajes disponibles"** — Muestra la lista de viajes disponibles en **formato de tarjetas** (NO tabla). Cada tarjeta tiene botones para "Crear variante", "Ver variantes" (desplegable) y "Ver detalle completo".
3. **"Inscribir socio a viaje"** — Muestra un input para buscar socio por DNI. Si se encuentra, se muestra un desplegable con los viajes disponibles. Al seleccionar un viaje, se abre un modal con las variantes de ese viaje para elegir y confirmar la inscripción con el monto abonado.

**Área de contenido dinámico (`currentAction`):**

- `'create'` → Mensaje indicando que el modal está abierto + botón para reabrirlo.
- `'list'` → **Grid de tarjetas de viajes** (componente `ViajeCard`). Cada tarjeta muestra: título, días/noches, fecha de salida, valor base, % comisión. Incluye:
  - Botón **"Ver variantes"** → al hacer clic, despliega/colapsa una sección inline debajo del card mostrando la lista de variantes del viaje (llamar `viajesService.listarVariantesDeViaje(idViaje)`).
  - Botón **"Crear variante"** → abre el modal `VarianteFormModal` con el `idViaje` correspondiente.
  - Botón **"Ver detalle completo"** → navega a `/viajes/:id`.
- `'inscribir'` → Formulario de inscripción paso a paso (ver sección 4.3).

**Modales que usa:**

- `ViajeFormModal` (crear viaje base)
- `VarianteFormModal` (crear variante desde la tarjeta de un viaje)
- `ConfirmModal` (confirmaciones, importar de `../../components/Common/ConfirmModal.vue`)

**Estado y lógica:**

- `currentAction: ref('none')` — controla qué sección se muestra.
- `expandedViajeId: ref(null)` — controla cuál viaje tiene el desplegable de variantes abierto.
- `variantes: ref([])` — variantes del viaje expandido.
- `toast: ref({ show, message, type })` — notificaciones toast (copiar el patrón exacto de SociosDashboard).
- Al seleccionar "Ver viajes disponibles", llamar `viajesService.listarViajesDisponibles()` y mostrar los resultados en tarjetas.
- Al hacer clic en "Ver variantes" de una tarjeta, llamar `viajesService.listarVariantesDeViaje(viaje.id)` y mostrar las variantes en la sección expandible de esa tarjeta. Si ya está abierto, colapsar.
- Al hacer clic en "Crear variante", abrir `VarianteFormModal` con el `idViaje` de esa tarjeta. Al guardar, recargar la lista y las variantes.
- Al hacer clic en "Ver detalle completo", navegar a `/viajes/${viaje.id}`.

---

### 3.2 `ViajeDetailView.vue` (vista de detalle de un viaje)

**Ruta:** `/viajes/:id` → obtiene el id del parámetro de ruta.

**Al montar:** Llamar `viajesService.verViajeCompleto(id)` y mostrar toda la información.

**Layout (de arriba a abajo):**

1. **Breadcrumb:** Inicio > Gestión de Viajes > {titulo del viaje}
2. **Header con info general** del viaje: título, días/noches, fecha de salida, valor base.
3. **Panel de estadísticas** (cards numéricas en una fila):
   - Total inscriptos
   - Total cancelados
   - Total recaudado (formato moneda $)
   - Total pendiente (formato moneda $)
   - Monto comisión (formato moneda $)
   - Monto para agencia (formato moneda $)
   - Total liberados
4. **Botones de acción:**
   - **"Agregar variante"** → Abre modal `VarianteFormModal`.
   - **"Inscribir socio"** → Abre modal/sección `InscripcionFormModal`.
5. **Sección de variantes** (acordeón o tabs): Para cada variante del viaje, mostrar:
   - Nombre variante, valor viaje, valor seña, régimen (texto legible: "Media Pensión" / "Pensión Completa"), tipo de butaca.
   - **Tabla de inscriptos** de esa variante con columnas:
     - Nombre socio
     - DNI
     - Teléfono
     - Monto abonado ($)
     - Monto pendiente ($)
     - Estado (badge: "Activo" verde / "Cancelado" rojo)
     - Acciones: **"Registrar pago"** (abre modal) y **"Cancelar inscripción"** (con confirmación).

**Modales que usa:**

- `VarianteFormModal` — Formulario para crear variante.
- `InscripcionFormModal` — Formulario para inscribir socio a una variante de este viaje.
- `PagoViajeModal` — Formulario para registrar un pago parcial (solo campo monto, muestra saldo pendiente).
- `ConfirmModal` — Para confirmar cancelaciones.

**Lógica de acciones:**

- **Registrar pago:** Llama `actualizarPago({ idInscripto, montoAbonado })`. Al completar, recargar el viaje completo.
- **Cancelar inscripción:** Llama `cancelarInscripcion(idInscripto)`. Al completar, recargar el viaje completo.
- **Agregar variante:** Llama `createVarianteViaje(dto)`. Al completar, recargar el viaje completo.
- **Inscribir socio:** Llama `inscribirSocio(dto)`. Al completar, recargar el viaje completo.

---

## 4. Componentes — `frontend/src/components/ModuloGestionViajes/`

### 4.1 `ViajeFormModal.vue`

**Props:** `isOpen: Boolean`
**Emits:** `close`, `save`

**Formulario con campos:**

| Campo                | Tipo     | Validación                 |
| -------------------- | -------- | -------------------------- |
| Título               | `text`   | Requerido                  |
| Días                 | `number` | Requerido, 1-365           |
| Noches               | `number` | Requerido, 1-365           |
| Fecha de salida      | `date`   | Requerido, debe ser futura |
| Ventas para liberado | `number` | Opcional                   |
| Valor base ($)       | `number` | Requerido, > 0             |
| % Comisión           | `number` | Requerido, > 0             |

**Al enviar:** Llamar `viajesService.createViaje(dto)`, emitir `save` en éxito, mostrar errores del backend.

**Estética:** Modal con overlay `bg-slate-900/40 backdrop-blur-sm`, card blanca `rounded-2xl`, botones primarios `bg-teal-600 hover:bg-teal-700`, secundarios con `ring-1 ring-slate-200`. Seguir exactamente el patrón de `SocioFormModal.vue`.

---

### 4.2 `VarianteFormModal.vue`

**Props:** `isOpen: Boolean`, `idViaje: Number`
**Emits:** `close`, `save`

**Formulario con campos:**

| Campo                | Tipo     | Validación                                                           |
| -------------------- | -------- | -------------------------------------------------------------------- |
| Nombre variante      | `text`   | Requerido                                                            |
| Valor del viaje ($)  | `number` | Requerido                                                            |
| Valor de la seña ($) | `number` | Requerido, > 0, ≤ valor viaje                                        |
| Régimen              | `select` | Requerido. Opciones: `{ 0: 'Media Pensión', 1: 'Pensión Completa' }` |
| Tipo de butaca       | `text`   | Requerido                                                            |

**Al enviar:** Llamar `viajesService.createVarianteViaje({ idViaje, ...formData })`.

---

### 4.3 Flujo de inscripción de socio a viaje

Este flujo se implementa **directamente en `ViajesDashboard.vue`** dentro del área de contenido dinámico cuando `currentAction === 'inscribir'`. No es un modal, sino una sección inline con pasos progresivos.

**Flujo paso a paso (wizard inline):**

**Paso 1 — Buscar socio por DNI:**

- Input de texto + botón "Buscar".
- Al buscar, llamar `SociosService.getByDni(dni)`.
- Si se encuentra, mostrar nombre y apellido del socio como confirmación visual (card pequeña con los datos del socio).
- Si no se encuentra, mostrar mensaje de error.

**Paso 2 — Seleccionar viaje (solo si el socio fue encontrado):**

- Se despliega un dropdown/select con los viajes disponibles (llamar `viajesService.getComboBoxViajes()`).
- Al seleccionar un viaje, se avanza al paso 3.

**Paso 3 — Seleccionar variante y confirmar (modal):**

- Se abre un modal (`InscripcionConfirmModal.vue`) que muestra:
  - Info del socio (nombre, DNI) — read-only.
  - Lista de variantes del viaje seleccionado (llamar `viajesService.listarVariantesDeViaje(idViaje)`).
  - Cada variante muestra: nombre, valor viaje ($), valor seña ($), régimen, tipo de butaca.
  - El usuario selecciona una variante (radio button o tarjeta clickeable).
  - Campo **"Monto a abonar"** (`number`, requerido, debe ser >= valor seña de la variante seleccionada y <= valor viaje).
  - Botón **"Confirmar inscripción"**.
- Al confirmar: `viajesService.inscribirSocio({ viajeVarianteId, socioId, montoAbonado })`.

> [!IMPORTANT]
> Para buscar el socio por DNI, importar el servicio existente: `import SociosService from '../../services/SociosService'`.

#### Componente auxiliar: `InscripcionConfirmModal.vue`

**Props:** `isOpen: Boolean`, `socio: Object` (datos del socio encontrado), `idViaje: Number`
**Emits:** `close`, `save`

- Al montarse/abrirse, carga las variantes del viaje con `viajesService.listarVariantesDeViaje(idViaje)`.
- Muestra las variantes como tarjetas seleccionables.
- Al seleccionar una variante, muestra el campo de monto a abonar (con mínimo = valor seña, máximo = valor viaje).
- Al confirmar, llama `viajesService.inscribirSocio()` y emite `save`.

---

### 4.4 `PagoViajeModal.vue`

**Props:** `isOpen: Boolean`, `inscripto: Object` (el objeto `InscriptosDto` con id, nombreSocio, montoPendiente, montoAbonado)
**Emits:** `close`, `save`

**Formulario:**

- Muestra info read-only: nombre del socio, monto ya abonado, saldo pendiente.
- Campo editable: **Monto a abonar** (`number`, requerido, > 0, ≤ saldo pendiente).
- Al enviar: `viajesService.actualizarPago({ idInscripto: inscripto.id, montoAbonado })`.

---

### 4.5 `ViajeCard.vue` (componente de tarjeta de viaje — OBLIGATORIO)

**Props:** `viaje: Object` (PreviewViajeDto), `isExpanded: Boolean`, `variantes: Array` (PreviewVarianteViajeDto[])
**Emits:** `view`, `toggle-variantes`, `create-variante`

Card individual para mostrar un viaje en la lista. **Formato tarjeta, NO tabla.**

**Contenido principal de la tarjeta:**

- Título (h3 bold)
- Fecha de salida (con icono de calendario)
- Días/Noches (ej: "5 días / 4 noches")
- Valor base ($)
- % Comisión
- Ventas para liberado (si tiene valor)

**Botones de acción (en una fila al pie de la tarjeta):**

- **"Ver variantes"** (toggle) → emite `toggle-variantes`. Muestra una flecha/chevron que rota al expandir.
- **"Crear variante"** → emite `create-variante`.
- **"Ver detalle completo"** → emite `view`.

**Sección expandible de variantes (debajo de la tarjeta, visible solo si `isExpanded`):**

- Si `variantes` está vacío: mensaje "Este viaje aún no tiene variantes".
- Si tiene variantes: lista/tabla compacta mostrando para cada una:
  - Nombre variante
  - Valor viaje ($)
  - Valor seña ($)
  - Régimen (texto: "Media Pensión" / "Pensión Completa")
  - Tipo de butaca
- Animación suave de apertura/cierre (transition `max-height` o `v-show` con transición).

Estética: Card blanca con borde `border-slate-200`, hover effect sutil (`hover:shadow-md`), botones primarios `bg-teal-600`.

---

## 5. Formato de Moneda y Fechas

- **Moneda:** Usar formato argentino → `$XX.XXX,XX`. Se puede hacer con:
  ```js
  new Intl.NumberFormat("es-AR", { style: "currency", currency: "ARS" }).format(
    valor,
  );
  ```
- **Fechas:** El backend retorna `DateOnly` como `"YYYY-MM-DD"`. Mostrar como `DD/MM/YYYY`:
  ```js
  new Date(dateString + "T00:00:00").toLocaleDateString("es-AR");
  ```
- **Régimen:** Mapear el int a texto legible:
  ```js
  const regimenLabels = { 0: "Media Pensión", 1: "Pensión Completa" };
  ```

---

## 6. Enum `RegimenViaje`

El backend usa un enum C# con valores:

- `0` = `MediaPension` → Mostrar como **"Media Pensión"**
- `1` = `PensionCompleta` → Mostrar como **"Pensión Completa"**

En el frontend, al enviar `CreateVarianteViajeDto`, el campo `regimen` debe ser el **número entero** (0 o 1).

---

## 7. Orden de implementación sugerido

1. `viajesService.js` — Servicio completo con todos los métodos.
2. `router/index.js` — Agregar las 2 rutas.
3. `ViajeFormModal.vue` — Modal de creación de viaje base.
4. `VarianteFormModal.vue` — Modal de creación de variante.
5. `ViajeCard.vue` — Tarjeta de viaje con desplegable de variantes.
6. `ViajesDashboard.vue` — Vista principal con acciones: crear viaje, listar viajes (con cards expandibles), inscribir socio (wizard DNI-first).
7. `InscripcionConfirmModal.vue` — Modal de confirmación de inscripción con selección de variante.
8. `PagoViajeModal.vue` — Modal de registro de pago.
9. `ViajeDetailView.vue` — Vista de detalle completo con variantes, inscriptos, estadísticas financieras y acciones.

---

## 8. Reglas generales

1. **Todos los archivos Vue** deben usar `<script setup>` (Composition API).
2. **Importar servicio** siempre como: `import ViajesService from '../../services/viajesService'` (ojo: el archivo empieza con minúscula `viajesService.js`).
3. **Toast notifications** deben seguir el patrón exacto de `SociosDashboard.vue` (no crear componente aparte, incluir el bloque de toast en cada vista que lo necesite).
4. **Loading states** usar el spinner SVG animado inline como en `SociosDashboard.vue`, o el componente `LoadingOverlay` de Common (importar de `../../components/Common/LoadingOverlay.vue`, prop `show`).
5. **Modales** deben seguir el patrón visual de `ConfirmModal.vue`: overlay con `bg-slate-900/40 backdrop-blur-sm`, card centrada con `rounded-2xl`, z-index alto (`z-[60]`).
6. **No usar axios**, siempre `fetch` con `credentials: 'include'`.
7. **Paleta de colores para este módulo:** usar `teal` como color primario (consistente con el HomeView donde el módulo de viajes ya tiene `text-teal-600` y `bg-teal-50`).
8. **Los componentes del módulo** van en `frontend/src/components/ModuloGestionViajes/`.
9. **Las vistas del módulo** van en `frontend/src/views/ModuloGestionViajes/`.
