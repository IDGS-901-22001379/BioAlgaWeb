import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  UsuarioDto,
  UsuarioCreateRequest,
  UsuarioUpdateRequest,
  PagedResponse,
} from '../models/usuario.models';

// 🔹 OPCIÓN 1: Conectar directo al backend (puerto actual del backend: 5241)
// const BASE = 'http://localhost:5241/api/usuarios';

// 🔹 OPCIÓN 2: Usar proxy.conf.json en Angular (recomendado para no cambiar puerto)
// const BASE = '/api/usuarios';

// Si todavía no tienes proxy, deja la opción 1 activa:
const BASE = 'http://localhost:5241/api/usuarios';

@Injectable({ providedIn: 'root' })
export class UsuariosService {
  private http = inject(HttpClient);

  /** Obtener todos los usuarios */
  getAll(): Observable<UsuarioDto[]> {
    return this.http.get<UsuarioDto[]>(BASE);
  }

  /** Buscar usuarios con filtros, paginación y estado */
  buscar(
    nombre: string,
    page = 1,
    pageSize = 10,
    activo?: boolean
  ): Observable<PagedResponse<UsuarioDto>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    if (nombre?.trim()) params = params.set('nombre', nombre.trim());
    if (activo !== undefined) params = params.set('activo', String(!!activo));

    return this.http.get<PagedResponse<UsuarioDto>>(`${BASE}/buscar`, { params });
  }

  /** Obtener un usuario por ID */
  getById(id: number): Observable<UsuarioDto> {
    return this.http.get<UsuarioDto>(`${BASE}/${id}`);
  }

  /** Crear un nuevo usuario */
  create(body: UsuarioCreateRequest): Observable<UsuarioDto> {
    return this.http.post<UsuarioDto>(BASE, body);
  }

  /** Actualizar un usuario existente */
  update(id: number, body: UsuarioUpdateRequest): Observable<UsuarioDto> {
    return this.http.put<UsuarioDto>(`${BASE}/${id}`, body);
  }

  /** Eliminar un usuario */
  remove(id: number): Observable<void> {
    return this.http.delete<void>(`${BASE}/${id}`);
  }
}
