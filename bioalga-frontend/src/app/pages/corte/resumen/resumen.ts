import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';

import { CorteService } from '../../../services/corte.service';       // ⬅️ servicio tipado
import { CorteResumenDto } from '../../../models/corte-resumen.model'; // ⬅️ modelo tipado
import Swal from 'sweetalert2';

@Component({
  selector: 'app-corte-resumen',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './resumen.html',
  styleUrls: ['./resumen.css'],
})
export class CorteResumenPage {
  private fb = inject(FormBuilder);
  private corteApi = inject(CorteService);   // ⬅️ inyección tipada

  cargando = signal(false);
  resumen  = signal<CorteResumenDto | null>(null);

  // si usas un input para el idTurno
  formTurno = this.fb.group({ idTurno: [null as number | null] });

  // helper opcional por si recibes el id por ruta o estado
  idTurno = signal<number | null>(null);

  totalMetodos(): number {
  const r = this.resumen();
  return (r?.ventas_Por_Metodo ?? []).reduce(
    (acc, m) => acc + (m?.monto ?? 0),
    0
  );
}

  // ================= Acciones =================
  cargarResumen(): void {
    const id = this.formTurno.value.idTurno ?? this.idTurno();
    if (!id || id <= 0) {
      this.alertInfo('Indica un Id de turno válido.');
      return;
    }

    this.cargando.set(true);
    this.corteApi.resumenPorTurno(id).subscribe({
      next: (r: CorteResumenDto) => {
        this.resumen.set(r);
        this.idTurno.set(r?.id_Turno ?? id);
      },
      error: (e: any) => {
        this.resumen.set(null);
        this.alertError(this.extractErr(e) || 'No fue posible obtener el resumen del turno.');
      },
      complete: () => this.cargando.set(false),
    });
  }

  // ================= SweetAlert helpers =================
  private alertInfo(msg: string)  { Swal.fire('Aviso', msg, 'info'); }
  private alertError(msg: string) { Swal.fire('Error', msg, 'error'); }

  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title)  return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }
}
