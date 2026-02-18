import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import AuthService from '../services/AuthService'

// Importaciones existentes
import AlquilerArticuloDetailView from '../views/ModuloAlquilerDeArticulos/AlquilerArticuloDetailView.vue'
import AlquilerArticulosDashboard from '../views/ModuloAlquilerDeArticulos/AlquilerArticulosDashboard.vue'
import AlquilerEspaciosDashboard from '../views/ModuloAlquilerDeEspacios/AlquilerEspaciosDashboard.vue'
import GestionCuotasDashboard from '../views/ModuloGestionCuotas/GestionCuotasDashboard.vue'
import HistorialCuotasView from '../views/ModuloGestionCuotas/HistorialCuotasView.vue'
import SocioDetailView from '../views/ModuloGestionSocios/SocioDetailView.vue'
import SociosDashboard from '../views/ModuloGestionSocios/SociosDashboard.vue'
import CobradorDashboard from '@/views/ModuloCobradores/CobradorDashboard.vue'
import OnlinePaymentView from '@/views/OnlinePaymentView.vue'
import GestionUsuariosView from '@/views/ModuloGestionUsuarios/GestionUsuariosView.vue'
import DashboardAnaliticas from '@/views/ModuloAnaliticas-Balances/DashboardAnaliticas.vue'
import ViajesDashboard from "@/views/ModuloGestionViajes/ViajesDashboard.vue";
import ViajeDetailView from "@/views/ModuloGestionViajes/ViajeDetailView.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { hideHeader: true }
    },
    {
      path: '/',
      name: 'home',
      component: HomeView,
      meta: { requiresAuth: true }
    },
    {
      path: '/usuarios',
      name: 'usuarios',
      component: GestionUsuariosView,
      meta: { module: 'usuarios', headerTitle: 'Gestión de Usuarios', requiresSuperAdmin: true, requiresAuth: true }
    },
    {
      path: '/socios',
      name: 'socios',
      component: SociosDashboard,
      meta: { module: 'socios', headerTitle: 'Gestión de Socios', headerDescription: 'Administración del padrón', requiresAuth: true }
    },
    {
      path: '/socios/:id',
      name: 'socio-detail',
      component: SocioDetailView,
      meta: { module: 'socios', headerTitle: 'Detalle de Socio', requiresAuth: true }
    },
    {
      path: '/reservas-salones',
      name: 'reservas-salones',
      component: AlquilerEspaciosDashboard,
      meta: { module: 'reservas', headerTitle: 'Gestión de Reservas', headerDescription: 'Calendario de espacios', requiresAuth: true }
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
      meta: { module: 'pagos', headerTitle: 'Registrar Pagos', headerDescription: 'Ingreso rápido de cuotas', requiresAuth: true }
    },
    {
      path: '/cuotas',
      name: 'cuotas',
      component: GestionCuotasDashboard,
      meta: { module: 'pagos', headerTitle: 'Gestión de Cuotas', headerDescription: 'Administración de cuotas', requiresAuth: true }
    },
    {
      path: '/cuotas/historial',
      name: 'cuotas-historial',
      component: HistorialCuotasView,
      meta: { module: 'pagos', headerTitle: 'Historial de Cuotas', headerDescription: 'Consulta de pagos registrados', requiresAuth: true }
    },
    {
      path: '/alquiler-articulos',
      name: 'alquiler-articulos',
      component: AlquilerArticulosDashboard,
      meta: { module: 'articulos', headerTitle: 'Alquiler de Artículos', headerDescription: 'Ortopedia e inventario', requiresAuth: true }
    },
    {
      path: '/ortopedia/alquileres/:id',
      name: 'alquiler-detail',
      component: AlquilerArticuloDetailView,
      meta: { module: 'articulos', headerTitle: 'Detalle de Alquiler', requiresAuth: true }
    },
    {
      path: '/cobradores',
      name: 'cobradores',
      component: CobradorDashboard,
      meta: { module: 'cobradores', headerTitle: 'Módulo de Cobradores', headerDescription: 'Gestión de rutas y cobros', requiresAuth: true }
    },
    {
      path: '/analiticas',
      name: 'analiticas',
      component: DashboardAnaliticas,
      meta: { module: 'analiticas', headerTitle: 'Estadísticas y Balances', headerDescription: 'Análisis de socios e ingresos', requiresAuth: true }
    },
    {
      path: '/viajes',
      name: 'viajes',
      component: ViajesDashboard,
      meta: { module: 'viajes', headerTitle: 'Gestión de Viajes', headerDescription: 'Organización de excursiones y turismo', requiresAuth: true }
    },
    {
      path: '/viajes/:id',
      name: 'viaje-detail',
      component: ViajeDetailView,
      meta: { module: 'viajes', headerTitle: 'Detalle de Viaje', requiresAuth: true }
    },
  ],
})

router.beforeEach(async (to, from, next) => {
  // 1. Si ruta pública, permitir
  if (to.name === 'pago-online') return next();

  // 2. Si va a login
  if (to.name === 'login') {
     // Verificar si ya está logueado
     let isAuthenticated = AuthService.isAuthenticated.value;
     if (!isAuthenticated) {
        isAuthenticated = await AuthService.checkAuth();
     }
     
     if (isAuthenticated) return next({ name: 'home' });
     return next();
  }

  // 3. Verificar Autenticación para el resto de rutas (requiresAuth por defecto true si no se especifica o explicito)
  // Asumimos que todo lo demás requiere Auth salvo indicado lo contrario
  
  let isAuthenticated = AuthService.isAuthenticated.value;
  if (!isAuthenticated) {
      // Intentar recuperar sesión (cookies)
      isAuthenticated = await AuthService.checkAuth();
  }

  if (!isAuthenticated) {
      return next({ name: 'login' });
  }

  // 4. Verificar Roles
  if (to.meta.requiresSuperAdmin && !AuthService.isSuperAdmin()) {
      alert('Acceso denegado. Se requieren permisos de administrador.');
      return next({ name: 'home' });
  }

  next();
})

export default router
