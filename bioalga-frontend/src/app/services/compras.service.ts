// src/app/services/compras.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { PagedResponse } from '../models/paged-response.model';
import { Compra } from '../models/compra.model';
import {
  AgregarRenglonDto,
  ConfirmarCompraResponse,
  CompraQueryParams,
  CrearCompraDto,
} from '../models/compra-dtos.model';
import { ProductoLookupDto } from '../models/producto-lookup.model';

@Injectable({ providedIn: 'root' })
export class ComprasService {
  private base = `${environment.apiUrl}/api/compras`;

  constructor(private http: HttpClient) {}

  // Crear borrador de compra
  crearBorrador(dto: CrearCompraDto): Observable<Compra> {
    return this.http.post<Compra>(this.base, dto);
  }

  // Obtener compra por id
  obtener(id: number): Observable<Compra> {
    return this.http.get<Compra>(`${this.base}/${id}`);
  }

  // Buscar compras (paginado)
  buscar(qp: CompraQueryParams): Observable<PagedResponse<Compra>> {
    let params = new HttpParams();
    if (qp.q) params = params.set('q', qp.q);
    if (qp.desde) params = params.set('desde', qp.desde);
    if (qp.hasta) params = params.set('hasta', qp.hasta);
    params = params.set('page', (qp.page ?? 1).toString());
    params = params.set('pageSize', (qp.pageSize ?? 10).toString());

    return this.http.get<PagedResponse<Compra>>(this.base, { params });
    // -> GET /api/compras?q=&desde=&hasta=&page=&pageSize=
  }

  // Agregar renglón
  agregarRenglon(idCompra: number, dto: AgregarRenglonDto): Observable<Compra> {
    return this.http.post<Compra>(`${this.base}/${idCompra}/renglones`, dto);
  }

  // Eliminar renglón
  eliminarRenglon(idCompra: number, idDetalle: number): Observable<Compra> {
    return this.http.delete<Compra>(`${this.base}/${idCompra}/renglones/${idDetalle}`);
  }

  // Confirmar compra (genera Entradas en inventario)
  confirmar(idCompra: number, idUsuario: number): Observable<ConfirmarCompraResponse> {
    const params = new HttpParams().set('idUsuario', idUsuario);
    return this.http.post<ConfirmarCompraResponse>(`${this.base}/${idCompra}/confirmar`, null, { params });
    // -> POST /api/compras/{id}/confirmar?idUsuario=...
  }

  // Autocomplete de productos (por nombre/SKU/código)
  buscarProductos(q: string, limit = 10): Observable<ProductoLookupDto[]> {
    const params = new HttpParams().set('q', q).set('limit', limit);
    return this.http.get<ProductoLookupDto[]>(`${this.base}/productos`, { params });
    // -> GET /api/compras/productos?q=&limit=
  }
}
