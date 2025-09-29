import { Routes } from '@angular/router';

export const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    loadChildren: () => import('./login/login.routes').then(m => m.LOGIN_ROUTES)
  },
  {
    path: 'register',
    loadChildren: () => import('./register/register.routes').then(m => m.REGISTER_ROUTES)
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
