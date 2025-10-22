import { Routes } from '@angular/router';
import { MovimientosListaPage } from './lista/lista';

export const MOVIMIENTOS_ROUTES: Routes = [
  { path: '', redirectTo: 'lista', pathMatch: 'full' },
  { path: 'lista', component: MovimientosListaPage, title: 'Movimientos de caja' }
];

// (opcional) export default para facilitar importaciones
export default MOVIMIENTOS_ROUTES;
