# Plan de Implementación: Modernización Visual del Módulo de Gestión de Socios

> **Objetivo:** Alinear visualmente el módulo de Gestión de Socios con la estética más moderna del módulo de Gestión de Viajes, especialmente su selector de opciones (pill-navbar) y transiciones coordinadas.

---

## 🔍 Análisis Comparativo

### Lo que tiene Viajes (más moderno) vs. lo que tiene Socios (más antiguo)

| Aspecto | Módulo Viajes ✅ | Módulo Socios ❌ |
|---|---|---|
| **Selector de acciones** | `rounded-3xl`, íconos más grandes (`h-8 w-8`), `group-hover:scale-110` en ícono | `rounded-xl`, íconos más chicos (`h-7 w-7`), sin animación en ícono |
| **Transición al seleccionar** | `<Transition mode="out-in">` con `opacity-0 translate-y-4` — fluida y coordinada | Sin transición — el contenido aparece abruptamente |
| **Navbar de navegación** | Pill-selector redondeado (`rounded-[2rem]`) con tab activo elevado (`shadow-md`, `scale-[1.02]`, `translate-y-[-1px]`) + botón "Volver" | No existe — las cards siempre visibles arriba, el contenido aparece abajo sin contexto |
| **Área de contenido** | `rounded-3xl`, `shadow-sm`, contenido animado con `animate-in fade-in` | `rounded-xl`, sin animaciones internas |
| **Indicador de acción activa** | Tab activo en el pill-selector | `ring-2 ring-blue-500` en la card (menos elegante) |
| **Flujo UX** | Selector → desaparece → aparece navbar + contenido | Selector siempre visible + contenido debajo (layout roto) |

---

## 🗂️ Archivos a Modificar

```
Frontend/src/views/ModuloGestionSocios/
  └── SociosDashboard.vue       ← CAMBIO PRINCIPAL (estructura completa)

Frontend/src/components/ModuloGestionSocios/
  └── SocioCard.vue             ← Modernizar card (rounded-2xl, botones mejorados)
```

---

## 📐 Plan Detallado por Pasos

### PASO 1 — `SociosDashboard.vue`: Adoptar el patrón de transición coordinada de Viajes

**Problema actual:** Las action cards siempre están visibles arriba, y el contenido dinámico aparece debajo sin ninguna transición. Esto crea un layout confuso y visualmente pesado.

**Solución:** Replicar exactamente el patrón de `ViajesDashboard.vue`:

```
Estado 'none'  →  Grid de 3 cards (con Transition out-in)
Estado activo  →  Pill-navbar + Content container (con Transition out-in)
```

**Cambios concretos:**
- Envolver todo en `<Transition mode="out-in">` con `enter-from-class="opacity-0 translate-y-4"` y `leave-to-class="opacity-0 -translate-y-4"`
- Cuando `currentAction === 'none'`: mostrar solo el grid de cards (igual que viajes)
- Cuando `currentAction !== 'none'`: mostrar el **pill-navbar** + **content container**

**Referencia (ViajesDashboard.vue líneas 224-435):**
```html
<Transition mode="out-in" 
  enter-active-class="transition duration-400 ease-out"
  enter-from-class="opacity-0 translate-y-4" 
  enter-to-class="opacity-100 translate-y-0"
  leave-active-class="transition duration-300 ease-in" 
  leave-from-class="opacity-100 translate-y-0"
  leave-to-class="opacity-0 -translate-y-4">

  <!-- FIRST VIEW: Initial Action Selector -->
  <div v-if="currentAction === 'none'" key="selector" class="grid ...">
    ...cards...
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

---

### PASO 2 — `SociosDashboard.vue`: Implementar el Pill-Navbar de navegación

**Problema actual:** No hay forma de cambiar de acción sin volver a ver todas las cards. No hay botón "Volver".

**Solución:** Copiar el pill-navbar de Viajes adaptado a los colores del módulo de socios:

```html
<div class="flex flex-col sm:flex-row items-center gap-4 mb-8">
  <div class="flex flex-wrap items-center gap-2 p-1.5 bg-slate-200/50 rounded-[2rem] w-full sm:w-auto border border-slate-200/50 backdrop-blur-sm shadow-inner">
    <button v-for="action in actions" :key="action.id" @click="selectAction(action.id)"
      class="flex-1 sm:flex-none flex items-center justify-center px-5 py-2.5 rounded-[1.5rem] transition-all duration-300 font-bold text-sm"
      :class="[
        currentAction === action.id
          ? 'bg-white text-blue-600 shadow-md border border-slate-200 translate-y-[-1px] scale-[1.02]'
          : 'text-slate-500 hover:text-slate-700 hover:bg-white/40'
      ]">
      <svg class="w-4 h-4 mr-2" ...>
        <path :d="action.icon" />
      </svg>
      {{ action.title }}
    </button>
  </div>

  <button @click="currentAction = 'none'"
    class="hidden sm:flex items-center px-4 py-2 text-slate-400 hover:text-red-500 font-bold text-sm transition-colors group">
    <svg class="w-4 h-4 mr-2 group-hover:-translate-x-1 transition-transform" ...>
      <path d="M10 19l-7-7m0 0l7-7m-7 7h18" />
    </svg>
    Volver
  </button>
</div>
```

---

### PASO 3 — `SociosDashboard.vue`: Modernizar las Action Cards

**Problema actual:** `rounded-xl`, íconos sin animación.

**Cambios concretos:**
- `rounded-xl` → `rounded-3xl`
- `p-3.5 rounded-lg` del ícono → `p-4 rounded-2xl` + `group-hover:scale-110`
- `h-7 w-7` del ícono → `h-8 w-8`
- Cambiar `hover:shadow-lg` → `hover:shadow-xl`
- Remover el `ring-2 ring-blue-500` condicional de la card activa (ya no es necesario con el nuevo flujo)
- El título hover color ya está (`group-hover:text-blue-700`), mantener

**Antes:**
```html
<button class="group relative flex flex-col p-6 bg-white rounded-xl border ... hover:shadow-lg ...">
  <div class="p-3.5 rounded-lg ...">
    <svg class="h-7 w-7" ...>
```

**Después:**
```html
<button class="group relative flex flex-col p-6 bg-white rounded-3xl border ... hover:shadow-xl ...">
  <div class="p-4 rounded-2xl ... group-hover:scale-110">
    <svg class="h-8 w-8" ...>
```

---

### PASO 4 — `SociosDashboard.vue`: Modernizar el contenedor de contenido y agregar animaciones internas

**Problema actual:** El contenido de cada sección aparece sin animación, contenedor básico.

**Cambios concretos en el contenedor:**
- `rounded-xl` → `rounded-3xl`
- Agregar `overflow-hidden` al contenedor

**Cambios por sección:**
- Sección `add`: agregar `class="animate-in fade-in zoom-in duration-300"` al wrapper
- Sección `search`: agregar `class="animate-in fade-in slide-in-from-bottom-4 duration-500"` al wrapper
- Sección `debtors`: agregar `class="animate-in fade-in slide-in-from-bottom-4 duration-500"` al wrapper

**Mejorar los inputs de búsqueda (alinear con estilo Viajes):**
- `rounded-md` → `rounded-xl`
- `py-2` → `py-3`
- `focus:ring-1` → `focus:ring-2`
- Botón buscar: `rounded-md` → `rounded-xl` + `shadow-lg`

**Mejorar sección deudores:**
- Título `text-lg font-medium` → `text-xl font-bold`
- Botón actualizar: agregar ícono de refresh como en Viajes + `font-bold`

---

### PASO 5 — `SocioCard.vue`: Modernizar la card de socio

**Problema actual:** `rounded-lg`, botones de acción como texto plano sin mucha distinción visual.

**Cambios concretos:**
- `rounded-lg` → `rounded-2xl`
- `hover:shadow-md` → `hover:shadow-md` + `transition-all duration-300` (en vez de `transition-shadow duration-200`)
- Badge de estado: mantener estilo actual pero agregar un dot-indicator como en `SocioDetailView.vue`
- Footer de acciones: mejorar padding `py-3` → `py-3.5`, agregar iconos SVG pequeños junto al texto

**Antes (footer):**
```html
<div class="bg-slate-50 px-5 py-3 border-t border-slate-100 flex justify-end gap-2">
  <button class="text-xs ...">Ver info</button>
  <button class="text-xs ...">Editar</button>
  <button class="text-xs ...">Baja</button>
</div>
```

**Después (footer con íconos):**
```html
<div class="bg-slate-50 px-5 py-3.5 border-t border-slate-100 flex justify-end gap-2">
  <button class="text-xs font-bold ... inline-flex items-center gap-1">
    <svg class="w-3.5 h-3.5" ...><!-- ojo icon --></svg>
    Ver info
  </button>
  <button class="text-xs font-bold ... inline-flex items-center gap-1">
    <svg class="w-3.5 h-3.5" ...><!-- pencil icon --></svg>
    Editar
  </button>
  <button class="text-xs font-bold ... inline-flex items-center gap-1">
    <svg class="w-3.5 h-3.5" ...><!-- trash icon --></svg>
    Baja
  </button>
</div>
```

---

## 🎨 Paleta de Colores a Mantener

El módulo de socios usa **azul/índigo/rosa** — se mantiene esa identidad, solo se mejora la estructura visual:

| Acción | Color | Mantener |
|---|---|---|
| Agregar socio | `text-blue-600` / `bg-blue-50` | ✅ |
| Buscar por DNI | `text-indigo-600` / `bg-indigo-50` | ✅ |
| Ver deudores | `text-rose-600` / `bg-rose-50` | ✅ |
| Tab activo en navbar | — | Usar `text-blue-600` (color primario del módulo) |

---

## ✅ Resumen de Prioridades

| Prioridad | Paso | Cambio | Impacto Visual |
|---|---|---|---|
| 🔴 Alta | 1 | Transición coordinada (Transition out-in) | Muy alto — cambia el flujo UX completamente |
| 🔴 Alta | 2 | Pill-navbar con tabs | Muy alto — navegación moderna |
| 🟡 Media | 3 | Modernizar action cards (rounded-3xl, animaciones) | Alto |
| 🟡 Media | 4 | Animaciones internas + mejorar inputs | Medio |
| 🟢 Baja | 5 | Modernizar SocioCard.vue | Bajo-Medio |

---

## 📎 Archivos de Referencia

- **Guía de estilo (fuente):** `Frontend/src/views/ModuloGestionViajes/ViajesDashboard.vue`
- **Card de referencia:** `Frontend/src/components/ModuloGestionViajes/ViajeCard.vue`
- **Dashboard a modificar:** `Frontend/src/views/ModuloGestionSocios/SociosDashboard.vue`
- **Card a modernizar:** `Frontend/src/components/ModuloGestionSocios/SocioCard.vue`
