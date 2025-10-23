import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CajaTurnoDto,
  AbrirTurnoRequest,
  CerrarTurnoRequest,
  CajaTurnoQueryParams
} from '../models/caja-turno-dtos.model';
import { PagedResponse } from '../models/paged-response.model';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CajaTurnosService {
  private http = inject(HttpClient);
  // Alineado con el controller: [Route("api/[controller]")] => api/CajaTurnos
  private baseUrl = `${environment.apiUrl}/api/CajaTurnos`;
  // Si usas proxy: private baseUrl = '/api/CajaTurnos';

  /** GET /api/CajaTurnos/buscar?{params} */
  buscar(q: CajaTurnoQueryParams): Observable<PagedResponse<CajaTurnoDto>> {
    let params = new HttpParams()
      .set('page', String(q.page ?? 1))
      .set('pageSize', String(q.pageSize ?? 20));

    if (q.idCaja != null)    params = params.set('idCaja', String(q.idCaja));
    if (q.idUsuario != null) params = params.set('idUsuario', String(q.idUsuario));
    if (q.desde)             params = params.set('desde', q.desde);
    if (q.hasta)             params = params.set('hasta', q.hasta);

    return this.http.get<PagedResponse<CajaTurnoDto>>(`${this.baseUrl}/buscar`, { params });
  }

  /** GET /api/CajaTurnos/{idTurno} */
  getById(idTurno: number): Observable<CajaTurnoDto> {
    return this.http.get<CajaTurnoDto>(`${this.baseUrl}/${idTurno}`);
  }

  /** GET /api/CajaTurnos/abierto-por-caja/{idCaja} */
  abiertoPorCaja(idCaja: number): Observable<CajaTurnoDto | null> {
    return this.http.get<CajaTurnoDto | null>(`${this.baseUrl}/abierto-por-caja/${idCaja}`);
  }

  /** GET /api/CajaTurnos/abierto-por-usuario/{idUsuario} */
  abiertoPorUsuario(idUsuario: number): Observable<CajaTurnoDto | null> {
    return this.http.get<CajaTurnoDto | null>(`${this.baseUrl}/abierto-por-usuario/${idUsuario}`);
  }

  /** POST /api/CajaTurnos/abrir */
  abrir(body: AbrirTurnoRequest): Observable<CajaTurnoDto> {
    return this.http.post<CajaTurnoDto>(`${this.baseUrl}/abrir`, body);
  }

  /** PUT /api/CajaTurnos/cerrar/{idTurno} */
  cerrar(idTurno: number, body: CerrarTurnoRequest): Observable<CajaTurnoDto> {
    return this.http.put<CajaTurnoDto>(`${this.baseUrl}/cerrar/${idTurno}`, body);
  }
}
