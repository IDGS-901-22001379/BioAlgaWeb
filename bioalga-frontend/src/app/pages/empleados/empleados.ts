import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';

import {
  EmpleadoDto,
  EmpleadoCreateRequest,
  EmpleadoUpdateRequest,
  PagedResponse
} from '../../models/empleado.model';

import { EmpleadosService } from '../../services/empleados.service';

declare var bootstrap: any;

@Component({
  selector: 'app-empleados-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './empleados.html',
  styleUrls: ['./empleados.css'],
})
export class EmpleadosPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(EmpleadosService);

  // UI State
  cargando = signal(false);
  editando = signal(false);
  editId: number | null = null;

  // Datos
  empleados: EmpleadoDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  totalPages = 1;

  // Filtros
  filtroNombre = '';
  filtroPuesto: string | undefined;
  filtroEstatus: 'Activo' | 'Inactivo' | 'Baja' | undefined;

  // Orden
  sortBy: 'nombre' | 'fecha_ingreso' | 'salario' | 'puesto' | 'estatus' = 'nombre';
  sortDir: 'asc' | 'desc' = 'asc';

  // Debounce búsqueda
  private search$ = new Subject<string>();

  // Formulario
  form = this.fb.group({
    id_Empleado: [0],
    nombre: ['', [Validators.required, Validators.minLength(2)]],
    apellido_Paterno: [''],
    apellido_Materno: [''],
    curp: [''],
    rfc: [''],
    correo: ['', [Validators.email]],
    telefono: [''],
    puesto: [''],
    salario: [0, [Validators.min(0)]],
    fecha_Ingreso: [''], // YYYY-MM-DD
    fecha_Baja: [''],    // YYYY-MM-DD
    estatus: ['Activo', [Validators.required]]
  });

  ngOnInit(): void {
    this.buscar(1);

    // Debounce input de nombre/apellidos
    this.search$
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => this.buscar(1));
  }

  // ========= CRUD / BUSCAR =========
  buscar(page: number) {
    this.cargando.set(true);
    this.page = page;

    this.api.buscar(
      this.filtroNombre,
      this.filtroPuesto,
      this.filtroEstatus,
      this.page,
      this.pageSize,
      this.sortBy,
      this.sortDir
    ).subscribe({
      next: (res: PagedResponse<EmpleadoDto>) => {
        this.empleados = res.items;
        this.total = res.total;
        this.totalPages = Math.max(1, Math.ceil(this.total / this.pageSize));
        this.cargando.set(false);
      },
      error: (err) => {
        console.error(err);
        this.cargando.set(false);
        Swal.fire('Error', 'No se pudieron cargar los empleados', 'error');
      }
    });
  }

  onNombreChange(val: string) {
    this.search$.next(val ?? '');
  }

  limpiarBusqueda() {
    this.filtroNombre = '';
    this.filtroPuesto = undefined;
    this.filtroEstatus = undefined;
    this.page = 1;
    this.buscar(1);
  }

  // ========= Paginación =========
  puedeAtras() { return this.page > 1; }
  puedeAdelante() { return this.page < this.totalPages; }
  irAtras() { if (this.puedeAtras()) this.buscar(this.page - 1); }
  irAdelante() { if (this.puedeAdelante()) this.buscar(this.page + 1); }

  trackById = (_: number, e: EmpleadoDto) => e.id_Empleado;

  // ========= Modal =========
  abrirModalNuevo() {
    this.editando.set(false);
    this.editId = null;
    this.form.reset({
      id_Empleado: 0,
      nombre: '',
      apellido_Paterno: '',
      apellido_Materno: '',
      curp: '',
      rfc: '',
      correo: '',
      telefono: '',
      puesto: '',
      salario: 0,
      fecha_Ingreso: '',
      fecha_Baja: '',
      estatus: 'Activo'
    });

    const modal = new bootstrap.Modal('#empleadoModal');
    modal.show();
  }

  editar(e: EmpleadoDto) {
    this.editando.set(true);
    this.editId = e.id_Empleado;

    this.form.reset({
      id_Empleado: e.id_Empleado,
      nombre: e.nombre,
      apellido_Paterno: e.apellido_Paterno ?? '',
      apellido_Materno: e.apellido_Materno ?? '',
      curp: e.curp ?? '',
      rfc: e.rfc ?? '',
      correo: e.correo ?? '',
      telefono: e.telefono ?? '',
      puesto: e.puesto ?? '',
      salario: e.salario ?? 0,
      fecha_Ingreso: e.fecha_Ingreso?.substring(0, 10) ?? '',
      fecha_Baja: e.fecha_Baja?.substring(0, 10) ?? '',
      estatus: e.estatus
    });

    const modal = new bootstrap.Modal('#empleadoModal');
    modal.show();
  }

  onCancelar() {
    const el = document.getElementById('empleadoModal');
    const modal = bootstrap.Modal.getInstance(el) || new bootstrap.Modal(el);
    modal.hide();
  }

  // ========= Guardar =========
  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const body = this.form.value;

    if (!this.editando()) {
      // Crear
      const req: EmpleadoCreateRequest = {
        nombre: body.nombre!,
        apellido_Paterno: body.apellido_Paterno || undefined,
        apellido_Materno: body.apellido_Materno || undefined,
        curp: body.curp || undefined,
        rfc: body.rfc || undefined,
        correo: body.correo || undefined,
        telefono: body.telefono || undefined,
        puesto: body.puesto || undefined,
        salario: Number(body.salario ?? 0),
        fecha_Ingreso: body.fecha_Ingreso || undefined,
        estatus: (body.estatus as any) ?? 'Activo',
      };

      this.api.create(req).subscribe({
        next: (nuevo) => {
          // === Optimistic update ===
          this.empleados = [nuevo, ...this.empleados];
          this.total += 1;
          this.totalPages = Math.max(1, Math.ceil(this.total / this.pageSize));

          this.onCancelar();
          Swal.fire('Éxito', 'Empleado creado', 'success');
          this.buscar(1); // Re-sync desde servidor
        },
        error: (err) => {
          console.error(err);
          Swal.fire('Error', 'No se pudo crear el empleado', 'error');
        }
      });

    } else {
      // Actualizar
      const req: EmpleadoUpdateRequest = {
        id_Empleado: this.editId!,
        nombre: body.nombre!,
        apellido_Paterno: body.apellido_Paterno || undefined,
        apellido_Materno: body.apellido_Materno || undefined,
        curp: body.curp || undefined,
        rfc: body.rfc || undefined,
        correo: body.correo || undefined,
        telefono: body.telefono || undefined,
        puesto: body.puesto || undefined,
        salario: Number(body.salario ?? 0),
        fecha_Ingreso: body.fecha_Ingreso || undefined,
        fecha_Baja: body.fecha_Baja || undefined,
        estatus: (body.estatus as any) ?? 'Activo',
      };

      this.api.update(this.editId!, req).subscribe({
        next: () => {
          // === Optimistic update ===
          const i = this.empleados.findIndex(x => x.id_Empleado === this.editId);
          if (i >= 0) {
            this.empleados[i] = {
              ...this.empleados[i],
              ...req,
              fecha_Ingreso: req.fecha_Ingreso ?? this.empleados[i].fecha_Ingreso,
              fecha_Baja: req.fecha_Baja ?? this.empleados[i].fecha_Baja
            };
            this.empleados = [...this.empleados]; // fuerza CD
          }

          this.onCancelar();
          Swal.fire('Éxito', 'Empleado actualizado', 'success');
          this.buscar(this.page); // Re-sync
        },
        error: (err) => {
          console.error(err);
          Swal.fire('Error', 'No se pudo actualizar el empleado', 'error');
        }
      });
    }
  }

  eliminar(e: EmpleadoDto) {
    Swal.fire({
      title: 'Confirmar',
      text: `¿Eliminar al empleado "${e.nombre}"?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar'
    }).then(res => {
      if (res.isConfirmed) {
        this.api.remove(e.id_Empleado).subscribe({
          next: () => {
            // === Optimistic update ===
            this.empleados = this.empleados.filter(x => x.id_Empleado !== e.id_Empleado);
            this.total = Math.max(0, this.total - 1);
            this.totalPages = Math.max(1, Math.ceil(this.total / this.pageSize));
            if (this.page > this.totalPages) this.page = this.totalPages;

            this.buscar(this.page); // Re-sync
            Swal.fire('Eliminado', 'Empleado eliminado', 'success');
          },
          error: (err) => {
            console.error(err);
            Swal.fire('Error', 'No se pudo eliminar el empleado', 'error');
          }
        });
      }
    });
  }
}

