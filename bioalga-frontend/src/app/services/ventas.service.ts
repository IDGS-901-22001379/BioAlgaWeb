import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  // existentes (para crear)
  VentaCreateRequest,
  VentaDto,
  VentaQueryParams,
  // nuevos (para listar/ver detalles)
  VentaResumenDto,
  VentaDetalleDto,
  MetodoPago,
  EstatusVenta,
} from '../models/venta-dtos.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({ providedIn: 'root' })
export class VentasService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/ventas';
  // Con proxy Angular: private baseUrl = '/api/ventas';

  /** Crear venta (POS) */
  create(body: VentaCreateRequest): Observable<VentaDto> {
    return this.http.post<VentaDto>(this.baseUrl, body);
  }

  /** Cancelar una venta */
  cancelar(idVenta: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${idVenta}/cancelar`, {});
  }

  // =========================================
  // NUEVO: Historial (resumen paginado)
  // GET /api/ventas
  // =========================================
  buscarResumen(params: VentaQueryParams): Observable<PagedResponse<VentaResumenDto>> {
    let hp = new HttpParams();

    // búsqueda libre
    if (params.q && params.q.trim()) hp = hp.set('q', params.q.trim());

    // filtros opcionales
    if (params.clienteId != null) hp = hp.set('clienteId', String(params.clienteId));
    if (params.usuarioId != null) hp = hp.set('usuarioId', String(params.usuarioId));
    if (params.estatus) hp = hp.set('estatus', params.estatus as EstatusVenta);
    if (params.metodoPago) hp = hp.set('metodoPago', params.metodoPago as MetodoPago);

    // rango de fechas (acepta ambos nombres)
    if (params.desde) hp = hp.set('desde', params.desde);
    if (params.hasta) hp = hp.set('hasta', params.hasta);
    if (params.fechaDesde) hp = hp.set('fechaDesde', params.fechaDesde);
    if (params.fechaHasta) hp = hp.set('fechaHasta', params.fechaHasta);

    // paginado / orden
    if (params.page) hp = hp.set('page', String(params.page));
    if (params.pageSize) hp = hp.set('pageSize', String(params.pageSize));
    if (params.sortBy) hp = hp.set('sortBy', params.sortBy);
    if (params.sortDir) hp = hp.set('sortDir', params.sortDir);

    return this.http.get<PagedResponse<VentaResumenDto>>(this.baseUrl, { params: hp });
  }

  // =========================================
  // NUEVO: Detalle de venta
  // GET /api/ventas/{id}
  // =========================================
  obtenerDetalle(idVenta: number): Observable<VentaDetalleDto> {
    return this.http.get<VentaDetalleDto>(`${this.baseUrl}/${idVenta}`);
  }

  /** Alias de compatibilidad (ahora devuelve el DETALLE) */
  getById(idVenta: number): Observable<VentaDetalleDto> {
    return this.obtenerDetalle(idVenta);
  }

  // =========================================
  // (Opcional) método combinado si quieres mantener tu firma original 'buscar'
  // y que ya devuelva el resumen de ventas:
  // =========================================
  buscar(params: VentaQueryParams): Observable<PagedResponse<VentaResumenDto>> {
    return this.buscarResumen(params);
  }
}
