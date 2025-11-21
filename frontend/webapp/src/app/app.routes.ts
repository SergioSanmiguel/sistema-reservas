import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  // redirige al módulo de reservas
  {
    path: '',
    redirectTo: 'reservas',
    pathMatch: 'full'
  },
  // fallback: redirigir a login si ruta desconocida
  {
    path: '**',
    redirectTo: 'login'
  }
];
