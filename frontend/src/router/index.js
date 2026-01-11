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
      meta: { hideHeader: true }
    },
    {
      path: '/socios',
      name: 'socios',
      component: SociosDashboard,
      meta: { module: 'socios', headerTitle: 'Gestión de Socios', headerDescription: 'Administración del padrón' }
    },
    {
      path: '/socios/:id',
      name: 'socio-detail',
      component: SocioDetailView,
      meta: { hideHeader: true }
    },
    {
      path: '/reservas-salones',
      name: 'reservas-salones',
      component: AlquilerEspaciosDashboard,
      meta: { module: 'reservas', headerTitle: 'Gestión de Reservas', headerDescription: 'Calendario de espacios' }
    },
    {
      path: '/reservas',
      redirect: '/reservas-salones'
    },

    {
      path: '/pago-online',
      name: 'pago-online',
      component: OnlinePaymentView,
      meta: { hideHeader: true }
    },
    {
      path: '/pagos',
      name: 'pagos',
      component: GestionCuotasDashboard,
      meta: { module: 'pagos', headerTitle: 'Registrar Pagos', headerDescription: 'Ingreso rápido de cuotas' }
    },
    {
      path: '/alquiler-articulos',
      name: 'alquiler-articulos',
      component: AlquilerArticulosDashboard,
      meta: { module: 'articulos', headerTitle: 'Alquiler de Artículos', headerDescription: 'Ortopedia e inventario' }
    },
    {
      path: '/ortopedia/alquileres/:id',
      name: 'alquiler-detail',
      component: AlquilerArticuloDetailView,
      meta: { hideHeader: true }
    },
    {
      path: '/cobradores',
      name: 'cobradores',
      component: CobradorDashboard,
      meta: { module: 'cobradores', headerTitle: 'Módulo de Cobradores', headerDescription: 'Gestión de rutas y cobros' }
    },
  ],
})

export default router
