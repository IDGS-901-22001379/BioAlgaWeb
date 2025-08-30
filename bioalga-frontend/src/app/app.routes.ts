import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

// Páginas (component/standalone)
import { LoginComponent } from './pages/login/login.component';
import { InicioComponent } from './pages/inicio/inicio.component';
import { UsuariosPageComponent } from './pages/usuarios/usuarios';
import { ClientesPageComponent } from './pages/clientes/clientes';
import { EmpleadosPageComponent } from './pages/empleados/empleados';
import { ProveedoresPageComponent } from './pages/proveedores/proveedores';
import { ProductosPageComponent } from './pages/productos/productos';

// ✅ NUEVOS (Compras + Inventario)
import { ComprasPageComponent } from './pages/compras/compras';
import { StockPageComponent } from './pages/inventario/stock/stock';

// ✅ NUEVO: Ventas (POS)
import { VentasPageComponent } from './pages/ventas/ventas';

// ✅ NUEVO: Devoluciones
import { DevolucionesPageComponent } from './pages/devoluciones/devoluciones';

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
          { path: 'stock', component: StockPageComponent },
        ]
      },

      // ✅ NUEVO: Ventas (POS)
      { path: 'ventas', component: VentasPageComponent },
      { path: 'pos', redirectTo: 'ventas' }, // alias cómodo

      // ✅ NUEVO: Devoluciones
      { path: 'devoluciones', component: DevolucionesPageComponent },
    ]
  },

  // Cualquier otra ruta → login
  { path: '**', redirectTo: 'login' }
];
