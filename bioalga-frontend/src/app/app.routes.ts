import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

// Páginas
import { LoginComponent } from './pages/login/login.component';
import { InicioComponent } from './pages/inicio/inicio.component';
import { UsuariosPageComponent } from './pages/usuarios/usuarios';
import { ClientesPageComponent } from './pages/clientes/clientes';
// import { DashboardPageComponent } from './pages/dashboard/dashboard';
// import { EmpleadosPageComponent } from './pages/empleados/empleados';
// import { ProveedoresPageComponent } from './pages/proveedores/proveedores';
// import { ProductosPageComponent } from './pages/productos/productos';

export const routes: Routes = [
  // Ruta por defecto → login
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  // Login público
  { path: 'login', component: LoginComponent },

  // Inicio protegido por guard
  {
    path: 'inicio',
    component: InicioComponent,
    canActivate: [authGuard],
    children: [
      // Redirigir al dashboard por defecto
      { path: '', pathMatch: 'full', redirectTo: 'clientes' },

      // { path: 'dashboard', component: DashboardPageComponent },
      { path: 'usuarios', component: UsuariosPageComponent },
      { path: 'clientes', component: ClientesPageComponent },
      // { path: 'empleados', component: EmpleadosPageComponent },
      // { path: 'proveedores', component: ProveedoresPageComponent },
      // { path: 'productos', component: ProductosPageComponent },
    ]
  },

  // Cualquier otra ruta → login
  { path: '**', redirectTo: 'login' }
];
