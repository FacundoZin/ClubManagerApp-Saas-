# Plan de Implementación: Modernización Visual del Módulo de Gestión de Cuotas

> **Objetivo:** Alinear visualmente el módulo de Gestión de Cuotas con la estética más moderna del módulo de Gestión de Viajes, adoptando su sistema de transiciones coordinadas, pill-navbar, action cards modernas, y selectores/inputs refinados.

> **Fecha:** 2026-02-18

---

## 🔍 Análisis Comparativo Detallado

### Estructura General del Dashboard (`GestionCuotasDashboard.vue`)

| Aspecto | Módulo Viajes ✅ (moderno) | Módulo Cuotas ❌ (actual) |
|---|---|---|
| **Flujo UX principal** | Selector de acciones → desaparece → aparece navbar + contenido (dos vistas alternas) | Cards de acciones **siempre visibles** arriba + contenido debajo (nunca se ocultan) |
| **Transiciones** | `<Transition mode="out-in">` con `opacity + translate-y` — fluida y coordinada | Sin ninguna transición — el contenido aparece abruptamente con `v-if` |
| **Navbar de navegación** | Pill-selector redondeado (`rounded-[2rem]`) con tab activo elevado (`shadow-md`, `scale-[1.02]`) + botón "Volver" | No existe — las cards siempre están arriba con `ring-2 ring-blue-500` en la activa |
| **Indicador de acción activa** | Tab activo en el pill-selector (color + sombra + escala) | `ring-2 ring-blue-500 border-transparent` en la card (anticuado) |

### Action Cards (Selector de Opciones)

| Aspecto | Módulo Viajes ✅ | Módulo Cuotas ❌ |
|---|---|---|
| **Border radius** | `rounded-3xl` (muy redondeado) | `rounded-xl` (rectangular) |
| **Ícono contenedor** | `p-4 rounded-2xl` + `group-hover:scale-110` | `p-3.5 rounded-lg` sin animación |
| **Tamaño del ícono** | `h-8 w-8` | `h-7 w-7` |
| **Hover en card** | `hover:shadow-xl hover:-translate-y-1` | `hover:shadow-lg hover:-translate-y-1` |
| **Hover en título** | `group-hover:text-teal-700` (color del módulo) | `group-hover:text-blue-700` (genérico) |

### Contenedor de Contenido Dinámico

| Aspecto | Módulo Viajes ✅ | Módulo Cuotas ❌ |
|---|---|---|
| **Border radius** | `rounded-3xl` | `rounded-xl` |
| **Overflow** | `overflow-hidden` | Sin overflow control |
| **Animaciones internas** | `animate-in fade-in zoom-in`, `slide-in-from-bottom-4` por sección | Sin animaciones internas |

### Inputs, Selects y Botones

| Aspecto | Módulo Viajes ✅ | Módulo Cuotas ❌ |
|---|---|---|
| **Input border radius** | `rounded-xl` | `rounded-md` |
| **Input padding** | `px-4 py-3` | `px-3 py-2` |
| **Input focus ring** | `focus:ring-2 focus:ring-blue-500` | `focus:ring-1 focus:ring-blue-500` |
| **Select border radius** | `rounded-xl` | `rounded-md` |
| **Botón border radius** | `rounded-xl` | `rounded-md` |
| **Botón padding** | `px-6 py-3` o `px-8 py-3` | `px-4 py-2` |
| **Botón peso de fuente** | `font-bold` | `font-medium` |
| **Botón sombra** | `shadow-lg` | `shadow-sm` o `shadow-md` |

### Sección de Pago (Resultados de Búsqueda)

| Aspecto | Módulo Viajes ✅ | Módulo Cuotas ❌ |
|---|---|---|
| **Contenedor de pasos** | `bg-slate-50 rounded-2xl border border-slate-200` con headers `uppercase tracking-wider` | `bg-slate-50 rounded-lg border border-slate-200` básico |
| **Resultado de socio** | Card con avatar, animación `animate-in fade-in slide-in-from-top-2` | Sin animación al aparecer |
| **Separación de secciones** | Headers estilizados con badges numerados (`Paso 1`, `Paso 2`) | Sin estructura de pasos |

### Vista de Historial (`HistorialCuotasView.vue`)

| Aspecto | Módulo Viajes ✅ | Módulo Cuotas ❌ |
|---|---|---|
| **Contenedor filtros** | N/A (no tiene filtros similares) | `rounded-xl` — aceptable pero puede mejorar |
| **Inputs de filtro** | `rounded-xl` con `py-3` | `rounded-md` con `py-2` |
| **Botón buscar** | `rounded-xl font-bold shadow-lg` | `rounded-md font-medium shadow-sm` |
| **Badges** | `rounded-full` con border sutil | `rounded-full` sin border (colores planos) |

---

## 🗂️ Archivos a Modificar

```
Frontend/src/views/ModuloGestionCuotas/
  ├── GestionCuotasDashboard.vue   ← CAMBIO PRINCIPAL (estructura completa del dashboard)
  └── HistorialCuotasView.vue      ← Modernizar filtros, inputs y badges

Frontend/src/components/ModuloGestionCuotas/
  └── SocioFeeCard.vue             ← Modernizar card (rounded-2xl, selectores de periodo, footer)
```

---

## 📐 Plan Detallado por Pasos

### PASO 1 — `GestionCuotasDashboard.vue`: Adoptar el patrón de transición coordinada

**Problema actual:** Las 3 action cards siempre están visibles arriba del contenido. Cuando el usuario selecciona "Registrar pago" o "Actualizar valor", aparece un contenedor debajo sin transición alguna. La card seleccionada tiene un `ring-2 ring-blue-500` que se ve anticuado.

**Particularidad:** La acción "Ver historial" (`history`) navega a otra ruta (`/cuotas/historial`) en vez de mostrar contenido inline. Esto significa que el pill-navbar solo tendrá 2 tabs funcionales (pay, update). La acción `history` seguirá navegando, pero se puede incluir como tab que redirija.

**Solución:** Replicar el patrón de dos vistas alternas de `ViajesDashboard.vue`:

```
Estado 'none'  →  Grid de 3 cards (con Transition out-in)
Estado activo  →  Pill-navbar + Content container (con Transition out-in)
```

**Cambios concretos:**

1. Envolver los dos bloques principales en `<Transition mode="out-in">`:
```html
<Transition mode="out-in" 
  enter-active-class="transition duration-400 ease-out"
  enter-from-class="opacity-0 translate-y-4" 
  enter-to-class="opacity-100 translate-y-0"
  leave-active-class="transition duration-300 ease-in" 
  leave-from-class="opacity-100 translate-y-0"
  leave-to-class="opacity-0 -translate-y-4">

  <!-- FIRST VIEW: Initial Action Selector -->
  <div v-if="currentAction === 'none'" key="selector" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 mb-10">
    ...3 cards...
  </div>

  <!-- SECOND VIEW: Active Module (Navbar + Content Container) -->
  <div v-else key="content-view">
    <!-- Pill Navbar -->
    ...
    <!-- Content Area -->
    ...
  </div>
</Transition>
```

2. Mover el bloque de action cards (líneas 227-275 actuales) dentro del `v-if="currentAction === 'none'"`.
3. Mover el content area (líneas 278-484 actuales) dentro del `v-else`.
4. Eliminar el `v-if="currentAction !== 'none'"` que controla el contenedor actual (línea 279).
5. Remover el `ring-2 ring-blue-500 border-transparent` condicional de las cards (línea 235).

---

### PASO 2 — `GestionCuotasDashboard.vue`: Implementar el Pill-Navbar

**Problema actual:** No hay navbar. El usuario debe desplazarse hacia arriba para ver las cards y cambiar de acción. No hay botón "Volver".

**Particularidad del módulo:** Este módulo tiene solo 2 acciones inline (`pay` y `update`) + 1 acción que navega a otra ruta (`history`). El pill-navbar debe manejar esto bien: las 2 acciones inline se muestran como tabs normales, y "Ver historial" debe funcionar como link/botón que navega.

**Solución:** Agregar el pill-navbar antes del contenedor de contenido:

```html
<div class="flex flex-col sm:flex-row items-center gap-4 mb-8">
  <div class="flex flex-wrap items-center gap-2 p-1.5 bg-slate-200/50 rounded-[2rem] w-full sm:w-auto border border-slate-200/50 backdrop-blur-sm shadow-inner">
    <!-- Solo tabs para acciones inline (pay, update) -->
    <button v-for="action in actions.filter(a => a.id !== 'history')" :key="action.id" 
      @click="selectAction(action.id)"
      class="flex-1 sm:flex-none flex items-center justify-center px-5 py-2.5 rounded-[1.5rem] transition-all duration-300 font-bold text-sm"
      :class="[
        currentAction === action.id
          ? 'bg-white text-emerald-600 shadow-md border border-slate-200 translate-y-[-1px] scale-[1.02]'
          : 'text-slate-500 hover:text-slate-700 hover:bg-white/40'
      ]">
      <svg class="w-4 h-4 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="action.icon" />
      </svg>
      {{ action.title }}
    </button>

    <!-- Tab separado para navegar al historial -->
    <button @click="selectAction('history')"
      class="flex-1 sm:flex-none flex items-center justify-center px-5 py-2.5 rounded-[1.5rem] transition-all duration-300 font-bold text-sm text-slate-500 hover:text-slate-700 hover:bg-white/40">
      <svg class="w-4 h-4 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
          d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
      </svg>
      Historial
      <!-- Indicador de link externo -->
      <svg class="w-3 h-3 ml-1 opacity-50" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14" />
      </svg>
    </button>
  </div>

  <button @click="currentAction = 'none'"
    class="hidden sm:flex items-center px-4 py-2 text-slate-400 hover:text-red-500 font-bold text-sm transition-colors group">
    <svg class="w-4 h-4 mr-2 group-hover:-translate-x-1 transition-transform" fill="none" viewBox="0 0 24 24"
      stroke="currentColor">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
    </svg>
    Volver
  </button>
</div>
```

**Nota de color:** Usar `text-emerald-600` como color primario del tab activo (ya que emerald es el color dominante del módulo de cuotas, asociado a dinero/pagos).

---

### PASO 3 — `GestionCuotasDashboard.vue`: Modernizar las Action Cards

**Problema actual:** `rounded-xl`, íconos de `h-7 w-7`, sin animación en hover del ícono.

**Cambios concretos en cada card:**

| Propiedad | Antes | Después |
|---|---|---|
| Card border-radius | `rounded-xl` | `rounded-3xl` |
| Card hover shadow | `hover:shadow-lg` | `hover:shadow-xl` |
| Ícono contenedor | `p-3.5 rounded-lg transition-colors` | `p-4 rounded-2xl transition-all group-hover:scale-110` |
| Ícono SVG | `h-7 w-7` | `h-8 w-8` |
| Hover título | `group-hover:text-blue-700` | `group-hover:text-emerald-700` (color del módulo) |

**Antes (línea 232):**
```html
<button class="group relative flex flex-col p-6 bg-white rounded-xl border border-slate-200 shadow-sm hover:shadow-lg hover:-translate-y-1 transition-all duration-300 text-left overflow-hidden">
```

**Después:**
```html
<button class="group relative flex flex-col p-6 bg-white rounded-3xl border border-slate-200 shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 text-left overflow-hidden">
```

**Antes (línea 244):**
```html
<div class="p-3.5 rounded-lg transition-colors duration-300 shadow-sm ring-1 ring-black/5" :class="[action.bg, action.color]">
```

**Después:**
```html
<div class="p-4 rounded-2xl transition-all duration-300 shadow-sm ring-1 ring-black/5 group-hover:scale-110" :class="[action.bg, action.color]">
```

**Antes (línea 249):**
```html
<svg class="h-7 w-7" ...>
```

**Después:**
```html
<svg class="h-8 w-8" ...>
```

---

### PASO 4 — `GestionCuotasDashboard.vue`: Modernizar el contenedor de contenido, inputs y formularios

**Problema actual:** Contenedor con `rounded-xl`, inputs con `rounded-md` y `py-2`, botones con `rounded-md`, sección de pago con `rounded-lg`.

#### 4.1 — Contenedor principal

**Antes (línea 280):**
```html
<div class="bg-white rounded-xl border border-slate-200 shadow-sm p-6 min-h-[400px]">
```

**Después:**
```html
<div class="bg-white rounded-3xl border border-slate-200 shadow-sm p-6 min-h-[400px] overflow-hidden">
```

#### 4.2 — Sección PAY: Input de búsqueda DNI

**Antes (líneas 304-311):**
```html
<input type="text" id="search-dni" v-model="searchDni" @keyup.enter="handleSearch"
  class="block w-full pl-10 pr-3 py-2 border border-slate-300 rounded-md bg-white focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500 sm:text-sm"
  placeholder="Ej: 12345678" />
```

**Después:**
```html
<input type="text" id="search-dni" v-model="searchDni" @keyup.enter="handleSearch"
  class="block w-full pl-10 pr-3 py-3 border border-slate-300 rounded-xl bg-white focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 sm:text-sm"
  placeholder="Ej: 12345678" />
```

#### 4.3 — Sección PAY: Botón "Buscar"

**Antes (línea 316):**
```html
<button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none disabled:opacity-50">
```

**Después:**
```html
<button class="inline-flex items-center px-6 py-3 border border-transparent text-sm font-bold rounded-xl shadow-lg text-white bg-emerald-600 hover:bg-emerald-700 focus:outline-none disabled:opacity-50 transition-all">
```

#### 4.4 — Sección PAY: Contenedor "Registrar Pago"

**Antes (línea 356):**
```html
<div class="mt-8 p-6 bg-slate-50 rounded-lg border border-slate-200">
```

**Después:**
```html
<div class="mt-8 p-6 bg-slate-50 rounded-2xl border border-slate-200">
```

#### 4.5 — Sección PAY: Select "Forma de Pago"

**Antes (línea 365):**
```html
<select class="block w-full py-2 px-3 border border-slate-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm">
```

**Después:**
```html
<select class="block w-full py-3 px-4 border border-slate-300 bg-white rounded-xl shadow-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 sm:text-sm">
```

#### 4.6 — Sección PAY: Botón "Confirmar Registro de Pago"

**Antes (línea 377):**
```html
<button class="w-full inline-flex justify-center items-center px-4 py-3 border border-transparent text-sm font-bold rounded-md shadow-md text-white bg-emerald-600 hover:bg-emerald-700 ...">
```

**Después:**
```html
<button class="w-full inline-flex justify-center items-center px-4 py-3 border border-transparent text-sm font-bold rounded-xl shadow-lg text-white bg-emerald-600 hover:bg-emerald-700 ...">
```

#### 4.7 — Sección UPDATE: Contenedor de información

**Antes (línea 408):**
```html
<div class="bg-blue-50 rounded-lg p-6 border border-blue-100 mb-8">
```

**Después:**
```html
<div class="bg-blue-50 rounded-2xl p-6 border border-blue-100 mb-8">
```

#### 4.8 — Sección UPDATE: Input "Nuevo Valor"

**Antes (línea 448):**
```html
<input type="number" class="block w-full pl-7 pr-3 py-2 border border-slate-300 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm" />
```

**Después:**
```html
<input type="number" class="block w-full pl-7 pr-3 py-3 border border-slate-300 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm" />
```

#### 4.9 — Sección UPDATE: Botón "Guardar Nuevo Valor"

**Antes (línea 457):**
```html
<button class="w-full inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 ...">
```

**Después:**
```html
<button class="w-full inline-flex justify-center items-center px-6 py-3 border border-transparent text-sm font-bold rounded-xl shadow-lg text-white bg-blue-600 hover:bg-blue-700 ...">
```

#### 4.10 — Animaciones internas por sección

Agregar clases de animación a cada sección cuando aparece:

- Sección `pay`: agregar `class="max-w-2xl mx-auto animate-in fade-in slide-in-from-bottom-4 duration-500"`
- Sección `update`: agregar `class="max-w-md mx-auto py-12 animate-in fade-in zoom-in duration-300"`

#### 4.11 — Agregar header de sección con título (como en Viajes)

Dentro de la sección `pay`, antes del input de búsqueda, agregar:
```html
<h3 class="text-xl font-bold text-slate-900 mb-6 flex items-center">
  <span class="flex items-center justify-center w-8 h-8 rounded-full bg-emerald-100 text-emerald-600 text-sm mr-3">1</span>
  Registrar Pago de Cuota
</h3>
```

---

### PASO 5 — `SocioFeeCard.vue`: Modernizar la card de socio

**Problema actual:** `rounded-lg`, badge de estado con colores planos (`bg-red-100/bg-green-100`), selectores de periodo con `rounded-md`, footer con botón minimalista.

**Cambios concretos:**

#### 5.1 — Card principal

**Antes (línea 73):**
```html
<div class="bg-white rounded-lg border border-slate-200 shadow-sm hover:shadow-md transition-shadow duration-200 overflow-hidden">
```

**Después:**
```html
<div class="bg-white rounded-2xl border border-slate-200 shadow-sm hover:shadow-md transition-all duration-300 overflow-hidden">
```

#### 5.2 — Badge de estado (Deudor / Al día)

**Antes (líneas 82-83):**
```html
<span class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
  :class="socio.adeudaCuotas ? 'bg-red-100 text-red-800' : 'bg-green-100 text-green-800'">
```

**Después:**
```html
<span class="inline-flex items-center rounded-full px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider border"
  :class="socio.adeudaCuotas ? 'bg-red-50 text-red-700 border-red-200' : 'bg-emerald-50 text-emerald-700 border-emerald-200'">
```

#### 5.3 — Selectores de periodo (checkboxes)

**Antes (línea 121):**
```html
<div class="flex items-center p-2 rounded-md border cursor-pointer transition-all text-sm"
  :class="isSelected(periodo) ? 'bg-indigo-50 border-indigo-200 text-indigo-700' : 'bg-slate-50 border-slate-200 text-slate-500'">
```

**Después:**
```html
<div class="flex items-center p-3 rounded-xl border cursor-pointer transition-all text-sm"
  :class="isSelected(periodo) ? 'bg-emerald-50 border-emerald-200 text-emerald-700 shadow-sm' : 'bg-slate-50 border-slate-200 text-slate-500 hover:border-slate-300'">
```

**Y los checkboxes (línea 131):**

**Antes:**
```html
<input type="checkbox" class="h-4 w-4 rounded border-slate-300 text-indigo-600 focus:ring-indigo-500 mr-2" />
```

**Después:**
```html
<input type="checkbox" class="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500 mr-2" />
```

> Aplicar el mismo cambio a los checkboxes de "Pago por adelantado" (líneas 150-163).

#### 5.4 — Footer de la card

**Antes (línea 196):**
```html
<div class="bg-slate-50 px-5 py-3 border-t border-slate-100 flex justify-end">
  <button class="text-xs font-medium text-slate-600 hover:text-blue-600 px-3 py-1 rounded hover:bg-slate-200 transition-colors">
    Ver legajo completo
  </button>
</div>
```

**Después:**
```html
<div class="bg-slate-50 px-5 py-3.5 border-t border-slate-100 flex justify-end">
  <button class="text-xs font-bold text-slate-600 hover:text-emerald-600 px-3 py-1.5 rounded-xl hover:bg-emerald-50 transition-all inline-flex items-center gap-1.5">
    <svg class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
        d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" 
        d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
    </svg>
    Ver legajo completo
  </button>
</div>
```

---

### PASO 6 — `HistorialCuotasView.vue`: Modernizar los filtros, inputs y badges

#### 6.1 — Contenedor de filtros

**Antes (línea 201):**
```html
<div class="bg-white rounded-xl border border-slate-200 shadow-sm p-6 mb-6">
```

**Después:**
```html
<div class="bg-white rounded-3xl border border-slate-200 shadow-sm p-6 mb-6">
```

#### 6.2 — Todos los selects e inputs de filtro

Aplicar globalmente a **todos** los inputs y selects de la sección de filtros:

**Antes:**
```html
<select class="block w-full py-2 px-3 border border-slate-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm">
<input class="block w-full py-2 px-3 border border-slate-300 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm" />
```

**Después:**
```html
<select class="block w-full py-3 px-4 border border-slate-300 bg-white rounded-xl shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm">
<input class="block w-full py-3 px-4 border border-slate-300 rounded-xl shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 sm:text-sm" />
```

Archivos/líneas afectadas:
- Select "Tipo de Filtro" (línea 210)
- Input "Fecha de Pago" (línea 223)
- Input "Año" (línea 236)
- Select "Semestre" (línea 243)

#### 6.3 — Botón "Buscar"

**Antes (línea 256):**
```html
<button class="w-full inline-flex justify-center items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none disabled:opacity-50">
```

**Después:**
```html
<button class="w-full inline-flex justify-center items-center px-6 py-3 border border-transparent text-sm font-bold rounded-xl shadow-lg text-white bg-blue-600 hover:bg-blue-700 focus:outline-none disabled:opacity-50 transition-all">
```

#### 6.4 — Badges de "Forma de Pago" en la tabla

**Antes (líneas 299-304):**
```html
<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
  :class="{
    'bg-blue-100 text-blue-800': item.formaDePago === 2,
    'bg-purple-100 text-purple-800': item.formaDePago === 1,
    'bg-orange-100 text-orange-800': item.formaDePago === 0,
  }">
```

**Después:**
```html
<span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-bold border"
  :class="{
    'bg-blue-50 text-blue-700 border-blue-200': item.formaDePago === 2,
    'bg-purple-50 text-purple-700 border-purple-200': item.formaDePago === 1,
    'bg-orange-50 text-orange-700 border-orange-200': item.formaDePago === 0,
  }">
```

---

## 🎨 Paleta de Colores a Mantener

El módulo de cuotas ya tiene una paleta coherente — se mantiene la identidad, solo se refinan los estilos:

| Acción | Color Principal | Se Mantiene |
|---|---|---|
| Registrar pago | `text-emerald-600` / `bg-emerald-50` | ✅ |
| Actualizar valor | `text-blue-600` / `bg-blue-50` | ✅ |
| Ver historial | `text-purple-600` / `bg-purple-50` | ✅ |
| Tab activo en navbar | — | Usar `text-emerald-600` (color primario del módulo) |

---

## ✅ Resumen de Prioridades

| Prioridad | Paso | Archivo | Cambio | Impacto Visual |
|---|---|---|---|---|
| 🔴 Alta | 1 | `GestionCuotasDashboard.vue` | Transición coordinada (Transition out-in) | 🔥 Muy alto — cambia el flujo UX completamente |
| 🔴 Alta | 2 | `GestionCuotasDashboard.vue` | Pill-navbar con tabs + botón Volver | 🔥 Muy alto — navegación moderna |
| 🟡 Media | 3 | `GestionCuotasDashboard.vue` | Modernizar action cards (rounded-3xl, scale-110, h-8) | Alto |
| 🟡 Media | 4 | `GestionCuotasDashboard.vue` | Modernizar contenedor, inputs, selects, botones + animaciones | Alto |
| 🟡 Media | 5 | `SocioFeeCard.vue` | Modernizar card, selectores de periodo, badge y footer | Medio-Alto |
| 🟢 Baja | 6 | `HistorialCuotasView.vue` | Modernizar filtros, inputs, botón buscar y badges | Medio |

---

## ⚠️ Notas Importantes

1. **La acción `history` navega a otra ruta** (`/cuotas/historial`), lo que significa que cuando se está en el dashboard con el pill-navbar visible, al clickear "Historial" el usuario sale de la vista actual. El pill-navbar debe mostrar este tab de forma diferenciada (ej: con un ícono de "link externo" ↗) para que el usuario sepa que va a navegar a otra página.

2. **El módulo tiene solo 2 acciones inline** (`pay` y `update`), lo que hace que el pill-navbar sea compacto y limpio. Esto es una ventaja visual respecto a módulos con más tabs.

3. **Los selectores de periodo en `SocioFeeCard.vue`** son un elemento interactivo clave del flujo de pago. Cambiar de `rounded-md` a `rounded-xl` y agregar `shadow-sm` al estado seleccionado los hace mucho más atractivos y táctiles, especialmente en mobile.

4. **El componente `DataTable.vue`** (usado en `HistorialCuotasView.vue`) es compartido (Common) — **no se debe modificar** para no afectar otros módulos. Los cambios visuales de la tabla se limitan a los templates de celdas (badges).

5. **El componente `LoadingOverlay.vue`** es compartido — no se modifica.

6. **Orden de implementación recomendado:** Pasos 1-2-3-4 juntos (son cambios en `GestionCuotasDashboard.vue`), luego 5 (`SocioFeeCard.vue`), luego 6 (`HistorialCuotasView.vue`) opcionalmente.

---

## 📎 Archivos de Referencia

- **Guía de estilo (fuente):** `Frontend/src/views/ModuloGestionViajes/ViajesDashboard.vue`
- **Card de referencia:** `Frontend/src/components/ModuloGestionViajes/ViajeCard.vue`
- **Dashboard a modificar:** `Frontend/src/views/ModuloGestionCuotas/GestionCuotasDashboard.vue`
- **Card a modernizar:** `Frontend/src/components/ModuloGestionCuotas/SocioFeeCard.vue`
- **Vista historial a modernizar:** `Frontend/src/views/ModuloGestionCuotas/HistorialCuotasView.vue`
- **Plan de referencia (mismo patrón):** `.agents/plan-modernizacion-modulo-reservas-espacios.md`
- **Plan de referencia (socios):** `.agents/plan-modernizacion-modulo-socios.md`
