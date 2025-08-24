// src/app/services/proveedores.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  ProveedorDto,
  ProveedorCreateRequest,
  ProveedorUpdateRequest,
  ProveedorQueryParams,
  PagedResponse
} from '../models/proveedor.model';

// ⚠️ Ajusta esta base según tu proyecto.
// Si usas environments, importa environment.apiUrl.
const API_BASE = 'http://localhost:5241/api/proveedores';

@Injectable({ providedIn: 'root' })
export class ProveedoresService {
  private http = inject(HttpClient);

  /** Buscar con filtros + paginación */
  buscar(params: ProveedorQueryParams) {
    let httpParams = new HttpParams();
    if (params.q?.trim())        httpParams = httpParams.set('q', params.q.trim());
    if (params.estatus)          httpParams = httpParams.set('estatus', params.estatus);
    if (params.pais)             httpParams = httpParams.set('pais', params.pais);
    if (params.ciudad)           httpParams = httpParams.set('ciudad', params.ciudad);
    if (params.page)             httpParams = httpParams.set('page', String(params.page));
    if (params.pageSize)         httpParams = httpParams.set('pageSize', String(params.pageSize));
    if (params.sortBy)           httpParams = httpParams.set('sortBy', params.sortBy);
    if (params.sortDir)          httpParams = httpParams.set('sortDir', params.sortDir);

    return this.http.get<PagedResponse<ProveedorDto>>(API_BASE, { params: httpParams });
  }

  /** Obtener por id */
  obtener(id: number) {
    return this.http.get<ProveedorDto>(`${API_BASE}/${id}`);
  }

  /** Crear proveedor */
  crear(payload: ProveedorCreateRequest) {
    return this.http.post<ProveedorDto>(API_BASE, payload);
  }

  /** Actualizar proveedor */
  actualizar(id: number, payload: ProveedorUpdateRequest) {
    return this.http.put<ProveedorDto>(`${API_BASE}/${id}`, payload);
  }

  /** Cambiar estatus (Activo/Inactivo) */
  cambiarEstatus(id: number, valor: 'Activo' | 'Inactivo') {
    return this.http.patch<void>(`${API_BASE}/${id}/estatus`, null, {
      params: new HttpParams().set('valor', valor)
    });
  }

  /** Eliminar duro (si lo usas) */
  eliminar(id: number) {
    return this.http.delete<void>(`${API_BASE}/${id}`);
  }
}
