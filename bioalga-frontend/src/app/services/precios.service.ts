import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PrecioDto, CrearPrecioDto, ActualizarPrecioDto } from '../models/precio.model';

@Injectable({ providedIn: 'root' })
export class PreciosService {
  private baseUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

  // GET /api/productos/{idProducto}/precios
  historial(idProducto: number): Observable<PrecioDto[]> {
    return this.http.get<PrecioDto[]>(`${this.baseUrl}/productos/${idProducto}/precios`);
  }

  // GET /api/productos/{idProducto}/precios/vigentes
  vigentes(idProducto: number): Observable<PrecioDto[]> {
    return this.http.get<PrecioDto[]>(`${this.baseUrl}/productos/${idProducto}/precios/vigentes`);
  }

  // POST /api/productos/{idProducto}/precios
  crear(idProducto: number, dto: CrearPrecioDto): Observable<PrecioDto> {
    return this.http.post<PrecioDto>(`${this.baseUrl}/productos/${idProducto}/precios`, dto);
  }

  // PUT /api/precios/{idPrecio}
  actualizar(idPrecio: number, dto: ActualizarPrecioDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/precios/${idPrecio}`, dto);
  }
}
