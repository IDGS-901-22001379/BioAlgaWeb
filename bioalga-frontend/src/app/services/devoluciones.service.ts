import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  DevolucionCreateRequest,
  DevolucionDto,
  DevolucionQueryParams,
  PagedResponse
} from '../models/devolucion-dtos.model';

@Injectable({ providedIn: 'root' })
export class DevolucionesService {
  private http = inject(HttpClient);

  // Si usas proxy, pon '/api/devoluciones'
  private baseUrl = 'http://localhost:5241/api/devoluciones';

  create(body: DevolucionCreateRequest) {
    return this.http.post<DevolucionDto>(this.baseUrl, body);
  }

  getById(id: number) {
    return this.http.get<DevolucionDto>(`${this.baseUrl}/${id}`);
  }

  buscar(qp: DevolucionQueryParams) {
    let params = new HttpParams();
    if (qp.q)        params = params.set('q', qp.q);
    if (qp.page)     params = params.set('page', qp.page);
    if (qp.pageSize) params = params.set('pageSize', qp.pageSize);
    if (qp.sortBy)   params = params.set('sortBy', qp.sortBy);
    if (qp.sortDir)  params = params.set('sortDir', qp.sortDir);

    return this.http.get<PagedResponse<DevolucionDto>>(this.baseUrl, { params });
  }
}
