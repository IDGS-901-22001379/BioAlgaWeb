import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  EstatusPedido,
  PedidoDto,
  PedidoListItemDto,
  PedidoListResponse,
  PedidoQueryParams,
  PedidoCreateRequest,
  PedidoUpdateHeaderRequest,
  PedidoReplaceLinesRequest,
  PedidoLineaEditRequest,
  PedidoConfirmarRequest,
  PedidoCambioEstatusRequest,
} from '../models/pedido-dtos.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({ providedIn: 'root' })
export class PedidosService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/pedidos';
  // Con proxy Angular: private baseUrl = '/api/pedidos';

  /** Helper opcional para enviar el usuario (si lo quieres setear) */
  private withUser(userId?: number) {
    if (userId == null) return {};
    return { headers: new HttpHeaders({ 'X-User-Id': String(userId) }) };
  }

  // =========================================
  // LISTAR / BUSCAR (paginado)
  // GET /api/pedidos
  // =========================================
  buscar(params: PedidoQueryParams): Observable<PagedResponse<PedidoListItemDto>> {
    let hp = new HttpParams();

    if (params.q && params.q.trim()) hp = hp.set('q', params.q.trim());
    if (params.estatus) hp = hp.set('estatus', params.estatus as EstatusPedido);

    if (params.page) hp = hp.set('page', String(params.page));
    if (params.pageSize) hp = hp.set('pageSize', String(params.pageSize));
    if (params.sortBy) hp = hp.set('sortBy', params.sortBy);
    if (params.sortDir) hp = hp.set('sortDir', params.sortDir);

    return this.http.get<PagedResponse<PedidoListItemDto>>(this.baseUrl, { params: hp });
  }

  // Alias de compatibilidad si acostumbras llamar "buscarResumen"
  buscarResumen(params: PedidoQueryParams) {
    return this.buscar(params);
  }

  // =========================================
  // OBTENER DETALLE
  // GET /api/pedidos/{id}
  // =========================================
  getById(idPedido: number): Observable<PedidoDto> {
    return this.http.get<PedidoDto>(`${this.baseUrl}/${idPedido}`);
  }

  // =========================================
  // CREAR (Borrador)
  // POST /api/pedidos
  // =========================================
  crear(body: PedidoCreateRequest, userId?: number): Observable<PedidoDto> {
    return this.http.post<PedidoDto>(this.baseUrl, body, this.withUser(userId));
    // Si no pasas userId, el backend usa 1 por defecto (dev).
  }

  // =========================================
  // EDITAR CABECERA (Borrador)
  // PUT /api/pedidos/header
  // =========================================
  updateHeader(body: PedidoUpdateHeaderRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/header`, body, this.withUser(userId));
  }

  // =========================================
  // REEMPLAZAR TODAS LAS LÍNEAS (Borrador)
  // PUT /api/pedidos/lines/replace
  // =========================================
  replaceLines(body: PedidoReplaceLinesRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/lines/replace`, body, this.withUser(userId));
  }

  // =========================================
  // EDITAR/AÑADIR UNA LÍNEA (Borrador)
  // PUT /api/pedidos/lines/edit
  // =========================================
  editLine(body: PedidoLineaEditRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/lines/edit`, body, this.withUser(userId));
  }

  // =========================================
  // CONFIRMAR (congelar precios, opc. reservar)
  // PUT /api/pedidos/confirm
  // =========================================
  confirmar(body: PedidoConfirmarRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/confirm`, body, this.withUser(userId));
  }

  // =========================================
  // CAMBIAR ESTATUS
  // PUT /api/pedidos/status
  // =========================================
  cambiarEstatus(body: PedidoCambioEstatusRequest, userId?: number): Observable<PedidoDto> {
    return this.http.put<PedidoDto>(`${this.baseUrl}/status`, body, this.withUser(userId));
  }
}
