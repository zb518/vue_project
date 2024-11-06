import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    component: () => import('@/components/layouts/MainLayout.vue'),
    children: [
      { path: '', name: "home", component: () => import('@/views/HomeView.vue'), meta: { requiresAuth: true } },
      { path: '/about', component: () => import('@/views/AboutView.vue'), meta: { requiresAuth: false } },
      { path: '/:xxx(.*)*', name: 'ErrorView', component: () => import('@/views/ErrorView.vue'), meta: { requiresAuth: false } }
    ]
  },
  { path: "/login", name: "login", component: () => import("@/views/LoginView.vue") }
]
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

export default router

declare module 'vue-router' {
  interface RouteMeta {
    // is optional
    isAdmin?: boolean
    // must be declared by every route
    requiresAuth: boolean
  }
}

router.beforeEach((to, from) => {
  //const authstore = useAuthStore()
  const isAuthenticated = sessionStorage.getItem("AuthUser") !== null;
  // instead of having to check every route record with
  // to.matched.some(record => record.meta.requiresAuth)
  if (to.meta.requiresAuth && !isAuthenticated) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    return {
      path: '/login',
      // save the location we were at to come back later
      query: { redirect: to.fullPath },
    }
  }
})
