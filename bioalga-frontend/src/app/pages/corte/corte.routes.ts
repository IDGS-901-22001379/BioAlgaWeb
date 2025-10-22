// src/app/pages/corte/corte.routes.ts
import { Routes } from '@angular/router';

// Standalone components
import { CorteTurnosPage } from './turnos/turnos';
import { CorteResumenPage } from './resumen/resumen';

export const CORTE_ROUTES: Routes = [
  {
    path: 'turnos',
    component: CorteTurnosPage,
    title: 'Corte de caja - Turnos',
  },
  {
    path: 'resumen',
    component: CorteResumenPage,
    title: 'Corte de caja - Resumen',
  },
  {
    // Permite abrir directamente un resumen por id de turno
    path: 'resumen/:idTurno',
    component: CorteResumenPage,
    title: 'Corte de caja - Resumen',
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'turnos',
  },
];
