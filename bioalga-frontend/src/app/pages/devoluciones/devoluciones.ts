import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, forkJoin, of, switchMap } from 'rxjs';
import Swal from 'sweetalert2';

// Servicios
import { DevolucionesService } from '../../services/devoluciones.service';
import { PreciosService } from '../../services/precios.service';
import { ComprasService } from '../../services/compras.service';
import { InventarioService } from '../../services/inventario.service';
import { UsuariosService } from '../../services/usuarios.service';

// Modelos
import {
  DevolucionCreateRequest,
  DevolucionLineaCreate,
  DevolucionDto
} from '../../models/devolucion-dtos.model';

import { ProductoLookupDto } from '../../models/producto-lookup.model';
import { StockResponse } from '../../models/inventario.model';
import { TipoPrecio } from '../../models/precio.model';
import { UsuarioDto, PagedResponse } from '../../models/usuario.models';

// Para mostrar en tarjetas/tabla de resultados
type ProductoCard = ProductoLookupDto & { precio?: number | null; stock?: number | null };

// Usuario “ligero” para el autocomplete
type UsuarioCard = {
  idUsuario: number;
  nombreUsuario?: string | null;
  empleadoNombreCompleto?: string | null;
  nombre?: string | null;
  apellidoPaterno?: string | null;
  apellidoMaterno?: string | null;
};

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
  private preciosApi = inject(PreciosService);
  private comprasApi = inject(ComprasService);
  private invApi = inject(InventarioService);
  private usuariosApi = inject(UsuariosService);

  cargando = signal(false);

  // ======= búsqueda de productos =======
  formBuscar = this.fb.group({ q: [''] });
  showSugerencias = signal(false);
  sugerencias = signal<ProductoCard[]>([]);
  resultados  = signal<ProductoCard[]>([]);
  selectedIndex = signal<number>(-1);

  // ======= búsqueda de usuarios (autocomplete) =======
  formUsuario = this.fb.group({ qUser: [''] });
  showUserSugs = signal(false);
  userSugs = signal<UsuarioCard[]>([]);
  userSelectedIndex = signal<number>(-1);

  // ======= formulario de devolución (cabecera)
  formDev = this.fb.group({
    ventaId: [null as number | null],
    referenciaVenta: [null as string | null, []],
    usuarioNombre: ['', [Validators.required, Validators.maxLength(120)]],
    motivo: ['', [Validators.required, Validators.maxLength(300)]],
    regresaInventario: [true],
    notas: [null as string | null]
  });

  // ======= líneas (detalle)
  lineas = signal<Array<DevolucionLineaCreate & { nombre?: string; precioUnitario?: number | null }>>([]);

  // ======= totales
  total = computed(() =>
    this.lineas().reduce((ac, l) => ac + (l.precioUnitario ? l.precioUnitario * l.cantidad : 0), 0)
  );

  // ======= Listado & Detalle (modales) =======
  showListadoModal = signal(false);
  listado = signal<DevolucionDto[]>([]);
  cargandoListado = signal(false);
  filtroDesde = signal<string>(this.hoyISO());
  filtroHasta = signal<string>(this.hoyISO());

  showDetalleModal = signal(false);
  detalleDevolucion = signal<DevolucionDto | null>(null);
  cargandoDetalle = signal(false);

  ngOnInit(): void {
    // ----- Autocomplete productos -----
    this.formBuscar.controls['q'].valueChanges?.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(q => {
        const s = (q ?? '').toString().trim();
        if (s.length < 2) { this.showSugerencias.set(false); this.selectedIndex.set(-1); return of([] as ProductoLookupDto[]); }
        this.showSugerencias.set(true);
        return this.comprasApi.buscarProductos(s, 12);
      })
    ).subscribe({
      next: list => { this.enriquecerResultados(list); this.resetSelection(); },
      error: () => { this.sugerencias.set([]); this.resultados.set([]); this.resetSelection(); }
    });

    // ----- Autocomplete usuarios (usa UsuariosService.buscar que devuelve PagedResponse<UsuarioDto>) -----
    this.formUsuario.controls['qUser'].valueChanges?.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(q => {
        const s = (q ?? '').toString().trim();
        if (s.length < 2) { this.showUserSugs.set(false); this.userSelectedIndex.set(-1); return of(null as PagedResponse<UsuarioDto> | null); }
        this.showUserSugs.set(true);
        // nombre, page=1, pageSize=10, activo? (true para activos)
        return this.usuariosApi.buscar(s, 1, 10, true);
      })
    ).subscribe({
      next: (resp) => {
        const users: UsuarioDto[] =
          (resp?.items as any[]) ??
          (resp as any)?.data ??
          (resp as any)?.results ??
          [];
        const mapped: UsuarioCard[] = users.map(u => ({
          idUsuario: (u as any).idUsuario ?? (u as any).id_Usuario ?? (u as any).id ?? 0,
          nombreUsuario: (u as any).nombreUsuario ?? (u as any).username ?? null,
          empleadoNombreCompleto: (u as any).empleadoNombreCompleto ?? (u as any).nombreCompleto ?? null,
          nombre: (u as any).nombre ?? (u as any).empleado?.nombre ?? null,
          apellidoPaterno: (u as any).apellidoPaterno ?? (u as any).empleado?.apellidoPaterno ?? null,
          apellidoMaterno: (u as any).apellidoMaterno ?? (u as any).empleado?.apellidoMaterno ?? null,
        }));
        this.userSugs.set(mapped);
        this.userSelectedIndex.set(mapped.length ? 0 : -1);
      },
      error: () => { this.userSugs.set([]); this.userSelectedIndex.set(-1); }
    });
  }

  // ======= helpers UI búsqueda productos =======
  limpiarBusqueda(): void {
    this.formBuscar.patchValue({ q: '' });
    this.sugerencias.set([]);
    this.resultados.set([]);
    this.showSugerencias.set(false);
    this.resetSelection();
  }
  forzarBusqueda(): void {
    const q = (this.formBuscar.value.q || '').toString().trim();
    if (q.length >= 2) {
      this.comprasApi.buscarProductos(q, 20).subscribe({
        next: list => { this.showSugerencias.set(false); this.enriquecerResultados(list); this.resetSelection(); },
        error: () => { this.resultados.set([]); this.resetSelection(); }
      });
    }
  }
  onBuscadorKey(ev: KeyboardEvent): void {
    const len = this.resultados().length;
    if (!len) return;
    if (ev.key === 'ArrowDown') { ev.preventDefault(); this.moveSelection(+1); }
    else if (ev.key === 'ArrowUp') { ev.preventDefault(); this.moveSelection(-1); }
    else if (ev.key === 'Enter') {
      ev.preventDefault();
      const idx = this.selectedIndex();
      const item = (idx >= 0 && idx < len) ? this.resultados()[idx] : this.resultados()[0];
      if (item) this.agregarProducto(item);
    }
  }
  setSelected(i: number): void { this.selectedIndex.set(i); }
  private moveSelection(delta: number): void {
    const len = this.resultados().length;
    if (!len) { this.selectedIndex.set(-1); return; }
    let idx = this.selectedIndex();
    idx = (idx < 0) ? 0 : idx + delta;
    if (idx < 0) idx = 0;
    if (idx > len - 1) idx = len - 1;
    this.selectedIndex.set(idx);
    const el = document.getElementById('row-res-' + idx);
    if (el) el.scrollIntoView({ block: 'nearest' });
  }
  private resetSelection(): void {
    this.selectedIndex.set(this.resultados().length ? 0 : -1);
  }

  // ======= helpers UI búsqueda usuarios =======
  limpiarBusquedaUsuario(): void {
    this.formUsuario.patchValue({ qUser: '' });
    this.userSugs.set([]);
    this.showUserSugs.set(false);
    this.userSelectedIndex.set(-1);
  }
  onUsuarioKey(ev: KeyboardEvent): void {
    const len = this.userSugs().length;
    if (!len) return;
    if (ev.key === 'ArrowDown') { ev.preventDefault(); this.userMoveSelection(+1); }
    else if (ev.key === 'ArrowUp') { ev.preventDefault(); this.userMoveSelection(-1); }
    else if (ev.key === 'Enter') {
      ev.preventDefault();
      const idx = this.userSelectedIndex();
      const item = (idx >= 0 && idx < len) ? this.userSugs()[idx] : this.userSugs()[0];
      if (item) this.elegirUsuario(item);
    }
  }
  setUserSelected(i: number): void { this.userSelectedIndex.set(i); }
  private userMoveSelection(delta: number): void {
    const len = this.userSugs().length;
    if (!len) { this.userSelectedIndex.set(-1); return; }
    let idx = this.userSelectedIndex();
    idx = (idx < 0) ? 0 : idx + delta;
    if (idx < 0) idx = 0;
    if (idx > len - 1) idx = len - 1;
    this.userSelectedIndex.set(idx);
    const el = document.getElementById('row-user-' + idx);
    if (el) el.scrollIntoView({ block: 'nearest' });
  }

  // Construye un nombre bonito del usuario
  getUserDisplay(u: UsuarioCard): string {
    if (u.empleadoNombreCompleto) return u.empleadoNombreCompleto;
    const partes = [u.nombre, u.apellidoPaterno, u.apellidoMaterno].filter(Boolean);
    if (partes.length) return partes.join(' ');
    return u.nombreUsuario || `Usuario #${u.idUsuario}`;
  }

  // Al elegir uno, llenamos el form
  elegirUsuario(u: UsuarioCard): void {
    this.formDev.patchValue({ usuarioNombre: this.getUserDisplay(u) });
    this.showUserSugs.set(false);
  }

  // ======= enriquecer resultados con precio/stock (para devoluciones sin venta)
  private enriquecerResultados(list: ProductoLookupDto[], soloPrecio = false): void {
    const tipo: TipoPrecio = 'Normal';
    const precios$ = forkJoin(list.map(p => this.preciosApi.obtenerVigente(p.id_Producto, tipo)));

    if (soloPrecio) {
      precios$.subscribe(prices => {
        const enriched: ProductoCard[] = list.map((p, i) => ({ ...p, precio: prices[i] ?? 0 }));
        this.sugerencias.set(enriched);
        this.resultados.set(enriched);
      });
      return;
    }

    const stocks$ = forkJoin(list.map(p => this.invApi.stockActual(p.id_Producto)));
    forkJoin([precios$, stocks$]).subscribe({
      next: ([prices, stocks]) => {
        const enriched: ProductoCard[] = list.map((p, i) => {
          const st = stocks[i] as StockResponse | null;
          const stock = st ? ((st as any).stock_Actual ?? (st as any).stock ?? 0) : 0;
          return { ...p, precio: prices[i] ?? 0, stock };
        });
        this.sugerencias.set(enriched);
        this.resultados.set(enriched);
      },
      error: () => {
        this.sugerencias.set(list as any);
        this.resultados.set(list as any);
      }
    });
  }

  // ======= líneas =======
  elegirProducto(p: ProductoLookupDto): void {
    this.agregarProducto(p);
  }

  agregarProducto(p: ProductoLookupDto): void {
    this.showSugerencias.set(false);

    const idx = this.lineas().findIndex(x => x.idProducto === p.id_Producto);
    if (idx >= 0) { this.cambiarCantidad(idx, +1); return; }

    const ventaId = this.formDev.value.ventaId;
    const precioVisible = (p as any).precio as number | undefined;

    const pushLinea = (precio?: number | null) => {
      this.lineas.update(arr => [...arr, {
        idProducto: p.id_Producto,
        productoNombre: (p as any).nombre ?? '',
        cantidad: 1,
        idDetalleVenta: null,
        precioUnitario: ventaId ? null : (precio ?? 0)
      }]);
    };

    if (ventaId) { pushLinea(null); return; }
    if (precioVisible != null) { pushLinea(+precioVisible); return; }

    this.cargando.set(true);
    this.preciosApi.obtenerVigente(p.id_Producto, 'Normal').subscribe({
      next: (precio) => pushLinea(precio ?? 0),
      error: () => pushLinea(0),
      complete: () => this.cargando.set(false)
    });
  }

  cambiarCantidad(idx: number, delta: number): void {
    this.lineas.update(arr => arr.map((l, i) => i === idx ? { ...l, cantidad: Math.max(1, l.cantidad + delta) } : l));
  }

  eliminarLinea(idx: number): void {
    this.lineas.update(arr => arr.filter((_, i) => i !== idx));
  }

  editarPrecio(idx: number, nuevo: number): void {
    const ventaId = this.formDev.value.ventaId;
    if (ventaId) return;
    this.lineas.update(arr => arr.map((l, i) => i === idx ? { ...l, precioUnitario: Math.max(0, +nuevo) } : l));
  }

  vaciar(): void {
    if (!this.lineas().length) return;
    Swal.fire({ title: '¿Vaciar líneas?', icon: 'warning', showCancelButton: true })
      .then(r => { if (r.isConfirmed) this.lineas.set([]); });
  }

  // ======= Guardar (POST) =======
  registrarDevolucion(): void { this.guardar(); }

  guardar(): void {
    if (!this.formDev.valid) { Swal.fire('Aviso', 'Completa los datos requeridos.', 'info'); return; }
    if (!this.lineas().length) { Swal.fire('Aviso', 'Agrega al menos una línea.', 'info'); return; }

    const ventaId = this.formDev.value.ventaId;
    if (!ventaId) {
      const sinPrecio = this.lineas().some(l => l.precioUnitario == null || isNaN(+l.precioUnitario!));
      if (sinPrecio) { Swal.fire('Aviso', 'Hay líneas sin precio unitario.', 'info'); return; }
    }

    const req: DevolucionCreateRequest = {
      ventaId: this.formDev.value.ventaId ?? null,
      referenciaVenta: this.formDev.value.referenciaVenta ?? null,
      usuarioNombre: this.formDev.value.usuarioNombre!,
      motivo: this.formDev.value.motivo!,
      regresaInventario: !!this.formDev.value.regresaInventario,
      notas: this.formDev.value.notas ?? null,
      lineas: this.lineas().map(l => ({
        idProducto: l.idProducto,
        productoNombre: l.productoNombre,
        cantidad: l.cantidad,
        idDetalleVenta: l.idDetalleVenta ?? null,
        precioUnitario: ventaId ? null : (l.precioUnitario ?? 0)
      }))
    };

    this.cargando.set(true);
    this.devApi.create(req).subscribe({
      next: (d: DevolucionDto) => {
        Swal.fire({
          title: `Devolución #${d.idDevolucion} registrada`,
          html: `
            <div class="text-start">
              <div>Total devuelto: <b>$${(d.totalDevuelto ?? 0).toFixed(2)}</b></div>
              <div class="small text-muted mt-2">Regresa a inventario: ${d.regresaInventario ? 'Sí' : 'No'}</div>
            </div>`,
          icon: 'success'
        });
        this.formDev.reset({
          ventaId: null,
          referenciaVenta: null,
          usuarioNombre: '',
          motivo: '',
          regresaInventario: true,
          notas: null
        });
        this.lineas.set([]);
        this.limpiarBusqueda();
        this.limpiarBusquedaUsuario();
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo registrar la devolución', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  // ======= Listado & Detalle =======
  abrirListado(): void { this.showListadoModal.set(true); this.cargarListado(); }
  cerrarListado(): void { this.showListadoModal.set(false); }

  cargarListado(): void {
    const desde = `${this.filtroDesde()}T00:00:00`;
    const hasta = `${this.filtroHasta()}T23:59:59`;

    this.cargandoListado.set(true);
    this.devApi.listar({ desde, hasta }).subscribe({
      next: (arr) => this.listado.set(arr || []),
      error: (e) => {
        this.listado.set([]);
        Swal.fire('Error', this.extractErr(e) || 'No se pudieron cargar las devoluciones', 'error');
      },
      complete: () => this.cargandoListado.set(false)
    });
  }

  verDetalle(idDevolucion: number): void {
    this.cargandoDetalle.set(true);
    this.devApi.getById(idDevolucion).subscribe({
      next: (dto) => { this.detalleDevolucion.set(dto); this.showDetalleModal.set(true); },
      error: (e) => {
        this.detalleDevolucion.set(null);
        Swal.fire('Error', this.extractErr(e) || 'No se pudo cargar el detalle', 'error');
      },
      complete: () => this.cargandoDetalle.set(false)
    });
  }
  cerrarDetalle(): void { this.showDetalleModal.set(false); this.detalleDevolucion.set(null); }

  // ======= util =======
  private hoyISO(): string {
    const d = new Date();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${d.getFullYear()}-${mm}-${dd}`;
  }

  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title) return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }
}
