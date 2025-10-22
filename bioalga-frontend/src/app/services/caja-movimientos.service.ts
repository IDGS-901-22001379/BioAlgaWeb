import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CajaMovimientoDto,
  CrearCajaMovimientoRequest,
  ActualizarCajaMovimientoRequest,
  CajaMovimientoQueryParams
} from '../models/caja-movimientos.model';
import { PagedResponse } from '../models/paged-response.model';

@Injectable({ providedIn: 'root' })
export class CajaMovimientosService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/cajamovimientos';
  // Con proxy: private baseUrl = '/api/cajamovimientos';

  buscarPorTurno(q: CajaMovimientoQueryParams): Observable<PagedResponse<CajaMovimientoDto>> {
    let params = new HttpParams()
      .set('page', String(q.page ?? 1))
      .set('pageSize', String(q.pageSize ?? 20));

    if (q.tipo) params = params.set('tipo', q.tipo);
    if (q.q)    params = params.set('q', q.q);

    return this.http.get<PagedResponse<CajaMovimientoDto>>(
      `${this.baseUrl}/por-turno/${q.idTurno}`, { params });
  }

  getById(id: number): Observable<CajaMovimientoDto> {
    return this.http.get<CajaMovimientoDto>(`${this.baseUrl}/${id}`);
  }

  create(body: CrearCajaMovimientoRequest): Observable<CajaMovimientoDto> {
    return this.http.post<CajaMovimientoDto>(this.baseUrl, body);
  }

  update(id: number, body: ActualizarCajaMovimientoRequest): Observable<CajaMovimientoDto> {
    return this.http.put<CajaMovimientoDto>(`${this.baseUrl}/${id}`, body);
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
