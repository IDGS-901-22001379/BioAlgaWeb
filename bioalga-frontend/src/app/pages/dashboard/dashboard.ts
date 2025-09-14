import {
  Component,
  OnInit,
  AfterViewInit,
  OnDestroy,
  inject,
  signal,
  computed,
  effect,
  EnvironmentInjector,
  runInInjectionContext,
  EffectRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import Swal from 'sweetalert2';

import { DashboardService } from '../../services/dashboard.service';
import {
  VentasResumen,
  TopProducto,
  TopCliente,
  VentasPorUsuario,
  DevolucionesPorUsuario,
  ComprasPorProveedor,
  DashboardFilters,
} from '../../models/dashboard';

// Chart.js por CDN (si lo instalas por npm, cambia por: import Chart from 'chart.js/auto')
declare const Chart: any;

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './dashboard.html',
})
export class DashboardPageComponent implements OnInit, AfterViewInit, OnDestroy {
  private fb = inject(FormBuilder);
  private api = inject(DashboardService);
  private renderEffect?: EffectRef;

  // Necesario para crear effect() en un contexto de inyección válido
  private injector = inject(EnvironmentInjector);

  // ========= Estado/UI =========
  cargando = signal(false);
  hoy = new Date();

  // Filtros
  form = this.fb.group({
    from: [this.isoTodayMinus(30)], // últimos 30 días
    to: [this.isoToday()],
    top: [10], // Top por defecto
  });

  // ========= Datos =========
  ventasResumen = signal<VentasResumen[]>([]);
  topProductos = signal<TopProducto[]>([]);
  topClientes = signal<TopCliente[]>([]);
  ventasUsuarios = signal<VentasPorUsuario[]>([]);
  devolucionesUsers = signal<DevolucionesPorUsuario[]>([]);
  comprasProv = signal<ComprasPorProveedor[]>([]);

  // ========= KPIs =========
  totalHoy = computed(() => {
    const hoyISO = this.isoToday();
    return this.ventasResumen()
      .filter((v) => v.dia?.startsWith(hoyISO))
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  totalSemana = computed(() => {
    const anio = this.hoy.getFullYear();
    const semana = this.isoWeekNumber(this.hoy);
    return this.ventasResumen()
      .filter((v) => v.anio === anio && v.semana === semana)
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  totalMes = computed(() => {
    const anio = this.hoy.getFullYear();
    const mes = this.hoy.getMonth() + 1;
    return this.ventasResumen()
      .filter((v) => v.anio === anio && v.mes === mes)
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  totalAnio = computed(() => {
    const anio = this.hoy.getFullYear();
    return this.ventasResumen()
      .filter((v) => v.anio === anio)
      .reduce((acc, v) => acc + (v.totalVentas ?? 0), 0);
  });

  totalTickets = computed(() =>
    this.ventasResumen().reduce((acc, v) => acc + (v.numTickets ?? 0), 0)
  );

  // ========= Charts =========
  private charts: Record<string, any> = {};
  private viewReady = false;

  // ===== Ciclo de vida =====
  ngOnInit(): void {
    this.cargarTodo();
    this.form.valueChanges.subscribe(() => this.cargarTodo());
  }

  ngAfterViewInit(): void {
    this.viewReady = true;

    // re-render cuando cambian las señales
    runInInjectionContext(this.injector, () => {
      this.renderEffect = effect(() => {
        this.ventasResumen();
        this.topProductos();
        this.topClientes();
        this.ventasUsuarios();
        this.devolucionesUsers();
        this.comprasProv();
        if (this.viewReady) queueMicrotask(() => this.renderAllCharts());
      });
    });

    queueMicrotask(() => this.renderAllCharts());
  }

  ngOnDestroy(): void {
    // Limpia charts y efectos para evitar fugas
    Object.keys(this.charts).forEach((k) => this.destroy(k));
    this.renderEffect?.destroy?.();
  }

  // ========= Carga de datos =========
  cargarTodo(): void {
    const filters: DashboardFilters = {
      from: this.form.value.from ?? undefined,
      to: this.form.value.to ?? undefined,
      top: this.form.value.top ?? undefined,
    };

    this.cargando.set(true);

    forkJoin({
      ventas: this.api.getVentasResumen(filters),
      tprod: this.api.getTopProductos(filters.top ?? undefined),
      tcli: this.api.getTopClientes(filters.top ?? undefined),
      vus: this.api.getVentasPorUsuarios(filters),
      dus: this.api.getDevolucionesPorUsuarios(filters),
      cprov: this.api.getComprasPorProveedores(filters.top ?? undefined, filters),
    }).subscribe({
      next: (r) => {
        this.ventasResumen.set(r.ventas || []);
        this.topProductos.set((r.tprod || []).slice(0, this.form.value.top ?? 10));
        this.topClientes.set((r.tcli || []).slice(0, this.form.value.top ?? 10));
        this.ventasUsuarios.set(r.vus || []);
        this.devolucionesUsers.set(r.dus || []);
        this.comprasProv.set(r.cprov || []);
        if (this.viewReady) queueMicrotask(() => this.renderAllCharts());
      },
      error: (e) => {
        this.resetAll();
        Swal.fire('Error', this.extractErr(e) || 'No se pudo cargar el dashboard', 'error');
      },
      complete: () => this.cargando.set(false),
    });
  }

  // ========= Render de todas las gráficas =========
  private renderAllCharts(): void {
    this.renderVentasLine();
    this.renderTopProductosIngreso();
    this.renderTopProductosUnidades();
    this.renderTopClientes();
    this.renderVentasUsuario();
    this.renderDevolucionesUsuario();
    this.renderComprasProveedor();
  }

  // ========= Utilities de Charts =========
  private ctx(id: string): CanvasRenderingContext2D | null {
    const el = document.getElementById(id) as HTMLCanvasElement | null;
    return el ? el.getContext('2d') : null;
  }

  private destroy(id: string) {
    if (this.charts[id]) {
      this.charts[id].destroy();
      delete this.charts[id];
    }
  }

  private palette(n: number): string[] {
    const colors = [
      '#4f46e5', '#22c55e', '#f59e0b', '#ef4444', '#06b6d4',
      '#a855f7', '#14b8a6', '#f97316', '#84cc16', '#e11d48',
    ];
    return Array.from({ length: n }, (_, i) => colors[i % colors.length]);
  }

  private moneyTooltip() {
    return {
      callbacks: {
        label: (ctx: any) =>
          new Intl.NumberFormat('es-MX', {
            style: 'currency',
            currency: 'MXN',
          }).format(ctx.parsed?.y ?? ctx.parsed ?? 0),
      },
    };
  }

  // ========= Gráficas individuales =========
  private renderVentasLine() {
    const id = 'chartVentasLine';
    this.destroy(id);

    const data = [...this.ventasResumen()].sort((a, b) =>
      (a.dia || '').localeCompare(b.dia || '')
    );
    const labels = data.length ? data.map((d) => (d.dia || '').slice(5)) : ['Sin datos'];
    const values = data.length ? data.map((d) => d.totalVentas ?? 0) : [0];

    const ctx = this.ctx(id);
    if (!ctx) return;

    this.charts[id] = new Chart(ctx, {
      type: 'line',
      data: {
        labels,
        datasets: [{
          label: 'Ventas',
          data: values,
          fill: true,
          tension: 0.35,
          borderColor: '#4f46e5',
          backgroundColor: 'rgba(79,70,229,.12)',
          pointRadius: 2,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,          // 👈 clave
        plugins: { legend: { display: false }, tooltip: this.moneyTooltip() },
        scales: { y: { beginAtZero: true } },
      },
    });
  }

  private renderTopProductosIngreso() {
    const id = 'chartTopProductosIngreso';
    this.destroy(id);

    const arr = this.topProductos();
    const labels = arr.length ? arr.map((x) => x.nombre) : ['Sin datos'];
    const values = arr.length ? arr.map((x) => x.ingresoTotal ?? 0) : [0];

    const ctx = this.ctx(id);
    if (!ctx) return;

    this.charts[id] = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Ingreso (MXN)',
          data: values,
          backgroundColor: this.palette(values.length),
          borderRadius: 8,
        }],
      },
      options: {
        indexAxis: 'y',
        responsive: true,
        maintainAspectRatio: false,          // 👈 clave
        plugins: { legend: { display: false }, tooltip: this.moneyTooltip() },
        scales: { x: { beginAtZero: true } },
      },
    });
  }

  private renderTopProductosUnidades() {
    const id = 'chartTopProductosUnidades';
    this.destroy(id);

    const arr = this.topProductos();
    const labels = arr.length ? arr.map((x) => x.nombre) : ['Sin datos'];
    const values = arr.length ? arr.map((x) => x.totalUnidades ?? 0) : [0];

    const ctx = this.ctx(id);
    if (!ctx) return;

    this.charts[id] = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Unidades',
          data: values,
          backgroundColor: this.palette(values.length),
          borderRadius: 8,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,          // 👈 clave
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true } },
      },
    });
  }

  private renderTopClientes() {
    const id = 'chartTopClientes';
    this.destroy(id);

    const arr = this.topClientes();
    const labels = arr.length ? arr.map((x) => x.nombreCompleto) : ['Sin datos'];
    const values = arr.length ? arr.map((x) => x.totalGastado ?? 0) : [0];

    const ctx = this.ctx(id);
    if (!ctx) return;

    this.charts[id] = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Total gastado (MXN)',
          data: values,
          backgroundColor: this.palette(values.length),
          borderRadius: 8,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,          // 👈 clave
        plugins: { legend: { display: false }, tooltip: this.moneyTooltip() },
        scales: { y: { beginAtZero: true } },
      },
    });
  }

  private renderVentasUsuario() {
    const id = 'chartVentasUsuario';
    this.destroy(id);

    const arr = this.ventasUsuarios();
    const labels = arr.length
      ? arr.map((x) => `${x.nombre} ${x.apellidoPaterno}`)
      : ['Sin datos'];
    const values = arr.length ? arr.map((x) => x.totalVendido ?? 0) : [0];

    const ctx = this.ctx(id);
    if (!ctx) return;

    this.charts[id] = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Ventas por usuario (MXN)',
          data: values,
          backgroundColor: this.palette(values.length),
          borderRadius: 8,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,          // 👈 clave
        plugins: { legend: { display: false }, tooltip: this.moneyTooltip() },
        scales: { y: { beginAtZero: true } },
      },
    });
  }

  private renderDevolucionesUsuario() {
    const id = 'chartDevolucionesUsuario';
    this.destroy(id);

    const arr = this.devolucionesUsers();
    const labels = arr.length ? arr.map((x) => x.nombreUsuario) : ['Sin datos'];
    const values = arr.length ? arr.map((x) => x.totalDevuelto ?? 0) : [0];

    const ctx = this.ctx(id);
    if (!ctx) return;

    this.charts[id] = new Chart(ctx, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          label: 'Total devuelto (MXN)',
          data: values,
          backgroundColor: this.palette(values.length),
          borderRadius: 8,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,          // 👈 clave
        plugins: { legend: { display: false }, tooltip: this.moneyTooltip() },
        scales: { y: { beginAtZero: true } },
      },
    });
  }

  private renderComprasProveedor() {
    const id = 'chartComprasProveedor';
    this.destroy(id);

    const arr = this.comprasProv();
    const labels = arr.length ? arr.map((x) => x.nombreEmpresa) : ['Sin datos'];
    const values = arr.length ? arr.map((x) => x.totalComprado ?? 0) : [0];

    const ctx = this.ctx(id);
    if (!ctx) return;

    this.charts[id] = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels,
        datasets: [{
          data: values,
          backgroundColor: this.palette(values.length),
          borderWidth: 1,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,          // 👈 clave para que no invada la pantalla
        plugins: { legend: { position: 'right' }, tooltip: this.moneyTooltip() },
        cutout: '55%',
      },
    });
  }

  // ========= Helpers =========
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

  private isoWeekNumber(d: Date): number {
    const date = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate()));
    const dayNum = date.getUTCDay() || 7;
    date.setUTCDate(date.getUTCDate() + 4 - dayNum);
    const yearStart = new Date(Date.UTC(date.getUTCFullYear(), 0, 1));
    return Math.ceil(((((date as any) - (yearStart as any)) / 86400000) + 1) / 7);
  }

  private resetAll(): void {
    this.ventasResumen.set([]);
    this.topProductos.set([]);
    this.topClientes.set([]);
    this.ventasUsuarios.set([]);
    this.devolucionesUsers.set([]);
    this.comprasProv.set([]);
    Object.keys(this.charts).forEach((k) => this.destroy(k));
  }

  private extractErr(e: any): string {
    if (e?.error?.detail) return e.error.detail;
    if (e?.error?.title) return e.error.title;
    if (typeof e?.error === 'string') return e.error;
    if (e?.message) return e.message;
    return '';
  }

  // ========= Acciones filtros =========
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
