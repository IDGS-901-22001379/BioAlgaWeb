import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, forkJoin, of } from 'rxjs';
import Swal from 'sweetalert2';

// Servicios
import { VentasService } from '../../services/ventas.service';
import { PreciosService } from '../../services/precios.service';
import { ClientesService } from '../../services/clientes.service';
import { ComprasService } from '../../services/compras.service';
import { InventarioService } from '../../services/inventario.service';

// Modelos (ventas)
import {
  VentaCreateRequest,
  VentaLineaCreate,
  MetodoPago,
  VentaResumenDto,
  VentaDetalleDto,
  VentaQueryParams,
} from '../../models/venta-dtos.model';

// Modelos (productos / precios / clientes / inv)
import { ProductoLookupDto } from '../../models/producto-lookup.model';
import { TipoPrecio } from '../../models/precio.model';
import { ClienteDto } from '../../models/cliente.model';
import { StockResponse } from '../../models/inventario.model';
import { PagedResponse } from '../../models/paged-response.model';

type ProductoCard = ProductoLookupDto & { precio?: number | null; stock?: number | null };

@Component({
  selector: 'app-ventas-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './ventas.html',
  styleUrls: ['./ventas.css']
})
export class VentasPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private ventasApi = inject(VentasService);
  private preciosApi = inject(PreciosService);
  private clientesApi = inject(ClientesService);
  private comprasApi = inject(ComprasService);
  private invApi = inject(InventarioService);

  cargando = signal(false);

  // ======= Búsqueda
  formBuscar = this.fb.group({ q: [''] });
  showSugerencias = signal(false);
  sugerencias = signal<ProductoCard[]>([]);
  resultados  = signal<ProductoCard[]>([]);

  // Índice seleccionado en la tabla de resultados
  selectedIndex = signal<number>(-1);

  // ======= Venta / Carrito
  formVenta = this.fb.group({
    clienteId: [null as number | null],
    metodoPago: ['Efectivo' as MetodoPago, Validators.required],
    efectivoRecibido: [0],
    ivaPct: [16],
    tipoPrecio: ['Normal' as TipoPrecio]
  });

  clienteDetectado = signal<ClienteDto | null>(null);
  lineas = signal<Array<VentaLineaCreate & { nombre?: string }>>([]);

  // Totales
  subtotal = computed(() =>
    this.lineas().reduce((acc, l) => acc + (l.precioUnitario - l.descuentoUnitario) * l.cantidad, 0)
  );
  impuestos = computed(() =>
    this.lineas().reduce((acc, l) => acc + (l.ivaUnitario * l.cantidad), 0)
  );
  total = computed(() => this.subtotal() + this.impuestos());
  cambio = computed(() => {
    const mp = this.formVenta.value.metodoPago;
    const recibido = +(this.formVenta.value.efectivoRecibido || 0);
    if (mp === 'Efectivo' || mp === 'Mixto') return Math.max(0, recibido - this.total());
    return 0;
  });

  // ======= MODALES =======
  showVentasDiaModal = signal(false);    // Ventas del día / por fecha
  showDetalleModal = signal(false);      // Detalle de una venta

  // ======= Ventas día/fecha =======
  fechaSeleccionada = signal<string>(this.hoyISO()); // yyyy-MM-dd
  ventasDelDia = signal<VentaResumenDto[]>([]);
  totalVentasDelDia = signal<number>(0);
  cargandoVentasDia = signal(false);

  // ======= Detalle de venta =======
  ventaDetalle = signal<VentaDetalleDto | null>(null);
  cargandoDetalle = signal(false);

  ngOnInit(): void {
    // Autocomplete + enriquecimiento (precio/stock)
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

    // Cambia cliente -> set tipoPrecio y refresca precios
    this.formVenta.controls['clienteId'].valueChanges?.pipe(
      debounceTime(250),
      distinctUntilChanged()
    ).subscribe(id => {
      if (!id) {
        this.clienteDetectado.set(null);
        this.formVenta.patchValue({ tipoPrecio: 'Normal' }, { emitEvent: true });
        return;
      }
      this.clientesApi.getById(+id).subscribe({
        next: cli => {
          this.clienteDetectado.set(cli);
          const tipo = (cli.tipo_Cliente as TipoPrecio) ?? 'Normal';
          this.formVenta.patchValue({ tipoPrecio: tipo }, { emitEvent: true });
          this.actualizarPreciosPorTipo();
        },
        error: () => {
          this.clienteDetectado.set(null);
          this.formVenta.patchValue({ tipoPrecio: 'Normal' }, { emitEvent: true });
          this.actualuarPreciosSiHayLineas();
        }
      });
    });

    // Cambia tipoPrecio -> refresca precios de líneas y resultados
    this.formVenta.controls['tipoPrecio'].valueChanges?.subscribe(() => {
      this.actualizarPreciosPorTipo();
      const current = this.resultados();
      if (current.length) this.enriquecerResultados(current, /*soloPrecio*/true);
    });

    // Cambia IVA% -> recalcular IVA unitario de cada línea
    this.formVenta.controls['ivaPct'].valueChanges?.subscribe(() => {
      this.recalcularIvaDeLineas();
    });
  }

  // ===== Helpers UI =====
  focusInput(): void {}
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
  elegirProducto(p: ProductoLookupDto): void {
    this.showSugerencias.set(false);
    this.agregarProductoRapido(p);
  }

  // ===== Navegación con teclado en el input de búsqueda
  onBuscadorKey(ev: KeyboardEvent): void {
    const len = this.resultados().length;
    if (!len) return;

    if (ev.key === 'ArrowDown') {
      ev.preventDefault();
      this.moveSelection(+1);
    } else if (ev.key === 'ArrowUp') {
      ev.preventDefault();
      this.moveSelection(-1);
    } else if (ev.key === 'Enter') {
      ev.preventDefault();
      const idx = this.selectedIndex();
      const item = (idx >= 0 && idx < len) ? this.resultados()[idx] : this.resultados()[0];
      if (item) this.agregarProductoRapido(item);
    }
  }
  setSelected(i: number): void {
    this.selectedIndex.set(i);
  }
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

  // ===== Enriquecer resultados con precio y stock
  private enriquecerResultados(list: ProductoLookupDto[], soloPrecio = false): void {
    const tipo = (this.formVenta.value.tipoPrecio as TipoPrecio) || 'Normal';
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

  // ===== Cálculos (ventas)
  private calcularIvaUnitario(precio: number): number {
    const pct = Number(this.formVenta.value.ivaPct ?? 0);
    return +(precio * (pct / 100)).toFixed(2);
  }
  private recalcularIvaDeLineas(): void {
    this.lineas.update(arr => arr.map(l => ({
      ...l,
      ivaUnitario: this.calcularIvaUnitario(l.precioUnitario)
    })));
  }

  // ===== Carrito (ventas)
  agregarProductoRapido(p: ProductoLookupDto): void {
    const idx = this.lineas().findIndex(x => x.idProducto === p.id_Producto);
    if (idx >= 0) { this.cambiarCantidad(idx, +1); return; }

    const tipo = (this.formVenta.value.tipoPrecio as TipoPrecio) || 'Normal';
    const agregar = (precio: number) => {
      const ivaU = this.calcularIvaUnitario(precio);
      this.lineas.update(arr => [...arr, {
        idProducto: p.id_Producto,
        cantidad: 1,
        precioUnitario: precio,
        descuentoUnitario: 0,
        ivaUnitario: ivaU,
        nombre: (p as any).nombre
      }]);
    };

    const precioCard = (p as any).precio;
    if (precioCard != null) { agregar(+precioCard); return; }

    this.cargando.set(true);
    this.preciosApi.obtenerVigente(p.id_Producto, tipo).subscribe({
      next: (precio) => agregar(precio ?? 0),
      error: () => agregar(0),
      complete: () => this.cargando.set(false)
    });
  }

  cambiarCantidad(idx: number, delta: number): void {
    this.lineas.update(arr => arr.map((l, i) => i === idx ? { ...l, cantidad: Math.max(1, l.cantidad + delta) } : l));
  }
  eliminarLinea(idx: number): void {
    this.lineas.update(arr => arr.filter((_, i) => i !== idx));
  }
  vaciarCarrito(): void {
    if (!this.lineas().length) return;
    Swal.fire({ title: '¿Vaciar carrito?', icon: 'warning', showCancelButton: true })
      .then(r => { if (r.isConfirmed) this.lineas.set([]); });
  }
  recalcular(): void {
    this.lineas.update(arr => [...arr]);
  }

  private actualizarPreciosPorTipo(): void {
    const tipo = (this.formVenta.value.tipoPrecio as TipoPrecio) || 'Normal';
    const arr = this.lineas();
    if (!arr.length) return;

    this.cargando.set(true);
    forkJoin(arr.map(l => this.preciosApi.obtenerVigente(l.idProducto, tipo))).subscribe({
      next: precios => {
        this.lineas.update(a => a.map((l, i) => ({
          ...l,
          precioUnitario: precios[i] ?? l.precioUnitario,
          ivaUnitario: this.calcularIvaUnitario(precios[i] ?? l.precioUnitario)
        })));
      },
      error: () => {},
      complete: () => this.cargando.set(false)
    });
  }
  private actualuarPreciosSiHayLineas(): void {
    if (this.lineas().length) this.actualizarPreciosPorTipo();
  }

  // ===== Acciones (ventas)
  cobrar(): void {
    if (!this.lineas().length) { Swal.fire('Aviso', 'Agrega al menos un producto.', 'info'); return; }

    const mp = this.formVenta.value.metodoPago! as MetodoPago;
    const req: VentaCreateRequest = {
      clienteId: this.formVenta.value.clienteId ?? null,
      metodoPago: mp,
      efectivoRecibido: this.formVenta.value.efectivoRecibido ?? 0,
      lineas: this.lineas().map(l => ({
        idProducto: l.idProducto,
        cantidad: l.cantidad,
        precioUnitario: l.precioUnitario,
        descuentoUnitario: l.descuentoUnitario,
        ivaUnitario: l.ivaUnitario
      }))
    };

    this.cargando.set(true);
    this.ventasApi.create(req).subscribe({
      next: v => {
        Swal.fire({
          title: `Venta #${v.idVenta} registrada`,
          html: this.ventaDialogHtml(v),
          icon: 'success'
        });
        this.lineas.set([]);
        this.formVenta.patchValue({ efectivoRecibido: 0 });
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo registrar la venta', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  // ============ MODAL VENTAS DEL DÍA / FECHA ============
  abrirVentasDiaModal(): void {
    this.showVentasDiaModal.set(true);
    this.cargarVentasDeFecha(this.fechaSeleccionada());
  }
  cerrarVentasDiaModal(): void {
    this.showVentasDiaModal.set(false);
  }
  onFechaCalendarioChange(fecha: string): void {
    // input type="date" da 'YYYY-MM-DD'
    this.fechaSeleccionada.set(fecha);
    this.cargarVentasDeFecha(fecha);
  }
  private cargarVentasDeFecha(fechaYYYYMMDD: string): void {
    const desde = `${fechaYYYYMMDD}T00:00:00`;
    const hasta = `${fechaYYYYMMDD}T23:59:59`;

    const params: VentaQueryParams = {
      fechaDesde: desde,
      fechaHasta: hasta,
      page: 1,
      pageSize: 50,
      sortBy: 'fecha_venta',
      sortDir: 'desc'
    };

    this.cargandoVentasDia.set(true);
    this.ventasApi.buscarResumen(params).subscribe({
      next: (resp: PagedResponse<VentaResumenDto>) => {
        this.ventasDelDia.set(resp.items || []);
        this.totalVentasDelDia.set(resp.total || resp.items.length);
      },
      error: (e) => {
        this.ventasDelDia.set([]);
        this.totalVentasDelDia.set(0);
        Swal.fire('Error', this.extractErr(e) || 'No se pudieron cargar las ventas', 'error');
      },
      complete: () => this.cargandoVentasDia.set(false)
    });
  }

  // ============ MODAL DETALLE DE VENTA ============
  verDetallesVenta(idVenta: number): void {
    this.cargandoDetalle.set(true);
    this.ventasApi.obtenerDetalle(idVenta).subscribe({
      next: (d) => {
        this.ventaDetalle.set(d);
        this.showDetalleModal.set(true);
      },
      error: (e) => {
        this.ventaDetalle.set(null);
        Swal.fire('Error', this.extractErr(e) || 'No se pudieron cargar los detalles', 'error');
      },
      complete: () => this.cargandoDetalle.set(false)
    });
  }
  cerrarDetalleModal(): void {
    this.showDetalleModal.set(false);
    this.ventaDetalle.set(null);
  }

  // ====== Util ======
  private hoyISO(): string {
    const d = new Date();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${d.getFullYear()}-${mm}-${dd}`;
  }

  private ventaDialogHtml(v: any): string {
    const total = (typeof v.total === 'number' && v.total.toFixed) ? v.total.toFixed(2) : v.total;
    const cambioHtml = (typeof v.cambio === 'number' && v.cambio > 0)
      ? `<div>Cambio: <b class="text-success">$${v.cambio.toFixed(2)}</b></div>` : '';
    return `
      <div class="text-start">
        <div>Total: <b>$${total}</b></div>
        ${cambioHtml}
        <div class="small text-muted mt-2">Método: ${v.metodoPago}</div>
      </div>`;
  }
  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title) return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }
}
