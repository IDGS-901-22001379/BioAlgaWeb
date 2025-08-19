// services/clientes.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClienteDto, ClienteCreateRequest, ClienteUpdateRequest, PagedResponse } from '../models/cliente.model';

@Injectable({ providedIn: 'root' })
export class ClientesService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/clientes';

  // Listar con filtros + paginación
  buscar(q: string, tipoCliente: string | undefined, page: number, pageSize: number):
    Observable<PagedResponse<ClienteDto>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (q) params = params.set('q', q); // <- importante: 'q' como en el backend
    if (tipoCliente) params = params.set('tipo_Cliente', tipoCliente);

    return this.http.get<PagedResponse<ClienteDto>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<ClienteDto> {
    return this.http.get<ClienteDto>(`${this.baseUrl}/${id}`);
  }

  create(body: ClienteCreateRequest): Observable<ClienteDto> {
    return this.http.post<ClienteDto>(this.baseUrl, body);
  }

  update(id: number, body: ClienteUpdateRequest): Observable<ClienteDto> {
    return this.http.put<ClienteDto>(`${this.baseUrl}/${id}`, body);
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
