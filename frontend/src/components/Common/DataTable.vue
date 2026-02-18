<script setup>
import Pagination from './Pagination.vue'

const props = defineProps({
  headers: {
    type: Array, // Array of { label: string, key: string, class?: string, headerClass?: string, textRight?: boolean }
    required: true,
  },
  items: {
    type: Array,
    required: true,
  },
  isLoading: {
    type: Boolean,
    default: false,
  },
  emptyMessage: {
    type: String,
    default: 'No se encontraron resultados.',
  },
  showPagination: {
    type: Boolean,
    default: false,
  },
  paginationData: {
    type: Object, // { pageNumber: number, totalPages: number, totalCount: number, pageSize: number }
    default: () => ({}),
  },
})

defineEmits(['change-page', 'row-click'])
</script>

<template>
  <div class="bg-white rounded-xl border border-slate-200 shadow-sm overflow-hidden">
    <div class="overflow-x-auto">
      <table class="min-w-full divide-y divide-slate-200">
        <thead class="bg-slate-50">
          <tr>
            <th
              v-for="(header, index) in headers"
              :key="index"
              class="px-6 py-3 text-xs font-bold text-slate-500 uppercase tracking-wider border-r border-slate-200 last:border-r-0"
              :class="[header.textRight ? 'text-right' : 'text-left', header.headerClass || '']"
            >
              {{ header.label }}
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-slate-200">
          <!-- Loading State -->
          <tr v-if="isLoading">
            <td :colspan="headers.length" class="px-6 py-12 text-center">
              <div class="flex justify-center">
                <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
              </div>
            </td>
          </tr>

          <!-- Empty State -->
          <tr v-else-if="items.length === 0">
            <td :colspan="headers.length" class="px-6 py-12 text-center text-slate-500">
              <slot name="empty">
                <div class="flex flex-col items-center">
                  <svg
                    class="mx-auto h-12 w-12 text-slate-400 mb-3 opacity-20"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="1.5"
                      d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                    />
                  </svg>
                  <p class="text-sm font-medium italic">{{ emptyMessage }}</p>
                </div>
              </slot>
            </td>
          </tr>

          <!-- Rows -->
          <tr
            v-else
            v-for="(item, itemIndex) in items"
            :key="itemIndex"
            class="hover:bg-slate-50 transition-colors group"
            @click="$emit('row-click', item)"
          >
            <td
              v-for="(header, hIndex) in headers"
              :key="hIndex"
              class="px-6 py-4 whitespace-nowrap text-sm border-r border-slate-200 last:border-r-0"
              :class="[
                header.textRight ? 'text-right' : 'text-left',
                header.class || 'text-slate-600',
              ]"
            >
              <slot :name="`cell(${header.key})`" :item="item">
                {{ item[header.key] }}
              </slot>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Paginación -->
    <div
      v-if="showPagination && items.length > 0"
      class="px-6 py-3 border-t border-slate-100 bg-slate-50/30"
    >
      <Pagination
        :current-page="paginationData.pageNumber"
        :total-pages="paginationData.totalPages"
        :total-count="paginationData.totalCount"
        :page-size="paginationData.pageSize"
        @change-page="$emit('change-page', $event)"
      />
    </div>
  </div>
</template>

<style scoped>
/* Estilos opcionales para bordes de celdas en hover si se desea */
</style>
