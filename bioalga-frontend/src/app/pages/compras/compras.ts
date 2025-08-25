import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';
import Swal from 'sweetalert2';
import { NgIf, NgForOf, NgClass, DatePipe } from '@angular/common';

import { ComprasService } from '../../services/compras.service';
import { Compra, DetalleCompra } from '../../models/compra.model';
import { CrearCompraDto, AgregarRenglonDto, ConfirmarCompraResponse } from '../../models/compra-dtos.model';
import { ProductoLookupDto } from '../../models/producto-lookup.model';

@Component({
  selector: 'app-compras-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './compras.html',
  styleUrls: ['./compras.css']
})
export class ComprasPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(ComprasService);

  cargando = signal(false);
  compra = signal<Compra | null>(null);
  sugerencias = signal<ProductoLookupDto[]>([]);
  showSugerencias = signal(false);

  // encabezado (borrador)
  formEnc = this.fb.group({
    proveedor_Id: [null as number | null],
    proveedor_Texto: [''],
    notas: [''],
    id_Usuario: [1, [Validators.required]] // TODO: reemplazar por el usuario logueado
  });

  // renglón
  formRen = this.fb.group({
    busqueda: [''],
    id_Producto: [null as number | null, Validators.required],
    cantidad: [1, [Validators.required, Validators.min(1)]],
    costo_Unitario: [0, [Validators.required, Validators.min(0)]],
    iva_Unitario: [0, [Validators.min(0)]]
  });

  ngOnInit(): void {
    // Autocomplete con debounce
    this.formRen.controls['busqueda'].valueChanges?.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(q => {
        const s = (q ?? '').toString().trim();
        if (s.length < 2) { this.showSugerencias.set(false); return of([] as ProductoLookupDto[]); }
        this.showSugerencias.set(true);
        return this.api.buscarProductos(s, 8);
      })
    ).subscribe({
      next: list => this.sugerencias.set(list),
      error: () => this.sugerencias.set([])
    });
  }

  // Crear borrador
  crearBorrador(): void {
    if (this.formEnc.invalid) { this.formEnc.markAllAsTouched(); return; }
    const dto = this.formEnc.getRawValue() as unknown as CrearCompraDto;
    this.cargando.set(true);
    this.api.crearBorrador(dto).subscribe({
      next: (c) => {
        this.compra.set(c);
        Swal.fire('Listo', `Compra #${c.id_Compra} creada en borrador.`, 'success');
      },
      error: (e) => Swal.fire('Error', e?.error || 'No se pudo crear la compra', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  // Elegir de sugerencias
  elegirProducto(p: ProductoLookupDto): void {
    this.formRen.patchValue({
      id_Producto: p.id_Producto,
      busqueda: `${p.nombre} (${p.sku})`
    });
    this.showSugerencias.set(false);
  }

  // Agregar renglón
  agregarRenglon(): void {
    const c = this.compra(); if (!c) { Swal.fire('Aviso', 'Primero crea la compra.', 'info'); return; }
    if (this.formRen.invalid) { this.formRen.markAllAsTouched(); return; }

    const dto: AgregarRenglonDto = {
      id_Producto: this.formRen.value.id_Producto!,
      cantidad: this.formRen.value.cantidad!,
      costo_Unitario: this.formRen.value.costo_Unitario!,
      iva_Unitario: this.formRen.value.iva_Unitario ?? 0
    };

    this.cargando.set(true);
    this.api.agregarRenglon(c.id_Compra, dto).subscribe({
      next: (updated) => {
        // sin recargar: actualiza estado local
        this.compra.set(updated);
        this.formRen.reset({ busqueda: '', id_Producto: null, cantidad: 1, costo_Unitario: 0, iva_Unitario: 0 });
        Swal.fire('Listo', 'Renglón agregado', 'success');
      },
      error: (e) => Swal.fire('Error', e?.error || 'No se pudo agregar el renglón', 'error'),
      complete: () => this.cargando.set(false)
    });
  }

  // Eliminar renglón
  eliminarRenglon(det: DetalleCompra): void {
    const c = this.compra(); if (!c) return;

    Swal.fire({
      title: '¿Eliminar renglón?',
      text: `ID Detalle ${det.id_Detalle}`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar'
    }).then(res => {
      if (!res.isConfirmed) return;

      this.cargando.set(true);
      this.api.eliminarRenglon(c.id_Compra, det.id_Detalle).subscribe({
        next: (updated) => {
          this.compra.set(updated);
          Swal.fire('Eliminado', 'Renglón eliminado', 'success');
        },
        error: (e) => Swal.fire('Error', e?.error || 'No se pudo eliminar', 'error'),
        complete: () => this.cargando.set(false)
      });
    });
  }

  // Confirmar compra
  confirmar(): void {
    const c = this.compra(); if (!c) return;
    if (!c.detalles?.length) { Swal.fire('Aviso', 'Agrega al menos un renglón.', 'info'); return; }

    Swal.fire({
      title: `Confirmar compra #${c.id_Compra}?`,
      text: `Se crearán entradas de inventario por cada renglón.`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Confirmar',
      cancelButtonText: 'Cancelar'
    }).then(res => {
      if (!res.isConfirmed) return;

      this.cargando.set(true);
      this.api.confirmar(c.id_Compra, this.formEnc.value.id_Usuario ?? 1).subscribe({
        next: (r: ConfirmarCompraResponse) => {
          Swal.fire('Confirmada', `Entradas: ${r.movimientosCreados}\nTotal: $${r.total.toFixed(2)}`, 'success');
          // Puedes marcar localmente el estado si lo devuelve el backend
          this.compra.update(x => x ? { ...x, estado: 'Confirmada' } as any : x);
        },
        error: (e) => Swal.fire('Error', e?.error || 'No se pudo confirmar', 'error'),
        complete: () => this.cargando.set(false)
      });
    });
  }

  get subtotal(): number { return this.compra()?.subtotal ?? 0; }
  get impuestos(): number { return this.compra()?.impuestos ?? 0; }
  get total(): number { return this.compra()?.total ?? 0; }
}
