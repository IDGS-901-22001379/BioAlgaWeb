import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import Swal from 'sweetalert2';

// Servicios
import { DevolucionesService } from '../../services/devoluciones.service';

// Modelos
import {
  DevolucionCreateRequest,
  DevolucionLineaCreate,
  DevolucionDto,
  DevolucionQueryParams,
  PagedResponse
} from '../../models/devolucion-dtos.model';

@Component({
  selector: 'app-devoluciones-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './devoluciones.html',
  styleUrls: ['./devoluciones.css']
})
export class DevolucionesPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private devApi = inject(DevolucionesService);

  cargando = signal(false);
  cargandoLista = signal(false);

  // ======= Formularios =======
  form = this.fb.group({
    motivo: ['', [Validators.required, Validators.minLength(3)]],
    referencia: [null as string | null],
    notas: [null as string | null],
    reingresaInventario: [true]
  });

  formLinea = this.fb.group({
    idProducto: [null as number | null, Validators.required],
    cantidad: [1, [Validators.required, Validators.min(1)]],
    importeLineaTotal: [0, [Validators.required, Validators.min(0)]],
    productoNombre: [null as string | null]
  });

  // ======= Líneas =======
  devLineas = signal<Array<DevolucionLineaCreate & { productoNombre?: string }>>([]);

  totalDevuelto = computed(() =>
    this.devLineas().reduce((acc, l) => acc + (l.importeLineaTotal || 0), 0)
  );

  // ======= Listado =======
  lista = signal<DevolucionDto[]>([]);
  total = signal(0);
  page = signal(1);
  pageSize = signal(10);
  filtroTexto = signal('');

  ngOnInit(): void {
    this.cargarLista();
  }

  // ======= Manejo de líneas =======
  agregarLinea(): void {
    const v = this.formLinea.getRawValue();
    if (!v.idProducto || v.cantidad! <= 0) {
      Swal.fire('Aviso', 'Debes ingresar un producto y cantidad válidos.', 'info');
      return;
    }

    const nueva: DevolucionLineaCreate & { productoNombre?: string } = {
      idProducto: v.idProducto!,
      cantidad: v.cantidad!,
      importeLineaTotal: v.importeLineaTotal ?? 0,
      productoNombre: v.productoNombre ?? undefined
    };

    this.devLineas.update(a => [...a, nueva]);
    this.formLinea.reset({ idProducto: null, cantidad: 1, importeLineaTotal: 0, productoNombre: null });
  }

  quitarLinea(i: number): void {
    this.devLineas.update(a => a.filter((_, idx) => idx !== i));
  }

  limpiarTodo(): void {
    this.form.reset({ motivo: '', referencia: null, notas: null, reingresaInventario: true });
    this.formLinea.reset({ idProducto: null, cantidad: 1, importeLineaTotal: 0, productoNombre: null });
    this.devLineas.set([]);
  }

  // ======= Guardar =======
  guardar(): void {
    if (this.form.invalid || !this.devLineas().length) {
      Swal.fire('Aviso', 'Debes llenar el formulario y agregar al menos una línea.', 'info');
      return;
    }

    const body: DevolucionCreateRequest = {
      motivo: this.form.value.motivo!.trim(),
      referencia: this.form.value.referencia || null,
      notas: this.form.value.notas || null,
      regresaInventario: !!this.form.value.reingresaInventario,
      lineas: this.devLineas().map(l => ({
        idProducto: l.idProducto,
        cantidad: l.cantidad,
        importeLineaTotal: l.importeLineaTotal
      }))
    };

    this.cargando.set(true);
    this.devApi.create(body).subscribe({
      next: (resp: DevolucionDto) => {
        Swal.fire('Devolución registrada', `Folio DEV #${resp.idDevolucion}`, 'success');
        this.limpiarTodo();
        this.cargarLista();
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo registrar la devolución', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  // ======= Listado =======
  cargarLista(): void {
    const q: DevolucionQueryParams = {
      q: this.filtroTexto() || undefined,
      page: this.page(),
      pageSize: this.pageSize(),
      sortBy: 'fecha',
      sortDir: 'desc'
    };

    this.cargandoLista.set(true);
    this.devApi.buscar(q).subscribe({
      next: (resp: PagedResponse<DevolucionDto>) => {
        this.lista.set(resp.items || []);
        this.total.set(resp.total || resp.items.length);
      },
      error: () => { this.lista.set([]); this.total.set(0); },
      complete: () => this.cargandoLista.set(false)
    });
  }

  cambiarPagina(delta: number): void {
    const nueva = this.page() + delta;
    if (nueva < 1) return;
    this.page.set(nueva);
    this.cargarLista();
  }

  verDetalle(d: DevolucionDto): void {
    const html = `
      <div class="text-start">
        <div><b>Folio:</b> #${d.idDevolucion}</div>
        <div><b>Fecha:</b> ${new Date(d.fecha!).toLocaleString()}</div>
        <div><b>Motivo:</b> ${d.motivo}</div>
        <div><b>Referencia:</b> ${d.referencia || '-'}</div>
        <div><b>Total devuelto:</b> $${(d.totalDevuelto ?? 0).toFixed(2)}</div>
      </div>`;
    Swal.fire({ title: 'Detalle de devolución', html, icon: 'info' });
  }

  // ======= Utils =======
  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title) return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }
}
