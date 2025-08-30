// src/app/services/devoluciones.service.ts
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

  // Igual que en ClientesService: usa URL absoluta a tu API
  private baseUrl = 'http://localhost:5241/api/devoluciones';
  // Si usas proxy Angular, podrías usar: private baseUrl = '/api/devoluciones';

  /** Crear una devolución */
  create(body: DevolucionCreateRequest) {
    return this.http.post<DevolucionDto>(this.baseUrl, body);
  }

  /** Obtener devolución por id */
  getById(id: number) {
    return this.http.get<DevolucionDto>(`${this.baseUrl}/${id}`);
  }

  /** Buscar/listar devoluciones con filtros, paginación y orden */
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
