import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

// Páginas (component/standalone)
import { LoginComponent } from './pages/login/login.component';
import { InicioComponent } from './pages/inicio/inicio.component';
import { UsuariosPageComponent } from './pages/usuarios/usuarios';
import { ClientesPageComponent } from './pages/clientes/clientes';
import { EmpleadosPageComponent } from './pages/empleados/empleados';
import { ProveedoresPageComponent } from './pages/proveedores/proveedores';
import { ProductosPageComponent } from './pages/productos/productos';   // ✅ ACTIVADO

// ✅ NUEVOS (Compras + Inventario)
import { ComprasPageComponent } from './pages/compras/compras';
import { StockPageComponent } from './pages/inventario/stock/stock';
// import { DashboardPageComponent } from './pages/dashboard/dashboard';

export const routes: Routes = [
  // Ruta por defecto → login
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  // Login (público)
  { path: 'login', component: LoginComponent },

  // Área privada protegida
  {
    path: 'inicio',
    component: InicioComponent,
    canActivate: [authGuard],
    children: [
      // Redirección por defecto dentro de inicio
      { path: '', pathMatch: 'full', redirectTo: 'clientes' },

      // { path: 'dashboard', component: DashboardPageComponent },
      { path: 'usuarios', component: UsuariosPageComponent },
      { path: 'clientes', component: ClientesPageComponent },
      { path: 'empleados', component: EmpleadosPageComponent },
      { path: 'proveedores', component: ProveedoresPageComponent },
      { path: 'productos', component: ProductosPageComponent },

      // ✅ NUEVAS RUTAS
      { path: 'compras', component: ComprasPageComponent },

      {
        path: 'inventario',
        children: [
          { path: '', pathMatch: 'full', redirectTo: 'stock' },
          { path: 'stock', component: StockPageComponent }, // Kardex / stock actual
        ]
      },
    ]
  },

  // Cualquier otra ruta → login
  { path: '**', redirectTo: 'login' }
];

