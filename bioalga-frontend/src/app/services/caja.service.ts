import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CajaAperturaCreate,
  CajaAperturaDto,
  CajaMovimientoCreate,
  CajaMovimientoDto,
  CajaCorteCreate,
  CajaCorteDto,
  CajaQueryParams
} from '../models/caja-dtos.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({ providedIn: 'root' })
export class CajaService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/caja';
  // Con proxy Angular: private baseUrl = '/api/caja';

  /** Abrir caja */
  abrir(body: CajaAperturaCreate): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/apertura`, body);
  }

  /** Registrar movimiento manual de caja (Ingreso/Egreso) */
  registrarMovimiento(body: CajaMovimientoCreate): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/movimiento`, body);
  }

  /** Realizar corte */
  corte(body: CajaCorteCreate): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/corte`, body);
  }

  // ===== GETs cuando los tengas en backend =====

  /** Obtener una apertura por id */
  getApertura(id: number): Observable<CajaAperturaDto> {
    return this.http.get<CajaAperturaDto>(`${this.baseUrl}/apertura/${id}`);
  }

  /** Listar movimientos de una apertura */
  listarMovimientos(idCajaApertura: number, params?: CajaQueryParams)
    : Observable<PagedResponse<CajaMovimientoDto>> {
    let hp = new HttpParams().set('idCajaApertura', String(idCajaApertura));
    if (params) {
      if (params.idUsuario != null) hp = hp.set('idUsuario', String(params.idUsuario));
      if (params.fechaDesde) hp = hp.set('fechaDesde', params.fechaDesde);
      if (params.fechaHasta) hp = hp.set('fechaHasta', params.fechaHasta);
      if (params.page) hp = hp.set('page', String(params.page));
      if (params.pageSize) hp = hp.set('pageSize', String(params.pageSize));
      if (params.sortBy) hp = hp.set('sortBy', params.sortBy);
      if (params.sortDir) hp = hp.set('sortDir', params.sortDir);
    }
    return this.http.get<PagedResponse<CajaMovimientoDto>>(`${this.baseUrl}/movimientos`, { params: hp });
  }

  /** Listar cortes */
  listarCortes(params?: CajaQueryParams): Observable<PagedResponse<CajaCorteDto>> {
    let hp = new HttpParams();
    if (params) {
      if (params.idUsuario != null) hp = hp.set('idUsuario', String(params.idUsuario));
      if (params.fechaDesde) hp = hp.set('fechaDesde', params.fechaDesde);
      if (params.fechaHasta) hp = hp.set('fechaHasta', params.fechaHasta);
      if (params.page) hp = hp.set('page', String(params.page));
      if (params.pageSize) hp = hp.set('pageSize', String(params.pageSize));
      if (params.sortBy) hp = hp.set('sortBy', params.sortBy);
      if (params.sortDir) hp = hp.set('sortDir', params.sortDir);
    }
    return this.http.get<PagedResponse<CajaCorteDto>>(`${this.baseUrl}/cortes`, { params: hp });
  }
}
