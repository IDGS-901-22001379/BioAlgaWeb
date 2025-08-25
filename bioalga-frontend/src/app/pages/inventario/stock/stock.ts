import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

// ⬇️ importa las directivas/pipes usados en el HTML
import { NgIf, NgForOf, NgClass, DatePipe } from '@angular/common';

import { debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';
import Swal from 'sweetalert2';

import { InventarioService } from '../../../services/inventario.service';
import { ComprasService } from '../../../services/compras.service';
import { ProductoLookupDto } from '../../../models/producto-lookup.model';
import { KardexItem, StockResponse } from '../../../models/inventario.model';

@Component({
  selector: 'app-stock-page',
  standalone: true,
  // ⬇️ agrega aquí las directivas/pipes
  imports: [FormsModule, ReactiveFormsModule, NgIf, NgForOf, NgClass, DatePipe],
  templateUrl: './stock.html',
  styleUrls: ['./stock.css']
})
export class StockPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private inv = inject(InventarioService);
  private comprasApi = inject(ComprasService);

  cargando = signal(false);

  form = this.fb.group({
    busqueda: [''],
    id_Producto: [null as number | null, Validators.required],
    desde: [null as string | null],
    hasta: [null as string | null],
  });

  sugerencias = signal<ProductoLookupDto[]>([]);
  showSugerencias = signal(false);

  stock = signal<StockResponse | null>(null);
  kardex = signal<KardexItem[]>([]);

  ngOnInit(): void {
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

  elegirProducto(p: ProductoLookupDto) {
    this.form.patchValue({ id_Producto: p.id_Producto, busqueda: `${p.nombre} (${p.sku})` });
    this.showSugerencias.set(false);
  }

  limpiarProducto() {
    this.form.patchValue({ busqueda: '', id_Producto: null });
    this.stock.set(null);
    this.kardex.set([]);
  }

  consultar() {
    if (this.form.invalid) { this.form.markAllAsTouched(); Swal.fire('Aviso', 'Selecciona un producto.', 'info'); return; }
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
}
