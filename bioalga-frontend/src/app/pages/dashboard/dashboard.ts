import { Component, OnInit, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import Swal from 'sweetalert2';

import { DashboardService } from '../../services/dashboard.service';

// Modelos
import {
  VentasResumen,
  TopProducto,
  TopCliente,
  VentasPorUsuario,
  DevolucionesPorUsuario,
  ComprasPorProveedor,
  DashboardFilters,
} from '../../models/dashboard';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './dashboard.html',
})
export class DashboardPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(DashboardService);

  // ========= UI / estado =========
  cargando = signal(false);
  hoy = new Date();

  // Filtros (desde/hasta/top). Usamos ISO-8601 (yyyy-MM-dd)
  form = this.fb.group({
    from: [this.isoTodayMinus(30)], // últimos 30 días por default
    to:   [this.isoToday()],
    top:  [5],
  });

  // ========= Datos del dashboard =========
  ventasResumen     = signal<VentasResumen[]>([]);
  topProductos      = signal<TopProducto[]>([]);
  topClientes       = signal<TopCliente[]>([]);
  ventasUsuarios    = signal<VentasPorUsuario[]>([]);
  devolucionesUsers = signal<DevolucionesPorUsuario[]>([]);
  comprasProv       = signal<ComprasPorProveedor[]>([]);

  // ========= KPIs (computados) =========
  // Total de hoy (sumando registros cuya 'dia' == hoy)
  totalHoy = computed(() => {
    const hoyISO = this.isoToday();
    return this.ventasResumen()
      .filter(v => v.dia.startsWith(hoyISO))
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  // Totales por semana/mes/año (aprovechamos los campos que ya vienen de la vista)
  totalSemana = computed(() => {
    const anio = this.hoy.getFullYear();
    const semana = this.isoWeekNumber(this.hoy);
    return this.ventasResumen()
      .filter(v => v.anio === anio && v.semana === semana)
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  totalMes = computed(() => {
    const anio = this.hoy.getFullYear();
    const mes = this.hoy.getMonth() + 1;
    return this.ventasResumen()
      .filter(v => v.anio === anio && v.mes === mes)
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  totalAnio = computed(() => {
    const anio = this.hoy.getFullYear();
    return this.ventasResumen()
      .filter(v => v.anio === anio)
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  // Tickets en el rango seleccionado
  totalTickets = computed(() =>
    this.ventasResumen().reduce((acc, v) => acc + (v.numTickets ?? 0), 0)
  );

  ngOnInit(): void {
    this.cargarTodo();

    // Si cambian filtros -> recargar
    this.form.valueChanges.subscribe(() => this.cargarTodo());
  }

  // ========= Carga de datos =========
  cargarTodo(): void {
    const filters: DashboardFilters = {
      from: this.form.value.from ?? undefined,
      to:   this.form.value.to ?? undefined,
      top:  this.form.value.top ?? undefined
    };

    this.cargando.set(true);

    // Los endpoints individuales no exigen filtros en tu backend actual,
    // pero tu servicio está listo para enviarlos; si el backend los ignora, no rompe nada.
    forkJoin({
      ventas: this.api.getVentasResumen(filters),
      tprod:  this.api.getTopProductos(filters.top ?? undefined),
      tcli:   this.api.getTopClientes(filters.top ?? undefined),
      vus:    this.api.getVentasPorUsuarios(filters),
      dus:    this.api.getDevolucionesPorUsuarios(filters),
      cprov:  this.api.getComprasPorProveedores(filters.top ?? undefined, filters),
    }).subscribe({
      next: r => {
        this.ventasResumen.set(r.ventas || []);
        this.topProductos.set(r.tprod || []);
        this.topClientes.set(r.tcli || []);
        this.ventasUsuarios.set(r.vus || []);
        this.devolucionesUsers.set(r.dus || []);
        this.comprasProv.set(r.cprov || []);
      },
      error: (e) => {
        this.resetAll();
        Swal.fire('Error', this.extractErr(e) || 'No se pudo cargar el dashboard', 'error');
      },
      complete: () => this.cargando.set(false)
    });
  }

  // ========= Helpers de UI =========
  isoToday(): string {
    const d = new Date();
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${d.getFullYear()}-${mm}-${dd}`;
  }
  isoTodayMinus(days: number): string {
    const d = new Date();
    d.setDate(d.getDate() - days);
    const mm = String(d.getMonth() + 1).padStart(2, '0');
    const dd = String(d.getDate()).padStart(2, '0');
    return `${d.getFullYear()}-${mm}-${dd}`;
  }

  // Semana ISO (Lunes-domingo). Tu vista usa WEEK(...,1) → lunes como primer día.
  private isoWeekNumber(d: Date): number {
    const date = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate()));
    const dayNum = date.getUTCDay() || 7;
    date.setUTCDate(date.getUTCDate() + 4 - dayNum);
    const yearStart = new Date(Date.UTC(date.getUTCFullYear(), 0, 1));
    return Math.ceil((((date as any) - (yearStart as any)) / 86400000 + 1) / 7);
  }

  private resetAll(): void {
    this.ventasResumen.set([]);
    this.topProductos.set([]);
    this.topClientes.set([]);
    this.ventasUsuarios.set([]);
    this.devolucionesUsers.set([]);
    this.comprasProv.set([]);
  }

  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title) return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }

  // ========= Form actions =========
  hoyBtn(): void {
    const t = this.isoToday();
    this.form.patchValue({ from: t, to: t });
  }
  ultimos7(): void {
    this.form.patchValue({ from: this.isoTodayMinus(7), to: this.isoToday() });
  }
  ultimos30(): void {
    this.form.patchValue({ from: this.isoTodayMinus(30), to: this.isoToday() });
  }
}
