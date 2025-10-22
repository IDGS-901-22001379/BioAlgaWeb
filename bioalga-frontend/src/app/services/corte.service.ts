import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CorteResumenDto } from '../models/corte-resumen.model';

@Injectable({ providedIn: 'root' })
export class CorteService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5241/api/corte';
  // Con proxy: private baseUrl = '/api/corte';

  /** Obtiene el resumen/arqueo para un turno (abierto o cerrado) */
  resumenPorTurno(idTurno: number): Observable<CorteResumenDto> {
    return this.http.get<CorteResumenDto>(`${this.baseUrl}/turno/${idTurno}/resumen`);
  }
}
