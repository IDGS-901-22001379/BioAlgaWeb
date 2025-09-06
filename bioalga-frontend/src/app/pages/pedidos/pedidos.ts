import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, forkJoin, of } from 'rxjs';
import Swal from 'sweetalert2';

// Servicios
import { PedidosService } from '../../services/pedidos.service';
import { PreciosService } from '../../services/precios.service';
import { ClientesService } from '../../services/clientes.service';
import { ComprasService } from '../../services/compras.service';
import { InventarioService } from '../../services/inventario.service';

// Modelos (pedidos)
import {
  EstatusPedido,
  PedidoDto,
  PedidoListItemDto,
  PedidoCreateRequest,
  PedidoUpdateHeaderRequest,
  PedidoReplaceLinesRequest,
  PedidoLineaEditRequest,
  PedidoConfirmarRequest,
  PedidoQueryParams,
} from '../../models/pedido-dtos.model';

// Modelos (productos / precios / clientes / inv)
import { ProductoLookupDto } from '../../models/producto-lookup.model';
import { TipoPrecio } from '../../models/precio.model';
import { ClienteDto } from '../../models/cliente.model';
import { StockResponse } from '../../models/inventario.model';

type ProductoCard = ProductoLookupDto & { precio?: number | null; stock?: number | null };

@Component({
  selector: 'app-pedidos-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './pedidos.html',
  styleUrls: ['./pedidos.css']
})
export class PedidosPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private pedidosApi = inject(PedidosService);
  private preciosApi = inject(PreciosService);
  private clientesApi = inject(ClientesService);
  private comprasApi = inject(ComprasService);
  private invApi = inject(InventarioService);

  // Exponer enum en el template
  public EstatusPedidoEnum = EstatusPedido;

  cargando = signal(false);

  // ================== BUSCADOR ==================
  formBuscar = this.fb.group({ q: [''] });
  showSugerencias = signal(false);
  sugerencias = signal<ProductoCard[]>([]);
  resultados  = signal<ProductoCard[]>([]);
  selectedIndex = signal<number>(-1);

  // ================== FORM PEDIDO ==================
  formPedido = this.fb.group({
    clienteId: [null as number | null],
    fechaRequerida: [''],                 // yyyy-MM-dd
    anticipo: [0, [Validators.min(0)]],
    ivaPct: [16],
    tipoPrecio: ['Normal' as TipoPrecio],
    reservarStock: [false]
  });

  // Estado local
  clienteDetectado = signal<ClienteDto | null>(null);
  lineas = signal<Array<{ idProducto: number; cantidad: number; precioUnitario: number; nombre?: string }>>([]);
  pedidoCreado = signal<PedidoDto | null>(null);
  ultGuardado = signal<Date | null>(null);

  // Totales (preview UI)
  subtotal = computed(() =>
    this.lineas().reduce((acc, l) => acc + (l.precioUnitario * l.cantidad), 0)
  );
  impuestos = computed(() => +(this.subtotal() * (Number(this.formPedido.value.ivaPct ?? 0) / 100)).toFixed(2));
  total = computed(() => +(this.subtotal() + this.impuestos()).toFixed(2));

  // ================== LISTADO / HISTORIAL ==================
  showPedidosModal = signal(false);
  showBorradoresModal = signal(false);
  cargandoLista = signal(false);

  filtroEstatus = signal<EstatusPedido | 'TODOS'>('TODOS');
  filtroTexto   = signal<string>('');
  page          = signal(1);
  pageSize      = signal(20);
  totalLista    = signal(0);
  pedidos       = signal<PedidoListItemDto[]>([]);

  // ================== HOOKS ==================
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

    // Cambia cliente → ajusta tipoPrecio y refresca precios
    this.formPedido.controls['clienteId'].valueChanges?.pipe(
      debounceTime(250),
      distinctUntilChanged()
    ).subscribe(id => {
      if (!id) {
        this.clienteDetectado.set(null);
        this.formPedido.patchValue({ tipoPrecio: 'Normal' }, { emitEvent: true });
        return;
      }
      this.clientesApi.getById(+id).subscribe({
        next: cli => {
          this.clienteDetectado.set(cli);
          const tipo = (cli.tipo_Cliente as TipoPrecio) ?? 'Normal';
          this.formPedido.patchValue({ tipoPrecio: tipo }, { emitEvent: true });
          this.actualizarPreciosPorTipo();
        },
        error: () => {
          this.clienteDetectado.set(null);
          this.formPedido.patchValue({ tipoPrecio: 'Normal' }, { emitEvent: true });
          this.actualizarPreciosSiHayLineas();
        }
      });
    });

    // Cambia tipoPrecio → refresca precios de líneas/resultados
    this.formPedido.controls['tipoPrecio'].valueChanges?.subscribe(() => {
      this.actualizarPreciosPorTipo();
      const curr = this.resultados();
      if (curr.length) this.enriquecerResultados(curr, /*soloPrecio*/true);
    });

    // Cambia IVA% → el total se recalcula con el computed
    this.formPedido.controls['ivaPct'].valueChanges?.subscribe(() => {
      this.forceRefreshTotals();
    });
  }

  // ================== Helpers UI ==================
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

  // Navegación con teclado
  onBuscadorKey(ev: KeyboardEvent): void {
    const len = this.resultados().length;
    if (!len) return;
    if (ev.key === 'ArrowDown') {
      ev.preventDefault(); this.moveSelection(+1);
    } else if (ev.key === 'ArrowUp') {
      ev.preventDefault(); this.moveSelection(-1);
    } else if (ev.key === 'Enter') {
      ev.preventDefault();
      const idx = this.selectedIndex();
      const item = (idx >= 0 && idx < len) ? this.resultados()[idx] : this.resultados()[0];
      if (item) this.agregarProductoRapido(item);
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

  // ================== Enriquecer resultados (precio/stock) ==================
  private enriquecerResultados(list: ProductoLookupDto[], soloPrecio = false): void {
    const tipo = (this.formPedido.value.tipoPrecio as TipoPrecio) || 'Normal';
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

  // ================== Carrito / Líneas ==================
  agregarProductoRapido(p: ProductoLookupDto): void {
    const idx = this.lineas().findIndex(x => x.idProducto === p.id_Producto);
    if (idx >= 0) { this.cambiarCantidad(idx, +1); return; }

    const tipo = (this.formPedido.value.tipoPrecio as TipoPrecio) || 'Normal';
    const agregar = (precio: number) => {
      this.lineas.update(arr => [...arr, {
        idProducto: p.id_Producto,
        cantidad: 1,
        precioUnitario: precio,
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
    Swal.fire({ title: '¿Vaciar líneas del pedido?', icon: 'warning', showCancelButton: true })
      .then(r => { if (r.isConfirmed) this.lineas.set([]); });
  }
  private forceRefreshTotals(): void { this.lineas.update(a => [...a]); }

  private actualizarPreciosPorTipo(): void {
    const tipo = (this.formPedido.value.tipoPrecio as TipoPrecio) || 'Normal';
    const arr = this.lineas();
    if (!arr.length) return;

    this.cargando.set(true);
    forkJoin(arr.map(l => this.preciosApi.obtenerVigente(l.idProducto, tipo))).subscribe({
      next: precios => {
        this.lineas.update(a => a.map((l, i) => ({ ...l, precioUnitario: precios[i] ?? l.precioUnitario })));
      },
      error: () => {},
      complete: () => this.cargando.set(false)
    });
  }
  private actualizarPreciosSiHayLineas(): void {
    if (this.lineas().length) this.actualizarPreciosPorTipo();
  }

  // ================== Acciones Pedido (API) ==================
  private buildCreateRequest(): PedidoCreateRequest {
    const fecha = (this.formPedido.value.fechaRequerida || '').toString().trim();
    return {
      idCliente: this.formPedido.value.clienteId ?? 0,
      fechaRequerida: fecha ? `${fecha}T00:00:00` : undefined,
      anticipo: Number(this.formPedido.value.anticipo ?? 0),
      notas: undefined,
      lineas: this.lineas().map(l => ({
        idProducto: l.idProducto,
        cantidad: l.cantidad,
        precioUnitarioOverride: undefined
      }))
    };
  }

  guardarBorrador(): void {
    if (!this.lineas().length) { Swal.fire('Aviso', 'Agrega al menos un producto.', 'info'); return; }
    const idCliente = this.formPedido.value.clienteId;
    if (!idCliente) { Swal.fire('Aviso', 'Selecciona un cliente para el pedido.', 'info'); return; }

    const body = this.buildCreateRequest();
    this.cargando.set(true);
    this.pedidosApi.crear(body).subscribe({
      next: (dto) => {
        this.pedidoCreado.set(dto);
        this.ultGuardado.set(new Date());
        Swal.fire({
          icon: 'success',
          title: `Pedido #${dto.idPedido} creado`,
          text: 'Precios se congelarán al confirmar.',
          timer: 2000
        });
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo crear el pedido', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  sincronizarLineas(): void {
    const ped = this.pedidoCreado();
    if (!ped) { Swal.fire('Aviso', 'Primero guarda el borrador.', 'info'); return; }

    const req: PedidoReplaceLinesRequest = {
      idPedido: ped.idPedido,
      lineas: this.lineas().map(l => ({
        idProducto: l.idProducto,
        cantidad: l.cantidad,
        precioUnitarioOverride: undefined
      }))
    };

    this.cargando.set(true);
    this.pedidosApi.replaceLines(req).subscribe({
      next: (dto) => {
        this.pedidoCreado.set(dto);
        this.ultGuardado.set(new Date());
        Swal.fire({ icon: 'success', title: 'Líneas sincronizadas', timer: 1500 });
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudieron actualizar las líneas', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  actualizarCabecera(): void {
    const ped = this.pedidoCreado();
    if (!ped) { Swal.fire('Aviso', 'Primero guarda el borrador.', 'info'); return; }

    const fecha = (this.formPedido.value.fechaRequerida || '').toString().trim();
    const req: PedidoUpdateHeaderRequest = {
      idPedido: ped.idPedido,
      idCliente: this.formPedido.value.clienteId ?? undefined,
      fechaRequerida: fecha ? `${fecha}T00:00:00` : undefined,
      anticipo: this.formPedido.value.anticipo ?? undefined,
      notas: undefined
    };

    this.cargando.set(true);
    this.pedidosApi.updateHeader(req).subscribe({
      next: (dto) => {
        this.pedidoCreado.set(dto);
        this.ultGuardado.set(new Date());
        Swal.fire({ icon: 'success', title: 'Cabecera actualizada', timer: 1500 });
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo actualizar la cabecera', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  confirmarPedido(): void {
    const ped = this.pedidoCreado();
    if (!ped) { Swal.fire('Aviso', 'Primero guarda el borrador.', 'info'); return; }

    // Recomendado: sincroniza líneas antes de confirmar
    this.sincronizarLineas();

    const req: PedidoConfirmarRequest = {
      idPedido: ped.idPedido,
      reservarStock: !!this.formPedido.value.reservarStock
    };

    this.cargando.set(true);
    this.pedidosApi.confirmar(req).subscribe({
      next: (dto) => {
        this.pedidoCreado.set(dto);
        Swal.fire({
          icon: 'success',
          title: `Pedido #${dto.idPedido} confirmado`,
          text: 'Precios congelados y listo para preparación.',
        });
      },
      error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo confirmar el pedido', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  // ================== Listado / búsqueda de pedidos ==================
  abrirPedidosModal(): void {
    this.filtroEstatus.set('TODOS');
    this.filtroTexto.set('');
    this.page.set(1);
    this.showPedidosModal.set(true);
    this.cargarPedidos();
  }
  cerrarPedidosModal(): void { this.showPedidosModal.set(false); }

  abrirBorradoresModal(): void {
    this.filtroEstatus.set(EstatusPedido.Borrador);
    this.filtroTexto.set('');
    this.page.set(1);
    this.showBorradoresModal.set(true);
    this.cargarPedidos(); // usa el mismo loader con filtro = Borrador
  }
  cerrarBorradoresModal(): void { this.showBorradoresModal.set(false); }

  // Atajos para abrir modal filtrado por estatus operativo
  verConfirmados(): void    { this.openModalEstatus(EstatusPedido.Confirmado); }
  verPreparacion(): void    { this.openModalEstatus(EstatusPedido.Preparacion); }
  verListo(): void          { this.openModalEstatus(EstatusPedido.Listo); }
  verFacturado(): void      { this.openModalEstatus(EstatusPedido.Facturado); }
  verEntregado(): void      { this.openModalEstatus(EstatusPedido.Entregado); }
  verCancelado(): void      { this.openModalEstatus(EstatusPedido.Cancelado); }

  private openModalEstatus(est: EstatusPedido): void {
    this.filtroEstatus.set(est);
    this.filtroTexto.set('');
    this.page.set(1);
    this.showPedidosModal.set(true);
    this.cargarPedidos();
  }

  onBuscarPedidosChange(q: string): void {
    this.filtroTexto.set((q || '').trim());
    this.page.set(1);
    this.cargarPedidos();
  }

  onChangeFiltroEstatus(est: EstatusPedido | 'TODOS'): void {
    this.filtroEstatus.set(est);
    this.page.set(1);
    this.cargarPedidos();
  }
  onChangeFiltroEstatusStr(val: string): void {
    const est: EstatusPedido | 'TODOS' = (val === 'TODOS') ? 'TODOS' : (val as EstatusPedido);
    this.onChangeFiltroEstatus(est);
  }

  irPagina(p: number): void {
    if (p < 1) return;
    this.page.set(p);
    this.cargarPedidos();
  }

  // Pública para usarla en el template
  cargarPedidos(): void {
    const est = this.filtroEstatus();
    const params: PedidoQueryParams = {
      q: this.filtroTexto() || undefined,
      estatus: est === 'TODOS' ? undefined : (est as EstatusPedido),
      page: this.page(),
      pageSize: this.pageSize(),
      sortBy: 'FechaPedido',
      sortDir: 'DESC',
    };

    this.cargandoLista.set(true);
    this.pedidosApi.buscar(params).subscribe({
      next: (resp) => {
        this.pedidos.set(resp.items ?? []);
        this.totalLista.set(resp.total ?? 0);
      },
      error: () => {
        this.pedidos.set([]);
        this.totalLista.set(0);
        Swal.fire('Error', 'No se pudieron cargar los pedidos', 'error');
      },
      complete: () => this.cargandoLista.set(false)
    });
  }

  // ===== Acciones sobre elementos del listado =====
  seleccionarBorrador(p: PedidoListItemDto): void {
    this.pedidosApi.getById(p.idPedido).subscribe({
      next: dto => {
        this.pedidoCreado.set(dto);

        // Hydrate form + líneas
        this.formPedido.patchValue({
          clienteId: dto.idCliente ?? null,
          fechaRequerida: dto.fechaRequerida ? dto.fechaRequerida.substring(0, 10) : '',
          anticipo: dto.anticipo ?? 0,
        });

        this.lineas.set(
          (dto.lineas || []).map(l => ({
            idProducto: l.idProducto,
            cantidad: l.cantidad,
            precioUnitario: l.precioUnitario,
            nombre: l.productoNombre
          }))
        );
      },
      error: () => Swal.fire('Error', 'No se pudo cargar el borrador', 'error')
    });
  }

  confirmarDesdeLista(p: PedidoListItemDto): void {
    Swal.fire({ title: `Confirmar pedido #${p.idPedido}?`, icon: 'question', showCancelButton: true })
      .then(r => {
        if (!r.isConfirmed) return;
        const req: PedidoConfirmarRequest = { idPedido: p.idPedido, reservarStock: false };
        this.pedidosApi.confirmar(req).subscribe({
          next: () => { Swal.fire('OK', 'Pedido confirmado', 'success'); this.cargarPedidos(); },
          error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo confirmar', 'error')
        });
      });
  }

  eliminarBorrador(p: PedidoListItemDto): void {
    Swal.fire({ title: `Eliminar borrador #${p.idPedido}?`, text: 'Esta acción no se puede deshacer', icon: 'warning', showCancelButton: true })
      .then(r => {
        if (!r.isConfirmed) return;
        this.pedidosApi.eliminar(p.idPedido).subscribe({
          next: () => { Swal.fire('OK', 'Borrador eliminado', 'success'); this.cargarPedidos(); },
          error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo eliminar', 'error')
        });
      });
  }

  cancelarPedido(p: PedidoListItemDto): void {
    Swal.fire({ title: `Cancelar pedido #${p.idPedido}?`, icon: 'warning', showCancelButton: true })
      .then(r => {
        if (!r.isConfirmed) return;
        this.pedidosApi.cambiarEstatus({ idPedido: p.idPedido, nuevoEstatus: EstatusPedido.Cancelado })
          .subscribe({
            next: () => { Swal.fire('OK', 'Pedido cancelado', 'success'); this.cargarPedidos(); },
            error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo cancelar', 'error')
          });
      });
  }

  // ======== NUEVAS: flujo operativo después de Confirmado ========
  /** Confirmado → Preparación */
  prepararPedido(p: PedidoListItemDto): void {
    if (p.estatus !== EstatusPedido.Confirmado) { return; }
    this.pedidosApi.cambiarEstatus({ idPedido: p.idPedido, nuevoEstatus: EstatusPedido.Preparacion })
      .subscribe({
        next: () => { Swal.fire('OK', 'Pasó a Preparación', 'success'); this.cargarPedidos(); },
        error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo actualizar', 'error')
      });
  }

  /** Preparación → Listo */
  marcarListo(p: PedidoListItemDto): void {
    if (p.estatus !== EstatusPedido.Preparacion) { return; }
    this.pedidosApi.cambiarEstatus({ idPedido: p.idPedido, nuevoEstatus: EstatusPedido.Listo })
      .subscribe({
        next: () => { Swal.fire('OK', 'Pedido listo', 'success'); this.cargarPedidos(); },
        error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo actualizar', 'error')
      });
  }

  /** Listo → Facturado (pagado) */
  facturarPedido(p: PedidoListItemDto): void {
    if (p.estatus !== EstatusPedido.Listo) { return; }
    Swal.fire({ title: `¿Marcar #${p.idPedido} como Facturado?`, icon: 'question', showCancelButton: true })
      .then(r => {
        if (!r.isConfirmed) return;
        this.pedidosApi.cambiarEstatus({ idPedido: p.idPedido, nuevoEstatus: EstatusPedido.Facturado })
          .subscribe({
            next: () => { Swal.fire('OK', 'Pedido facturado', 'success'); this.cargarPedidos(); },
            error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo facturar', 'error')
          });
      });
  }

  /** Facturado → Entregado */
  marcarEntregado(p: PedidoListItemDto): void {
    if (p.estatus !== EstatusPedido.Facturado) { return; }
    this.pedidosApi.cambiarEstatus({ idPedido: p.idPedido, nuevoEstatus: EstatusPedido.Entregado })
      .subscribe({
        next: () => { Swal.fire('OK', 'Pedido entregado', 'success'); this.cargarPedidos(); },
        error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo marcar como entregado', 'error')
      });
  }

  /** Eliminar definitivamente un cancelado */
  eliminarCancelado(p: PedidoListItemDto): void {
    if (p.estatus !== EstatusPedido.Cancelado) { return; }
    Swal.fire({ title: `Eliminar cancelado #${p.idPedido}?`, text: 'Esta acción no se puede deshacer', icon: 'warning', showCancelButton: true })
      .then(r => {
        if (!r.isConfirmed) return;
        this.pedidosApi.eliminar(p.idPedido).subscribe({
          next: () => { Swal.fire('OK', 'Pedido eliminado', 'success'); this.cargarPedidos(); },
          error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo eliminar', 'error')
        });
      });
  }

  /** Helper genérico (lo puedes seguir usando en botones sueltos) */
  avanzarEstatus(p: PedidoListItemDto, nuevo: EstatusPedido): void {
    this.pedidosApi.cambiarEstatus({ idPedido: p.idPedido, nuevoEstatus: nuevo })
      .subscribe({
        next: () => { Swal.fire('OK', 'Estatus actualizado', 'success'); this.cargarPedidos(); },
        error: (e) => Swal.fire('Error', this.extractErr(e) || 'No se pudo actualizar', 'error')
      });
  }

  // ===== Helpers de UI =====
  /** Sugerencia para mostrar/ocultar acciones en cada fila */
  puedeIrA(p: PedidoListItemDto, destino: EstatusPedido): boolean {
    const a = p.estatus;
    return (a === EstatusPedido.Confirmado   && destino === EstatusPedido.Preparacion)
        || (a === EstatusPedido.Preparacion  && destino === EstatusPedido.Listo)
        || (a === EstatusPedido.Listo        && destino === EstatusPedido.Facturado)
        || (a === EstatusPedido.Facturado    && destino === EstatusPedido.Entregado);
  }

  // Badge helper para la UI
  badgeClass(est: EstatusPedido): string {
    switch (est) {
      case EstatusPedido.Borrador:     return 'bg-secondary';
      case EstatusPedido.Confirmado:   return 'bg-primary';
      case EstatusPedido.Preparacion:  return 'bg-info';
      case EstatusPedido.Listo:        return 'bg-warning text-dark';
      case EstatusPedido.Facturado:    return 'bg-indigo'; // define en CSS si no existe
      case EstatusPedido.Entregado:    return 'bg-success';
      case EstatusPedido.Cancelado:    return 'bg-danger';
      default: return 'bg-light text-dark';
    }
  }

  estatusText(est: EstatusPedido): string {
    // Etiquetas bonitas (acentos)
    const map: Record<string,string> = {
      [EstatusPedido.Borrador]:    'Borrador',
      [EstatusPedido.Confirmado]:  'Confirmado',
      [EstatusPedido.Preparacion]: 'Preparación',
      [EstatusPedido.Listo]:       'Listo',
      [EstatusPedido.Facturado]:   'Facturado',
      [EstatusPedido.Entregado]:   'Entregado',
      [EstatusPedido.Cancelado]:   'Cancelado',
    };
    return map[est] ?? est;
  }

  // ================== Util ==================
  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title) return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }
}
