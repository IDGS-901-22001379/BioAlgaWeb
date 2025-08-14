import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './pages/login/login.component';
import { InicioComponent } from './pages/inicio/inicio.component';
import { UsuariosPageComponent } from './pages/usuarios/usuarios';
//import { DashboardPageComponent } from './pages/dashboard/dashboard';
//import { EmpleadosPageComponent } from './pages/empleados/empleados';
//import { ProveedoresPageComponent } from './pages/proveedores/proveedores';
//import { ProductosPageComponent } from './pages/productos/productos';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  { path: 'login', component: LoginComponent },

  {
    path: 'inicio',
    component: InicioComponent,
    canActivate: [authGuard],
    children: [
      // Dashboard por defecto
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },

      //{ path: 'dashboard', component: DashboardPageComponent },
      { path: 'usuarios', component: UsuariosPageComponent },
      //{ path: 'empleados', component: EmpleadosPageComponent },
     // { path: 'proveedores', component: ProveedoresPageComponent },
     // { path: 'productos', component: ProductosPageComponent },
    ]
  },

  // Cualquier otra ruta → login
  { path: '**', redirectTo: 'login' }
];
