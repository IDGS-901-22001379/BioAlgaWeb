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

  math = Math;   // <-- expone Math al template
  valOr0(v: number | null | undefined): number { return v ?? 0; }


  // Alias que usa el HTML: lista()
  lista = computed(() => this.turnos());

  seleccionado = signal<CajaTurnoDto | null>(null);

  // ====================== Filtros ======================
  formFiltros = this.fb.group({
    idCaja: [null as number | null],
    idUsuario: [null as number | null],
    desde: [''],
    hasta: [''],
    page: [1],
    pageSize: [20]
  });

  // ====================== Abrir turno ======================
  mostrandoAbrir = signal(false);
  formAbrir = this.fb.group({
    id_Caja: [null as number | null, Validators.required],
    id_Usuario: [null as number | null, Validators.required],
    saldo_Inicial: [0, [Validators.required, Validators.min(0)]],
    observaciones: [null as string | null]
  });

  // ====================== Cerrar turno ======================
  mostrandoCerrar = signal(false);
  formCerrar = this.fb.group({
    id_Turno: [null as number | null, Validators.required],
    saldo_Cierre: [0, [Validators.required, Validators.min(0)]],
    observaciones: [null as string | null]
  });

  // ====================== Lifecycle ======================
  ngOnInit(): void {
    // Prefija usuario logueado si existe (según tu AuthService)
    const u = this.auth.currentUser;
    if (u?.id_Usuario) {
      this.formAbrir.patchValue({ id_Usuario: u.id_Usuario });
      this.formFiltros.patchValue({ idUsuario: u.id_Usuario });
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
      const u = this.auth.currentUser;
      if (u?.id_Usuario) this.formAbrir.patchValue({ id_Usuario: u.id_Usuario });
      if (!this.formAbrir.value.saldo_Inicial) this.formAbrir.patchValue({ saldo_Inicial: 0 });
    }
  }

  abrir(): void {
    if (this.formAbrir.invalid) {
      this.formAbrir.markAllAsTouched();
      this.alertInfo('Completa los datos para abrir el turno.');
      return;
    }
    const req = this.formAbrir.value as unknown as AbrirTurnoRequest;

    this.cargandoAccion.set(true);
    this.turnosApi.abrir(req).subscribe({
      next: (t) => {
        this.alertOk(`Turno #${t.id_Turno} abierto`);
        this.mostrandoAbrir.set(false);
        // deja usuario por defecto para abrir otro rápido
        const uId = this.auth.currentUser?.id_Usuario ?? null;
        this.formAbrir.reset({ id_Caja: null, id_Usuario: uId, saldo_Inicial: 0, observaciones: null });
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

    this.formCerrar.reset({
      id_Turno: target.id_Turno,
      saldo_Cierre: target.saldo_Cierre ?? 0,
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
    const v = this.formCerrar.value;
    const idTurno = v.id_Turno!;
    const req: CerrarTurnoRequest = {
      id_Turno: idTurno,
      saldo_Cierre: v.saldo_Cierre!,
      observaciones: v.observaciones ?? null
    };

    this.cargandoAccion.set(true);
    this.turnosApi.cerrar(idTurno, req).subscribe({
      next: (t) => {
        this.alertOk(`Turno #${t.id_Turno} cerrado`);
        this.mostrandoCerrar.set(false);
        this.formCerrar.reset({ id_Turno: null, saldo_Cierre: 0, observaciones: null });
        this.cargarTurnos();
        this.seleccionado.set(t);
      },
      error: (e) => this.alertError(this.extractErr(e) || 'No se pudo cerrar el turno'),
      complete: () => this.cargandoAccion.set(false)
    });
  }

  // ====================== Helpers ======================
  limpiarFiltros(): void {
    const uId = this.auth.currentUser?.id_Usuario ?? null;
    this.formFiltros.reset({
      idCaja: null,
      idUsuario: uId,
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
