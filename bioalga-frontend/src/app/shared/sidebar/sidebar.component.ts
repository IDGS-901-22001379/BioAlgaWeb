import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

interface MenuItem {
  key: 'dashboard' | 'usuarios' | 'empleados' | 'proveedores' | 'productos';
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
  constructor(private router: Router, private auth: AuthService) {}

  collapsed = signal(false);

  // Iconos SVG
  private icoDash = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><rect x="3" y="3" width="7" height="7" rx="1.5"/><rect x="14" y="3" width="7" height="7" rx="1.5"/><rect x="3" y="14" width="7" height="7" rx="1.5"/><rect x="14" y="14" width="7" height="7" rx="1.5"/></svg>`;
  private icoUser = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 4-6 8-6s8 2 8 6"/></svg>`;
  private icoTruck = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 7h10v7H3z"/><path d="M13 9h5l3 3v2h-8z"/><circle cx="7" cy="18" r="2"/><circle cx="17" cy="18" r="2"/></svg>`;
  private icoFactory = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M3 21V9l6 3V9l6 3V6l6 3v12z"/><path d="M6 21V9"/><path d="M9 21V12"/></svg>`;
  private icoBox = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor"><path d="M21 16V8l-9-5-9 5v8l9 5 9-5z"/><path d="M3.3 7.3 12 12l8.7-4.7"/></svg>`;

  // Menú
  menu: MenuItem[] = [
    { key: 'dashboard', label: 'Dashboard', route: '/inicio/dashboard', svg: this.icoDash },
    { key: 'usuarios', label: 'Usuarios', route: '/inicio/usuarios', svg: this.icoUser },
    { key: 'empleados', label: 'Empleados', route: '/inicio/empleados', svg: this.icoFactory },
    { key: 'proveedores', label: 'Proveedores', route: '/inicio/proveedores', svg: this.icoTruck },
    { key: 'productos', label: 'Productos', route: '/inicio/productos', svg: this.icoBox },
  ];

  toggleCollapse() {
    this.collapsed.update(v => !v);
  }

  /**
   * Si no hay sesión y se clickea un módulo que requiere login, redirige al /login
   * y pasa la ruta de retorno como query param (?redirectTo=...).
   */
  async maybePromptLogin(m: MenuItem, ev: Event) {
    // Si no requiere login, no hacemos nada
    if (m.key !== 'usuarios') return;

    if (!this.auth.isLoggedIn) {
      ev.preventDefault();
      this.router.navigate(['/login'], { queryParams: { redirectTo: m.route } });
    }
  }
}
