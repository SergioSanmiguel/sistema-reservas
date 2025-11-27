import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },

  {
    path: 'reservas',
    canActivate: [() => import('./core/auth/auth.guard').then(m => m.authGuard)],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/reservas/listar/reservas-listar.component')
            .then(m => m.ReservasListarComponent)
      },
      {
        path: 'crear',
        loadComponent: () =>
          import('./features/reservas/crear/reservas-crear.component')
            .then(m => m.ReservasCrearComponent)
      },
      {
        path: 'detalle/:id',
        loadComponent: () =>
          import('./features/reservas/detalle/reservas-detalle.component')
            .then(m => m.ReservasDetalleComponent)
      },
      {
        path: 'calendario/:espacioId',
        loadComponent: () =>
          import('./features/reservas/calendario/reservas-calendario.component')
            .then(m => m.ReservasCalendarioComponent)
      }
    ]
  },

  { path: '', redirectTo: 'reservas', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }
];
