import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

// Modelos del dashboard
import {
  VentasResumen,
  TopProducto,
  TopCliente,
  VentasPorUsuario,
  DevolucionesPorUsuario,
  ComprasPorProveedor,
  DashboardFilters,
} from '../models/dashboard';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/dashboard';
  // Con proxy Angular: private baseUrl = '/api/dashboard';

  // =========================================================
  // Ventas resumen (día, semana, mes, año)
  // GET /api/dashboard/ventas/resumen
  // params opcionales: from=YYYY-MM-DD, to=YYYY-MM-DD, top=number
  // =========================================================
  getVentasResumen(filters?: DashboardFilters): Observable<VentasResumen[]> {
    let params = new HttpParams();

    if (filters?.from) params = params.set('from', filters.from);
    if (filters?.to) params = params.set('to', filters.to);
    if (filters?.top != null) params = params.set('top', String(filters.top));

    return this.http.get<VentasResumen[]>(`${this.baseUrl}/ventas/resumen`, { params });
  }

  // =========================================================
  // Top productos (por ingreso/unidades según vista del backend)
  // GET /api/dashboard/top/productos
  // =========================================================
  getTopProductos(top?: number): Observable<TopProducto[]> {
    let params = new HttpParams();
    if (top != null) params = params.set('top', String(top));
    return this.http.get<TopProducto[]>(`${this.baseUrl}/top/productos`, { params });
  }

  // =========================================================
  // Top clientes
  // GET /api/dashboard/top/clientes
  // =========================================================
  getTopClientes(top?: number): Observable<TopCliente[]> {
    let params = new HttpParams();
    if (top != null) params = params.set('top', String(top));
    return this.http.get<TopCliente[]>(`${this.baseUrl}/top/clientes`, { params });
  }

  // =========================================================
  // Ventas por usuario
  // GET /api/dashboard/ventas/usuarios
  // =========================================================
  getVentasPorUsuarios(filters?: DashboardFilters): Observable<VentasPorUsuario[]> {
    let params = new HttpParams();
    if (filters?.from) params = params.set('from', filters.from);
    if (filters?.to) params = params.set('to', filters.to);
    if (filters?.top != null) params = params.set('top', String(filters.top));

    return this.http.get<VentasPorUsuario[]>(`${this.baseUrl}/ventas/usuarios`, { params });
  }

  // =========================================================
  // Devoluciones por usuario
  // GET /api/dashboard/devoluciones/usuarios
  // =========================================================
  getDevolucionesPorUsuarios(filters?: DashboardFilters): Observable<DevolucionesPorUsuario[]> {
    let params = new HttpParams();
    if (filters?.from) params = params.set('from', filters.from);
    if (filters?.to) params = params.set('to', filters.to);
    if (filters?.top != null) params = params.set('top', String(filters.top));

    return this.http.get<DevolucionesPorUsuario[]>(`${this.baseUrl}/devoluciones/usuarios`, { params });
  }

  // =========================================================
  // Compras por proveedor
  // GET /api/dashboard/compras/proveedores
  // =========================================================
  getComprasPorProveedores(top?: number, filters?: DashboardFilters): Observable<ComprasPorProveedor[]> {
    let params = new HttpParams();
    if (filters?.from) params = params.set('from', filters.from);
    if (filters?.to) params = params.set('to', filters.to);
    if (top != null) params = params.set('top', String(top));

    return this.http.get<ComprasPorProveedor[]>(`${this.baseUrl}/compras/proveedores`, { params });
  }
}
