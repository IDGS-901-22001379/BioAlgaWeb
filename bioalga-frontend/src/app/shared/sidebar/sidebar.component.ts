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
    | 'ventas'      // ⬅️ NUEVO
    | 'inventario';
  label: string;
  route: string;
  svg: string;
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

  // Iconos (inline SVG)
  private icoDash = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><rect x="3" y="3" width="7" height="7" rx="1.5"/><rect x="14" y="3" width="7" height="7" rx="1.5"/><rect x="3" y="14" width="7" height="7" rx="1.5"/><rect x="14" y="14" width="7" height="7" rx="1.5"/></svg>`;
  private icoUser = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 4-6 8-6s8 2 8 6"/></svg>`;
  private icoTruck = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 7h10v7H3z"/><path d="M13 9h5l3 3v2h-8z"/><circle cx="7" cy="18" r="2"/><circle cx="17" cy="18" r="2"/></svg>`;
  private icoFactory = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 21V9l6 3V9l6 3V6l6 3v12z"/><path d="M6 21V9"/><path d="M9 21V12"/></svg>`;
  private icoBox = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M21 16V8l-9-5-9 5v8l9 5 9-5z"/><path d="M3.3 7.3 12 12l8.7-4.7"/></svg>`;
  private icoClientes = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="9" cy="7" r="4"/><circle cx="17" cy="7" r="4"/><path d="M2 21c0-4 3-7 7-7s7 3 7 7"/><path d="M12 21c0-3 2-5 5-5s5 2 5 5"/></svg>`;
  private icoCart = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="9" cy="20" r="1.8"/><circle cx="18" cy="20" r="1.8"/><path d="M3 4h2l2 12h11l2-8H7"/></svg>`;
  private icoInventory = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 7l9-4 9 4v10l-9 4-9-4z"/><path d="M3 7l9 4 9-4"/><path d="M12 11v10"/></svg>`;
  private icoPos = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><rect x="3" y="4" width="18" height="8" rx="2"/><path d="M7 12v6a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2v-6"/><path d="M9 16h6"/><path d="M8 8h.01M12 8h.01M16 8h.01"/></svg>`; // cajón POS

  menu: MenuItem[] = [
    { key: 'dashboard',  label: 'Dashboard',   route: '/inicio/dashboard',  svg: this.icoDash },
    { key: 'usuarios',   label: 'Usuarios',    route: '/inicio/usuarios',   svg: this.icoUser },
    { key: 'clientes',   label: 'Clientes',    route: '/inicio/clientes',   svg: this.icoClientes },
    { key: 'empleados',  label: 'Empleados',   route: '/inicio/empleados',  svg: this.icoFactory },
    { key: 'proveedores',label: 'Proveedores', route: '/inicio/proveedores',svg: this.icoTruck },
    { key: 'productos',  label: 'Productos',   route: '/inicio/productos',  svg: this.icoBox },

    // 👇 NUEVOS
    { key: 'compras',    label: 'Compras',     route: '/inicio/compras',            svg: this.icoCart },
    { key: 'ventas',     label: 'Ventas (POS)',route: '/inicio/ventas',             svg: this.icoPos },      // ⬅️ NUEVO
    { key: 'inventario', label: 'Inventario',  route: '/inicio/inventario/stock',   svg: this.icoInventory },
  ];

  toggleCollapse() { this.collapsed.update(v => !v); }

  titleFor(m: MenuItem) { return this.collapsed() ? m.label : ''; }

  maybePromptLogin(m: MenuItem, ev: Event) {
    const requiere = [
      'usuarios','clientes','empleados','proveedores','productos','compras','ventas','inventario'
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
}
