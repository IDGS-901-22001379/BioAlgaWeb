// src/app/pages/inventario/stock/stock.ts
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

// ⬇️ Directivas/Pipes usados en el HTML
import { NgIf, NgForOf, NgClass, DatePipe } from '@angular/common';

import { debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';
import Swal from 'sweetalert2';

import { InventarioService } from '../../../services/inventario.service';
import { ComprasService } from '../../../services/compras.service';
import { ProductoLookupDto } from '../../../models/producto-lookup.model';
import {
  KardexItem,
  StockResponse,
  AjusteInventarioDto,
  MovimientoResultDto
} from '../../../models/inventario.model';

@Component({
  selector: 'app-stock-page',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, NgIf, NgForOf, NgClass, DatePipe],
  templateUrl: './stock.html',
  styleUrls: ['./stock.css']
})
export class StockPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private inv = inject(InventarioService);
  private comprasApi = inject(ComprasService);

  cargando = signal(false);

  // ===== Filtros / búsqueda producto =====
  form = this.fb.group({
    busqueda: [''],
    id_Producto: [null as number | null, Validators.required],
    desde: [null as string | null],
    hasta: [null as string | null],
  });

  sugerencias = signal<ProductoLookupDto[]>([]);
  showSugerencias = signal(false);

  // ===== Resultados =====
  stock = signal<StockResponse | null>(null);
  kardex = signal<KardexItem[]>([]);

  // ===== Ajuste manual (Agregar/Quitar) =====
  formAjuste = this.fb.group({
    cantidad: [1, [Validators.required, Validators.min(1)]],
    motivo: [''],
    id_Usuario: [1, [Validators.required]] // TODO: reemplazar con el usuario logueado
  });

  ngOnInit(): void {
    // Autocomplete productos
    this.form.controls['busqueda'].valueChanges?.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap((q) => {
        const s = (q ?? '').toString().trim();
        if (s.length < 2) { this.showSugerencias.set(false); return of([] as ProductoLookupDto[]); }
        this.showSugerencias.set(true);
        return this.comprasApi.buscarProductos(s, 10);
      })
    ).subscribe({
      next: list => this.sugerencias.set(list),
      error: () => this.sugerencias.set([])
    });
  }

  // ===== UI helpers =====
  elegirProducto(p: ProductoLookupDto) {
    this.form.patchValue({ id_Producto: p.id_Producto, busqueda: `${p.nombre} (${p.sku})` });
    this.showSugerencias.set(false);
  }

  limpiarProducto() {
    this.form.patchValue({ busqueda: '', id_Producto: null });
    this.stock.set(null);
    this.kardex.set([]);
  }

  // ===== Consultas =====
  consultar() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      Swal.fire('Aviso', 'Selecciona un producto.', 'info');
      return;
    }
    const id = this.form.value.id_Producto!;
    const desde = this.form.value.desde || undefined;
    const hasta = this.form.value.hasta || undefined;

    this.cargando.set(true);
    Promise.all([
      this.inv.stockActual(id).toPromise(),
      this.inv.kardex(id, desde, hasta).toPromise()
    ]).then(([stock, kardex]) => {
      this.stock.set(stock ?? null);
      this.kardex.set(kardex ?? []);
    }).catch(() => {
      Swal.fire('Error', 'No se pudo obtener la información de inventario.', 'error');
    }).finally(() => this.cargando.set(false));
  }

  // ===== Ajuste manual: agregar/quitar sin compra =====
  ajustar(tipo: 'agregar' | 'quitar') {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      Swal.fire('Aviso', 'Selecciona un producto.', 'info');
      return;
    }
    if (this.formAjuste.invalid) {
      this.formAjuste.markAllAsTouched();
      return;
    }

    const id = this.form.value.id_Producto!;
    const dto: AjusteInventarioDto = {
      id_Producto: id,
      cantidad: this.formAjuste.value.cantidad!,
      id_Usuario: this.formAjuste.value.id_Usuario!,
      motivo: this.formAjuste.value.motivo || undefined
    };

    const req$ = (tipo === 'agregar') ? this.inv.agregar(dto) : this.inv.quitar(dto);

    this.cargando.set(true);
    req$.subscribe({
      next: (r: MovimientoResultDto) => {
        const verb = (tipo === 'agregar') ? 'agregadas' : 'retiradas';
        Swal.fire('Listo',
          `Unidades ${verb}: ${dto.cantidad}<br/>Stock ahora: ${r.stock_Despues}`,
          'success');

        // Refrescar datos visibles
        this.consultar();
        // Deja motivo, reinicia cantidad
        this.formAjuste.patchValue({ cantidad: 1 });
      },
      error: (e) => {
        const msg = e?.error || 'No se pudo registrar el movimiento.';
        Swal.fire('Error', msg, 'error');
      },
      complete: () => this.cargando.set(false)
    });
  }
}

