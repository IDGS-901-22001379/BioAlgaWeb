import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResponse } from '../models/paged-response.model';
import { ProductoDto, CrearProductoDto, ActualizarProductoDto } from '../models/producto.model';

@Injectable({ providedIn: 'root' })
export class ProductosService {
  private baseUrl = `${environment.apiUrl}/api/productos`;

  constructor(private http: HttpClient) {}

  // GET /api/productos?q=&tipo=&idCategoria=&page=&pageSize=&sortBy=&sortDir=
  buscar(opts: {
    q?: string;
    tipo?: string;
    idCategoria?: number;
    idMarca?: number;
    idUnidad?: number;
    estatus?: string;
    page?: number;
    pageSize?: number;
    sortBy?: string;
    sortDir?: 'asc' | 'desc';
  } = {}): Observable<PagedResponse<ProductoDto>> {
    let params = new HttpParams();
    if (opts.q) params = params.set('q', opts.q);
    if (opts.tipo) params = params.set('tipo', opts.tipo);
    if (opts.idCategoria != null) params = params.set('idCategoria', opts.idCategoria);
    if (opts.idMarca != null) params = params.set('idMarca', opts.idMarca);
    if (opts.idUnidad != null) params = params.set('idUnidad', opts.idUnidad);
    if (opts.estatus) params = params.set('estatus', opts.estatus);
    if (opts.page) params = params.set('page', opts.page);
    if (opts.pageSize) params = params.set('pageSize', opts.pageSize);
    if (opts.sortBy) params = params.set('sortBy', opts.sortBy);
    if (opts.sortDir) params = params.set('sortDir', opts.sortDir);

    return this.http.get<PagedResponse<ProductoDto>>(this.baseUrl, { params });
  }

  // GET /api/productos/{id}
  obtenerPorId(id: number): Observable<ProductoDto> {
    return this.http.get<ProductoDto>(`${this.baseUrl}/${id}`);
  }

  // POST /api/productos
  crear(dto: CrearProductoDto): Observable<ProductoDto> {
    return this.http.post<ProductoDto>(this.baseUrl, dto);
  }

  // PUT /api/productos/{id}
  actualizar(id: number, dto: ActualizarProductoDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  // DELETE /api/productos/{id}
  eliminar(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
