import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CajaDto,
  CajaCreateRequest,
  CajaUpdateRequest
} from '../models/caja-dtos.model';

@Injectable({ providedIn: 'root' })
export class CajasService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/cajas';
  // Con proxy: private baseUrl = '/api/cajas';

  listar(): Observable<CajaDto[]> {
    return this.http.get<CajaDto[]>(this.baseUrl);
  }

  getById(id: number): Observable<CajaDto> {
    return this.http.get<CajaDto>(`${this.baseUrl}/${id}`);
  }

  create(body: CajaCreateRequest): Observable<CajaDto> {
    return this.http.post<CajaDto>(this.baseUrl, body);
  }

  update(id: number, body: CajaUpdateRequest): Observable<CajaDto> {
    return this.http.put<CajaDto>(`${this.baseUrl}/${id}`, body);
  }

  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
