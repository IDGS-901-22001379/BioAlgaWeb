// src/app/pages/movimientos/lista/lista.ts
import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';

// Services
import { CajaMovimientosService } from '../../../services/caja-movimientos.service';
import { CajaTurnosService } from '../../../services/caja-turnos.service';

// Models
import {
  CajaMovimientoDto,
  CrearCajaMovimientoRequest,
  ActualizarCajaMovimientoRequest,
  CajaMovimientoQueryParams,
  TipoMovimiento
} from '../../../models/caja-movimientos.model';
import { CajaTurnoDto } from '../../../models/caja-turno-dtos.model';
import { PagedResponse } from '../../../models/paged-response.model';

@Component({
  selector: 'app-movimientos-lista',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './lista.html',
  styleUrls: ['./lista.css']
})
export class MovimientosListaPage implements OnInit {
  private fb = inject(FormBuilder);
  private movApi = inject(CajaMovimientosService);
  private turnosApi = inject(CajaTurnosService);

  cargando = signal(false);
  cargandoTurno = signal(false);

  // ========= Turno activo / seleccionado =========
  turno = signal<CajaTurnoDto | null>(null);

  // Si llegas a esta pantalla ya con un turno, puedes setearlo desde fuera.
  // Aquí permitimos buscar por idTurno manual.
  formTurno = this.fb.group({
    idTurno: [null as number | null, Validators.required]
  });

  // ========= Filtros y paginación =========
  formFiltros = this.fb.group({
    tipo: ['' as '' | TipoMovimiento],         // 'Ingreso' | 'Egreso' | ''
    q: [''],                                   // texto libre por "concepto"
    page: [1],
    pageSize: [20],
  });

  movimientos = signal<CajaMovimientoDto[]>([]);
  total = signal(0);

  // Totales rápidos
  totalIngresos = computed(() =>
    this.movimientos().filter(m => m.tipo === 'Ingreso').reduce((a, m) => a + m.monto, 0)
  );
  totalEgresos = computed(() =>
    this.movimientos().filter(m => m.tipo === 'Egreso').reduce((a, m) => a + m.monto, 0)
  );
  neto = computed(() => this.totalIngresos() - this.totalEgresos());

  // ========= Crear / Editar =========
  creando = signal(false);
  editandoId = signal<number | null>(null);

  formMovimiento = this.fb.group({
    tipo: ['Ingreso' as TipoMovimiento, Validators.required],
    concepto: ['', [Validators.required, Validators.maxLength(140)]],
    monto: [0, [Validators.required, Validators.min(0.01)]],
    referencia: ['']
  });

  ngOnInit(): void {
    // Si cambian filtros -> recarga
    this.formFiltros.valueChanges
      .pipe(debounceTime(200), distinctUntilChanged())
      .subscribe(() => this.cargarMovimientos());

    // Si el usuario teclea idTurno y presiona Enter desde el template, llamas buscarTurno()
    // Aquí escuchamos cambios por si quieres auto-cargar al escribir:
    // this.formTurno.controls.idTurno.valueChanges?.pipe(debounceTime(300), distinctUntilChanged())
    //   .subscribe(() => this.buscarTurno());
  }

  // =====================================================
  // Turno
  // =====================================================
  buscarTurno(): void {
    const id = this.formTurno.value.idTurno;
    if (!id || id <= 0) {
      Swal.fire('Aviso', 'Captura un Id de turno válido.', 'info');
      return;
    }

    this.cargandoTurno.set(true);
    this.turnosApi.getById(+id).subscribe({
      next: t => {
        this.turno.set(t);
        this.cargarMovimientos(true);
      },
      error: (e) => {
        this.turno.set(null);
        this.movimientos.set([]);
        this.total.set(0);
        Swal.fire('No encontrado', this.extractErr(e) || 'No se encontró el turno.', 'warning');
      },
      complete: () => this.cargandoTurno.set(false)
    });
  }

  limpiarTurno(): void {
    this.turno.set(null);
    this.formTurno.reset({ idTurno: null });
    this.movimientos.set([]);
    this.total.set(0);
  }

  // =====================================================
  // Listado
  // =====================================================
  cargarMovimientos(resetPage = false): void {
    const t = this.turno();
    if (!t) return;

    const f = this.formFiltros.value;
    const params: CajaMovimientoQueryParams = {
      idTurno: t.id_Turno,
      tipo: (f.tipo || undefined) as any,
      q: (f.q || undefined) as any,
      page: resetPage ? 1 : (f.page ?? 1),
      pageSize: f.pageSize ?? 20
    };

    if (resetPage) this.formFiltros.patchValue({ page: 1 }, { emitEvent: false });

    this.cargando.set(true);
    this.movApi.buscarPorTurno(params).subscribe({
      next: (resp: PagedResponse<CajaMovimientoDto>) => {
        this.movimientos.set(resp.items || []);
        this.total.set(resp.total || 0);
      },
      error: (e) => {
        this.movimientos.set([]);
        this.total.set(0);
        Swal.fire('Error', this.extractErr(e) || 'No se pudieron cargar los movimientos.', 'error');
      },
      complete: () => this.cargando.set(false)
    });
  }

  // Paginación sencilla
  gotoPage(p: number): void {
    if (p <= 0) return;
    this.formFiltros.patchValue({ page: p }); // dispara cargarMovimientos por valueChanges
  }

  // =====================================================
  // Crear Movimiento
  // =====================================================
  abrirCrear(tipo: TipoMovimiento): void {
    this.creando.set(true);
    this.editandoId.set(null);
    this.formMovimiento.reset({
      tipo,
      concepto: '',
      monto: 0,
      referencia: ''
    });
  }

  guardarNuevo(): void {
    const t = this.turno();
    if (!t) { Swal.fire('Aviso', 'Selecciona un turno primero.', 'info'); return; }

    if (this.formMovimiento.invalid) {
      Swal.fire('Campos requeridos', 'Completa los datos del movimiento.', 'warning');
      return;
    }

    const body: CrearCajaMovimientoRequest = {
      id_Turno: t.id_Turno,
      tipo: this.formMovimiento.value.tipo as TipoMovimiento,
      concepto: this.formMovimiento.value.concepto?.trim() || '',
      monto: +this.formMovimiento.value.monto!,
      referencia: this.formMovimiento.value.referencia?.trim() || undefined
    };

    this.cargando.set(true);
    this.movApi.create(body).subscribe({
      next: m => {
        Swal.fire('Registrado', `Movimiento ${m.tipo.toLowerCase()} por $${m.monto.toFixed(2)} creado.`, 'success');
        this.creando.set(false);
        this.cargarMovimientos(true);
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo crear el movimiento.', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  cancelarCrear(): void {
    this.creando.set(false);
  }

  // =====================================================
  // Editar / Eliminar
  // =====================================================
  abrirEditar(m: CajaMovimientoDto): void {
    this.editandoId.set(m.id_Mov);
    this.creando.set(false);
    this.formMovimiento.reset({
      tipo: m.tipo,
      concepto: m.concepto,
      monto: m.monto,
      referencia: m.referencia || ''
    });
  }

  guardarEdicion(): void {
    const id = this.editandoId();
    if (!id) return;

    if (this.formMovimiento.invalid) {
      Swal.fire('Campos requeridos', 'Completa los datos del movimiento.', 'warning');
      return;
    }

    const body: ActualizarCajaMovimientoRequest = {
      tipo: this.formMovimiento.value.tipo as TipoMovimiento,
      concepto: this.formMovimiento.value.concepto?.trim() || '',
      monto: +this.formMovimiento.value.monto!,
      referencia: this.formMovimiento.value.referencia?.trim() || undefined
    };

    this.cargando.set(true);
    this.movApi.update(id, body).subscribe({
      next: m => {
        Swal.fire('Actualizado', `Movimiento #${m.id_Mov} modificado.`, 'success');
        this.editandoId.set(null);
        this.cargarMovimientos();
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo actualizar el movimiento.', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  cancelarEdicion(): void {
    this.editandoId.set(null);
  }

  eliminar(m: CajaMovimientoDto): void {
    Swal.fire({
      title: 'Eliminar movimiento',
      html: `
        <div class="text-start">
          <div>Tipo: <b>${m.tipo}</b></div>
          <div>Monto: <b>$${m.monto.toFixed(2)}</b></div>
          <div class="small text-muted">Concepto: ${this.escapeHtml(m.concepto)}</div>
        </div>`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    }).then(res => {
      if (!res.isConfirmed) return;

      this.cargando.set(true);
      this.movApi.remove(m.id_Mov).subscribe({
        next: () => {
          Swal.fire('Eliminado', 'Movimiento eliminado.', 'success');
          this.cargarMovimientos();
        },
        error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo eliminar.', 'error'),
        complete: () => this.cargando.set(false)
      });
    });
  }

  // =====================================================
  // Utils
  // =====================================================
  private extractErr(e: any): string {
    if (e instanceof HttpErrorResponse) {
      const err = e.error;
      if (err?.detail) return err.detail;
      if (err?.title) return err.title;
      if (typeof err === 'string') return err;
    }
    if (typeof e?.message === 'string') return e.message;
    return '';
  }

  private escapeHtml(s: string): string {
    return (s || '').replace(/[&<>"']/g, (m) =>
      ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' } as any)[m]
    );
  }
}
