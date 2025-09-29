import { Routes } from '@angular/router';
import { AuthGuard, GuestGuard } from './core';
import { AuthLayoutComponent, AdminLayoutComponent } from './shared/layouts';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/dashboard' }, // CHANGED FOR TESTING - GO DIRECTLY TO DASHBOARD

  // Authentication routes with AuthLayout (AUTH DISABLED FOR TESTING)
  {
    path: 'auth',
    component: AuthLayoutComponent,
    // canActivate: [GuestGuard], // COMMENTED OUT FOR TESTING
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },

  // Direct login route (shortcut)
  {
    path: 'login',
    component: AuthLayoutComponent,
    // canActivate: [GuestGuard], // COMMENTED OUT FOR TESTING
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },

  // All main app routes go through AdminLayout (AUTH DISABLED FOR TESTING)
  {
    path: '',
    component: AdminLayoutComponent,
    // canActivate: [AuthGuard], // COMMENTED OUT FOR TESTING
    children: [
      { path: 'dashboard', loadChildren: () => import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES) },
      // Add other routes here as needed
      // { path: 'products', loadChildren: () => import('./features/products/products.routes').then(m => m.PRODUCTS_ROUTES) },
      // { path: 'users', loadChildren: () => import('./features/users/users.routes').then(m => m.USERS_ROUTES) },
    ]
  },

  { path: '**', redirectTo: '/dashboard' } // CHANGED FOR TESTING
];
