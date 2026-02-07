<script setup>
import { onMounted, ref } from 'vue'
import CobranzasService from '../../../services/CobranzasService'

const props = defineProps({
    showToast: {
        type: Function,
        required: true
    }
})

// Estado
const cobradores = ref([])
const loadingCobradores = ref(false)
const selectedCobrador = ref(null)
const historialData = ref(null)
const loadingHistorial = ref(false)
const period = ref({
    mes: new Date().getMonth() + 1,
    anio: new Date().getFullYear(),
})

const loadCobradores = async () => {
    loadingCobradores.value = true
    try {
        const data = await CobranzasService.verListadoDeCobradores()
        cobradores.value = data
    } catch (e) {
        props.showToast('Error al cargar cobradores: ' + e.message, 'error')
    } finally {
        loadingCobradores.value = false
    }
}

const loadHistorial = async () => {
    if (!selectedCobrador.value) return
    loadingHistorial.value = true
    try {
        const data = await CobranzasService.verHistorialDeCobradorByMes(
            selectedCobrador.value.idCobrador,
            period.value.mes,
            period.value.anio,
        )
        historialData.value = data
    } catch (e) {
        props.showToast('Error al cargar historial: ' + e.message, 'error')
    } finally {
        loadingHistorial.value = false
    }
}

const selectCobrador = (cobrador) => {
    selectedCobrador.value = cobrador
    loadHistorial()
}

const handlePeriodChange = () => {
    loadHistorial()
}

onMounted(() => {
    loadCobradores()
})
</script>

<template>
    <div class="animate-in fade-in duration-500">
        <div v-if="loadingCobradores || cobradores.length > 0"
            class="bg-white rounded-2xl shadow-sm border border-slate-200 overflow-hidden">
            <div class="grid grid-cols-1 lg:grid-cols-4 min-h-[400px] lg:min-h-[600px]">
                <!-- Lateral: Listado de Cobradores -->
                <div
                    class="lg:col-span-1 border-r border-slate-100 bg-slate-50/30 flex flex-col max-h-[300px] lg:max-h-none">
                    <div class="p-4 sm:p-5 border-b border-slate-100 bg-white/50">
                        <h3 class="font-bold text-slate-800 flex items-center gap-2">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-teal-600" viewBox="0 0 20 20"
                                fill="currentColor">
                                <path
                                    d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a8.113 8.113 0 00-4.03-7.062 8.054 8.054 0 00-4.193.684A4.6 4.6 0 003 14.113V17h9.93zM18.477 14.521c.384-2.625-1.455-5.147-4.135-5.607a7.977 7.977 0 00-3.328.096 8.039 8.039 0 013.355 5.8 8.016 8.016 0 01-.127 1.291h4.235v-.58z" />
                            </svg>
                            Cobradores
                        </h3>
                    </div>

                    <div class="flex-1 overflow-y-auto p-4 space-y-2">
                        <div v-if="loadingCobradores" class="flex justify-center py-8">
                            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-teal-600"></div>
                        </div>

                        <template v-else>
                            <button v-for="cobrador in cobradores" :key="cobrador.idCobrador"
                                @click="selectCobrador(cobrador)" :class="[
                                    selectedCobrador?.idCobrador === cobrador.idCobrador
                                        ? 'bg-teal-600 text-white shadow-md ring-1 ring-teal-600'
                                        : 'bg-white text-slate-600 hover:bg-slate-50 border border-slate-100',
                                    'w-full text-left px-4 py-3 rounded-xl font-semibold transition-all duration-200 flex items-center gap-3 group',
                                ]">
                                <div :class="[
                                    selectedCobrador?.idCobrador === cobrador.idCobrador
                                        ? 'bg-white/20'
                                        : 'bg-teal-50 text-teal-600 group-hover:bg-teal-100',
                                    'w-9 h-9 rounded-full flex items-center justify-center text-sm font-bold uppercase shrink-0',
                                ]">
                                    {{ cobrador.nombreCompleto.charAt(0) }}
                                </div>
                                <span class="truncate">{{ cobrador.nombreCompleto }}</span>
                            </button>
                        </template>
                    </div>
                </div>

                <!-- Principal: Historial -->
                <div class="lg:col-span-3 flex flex-col bg-white">
                    <div v-if="!selectedCobrador"
                        class="flex-1 flex flex-col items-center justify-center p-12 text-center text-slate-400">
                        <div class="w-20 h-20 bg-slate-50 rounded-full flex items-center justify-center mb-4">
                            <svg class="h-10 w-10 text-slate-300" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"
                                    d="M15 15l-2 5L9 9l11 4-5 2zm0 0l5 5M7.188 2.239l.777 2.897M5.136 7.965l-2.898-.777M13.95 4.05l-2.122 2.122m-5.657 5.656l-2.12 guest-2.122" />
                            </svg>
                        </div>
                        <h3 class="text-lg font-bold text-slate-800">Seleccione un cobrador</h3>
                        <p class="max-w-[250px] mx-auto mt-1">
                            Elija un cobrador de la lista lateral para visualizar su actividad mensual.
                        </p>
                    </div>

                    <div v-else class="flex flex-col h-full">
                        <!-- Header del Historial -->
                        <div
                            class="p-4 sm:p-6 border-b border-slate-100 bg-slate-50/50 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                            <div>
                                <h3 class="text-lg font-bold text-slate-900">
                                    Historial: {{ selectedCobrador.nombreCompleto }}
                                </h3>
                                <div class="flex items-center gap-3 mt-1">
                                    <span
                                        class="text-xs font-semibold px-2 py-0.5 bg-teal-100 text-teal-700 rounded-md">Total
                                        recaudado</span>
                                    <span class="text-sm font-bold text-slate-700">${{
                                        historialData?.montoTotalCobrado?.toLocaleString() || '0' }}</span>
                                </div>
                            </div>

                            <!-- Formulario Periodo Compacto -->
                            <div
                                class="flex items-center gap-2 bg-white p-1.5 rounded-xl border border-slate-200 shadow-sm">
                                <select v-model="period.mes" @change="handlePeriodChange"
                                    class="bg-transparent text-sm font-bold text-slate-600 border-none focus:ring-0 cursor-pointer pl-3">
                                    <option v-for="m in 12" :key="m" :value="m">
                                        {{ new Date(0, m - 1).toLocaleString('es-ES', { month: 'short' }) }}
                                    </option>
                                </select>
                                <div class="h-4 w-px bg-slate-200"></div>
                                <input type="number" v-model="period.anio" @change="handlePeriodChange"
                                    class="w-20 bg-transparent text-sm font-bold text-slate-600 border-none focus:ring-0 pr-3" />
                                <button @click="loadHistorial"
                                    class="bg-teal-600 text-white p-2 rounded-lg hover:bg-teal-700 transition-colors">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none"
                                        viewBox="0 0 24 24" stroke="currentColor">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                                    </svg>
                                </button>
                            </div>
                        </div>

                        <!-- Tabla Content -->
                        <div class="flex-1 overflow-x-auto">
                            <div v-if="loadingHistorial"
                                class="p-12 flex flex-col items-center border-b border-slate-50">
                                <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-teal-600"></div>
                                <p class="mt-4 text-sm text-slate-500 font-medium">Actualizando registros...</p>
                            </div>

                            <template v-else-if="historialData && historialData.cobrosRealizados.length > 0">
                                <table class="w-full text-left border-collapse">
                                    <thead>
                                        <tr class="bg-slate-50/50 border-b border-slate-100">
                                            <th
                                                class="px-6 py-4 text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                Fecha de Cobro</th>
                                            <th
                                                class="px-6 py-4 text-xs font-semibold text-slate-500 uppercase tracking-wider">
                                                Nombre del Socio</th>
                                            <th
                                                class="px-6 py-4 text-xs font-semibold text-slate-500 uppercase tracking-wider text-right">
                                                Monto</th>
                                        </tr>
                                    </thead>
                                    <tbody class="divide-y divide-slate-50">
                                        <tr v-for="(cobro, idx) in historialData.cobrosRealizados" :key="idx"
                                            class="hover:bg-slate-50/50 transition-colors">
                                            <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600">{{ new
                                                Date(cobro.fechaCobro).toLocaleDateString() }}</td>
                                            <td class="px-6 py-4 whitespace-nowrap">
                                                <div class="text-sm font-semibold text-slate-900">{{ cobro.nombreSocio
                                                    }}</div>
                                            </td>
                                            <td class="px-6 py-4 whitespace-nowrap text-right">
                                                <span
                                                    class="text-sm font-bold text-teal-600 bg-teal-50 px-3 py-1 rounded-lg">
                                                    ${{ cobro.montoCobrado.toLocaleString() }}
                                                </span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </template>

                            <div v-else class="p-16 text-center">
                                <div
                                    class="w-16 h-16 bg-slate-50 rounded-full flex items-center justify-center mx-auto mb-4">
                                    <svg class="h-8 w-8 text-slate-300" fill="none" viewBox="0 0 24 24"
                                        stroke="currentColor">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                                    </svg>
                                </div>
                                <h4 class="text-slate-800 font-bold">Sin registros</h4>
                                <p class="text-sm text-slate-500 mt-1">No se encontraron cobranzas registradas para este
                                    periodo.</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div v-else
            class="bg-white border border-dashed border-slate-300 rounded-2xl p-12 text-center animate-in zoom-in duration-300 shadow-sm">
            <div
                class="w-16 h-16 bg-slate-50 text-slate-300 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24"
                    stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5"
                        d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
            </div>
            <h3 class="text-xl font-bold text-slate-800 mb-2">No hay cobradores registrados</h3>
            <p class="text-slate-500 max-w-md mx-auto text-sm leading-relaxed">Todavía no se han registrado cobradores
                en el
                sistema.</p>
        </div>
    </div>
</template>
