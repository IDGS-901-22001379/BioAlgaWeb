import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

interface MenuItem {
  key:
    | 'dashboard'
    | 'usuarios'
    | 'empleados'
    | 'proveedores'
    | 'productos'
    | 'clientes'
    | 'compras'
    | 'ventas'
    | 'pedidos'
    | 'inventario'
    | 'devoluciones';
  label: string;
  route: string;
  icon: string; // <- clase Bootstrap Icons (sin "bi " porque lo ponemos en la vista)
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css'],
})
export class SidebarComponent {
  constructor(private router: Router, public auth: AuthService) {}

  collapsed = signal(false);

  menu: MenuItem[] = [
    { key: 'dashboard',  label: 'Dashboard',    route: '/inicio/dashboard',         icon: 'bi-grid' },
    { key: 'usuarios',   label: 'Usuarios',     route: '/inicio/usuarios',          icon: 'bi-person-gear' },
    { key: 'clientes',   label: 'Clientes',     route: '/inicio/clientes',          icon: 'bi-people-fill' },
    { key: 'empleados',  label: 'Empleados',    route: '/inicio/empleados',         icon: 'bi-people' },
    { key: 'proveedores',label: 'Proveedores',  route: '/inicio/proveedores',       icon: 'bi-truck' },
    { key: 'productos',  label: 'Productos',    route: '/inicio/productos',         icon: 'bi-box-seam' },
    { key: 'compras',    label: 'Compras',      route: '/inicio/compras',           icon: 'bi-bag-check' },
    { key: 'ventas',     label: 'Ventas (POS)', route: '/inicio/ventas',            icon: 'bi-cash-coin' },
    { key: 'pedidos',    label: 'Pedidos',      route: '/inicio/pedidos',           icon: 'bi-clipboard-check' },
    { key: 'inventario', label: 'Inventario',   route: '/inicio/inventario/stock',  icon: 'bi-archive' },
    { key: 'devoluciones', label: 'Devoluciones', route: '/inicio/devoluciones',    icon: 'bi-arrow-return-left' },
  ];

  toggleCollapse() { this.collapsed.update(v => !v); }
  titleFor(m: MenuItem) { return this.collapsed() ? m.label : ''; }

  maybePromptLogin(m: MenuItem, ev: Event) {
    const requiere = [
      'usuarios','clientes','empleados','proveedores','productos','compras','ventas','pedidos','inventario','devoluciones'
    ].includes(m.key);
    if (!requiere) return;
    if (!this.auth.isLoggedIn) {
      ev.preventDefault();
      this.router.navigate(['/login'], { queryParams: { redirectTo: m.route } });
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  /** Cambia el tema del SIDEBAR: 'blue' | 'dark' | 'light' | 'teal' */
setSidebarTheme(theme: 'blue' | 'dark' | 'light' | 'teal') {
  const el = document.querySelector('.sidebar') as HTMLElement | null;
  if (!el) return;
  el.setAttribute('data-theme', theme);
  try { localStorage.setItem('sidebar-theme', theme); } catch {}
}

/** Restaura el último tema guardado (llámala cuando quieras) */
restoreSidebarTheme() {
  try {
    const t = (localStorage.getItem('sidebar-theme') as any) || 'blue';
    this.setSidebarTheme(t);
  } catch {
    this.setSidebarTheme('blue');
  }
}

/** Alterna cíclicamente entre temas (útil para probar) */
cycleSidebarTheme() {
  const el = document.querySelector('.sidebar') as HTMLElement | null;
  const current = (el?.getAttribute('data-theme') as any) || 'blue';
  const order = ['blue','dark','light','teal'] as const;
  const next = order[(order.indexOf(current as any) + 1) % order.length];
  this.setSidebarTheme(next);
}


}

