<script setup>
import { onMounted, reactive, ref } from 'vue'
import AlquilerService from '../../../services/AlquilerService'
import ArticuloService from '../../../services/ArticuloService'

const props = defineProps({
  isOpen: Boolean,
  alquilerId: Number,
})

const emit = defineEmits(['close', 'save'])

const form = reactive({
  itemId: '', // This binds to "ArticuloId" in logic but let's check DTO: AddItemToAlquilerDto has "ItemId" and "Cantidad"
  cantidad: 1,
})

const articulos = ref([])
const isSubmitting = ref(false)
const errorMessage = ref('')
const loadingArticulos = ref(false)

onMounted(async () => {
  loadingArticulos.value = true
  try {
    const result = await ArticuloService.getAll()
    articulos.value = result
  } catch (e) {
    console.error('Failed to load articles', e)
  } finally {
    loadingArticulos.value = false
  }
})

const handleSubmit = async () => {
  if (!form.itemId || form.cantidad < 1) {
    errorMessage.value = 'Seleccione un artículo y una cantidad válida.'
    return
  }

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    await AlquilerService.addItem(props.alquilerId, {
      ArticuloId: form.itemId,
      Cantidad: form.cantidad,
    })
    emit('save')
    form.itemId = ''
    form.cantidad = 1
  } catch (e) {
    errorMessage.value = e.message
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-50 overflow-y-auto"
    aria-labelledby="modal-title"
    role="dialog"
    aria-modal="true"
  >
    <div
      class="fixed inset-0 bg-slate-900/30 backdrop-blur-sm transition-opacity"
      @click="$emit('close')"
    ></div>

    <div class="flex min-h-full items-center justify-center p-4 text-center sm:p-0">
      <div
        class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg border border-slate-200"
      >
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4 border-b border-slate-100">
          <h3 class="text-lg font-semibold leading-6 text-slate-900">
            Agregar Artículo al Alquiler
          </h3>
        </div>

        <form @submit.prevent="handleSubmit">
          <div class="px-4 py-5 sm:p-6 space-y-4">
            <div v-if="errorMessage" class="p-3 rounded-md bg-red-50 text-red-700 text-sm mb-4">
              {{ errorMessage }}
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700">Artículo</label>
              <select
                v-model="form.itemId"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm px-3 py-2 border"
              >
                <option value="" disabled>Seleccione un artículo</option>
                <option v-for="art in articulos" :key="art.id" :value="art.id">
                  {{ art.nombre }} (${{ art.precioAlquiler }}/mes)
                </option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700">Cantidad</label>
              <input
                type="number"
                v-model="form.cantidad"
                min="1"
                required
                class="mt-1 block w-full rounded-md border-slate-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm px-3 py-2 border"
              />
            </div>
          </div>

          <div
            class="bg-slate-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6 border-t border-slate-200"
          >
            <button
              type="submit"
              :disabled="isSubmitting"
              class="inline-flex w-full justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 sm:ml-3 sm:w-auto disabled:opacity-50 transition-colors"
            >
              {{ isSubmitting ? 'Agregando...' : 'Agregar Item' }}
            </button>
            <button
              type="button"
              @click="$emit('close')"
              class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-slate-900 shadow-sm ring-1 ring-inset ring-slate-300 hover:bg-slate-50 sm:mt-0 sm:w-auto transition-colors"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>
