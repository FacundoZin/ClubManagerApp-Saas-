# Plan de Implementación: Mejoras Estéticas y de Usabilidad para Móviles (UI/UX)

## Objetivo

Implementar micro-mejoras visuales en el frontend de la aplicación para optimizar la experiencia de usuario (UX) y la visibilidad en dispositivos móviles. **Restricción estricta:** No realizar modificaciones en la lógica (bloques `<script>`), ni cambios drásticos en la estructura de los componentes (como rehacer grids complejos). Todas las modificaciones deben limitarse a clases utilitarias de CSS (Tailwind) enfocadas en la usabilidad móvil.

## 1. Prevención del "Auto-Zoom" y Legibilidad en Formularios (Inputs/Selects)

**Problema:** En dispositivos móviles (especialmente iOS), si un `<input>` o `<select>` tiene un tamaño de fuente menor a 16px, el navegador reacciona haciendo un zoom automático hacia el campo, rompiendo la vista del formulario y frustrando al usuario. Actualmente, los inputs tienen clase `sm:text-sm`, pero en móviles heredan `text-sm` (que es 14px en Tailwind por defecto).
**Tarea:**

- En todos los modales y formularios (ej. `SocioFormModal.vue`, `ViajeFormModal.vue`, `ReservaFormModal.vue`, `SocioUpdateModal.vue` y relacionados).
- Buscar los campos `<input>` y `<select>`, e ingresar o asegurar la clase `text-base` globalmente, y limitar el `text-sm` solo para pantallas medianas o grandes mediante `sm:text-sm`.
- Resultado esperado de la clase: `text-base sm:text-sm ...`

## 2. Áreas de Contacto (Touch Targets) en Tarjetas y Tablas

**Problema:** Los botones de "Editar", "Ver" o "Baja" en las tarjetas (como `SocioCard.vue` u otras implementaciones similares para viajes/lotes) tienen un tamaño extremadamente reducido (`text-[11px] px-2 py-1`). En un dispositivo móvil, es muy fácil equivocarse de botón (misclick) debido al bajo padding vertical.
**Tarea:**

- Ajustar las clases de estos botones (iconos + texto) para tener un toque más espacioso en móvil sin desarmar la vista en PC.
- Sugerencia de ajuste: pasar de `px-2 py-1` a `px-3 py-2 sm:px-2 sm:py-1`. Alternativamente, forzar un alto mínimo como `min-h-[32px] sm:min-h-0`.
- Esto aplica también para el gap de los botones del footer en tarjetas, pasar a `gap-2 sm:gap-1`.

## 3. Legibilidad y "Respiro" (Espaciado) en Tarjetas y Listas

**Problema:** Los datos condensados (DNI, Teléfono, Dirección) en componentes como `SocioCard.vue` pueden sentirse apretados (`space-y-1.5` fijo).
**Tarea:**

- Incrementar sutilmente el espaciado vertical de la información interna de las cards en versión mobile, y mantener la compacidad en escritorio.
- Ajuste propuesto: Cambiar `space-y-1.5` por un espaciado ligeramente mayor en mobile y el original en tablet/PC `space-y-2.5 sm:space-y-1.5`.
- Aumentar el tamaño de la fuente de la información secundaria de `text-xs` a `text-sm sm:text-xs` si el texto es muy poco legible bajo luz del sol.

## 4. Mejoras Visuales Mínimas en el Header (`AppHeader.vue`)

**Problema:** El subtítulo que indica en qué módulo está el usuario es extremadamente pequeño (`text-[10px]`).
**Tarea:**

- Modificar las clases para el texto secundario del header.
- Ajuste: `text-[11px] sm:text-[10px]` para mejorar la lectura inicial.

## 5. Padding de Botones de Confirmación (Modales)

**Problema:** Los botones principales de acción (ej. "Guardar Socio", "Crear Viaje", "Cancelar") suelen usar `py-2.5` igual para todas las resoluciones.
**Tarea:**

- Modificar los botones de acciones inferiores en la estructura del `<form>` de los Modales.
- Utilizar `py-3 sm:py-2.5` para garantizar un botón más "gordo" y fácil de presionar en las interfaces touch.

## Instrucciones Operativas para el Agente Programador

1. **Inspeccionar código fuente:** Buscar las ocurrencias en los `.vue` dentro de las carpetas de `src/components/Modulo...` y `src/components/Common`.
2. **Respetar el mandato:** No agregar `v-if`, no alterar variables reactivas, no cambiar nombres de funciones.
3. **Tailwind First:** Implementar las mejoras estrictamente reemplazando/añadiendo los modificadores responsivos de Tailwindcss (`sm:`, `md:`).
4. **Validación Visual:** Si un cambio propuesto en este documento resulta en un elemento visual que se sale de su caja grid (overflow), ajustarlo conservando la intención de la mejora de usabilidad.
