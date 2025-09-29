import { Routes } from '@angular/router';
import { AuthGuard, GuestGuard } from './core';
import { AuthLayoutComponent, AdminLayoutComponent } from './shared/layouts';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/login' },

  // Authentication routes with AuthLayout
  {
    path: 'auth',
    component: AuthLayoutComponent,
    canActivate: [GuestGuard],
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },

  // Direct login route (shortcut)
  {
    path: 'login',
    component: AuthLayoutComponent,
    canActivate: [GuestGuard],
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },

  // All main app routes go through AdminLayout (requires login)
  {
    path: '',
    component: AdminLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', loadChildren: () => import('./features/welcome/welcome.routes').then(m => m.WELCOME_ROUTES) },
      { path: 'welcome', redirectTo: '/dashboard' },
      // Add other routes here as needed
      // { path: 'products', loadChildren: () => import('./features/products/products.routes').then(m => m.PRODUCTS_ROUTES) },
      // { path: 'users', loadChildren: () => import('./features/users/users.routes').then(m => m.USERS_ROUTES) },
    ]
  },

  { path: '**', redirectTo: '/login' }
];
