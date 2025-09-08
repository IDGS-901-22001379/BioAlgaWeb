import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

// Páginas (component/standalone)
import { LoginComponent } from './pages/login/login.component';
import { InicioComponent } from './pages/inicio/inicio.component';

// Módulos principales
import { DashboardPageComponent } from './pages/dashboard/dashboard';
import { UsuariosPageComponent } from './pages/usuarios/usuarios';
import { ClientesPageComponent } from './pages/clientes/clientes';
import { EmpleadosPageComponent } from './pages/empleados/empleados';
import { ProveedoresPageComponent } from './pages/proveedores/proveedores';
import { ProductosPageComponent } from './pages/productos/productos';

// ✅ Compras + Inventario
import { ComprasPageComponent } from './pages/compras/compras';
import { StockPageComponent } from './pages/inventario/stock/stock';

// ✅ Ventas (POS)
import { VentasPageComponent } from './pages/ventas/ventas';

// ✅ Devoluciones
import { DevolucionesPageComponent } from './pages/devoluciones/devoluciones';

// ✅ Pedidos
import { PedidosPageComponent } from './pages/pedidos/pedidos';

export const routes: Routes = [
  // Ruta raíz → login
  { path: '', pathMatch: 'full', redirectTo: 'login' },

  // Login (público)
  { path: 'login', component: LoginComponent },

  // Área privada protegida con sidebar
  {
    path: 'inicio',
    component: InicioComponent,
    canActivate: [authGuard],
    children: [
      // Redirección por defecto dentro de inicio → Dashboard
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },

      // Dashboard
      { path: 'dashboard', component: DashboardPageComponent },

      // Usuarios / gestión
      { path: 'usuarios', component: UsuariosPageComponent },
      { path: 'clientes', component: ClientesPageComponent },
      { path: 'empleados', component: EmpleadosPageComponent },
      { path: 'proveedores', component: ProveedoresPageComponent },
      { path: 'productos', component: ProductosPageComponent },

      // Compras + Inventario
      { path: 'compras', component: ComprasPageComponent },
      {
        path: 'inventario',
        children: [
          { path: '', pathMatch: 'full', redirectTo: 'stock' },
          { path: 'stock', component: StockPageComponent },
        ]
      },

      // Ventas (POS)
      { path: 'ventas', component: VentasPageComponent },
      { path: 'pos', redirectTo: 'ventas' }, // alias

      // Devoluciones
      { path: 'devoluciones', component: DevolucionesPageComponent },

      // Pedidos
      { path: 'pedidos', component: PedidosPageComponent },
      // { path: 'ordenes', redirectTo: 'pedidos' }, // alias opcional
    ]
  },

  // Cualquier otra ruta → login
  { path: '**', redirectTo: 'login' }
];
