import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  VentaCreateRequest,
  VentaDto,
  VentaQueryParams
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

  /** Obtener venta por id */
  getById(idVenta: number): Observable<VentaDto> {
    return this.http.get<VentaDto>(`${this.baseUrl}/${idVenta}`);
  }

  /** Buscar / Listar ventas (cuando tengas GET en backend) */
  buscar(params: VentaQueryParams): Observable<PagedResponse<VentaDto>> {
    let hp = new HttpParams();
    if (params.q?.trim()) hp = hp.set('q', params.q.trim());
    if (params.fechaDesde) hp = hp.set('fechaDesde', params.fechaDesde);
    if (params.fechaHasta) hp = hp.set('fechaHasta', params.fechaHasta);
    if (params.page) hp = hp.set('page', String(params.page));
    if (params.pageSize) hp = hp.set('pageSize', String(params.pageSize));
    if (params.sortBy) hp = hp.set('sortBy', params.sortBy);
    if (params.sortDir) hp = hp.set('sortDir', params.sortDir);

    return this.http.get<PagedResponse<VentaDto>>(this.baseUrl, { params: hp });
  }
}
