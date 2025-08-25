// pages/productos/productos.ts
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';

import { ProductosService } from '../../services/productos.service';
import { PreciosService } from '../../services/precios.service';

import {
  ProductoDto,
  CrearProductoDto,
  ActualizarProductoDto,
  TipoProducto
} from '../../models/producto.model';

import {
  PrecioDto,
  TipoPrecio,
  CrearPrecioDto,
  ActualizarPrecioDto
} from '../../models/precio.model';

declare var bootstrap: any;

@Component({
  selector: 'app-productos-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './productos.html',
  styleUrls: ['./productos.css'],
})
export class ProductosPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(ProductosService);
  private preciosApi = inject(PreciosService);

  // ======= UI / estado general =======
  cargando = signal(false);
  editandoId = signal<number | null>(null);
  private modalProductoRef: any;
  private modalPreciosRef: any;

  // ======= Tabla =======
  productos: ProductoDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;

  // ======= Filtros =======
  filtroQ = '';
  filtroTipo: TipoProducto | string | undefined = undefined;
  filtroEstado: 'Activo' | 'Inactivo' | undefined = undefined;

  sortBy: 'nombre' | 'created_at' = 'nombre';
  sortDir: 'asc' | 'desc' = 'asc';

  private qSearch$ = new Subject<string>();

  // ======= Form Producto (Reactive strict) =======
  formProducto = this.fb.group({
    nombre: this.fb.control<string>('', { nonNullable: true, validators: [Validators.required, Validators.minLength(2)] }),
    descripcion: this.fb.control<string | null>(null),
    tipo: this.fb.control<string>('Componente', { nonNullable: true }),
    codigoSku: this.fb.control<string>('', { nonNullable: true, validators: [Validators.required] }),
    codigoBarras: this.fb.control<string | null>(null),
    estatus: this.fb.control<'Activo' | 'Inactivo'>('Activo', { nonNullable: true }),
    idCategoria: this.fb.control<number | null>(null),
    idMarca: this.fb.control<number | null>(null),
    idUnidad: this.fb.control<number | null>(null),
    proveedorPreferenteId: this.fb.control<number | null>(null),
  });

  tipos: (TipoProducto | string)[] = [
    'Componente','Sensor','Actuador','Módulo','Accesorio','Cable',
    'Alimentación','Herramienta','Microcontrolador','Biorreactor','Alga','Otro'
  ];

  // ======= Precios =======
  tiposPrecio: (TipoPrecio | string)[] = ['Normal','Mayoreo','Descuento','Especial'];
  productoSeleccionado: ProductoDto | null = null;
  preciosVigentes: PrecioDto[] = [];
  preciosHistorial: PrecioDto[] = [];

  formPrecio = this.fb.group({
    tipoPrecio: this.fb.control<string>('Normal', { nonNullable: true }),
    precio: this.fb.control<number | null>(null, { validators: [Validators.required, Validators.min(0)] }),
    vigenteDesde: this.fb.control<string | null>(null),
    vigenteHasta: this.fb.control<string | null>(null),
    activo: this.fb.control<boolean>(true, { nonNullable: true }),
  });

  constructor() {
    this.qSearch$
      .pipe(debounceTime(250), distinctUntilChanged())
      .subscribe(() => this.buscar(1));
  }

  get totalPages(): number {
    return Math.ceil(this.total / (this.pageSize || 1));
  }

  ngOnInit(): void {
    this.buscar(1);
  }

  // ======= Filtros =======
  onQChange(valor: string) {
    this.filtroQ = valor ?? '';
    this.qSearch$.next(this.filtroQ);
  }

  // ======= Listado =======
  buscar(page: number = this.page): void {
    this.cargando.set(true);
    this.page = page;

    this.api.buscar({
      q: this.filtroQ?.trim() || undefined,
      tipo: this.filtroTipo || undefined,
      estatus: this.filtroEstado || undefined,
      page: this.page,
      pageSize: this.pageSize,
      sortBy: this.sortBy,
      sortDir: this.sortDir
    }).subscribe({
      next: (res) => {
        this.productos = res.items;
        this.total = res.total;
        this.page = res.page;
        this.pageSize = res.pageSize;
        this.cargando.set(false);
      },
      error: () => {
        this.cargando.set(false);
        Swal.fire('Error', 'No se pudo cargar la lista de productos', 'error');
      }
    });
  }

  limpiarBusqueda(): void {
    this.filtroQ = '';
    this.filtroTipo = undefined;
    this.filtroEstado = undefined;
    this.sortBy = 'nombre';
    this.sortDir = 'asc';
    this.buscar(1);
  }

  // ======= Modal Producto =======
  private showProductoModal(): void {
    const el = document.getElementById('productoModal');
    if (el) {
      this.modalProductoRef = new bootstrap.Modal(el, { backdrop: 'static' });
      this.modalProductoRef.show();
    }
  }

  private hideProductoModal(): void {
    if (this.modalProductoRef) this.modalProductoRef.hide();
  }

  onCancelar(): void {
    this.formProducto.reset();
    this.hideProductoModal();
  }

  abrirModalNuevo(): void {
    this.editandoId.set(null);
    this.formProducto.reset({
      nombre: '',
      descripcion: '',
      tipo: 'Componente',
      codigoSku: '',
      codigoBarras: '',
      estatus: 'Activo',
      idCategoria: null,
      idMarca: null,
      idUnidad: null,
      proveedorPreferenteId: null,
    });
    this.showProductoModal();
  }

  editar(p: ProductoDto): void {
    this.editandoId.set(p.id_Producto);
    this.formProducto.reset({
      nombre: p.nombre,
      descripcion: p.descripcion ?? '',
      tipo: (p.tipo as any) ?? 'Componente',
      codigoSku: p.codigo_Sku,
      codigoBarras: p.codigo_Barras ?? '',
      estatus: p.estatus,
      idCategoria: p.id_Categoria ?? null,
      idMarca: p.id_Marca ?? null,
      idUnidad: p.id_Unidad ?? null,
      proveedorPreferenteId: p.proveedor_Preferente_Id ?? null,
    });
    this.showProductoModal();
  }

  // ======= Crear / Actualizar Producto =======
  submit(): void {
    if (this.formProducto.invalid) {
      this.formProducto.markAllAsTouched();
      return;
    }

    const id = this.editandoId();
    const v = this.formProducto.value;

    if (id === null) {
      const payload: CrearProductoDto = {
        nombre: v.nombre!,
        descripcion: v.descripcion || null,
        tipo: v.tipo as any,
        codigoSku: v.codigoSku!,
        codigoBarras: v.codigoBarras || null,
        estatus: v.estatus as any,
        idCategoria: v.idCategoria ?? null,
        idMarca: v.idMarca ?? null,
        idUnidad: v.idUnidad ?? null,
        proveedorPreferenteId: v.proveedorPreferenteId ?? null,
      };

      this.cargando.set(true);
      this.api.crear(payload).subscribe({
        next: (nuevo) => {
          Swal.fire('Listo', 'Producto creado correctamente', 'success');
          this.productos.unshift(nuevo);
          this.total++;
          this.cargando.set(false);
          this.hideProductoModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = e?.error?.message || 'No se pudo crear el producto';
          Swal.fire('Error', msg, 'error');
        }
      });

    } else {
      const payload: ActualizarProductoDto = {
        nombre: v.nombre!,
        descripcion: v.descripcion || null,
        tipo: v.tipo as any,
        codigoSku: v.codigoSku!,
        codigoBarras: v.codigoBarras || null,
        estatus: v.estatus as any,
        idCategoria: v.idCategoria ?? null,
        idMarca: v.idMarca ?? null,
        idUnidad: v.idUnidad ?? null,
        proveedorPreferenteId: v.proveedorPreferenteId ?? null,
      };

      this.cargando.set(true);
      this.api.actualizar(id, payload).subscribe({
        next: () => {
          Swal.fire('Listo', 'Producto actualizado correctamente', 'success');
          const i = this.productos.findIndex(x => x.id_Producto === id);
          if (i !== -1) {
            this.productos[i] = {
              ...this.productos[i],
              nombre: payload.nombre,
              descripcion: payload.descripcion ?? '',
              tipo: payload.tipo,
              codigo_Sku: payload.codigoSku,
              codigo_Barras: payload.codigoBarras ?? '',
              estatus: payload.estatus,
              id_Categoria: payload.idCategoria ?? null,
              id_Marca: payload.idMarca ?? null,
              id_Unidad: payload.idUnidad ?? null,
              proveedor_Preferente_Id: payload.proveedorPreferenteId ?? null,
            };
          }
          this.cargando.set(false);
          this.hideProductoModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = e?.error?.message || 'No se pudo actualizar el producto';
          Swal.fire('Error', msg, 'error');
        }
      });
    }
  }

  eliminar(p: ProductoDto): void {
    Swal.fire({
      title: '¿Eliminar producto?',
      text: `${p.nombre} (${p.codigo_Sku})`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
    }).then(res => {
      if (!res.isConfirmed) return;

      this.cargando.set(true);
      this.api.eliminar(p.id_Producto).subscribe({
        next: () => {
          Swal.fire('Eliminado', 'Producto eliminado correctamente', 'success');
          this.productos = this.productos.filter(x => x.id_Producto !== p.id_Producto);
          this.total--;
          this.cargando.set(false);

          const vacia = (this.page - 1) * this.pageSize >= this.total && this.page > 1;
          if (vacia) this.buscar(this.page - 1);
        },
        error: () => {
          this.cargando.set(false);
          Swal.fire('Error', 'No se pudo eliminar el producto', 'error');
        }
      });
    });
  }

  // ======= Modal de Precios =======
  abrirModalPrecios(p: ProductoDto): void {
    this.productoSeleccionado = p;
    this.formPrecio.reset({
      tipoPrecio: 'Normal',
      precio: null,
      vigenteDesde: null,
      vigenteHasta: null,
      activo: true
    });

    // Cargar vigentes + histórico
    Promise.all([
      this.preciosApi.vigentes(p.id_Producto).toPromise(),
      this.preciosApi.historial(p.id_Producto).toPromise()
    ]).then(([vig, hist]) => {
      this.preciosVigentes = vig ?? [];
      this.preciosHistorial = hist ?? [];
      const el = document.getElementById('preciosModal');
      if (el) {
        this.modalPreciosRef = new bootstrap.Modal(el, { backdrop: 'static' });
        this.modalPreciosRef.show();
      }
    }).catch(() => {
      Swal.fire('Error', 'No se pudieron cargar los precios', 'error');
    });
  }

  cerrarModalPrecios(): void {
    if (this.modalPreciosRef) this.modalPreciosRef.hide();
    this.productoSeleccionado = null;
    this.preciosVigentes = [];
    this.preciosHistorial = [];
  }

  crearPrecio(): void {
    if (!this.productoSeleccionado) return;
    if (this.formPrecio.invalid) {
      this.formPrecio.markAllAsTouched();
      return;
    }

    const v = this.formPrecio.value;
    const dto: CrearPrecioDto = {
      tipoPrecio: v.tipoPrecio as any,
      precio: Number(v.precio),
      vigenteDesde: v.vigenteDesde || null,
      vigenteHasta: v.vigenteHasta || null,
      activo: v.activo ?? true
    };

    this.preciosApi.crear(this.productoSeleccionado.id_Producto, dto).subscribe({
      next: (creado) => {
        Swal.fire('Listo', 'Precio creado. Se desactivó el vigente anterior del mismo tipo.', 'success');
        // refrescar listas sin cerrar modal
        this.refrescarPrecios(this.productoSeleccionado!.id_Producto);
      },
      error: (e) => {
        const msg = e?.error?.message || 'No se pudo crear el precio';
        Swal.fire('Error', msg, 'error');
      }
    });
  }

  cerrarPrecio(pre: PrecioDto): void {
    // “Cerrar” = setear vigenteHasta ahora ó activo=false
    Swal.fire({
      title: '¿Cerrar este precio?',
      text: `${pre.tipo_Precio} -> $${pre.precio}`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Sí, cerrar',
      cancelButtonText: 'Cancelar'
    }).then(res => {
      if (!res.isConfirmed) return;

      const dto: ActualizarPrecioDto = {
        precio: pre.precio,
        vigenteHasta: new Date().toISOString(),
        activo: false
      };
      this.preciosApi.actualizar(pre.id_Precio, dto).subscribe({
        next: () => {
          Swal.fire('Cerrado', 'El precio fue cerrado correctamente', 'success');
          if (this.productoSeleccionado) {
            this.refrescarPrecios(this.productoSeleccionado.id_Producto);
          }
        },
        error: () => Swal.fire('Error', 'No se pudo cerrar el precio', 'error')
      });
    });
  }

  private refrescarPrecios(idProducto: number): void {
    this.preciosApi.vigentes(idProducto).subscribe(v => this.preciosVigentes = v);
    this.preciosApi.historial(idProducto).subscribe(h => this.preciosHistorial = h);
  }

  // ======= Paginación =======
  puedeAtras(): boolean { return this.page > 1; }
  puedeAdelante(): boolean { return this.page * this.pageSize < this.total; }
  irAtras(): void { if (this.puedeAtras()) this.buscar(this.page - 1); }
  irAdelante(): void { if (this.puedeAdelante()) this.buscar(this.page + 1); }

  trackById = (_: number, p: ProductoDto) => p.id_Producto;
}
