import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ClienteDto,
  ClienteCreateRequest,
  ClienteUpdateRequest,
  PagedResponse
} from '../models/cliente.model';

@Injectable({ providedIn: 'root' })
export class ClientesService {
  private http = inject(HttpClient);

  // Usa environment si ya lo tienes configurado. Si no, deja la URL directa:
  private baseUrl = 'http://localhost:5241/api/clientes';
  // Con proxy Angular: private baseUrl = '/api/clientes';

  /** Buscar clientes con texto libre + filtros + paginación + ordenamiento */
  buscar(
    q: string | undefined,
    tipoCliente: ('Normal'|'Mayoreo'|'Especial'|'Descuento') | undefined,
    estado: ('Activo'|'Inactivo') | undefined,
    page: number,
    pageSize: number,
    sortBy: 'nombre' | 'fecha' = 'nombre',
    sortDir: 'asc' | 'desc' = 'asc'
  ): Observable<PagedResponse<ClienteDto>> {

    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize))
      .set('sortBy', sortBy)
      .set('sortDir', sortDir);

    if (q?.trim()) params = params.set('q', q.trim());
    if (tipoCliente) params = params.set('tipo_Cliente', tipoCliente);
    if (estado) params = params.set('estado', estado);

    return this.http.get<PagedResponse<ClienteDto>>(this.baseUrl, { params });
  }

  /** Obtener un cliente por id */
  getById(id: number): Observable<ClienteDto> {
    return this.http.get<ClienteDto>(`${this.baseUrl}/${id}`);
  }

  /** Crear cliente */
  create(body: ClienteCreateRequest): Observable<ClienteDto> {
    return this.http.post<ClienteDto>(this.baseUrl, body);
  }

  /** Actualizar cliente */
  update(id: number, body: ClienteUpdateRequest): Observable<ClienteDto> {
    return this.http.put<ClienteDto>(`${this.baseUrl}/${id}`, body);
  }

  /** Eliminar cliente */
  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
