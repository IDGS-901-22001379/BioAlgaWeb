import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  DevolucionCreateRequest,
  DevolucionDto,
  DevolucionQueryParams
} from '../models/devolucion-dtos.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({ providedIn: 'root' })
export class DevolucionesService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/devoluciones';
  // Con proxy Angular: private baseUrl = '/api/devoluciones';

  /** Crear devolución */
  create(body: DevolucionCreateRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl, body);
  }

  /** Obtener devolución por id (cuando lo expongas en backend) */
  getById(id: number): Observable<DevolucionDto> {
    return this.http.get<DevolucionDto>(`${this.baseUrl}/${id}`);
  }

  /** Buscar / Listar devoluciones */
  buscar(params: DevolucionQueryParams): Observable<PagedResponse<DevolucionDto>> {
    let hp = new HttpParams();
    if (params.idVenta != null) hp = hp.set('idVenta', String(params.idVenta));
    if (params.fechaDesde) hp = hp.set('fechaDesde', params.fechaDesde);
    if (params.fechaHasta) hp = hp.set('fechaHasta', params.fechaHasta);
    if (params.page) hp = hp.set('page', String(params.page));
    if (params.pageSize) hp = hp.set('pageSize', String(params.pageSize));
    if (params.sortBy) hp = hp.set('sortBy', params.sortBy);
    if (params.sortDir) hp = hp.set('sortDir', params.sortDir);

    return this.http.get<PagedResponse<DevolucionDto>>(this.baseUrl, { params: hp });
  }
}
