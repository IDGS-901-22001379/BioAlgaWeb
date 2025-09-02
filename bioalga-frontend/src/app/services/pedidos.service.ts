// src/app/services/pedidos.service.ts
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

// import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PedidosService {
  private http = inject(HttpClient);
  // Recomendado: usar environment/proxy de Angular:
  // private baseUrl = `${environment.apiUrl}/api/pedidos`;
  // o con proxy.conf.json → '/api/pedidos'
  private baseUrl = 'http://localhost:5241/api/pedidos';

  /** Opcional: header con el usuario que ejecuta la acción */
  private withUser(userId?: number) {
    if (userId == null) return {};
    return { headers: new HttpHeaders({ 'X-User-Id': String(userId) }) };
  }

  // ===================== LISTAR / BUSCAR =====================
  /** GET /api/pedidos */
  buscar(params: PedidoQueryParams): Observable<PedidoListResponse> {
    let hp = new HttpParams();

    if (params.q && params.q.trim())     hp = hp.set('q', params.q.trim());
    if (params.estatus)                  hp = hp.set('estatus', String(params.estatus));
    if (params.page)                     hp = hp.set('page', String(params.page));
    if (params.pageSize)                 hp = hp.set('pageSize', String(params.pageSize));
    if (params.sortBy)                   hp = hp.set('sortBy', params.sortBy);
    if (params.sortDir)                  hp = hp.set('sortDir', params.sortDir);

    return this.http.get<PedidoListResponse>(this.baseUrl, { params: hp });
  }

  /** Atajo para listar solo Borradores */
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

  /** Atajo para listar Confirmados+ (excluye borradores) */
  buscarActivos(page = 1, pageSize = 20, q = ''): Observable<PedidoListResponse> {
    // Si quieres filtrar múltiples estatus en backend, expándelo allí;
    // aquí dejamos un helper simple (front puede pedir por cada estatus).
    return this.buscar({
      q,
      estatus: EstatusPedido.Confirmado,
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

  // ===================== ELIMINAR (solo Borrador) =====================
  /** DELETE /api/pedidos/{id} */
  eliminar(idPedido: number, userId?: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${idPedido}`, this.withUser(userId));
  }
}
