import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  EmpleadoDto,
  EmpleadoCreateRequest,
  EmpleadoUpdateRequest,
  PagedResponse
} from '../models/empleado.model';

@Injectable({ providedIn: 'root' })
export class EmpleadosService {
  private http = inject(HttpClient);

  // Igual que en ClientesService:
  private baseUrl = 'http://localhost:5241/api/empleados';
  // Si usas proxy Angular:  private baseUrl = '/api/empleados';

  /** Buscar empleados con texto libre + filtros + paginación + ordenamiento */
  buscar(
    q: string | undefined,
    puesto: string | undefined,
    estatus: 'Activo' | 'Inactivo' | 'Baja' | undefined,
    page: number,
    pageSize: number,
    // nombres válidos en backend: nombre | fecha_ingreso | salario | puesto | estatus
    sortBy: 'nombre' | 'fecha_ingreso' | 'salario' | 'puesto' | 'estatus' = 'nombre',
    sortDir: 'asc' | 'desc' = 'asc'
  ): Observable<PagedResponse<EmpleadoDto>> {

    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize))
      .set('sortBy', sortBy)
      .set('sortDir', sortDir)
      .set('_ts', Date.now().toString());

    if (q?.trim()) params = params.set('q', q.trim());
    if (puesto) params = params.set('puesto', puesto);
    if (estatus) params = params.set('estatus', estatus);

    return this.http.get<PagedResponse<EmpleadoDto>>(this.baseUrl, { params });
  }

  /** Obtener empleado por id */
  getById(id: number): Observable<EmpleadoDto> {
    return this.http.get<EmpleadoDto>(`${this.baseUrl}/${id}`);
  }

  /** Crear empleado */
  create(body: EmpleadoCreateRequest): Observable<EmpleadoDto> {
    return this.http.post<EmpleadoDto>(this.baseUrl, body);
  }

  /** Actualizar empleado */
  update(id: number, body: EmpleadoUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, body);
  }

  /** Eliminar empleado */
  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
