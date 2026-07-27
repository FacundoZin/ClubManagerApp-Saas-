import { computed } from 'vue'

export function usePeriodosDisponibles(periodosAdeudados) {
  const futurePeriods = computed(() => {
    const currentYear = new Date().getFullYear()

    const todosLosPeriodos = [
      { anio: currentYear, semestre: 1 },
      { anio: currentYear, semestre: 2 },
      { anio: currentYear + 1, semestre: 1 },
      { anio: currentYear + 1, semestre: 2 },
    ]

    const deudas = periodosAdeudados.value || []
    return todosLosPeriodos.filter(
      (p) => !deudas.some((d) => d.anio === p.anio && d.semestre === p.semestre),
    )
  })

  return { futurePeriods }
}
