// src/app/services/pedidos.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  EstatusPedido,
  PedidoDto,
  PedidoListResponse,
  PedidoQueryParams,
  PedidoCreateRequest,
  PedidoUpdateHeaderRequest,
  PedidoReplaceLinesRequest,
  PedidoLineaEditRequest,
  PedidoConfirmarRequest,
  PedidoCambioEstatusRequest,
} from '../models/pedido-dtos.model';

// Si usas environments/proxy, cambia esta línea por environment.apiUrl o '/api/pedidos'
@Injectable({ providedIn: 'root' })
export class PedidosService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/pedidos';

  /** Header opcional para enviar el usuario (X-User-Id) */
  private withUser(userId?: number) {
    if (userId == null) return {};
    return { headers: new HttpHeaders({ 'X-User-Id': String(userId) }) };
  }

  // ===================== LISTAR / BUSCAR =====================
  /** GET /api/pedidos */
  buscar(params: PedidoQueryParams): Observable<PedidoListResponse> {
    let hp = new HttpParams();

    if (params.q && params.q.trim()) hp = hp.set('q', params.q.trim());
    if (params.estatus)              hp = hp.set('estatus', String(params.estatus)); // Enum serializado como string
    if (params.page)                 hp = hp.set('page', String(params.page));
    if (params.pageSize)             hp = hp.set('pageSize', String(params.pageSize));
    if (params.sortBy)               hp = hp.set('sortBy', params.sortBy);
    if (params.sortDir)              hp = hp.set('sortDir', params.sortDir);

    return this.http.get<PedidoListResponse>(this.baseUrl, { params: hp });
  }

  /** Alias opcional si quieres llamarlo "buscarResumen" desde otros componentes */
  buscarResumen(params: PedidoQueryParams) {
    return this.buscar(params);
  }

  /** Helpers convenientes (opcionales) */
  buscarBorradores(page = 1, pageSize = 20, q = ''): Observable<PedidoListResponse> {
    return this.buscar({
      q,
      estatus: EstatusPedido.Borrador,
      page,
      pageSize,
      sortBy: 'FechaPedido',
      sortDir: 'DESC',
    });
  }

  // ===================== OBTENER DETALLE =====================
  /** GET /api/pedidos/{id} */
  getById(idPedido: number): Observable<PedidoDto> {
    return this.http.get<PedidoDto>(`${this.baseUrl}/${idPedido}`);
  }

  // ===================== CREAR (BORRADOR) =====================
  /** POST /api/pedidos */
  crear(body: PedidoCreateRequest, userId?: number): Observable<PedidoDto> {
    return this.http.post<PedidoDto>(this.baseUrl, body, this.withUser(userId));
  }

  // ===================== EDITAR CABECERA (BORRADOR) =====================
  /** PUT /api/pedidos/header */
  updateHeader(body: PedidoUpdateHeaderRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/header`, body, this.withUser(userId));
  }

  // ===================== REEMPLAZAR TODAS LAS LÍNEAS (BORRADOR) =====================
  /** PUT /api/pedidos/lines/replace */
  replaceLines(body: PedidoReplaceLinesRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/lines/replace`, body, this.withUser(userId));
  }

  // ===================== EDITAR/AÑADIR UNA LÍNEA (BORRADOR) =====================
  /** PUT /api/pedidos/lines/edit */
  editLine(body: PedidoLineaEditRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/lines/edit`, body, this.withUser(userId));
  }

  // ===================== CONFIRMAR (congelar precios, opc. reservar) =====================
  /** PUT /api/pedidos/confirm */
  confirmar(body: PedidoConfirmarRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/confirm`, body, this.withUser(userId));
  }

  // ===================== CAMBIAR ESTATUS =====================
  /** PUT /api/pedidos/status */
  cambiarEstatus(body: PedidoCambioEstatusRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/status`, body, this.withUser(userId));
  }

  // ===================== ELIMINAR (Borrador o Cancelado) =====================
  /** DELETE /api/pedidos/{id} */
  eliminar(idPedido: number, userId?: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${idPedido}`, this.withUser(userId));
  }
}
