import CobradorDashboard from '@/views/ModuloCobradores/CobradorDashboard.vue'
import OnlinePaymentView from '@/views/OnlinePaymentView.vue'
import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import AlquilerArticuloDetailView from '../views/ModuloAlquilerDeArticulos/AlquilerArticuloDetailView.vue'
import AlquilerArticulosDashboard from '../views/ModuloAlquilerDeArticulos/AlquilerArticulosDashboard.vue'
import AlquilerEspaciosDashboard from '../views/ModuloAlquilerDeEspacios/AlquilerEspaciosDashboard.vue'
import GestionCuotasDashboard from '../views/ModuloGestionCuotas/GestionCuotasDashboard.vue'
import SocioDetailView from '../views/ModuloGestionSocios/SocioDetailView.vue'
import SociosDashboard from '../views/ModuloGestionSocios/SociosDashboard.vue'

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
      component: SociosDashboard,
    },
    {
      path: '/socios/:id',
      name: 'socio-detail',
      component: SocioDetailView,
    },
    {
      path: '/reservas-salones',
      name: 'reservas-salones',
      component: AlquilerEspaciosDashboard,
    },
    {
      path: '/reservas',
      redirect: '/reservas-salones'
    },

    {
      path: '/pago-online',
      name: 'pago-online',
      component: OnlinePaymentView,
    },
    {
      path: '/pagos',
      name: 'pagos',
      component: GestionCuotasDashboard,
    },
    {
      path: '/alquiler-articulos',
      name: 'alquiler-articulos',
      component: AlquilerArticulosDashboard,
    },
    {
      path: '/ortopedia/alquileres/:id',
      name: 'alquiler-detail',
      component: AlquilerArticuloDetailView,
    },
    {
      path: '/cobradores',
      name: 'cobradores',
      component: CobradorDashboard,
    },
  ],
})

export default router
