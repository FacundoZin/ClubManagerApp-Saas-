import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/socios',
      name: 'socios',
      component: () => import('../views/SociosView.vue'),
    },
    {
      path: '/socios/:id',
      name: 'socio-detail',
      component: () => import('../views/SocioDetailView.vue'),
    },
    {
      path: '/reservas-salones',
      name: 'reservas-salones',
      component: () => import('../views/ReservasSalonesView.vue'),
    },
    {
      path: '/reservas',
      redirect: '/reservas-salones'
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: '/pago-online',
      name: 'pago-online',
      component: () => import('../views/OnlinePaymentView.vue'),
    },
    {
      path: '/pagos',
      name: 'pagos',
      component: () => import('../views/CuotasView.vue'),
    },
    {
      path: '/alquiler-articulos',
      name: 'alquiler-articulos',
      component: () => import('../views/OrtopediaDashboardView.vue')
    },
    {
      path: '/ortopedia/alquileres/:id',
      name: 'alquiler-detail',
      component: () => import('../views/AlquilerDetailView.vue')
    },
  ],
})

export default router
