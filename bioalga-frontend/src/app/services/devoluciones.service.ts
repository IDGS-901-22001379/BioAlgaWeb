// src/app/services/devoluciones.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  DevolucionCreateRequest,
  DevolucionDto,
  DevolucionQueryParams
} from '../models/devolucion-dtos.model';

@Injectable({ providedIn: 'root' })
export class DevolucionesService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/devoluciones';
  // Con proxy Angular: private baseUrl = '/api/devoluciones';

  /** Crear devolución */
  create(body: DevolucionCreateRequest): Observable<DevolucionDto> {
    return this.http.post<DevolucionDto>(this.baseUrl, body);
  }

  /** Obtener devolución por ID */
  getById(id: number): Observable<DevolucionDto> {
    return this.http.get<DevolucionDto>(`${this.baseUrl}/${id}`);
  }

  /** Listar devoluciones con filtros opcionales */
  listar(params: DevolucionQueryParams): Observable<DevolucionDto[]> {
    let hp = new HttpParams();

    if (params.q && params.q.trim()) hp = hp.set('q', params.q.trim());
    if (params.desde) hp = hp.set('desde', params.desde);
    if (params.hasta) hp = hp.set('hasta', params.hasta);
    if (params.regresaInventario != null) {
      hp = hp.set('regresaInventario', String(params.regresaInventario));
    }

    return this.http.get<DevolucionDto[]>(this.baseUrl, { params: hp });
  }
}
