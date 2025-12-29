<template>
  <div class="max-w-xl mx-auto mt-12 animate-fadeIn">
    <div
      class="bg-white rounded-2xl shadow-lg border border-slate-100 overflow-hidden text-center pb-12"
    >
      <!-- Icon Header -->
      <div
        :class="[
          'p-10 flex flex-col items-center justify-center border-b border-slate-100',
          validationError.type === 'used'
            ? 'bg-blue-50/50'
            : validationError.type === 'expired'
              ? 'bg-amber-50/50'
              : 'bg-slate-50/50',
        ]"
      >
        <div
          class="w-24 h-24 bg-white rounded-full flex items-center justify-center shadow-sm mb-4"
        >
          <!-- Icono dinámico -->
          <svg
            v-if="validationError.type === 'used'"
            xmlns="http://www.w3.org/2000/svg"
            class="h-12 w-12 text-blue-500"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <svg
            v-else-if="validationError.type === 'expired'"
            xmlns="http://www.w3.org/2000/svg"
            class="h-12 w-12 text-amber-500"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
          <svg
            v-else
            xmlns="http://www.w3.org/2000/svg"
            class="h-12 w-12 text-slate-400"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M9.172 9.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
            />
          </svg>
        </div>
        <h3 class="text-2xl font-bold text-slate-800">{{ validationError.title }}</h3>
      </div>

      <!-- Content -->
      <div class="px-8 mt-8">
        <p class="text-slate-600 text-lg leading-relaxed">
          {{ validationError.message }}
        </p>
        <div
          :class="[
            'mt-6 p-5 rounded-xl inline-block border',
            validationError.type === 'used'
              ? 'bg-blue-50 border-blue-100 text-blue-800'
              : validationError.type === 'expired'
                ? 'bg-amber-50 border-amber-100 text-amber-800'
                : 'bg-slate-50 border-slate-100 text-slate-800',
          ]"
        >
          <p class="text-sm font-medium flex items-center gap-2 justify-center">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-5 w-5"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              />
            </svg>
            {{ validationError.subtext }}
          </p>
        </div>

        <div v-if="validationError.type !== 'used'" class="mt-8">
          <button
            @click="$emit('go-back')"
            class="px-8 py-3 bg-slate-100 text-slate-700 font-semibold rounded-lg hover:bg-slate-200 transition-all duration-300"
          >
            Volver
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'PaymentValidationError',
  props: {
    validationError: {
      type: Object,
      required: true,
    },
  },
}
</script>

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.5s ease-out;
}
</style>
