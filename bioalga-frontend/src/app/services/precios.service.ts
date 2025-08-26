import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators'; // NUEVO
import { environment } from '../../environments/environment';
import { PrecioDto, CrearPrecioDto, ActualizarPrecioDto, TipoPrecio } from '../models/precio.model'; // NUEVO: TipoPrecio

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

  // =========================
  // NUEVO: PRECIO VIGENTE POR TIPO (sin tocar backend)
  // =========================

  /** Devuelve el PrecioDto vigente para el tipo indicado. Si no existe ese tipo, intenta 'Normal'; si no hay, null. */
  vigenteDto(idProducto: number, tipo: TipoPrecio = 'Normal'): Observable<PrecioDto | null> {
    return this.vigentes(idProducto).pipe(
      map(list => {
        if (!list || !list.length) return null;

        // Normaliza y selecciona por tipo primero
        const porTipo = list.filter(p => (p.tipo_Precio as string) === tipo);

        // Si no hay de ese tipo, intenta Normal
        const candidatos = porTipo.length ? porTipo : list.filter(p => (p.tipo_Precio as string) === 'Normal');

        if (!candidatos.length) return null;

        // Toma el más reciente por vigente_Desde
        const parse = (s?: string | null) => (s ? new Date(s).getTime() : 0);
        return candidatos.sort((a, b) => parse(b.vigente_Desde) - parse(a.vigente_Desde))[0];
      })
    );
  }

  /** Devuelve solo el número del precio vigente para el tipo indicado. Fallback a 0 si no hay. */
  obtenerVigente(idProducto: number, tipo: TipoPrecio = 'Normal'): Observable<number> {
    return this.vigenteDto(idProducto, tipo).pipe(
      map(dto => dto?.precio ?? 0)
    );
  }
}
