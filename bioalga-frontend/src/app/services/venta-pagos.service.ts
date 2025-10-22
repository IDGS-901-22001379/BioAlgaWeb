import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  VentaPagoDto,
  CrearVentaPagoRequest
} from '../models/venta-pagos.model';

@Injectable({ providedIn: 'root' })
export class VentaPagosService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/ventapagos';
  // Con proxy: private baseUrl = '/api/ventapagos';

  listarPorVenta(idVenta: number): Observable<VentaPagoDto[]> {
    return this.http.get<VentaPagoDto[]>(`${this.baseUrl}/por-venta/${idVenta}`);
  }

  crear(body: CrearVentaPagoRequest): Observable<VentaPagoDto> {
    return this.http.post<VentaPagoDto>(this.baseUrl, body);
  }

  eliminar(idPago: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${idPago}`);
  }
}
