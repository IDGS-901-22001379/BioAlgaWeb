// src/app/services/inventario.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { InventarioMovimiento, KardexItem, StockResponse } from '../models/inventario.model';

@Injectable({ providedIn: 'root' })
export class InventarioService {
  private base = `${environment.apiUrl}/api/inventario`;

  constructor(private http: HttpClient) {}

  // Kardex por producto y rango de fechas
  kardex(productoId: number, desde?: string, hasta?: string): Observable<KardexItem[]> {
    let params = new HttpParams().set('producto_id', productoId);
    if (desde) params = params.set('desde', desde);
    if (hasta) params = params.set('hasta', hasta);
    return this.http.get<KardexItem[]>(`${this.base}/kardex`, { params });
    // -> GET /api/inventario/kardex?producto_id=&desde=&hasta=
  }

  // Stock actual por producto
  stockActual(productoId: number): Observable<StockResponse> {
    const params = new HttpParams().set('producto_id', productoId);
    return this.http.get<StockResponse>(`${this.base}/stock-actual`, { params });
    // -> GET /api/inventario/stock-actual?producto_id=
  }

  // (opcional) listar movimientos por origen
  movimientosPorOrigen(origenTipo: string, origenId: number): Observable<InventarioMovimiento[]> {
    const params = new HttpParams().set('origen_tipo', origenTipo).set('origen_id', origenId);
    return this.http.get<InventarioMovimiento[]>(`${this.base}/movimientos`, { params });
    // -> GET /api/inventario/movimientos?origen_tipo=Compra&origen_id=123
  }
}
