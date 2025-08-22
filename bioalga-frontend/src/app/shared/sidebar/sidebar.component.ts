import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';     // 👈 necesario para *ngIf / *ngFor
import { Router, RouterModule } from '@angular/router'; // 👈 necesario para routerLink / routerLinkActive
import { AuthService } from '../../services/auth.service';

interface MenuItem {
  key: 'dashboard' | 'usuarios' | 'empleados' | 'proveedores' | 'productos' | 'clientes';
  label: string;
  route: string;
  svg: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,                     // 👈 standalone
  imports: [CommonModule, RouterModule],// 👈 módulos que habilitan directivas
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css'],
})
export class SidebarComponent {
  constructor(private router: Router, public auth: AuthService) {}

  collapsed = signal(false);

  // Iconos
  private icoDash = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><rect x="3" y="3" width="7" height="7" rx="1.5"/><rect x="14" y="3" width="7" height="7" rx="1.5"/><rect x="3" y="14" width="7" height="7" rx="1.5"/><rect x="14" y="14" width="7" height="7" rx="1.5"/></svg>`;
  private icoUser = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 4-6 8-6s8 2 8 6"/></svg>`;
  private icoTruck = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 7h10v7H3z"/><path d="M13 9h5l3 3v2h-8z"/><circle cx="7" cy="18" r="2"/><circle cx="17" cy="18" r="2"/></svg>`;
  private icoFactory = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 21V9l6 3V9l6 3V6l6 3v12z"/><path d="M6 21V9"/><path d="M9 21V12"/></svg>`;
  private icoBox = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M21 16V8l-9-5-9 5v8l9 5 9-5z"/><path d="M3.3 7.3 12 12l8.7-4.7"/></svg>`;
  private icoClientes = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="9" cy="7" r="4"/><circle cx="17" cy="7" r="4"/><path d="M2 21c0-4 3-7 7-7s7 3 7 7"/><path d="M12 21c0-3 2-5 5-5s5 2 5 5"/></svg>`;

  menu: MenuItem[] = [
    { key: 'dashboard',  label: 'Dashboard',  route: '/inicio/dashboard',  svg: this.icoDash },
    { key: 'usuarios',   label: 'Usuarios',   route: '/inicio/usuarios',   svg: this.icoUser },
    { key: 'clientes',   label: 'Clientes',   route: '/inicio/clientes',   svg: this.icoClientes },
    { key: 'empleados',  label: 'Empleados',  route: '/inicio/empleados',  svg: this.icoFactory },
    { key: 'proveedores',label: 'Proveedores',route: '/inicio/proveedores',svg: this.icoTruck },
    { key: 'productos',  label: 'Productos',  route: '/inicio/productos',  svg: this.icoBox },
  ];

  toggleCollapse() { this.collapsed.update(v => !v); }

  maybePromptLogin(m: MenuItem, ev: Event) {
    // Protege también empleados (igual que usuarios/clientes)
    if (m.key !== 'usuarios' && m.key !== 'clientes' && m.key !== 'empleados') return;

    if (!this.auth.isLoggedIn) {
      ev.preventDefault();
      this.router.navigate(['/login'], { queryParams: { redirectTo: m.route } });
    }
  }
}
