// src/app/pages/corte/turnos/turnos.ts
import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import Swal from 'sweetalert2';

import { CajaTurnosService } from '../../../services/caja-turnos.service';
import { AuthService } from '../../../services/auth.service';

import {
  CajaTurnoDto,
  AbrirTurnoRequest,
  CerrarTurnoRequest,
  CajaTurnoQueryParams
} from '../../../models/caja-turno-dtos.model';

@Component({
  selector: 'app-corte-turnos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './turnos.html',
  styleUrls: ['./turnos.css']
})
export class CorteTurnosPage {
  private fb = inject(FormBuilder);
  private turnosApi = inject(CajaTurnosService);
  private auth = inject(AuthService);

  // ====================== Estado ======================
  cargando       = signal(false);
  cargandoAccion = signal(false);

  turnos = signal<CajaTurnoDto[]>([]);
  total  = signal(0);

  seleccionado = signal<CajaTurnoDto | null>(null);
  turnoParaCerrarId = signal<number | null>(null);

  math = Math;   // para pipes simples en template
  valOr0(v: number | null | undefined): number { return v ?? 0; }

  // Alias que usa el HTML
  lista = computed(() => this.turnos());

  // ====================== Filtros ======================
  formFiltros = this.fb.group({
    idCaja: [null as number | null],
    idUsuario: [null as number | null],
    desde: [''],
    hasta: [''],
    page: [1],
    pageSize: [20]
  });

  // ====================== Abrir turno (NUEVO contrato) ======================
  // Ahora se abre por NOMBRE de usuario y caja (no por IDs)
  mostrandoAbrir = signal(false);
  formAbrir = this.fb.group({
    nombreUsuario: ['', [Validators.required, Validators.maxLength(50)]],
    nombreCaja: ['', [Validators.required, Validators.maxLength(50)]],
    saldoInicial: [0, [Validators.required, Validators.min(0)]],
    observaciones: [null as string | null]
    // descripcionCaja?: Si tu backend lo acepta, podrías agregar aquí otro control
  });

  // ====================== Cerrar turno (id en la ruta) ======================
  mostrandoCerrar = signal(false);
  formCerrar = this.fb.group({
    saldoCierre: [0, [Validators.required, Validators.min(0)]],
    observaciones: [null as string | null]
  });

  // ====================== Lifecycle ======================
  ngOnInit(): void {
    // Prefija el usuario logueado (según tu AuthService)
    // Ajusta la propiedad según tu modelo de usuario en el front:
    // puede ser u?.nombreUsuario o u?.nombre_Usuario
    const u = this.auth.currentUser as any;
    const nombreUsuario = u?.nombreUsuario ?? u?.nombre_Usuario ?? '';
    if (nombreUsuario) {
      this.formAbrir.patchValue({ nombreUsuario });
      this.formFiltros.patchValue({ idUsuario: u?.idUsuario ?? u?.id_Usuario ?? null });
    }

    this.cargarTurnos();
  }

  // ====================== Carga de datos ======================
  cargarTurnos(): void {
    const f = this.formFiltros.value;
    const qp: CajaTurnoQueryParams = {
      idCaja: f.idCaja ?? undefined,
      idUsuario: f.idUsuario ?? undefined,
      desde: f.desde || undefined,
      hasta: f.hasta || undefined,
      page: f.page || 1,
      pageSize: f.pageSize || 20
    };

    this.cargando.set(true);
    this.turnosApi.buscar(qp).subscribe({
      next: r => {
        this.turnos.set(r.items || []);
        this.total.set(r.total || 0);
      },
      error: e => {
        this.turnos.set([]);
        this.total.set(0);
        this.alertError(this.extractErr(e) || 'No se pudieron cargar los turnos');
      },
      complete: () => this.cargando.set(false)
    });
  }

  seleccionar(t: CajaTurnoDto): void {
    this.seleccionado.set(t);
  }

  // ====================== Abrir turno ======================
  toggleAbrir(): void {
    this.mostrandoAbrir.update(v => !v);
    if (this.mostrandoAbrir()) {
      // Defaults convenientes
      if (!this.formAbrir.value.saldoInicial) this.formAbrir.patchValue({ saldoInicial: 0 });
      if (!this.formAbrir.value.nombreCaja) this.formAbrir.patchValue({ nombreCaja: '' });
    }
  }

  abrir(): void {
    if (this.formAbrir.invalid) {
      this.formAbrir.markAllAsTouched();
      this.alertInfo('Completa los datos para abrir el turno.');
      return;
    }
    const req = this.formAbrir.value as AbrirTurnoRequest;

    this.cargandoAccion.set(true);
    this.turnosApi.abrir(req).subscribe({
      next: (t) => {
        this.alertOk(`Turno #${t.idTurno} abierto`);
        this.mostrandoAbrir.set(false);
        // deja usuario por defecto para abrir otro rápido, limpia caja/saldo/obs
        const nombreUsuario = this.formAbrir.value.nombreUsuario || '';
        this.formAbrir.reset({ nombreUsuario, nombreCaja: '', saldoInicial: 0, observaciones: null });
        this.cargarTurnos();
        this.seleccionado.set(t);
      },
      error: (e) => this.alertError(this.extractErr(e) || 'No se pudo abrir el turno'),
      complete: () => this.cargandoAccion.set(false)
    });
  }

  // ====================== Cerrar turno ======================
  toggleCerrar(t?: CajaTurnoDto): void {
    const target = t ?? this.seleccionado();
    if (!target) { this.alertInfo('Selecciona un turno.'); return; }

    this.turnoParaCerrarId.set(target.idTurno);
    this.formCerrar.reset({
      saldoCierre: target.saldoCierre ?? 0,
      observaciones: null
    });
    this.mostrandoCerrar.set(true);
  }

  cerrar(): void {
    if (this.formCerrar.invalid) {
      this.formCerrar.markAllAsTouched();
      this.alertInfo('Indica el saldo de cierre.');
      return;
    }
    const idTurno = this.turnoParaCerrarId();
    if (!idTurno) {
      this.alertInfo('No hay turno seleccionado.');
      return;
    }

    const req: CerrarTurnoRequest = {
      saldoCierre: this.formCerrar.value.saldoCierre!,
      observaciones: this.formCerrar.value.observaciones ?? null
    };

    this.cargandoAccion.set(true);
    this.turnosApi.cerrar(idTurno, req).subscribe({
      next: (t) => {
        this.alertOk(`Turno #${t.idTurno} cerrado`);
        this.mostrandoCerrar.set(false);
        this.formCerrar.reset({ saldoCierre: 0, observaciones: null });
        this.cargarTurnos();
        this.seleccionado.set(t);
        this.turnoParaCerrarId.set(null);
      },
      error: (e) => this.alertError(this.extractErr(e) || 'No se pudo cerrar el turno'),
      complete: () => this.cargandoAccion.set(false)
    });
  }

  // ====================== Helpers ======================
  limpiarFiltros(): void {
    const u = this.auth.currentUser as any;
    const idUsuario = u?.idUsuario ?? u?.id_Usuario ?? null;

    this.formFiltros.reset({
      idCaja: null,
      idUsuario,
      desde: '',
      hasta: '',
      page: 1,
      pageSize: 20
    });
    this.cargarTurnos();
  }

  // ====================== Sweet Alerts ======================
  private alertOk(msg: string)   { Swal.fire('OK', msg, 'success'); }
  private alertInfo(msg: string) { Swal.fire('Aviso', msg, 'info'); }
  private alertError(msg: string){ Swal.fire('Error', msg, 'error'); }

  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title) return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }
}
