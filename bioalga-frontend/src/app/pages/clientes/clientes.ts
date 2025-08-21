// pages/clientes/clientes.ts
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';

import { ClientesService } from '../../services/clientes.service';
import {
  ClienteDto,
  ClienteCreateRequest,
  ClienteUpdateRequest,
  nombreCompleto
} from '../../models/cliente.model';

declare var bootstrap: any;

@Component({
  selector: 'app-clientes-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './clientes.html',
  styleUrls: ['./clientes.css'],
})
export class ClientesPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(ClientesService);

  // UI
  cargando = signal(false);
  editandoId = signal<number | null>(null);
  private modalRef: any;

  // Data / tabla
  clientes: ClienteDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;

  // Filtros (coinciden con backend)
  filtroNombre = ''; // -> q (nombre/apellidos/correo/teléfono en backend)
  filtroTipo: 'Normal' | 'Mayoreo' | 'Especial' | 'Descuento' | undefined = undefined; // -> tipo_Cliente
  filtroEstado: 'Activo' | 'Inactivo' | undefined = undefined; // -> estado

  // Orden (opcional; backend lo soporta)
  sortBy: 'nombre' | 'fecha' = 'nombre';
  sortDir: 'asc' | 'desc' = 'asc';

  // Búsqueda en vivo por nombre
  private nombreSearch$ = new Subject<string>();

  // Form modal (⚠️ ahora con apellidos paterno y materno)
  formCliente = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(2)]],
    apellido_Paterno: [''],
    apellido_Materno: [''],
    correo: ['', [Validators.email]],
    telefono: [''],
    direccion: [''],
    tipo_Cliente: ['Normal', [Validators.required]], // Normal/Mayoreo/Especial/Descuento
    estado: ['Activo', [Validators.required]], // Activo/Inactivo
  });

  constructor() {
    // Suscripción para búsqueda en vivo (debounce)
    this.nombreSearch$
      .pipe(debounceTime(250), distinctUntilChanged())
      .subscribe(() => this.buscar(1));
  }

  get totalPages(): number {
    return Math.ceil(this.total / (this.pageSize || 1));
  }

  ngOnInit(): void {
    this.buscar(1);
  }

  // Llamado por (ngModelChange) del input de búsqueda
  onNombreChange(valor: string) {
    this.filtroNombre = valor ?? '';
    this.nombreSearch$.next(this.filtroNombre);
  }

  // ==============================
  // Listar con filtros
  // ==============================
  buscar(page: number = this.page): void {
    this.cargando.set(true);
    this.page = page;

    const q = this.filtroNombre?.trim() || '';
    const tipo = this.filtroTipo || undefined;
    const estado = this.filtroEstado || undefined;

    this.api.buscar(q, tipo, estado, this.page, this.pageSize, this.sortBy, this.sortDir)
      .subscribe({
        next: (res) => {
          this.clientes = res.items;
          this.total = res.total;
          this.page = res.page;
          this.pageSize = res.pageSize;
          this.cargando.set(false);
        },
        error: (e) => {
          this.cargando.set(false);
          Swal.fire('Error', 'No se pudo cargar la lista de clientes', 'error');
          console.error(e);
        }
      });
  }

  limpiarBusqueda(): void {
    this.filtroNombre = '';
    this.filtroTipo = undefined;
    this.filtroEstado = undefined;
    this.sortBy = 'nombre';
    this.sortDir = 'asc';
    this.buscar(1);
  }

  // ==============================
  // Modal Bootstrap
  // ==============================
  private showModal(): void {
    const el = document.getElementById('clienteModal');
    if (el) {
      this.modalRef = new bootstrap.Modal(el, { backdrop: 'static' });
      this.modalRef.show();
    }
  }

  private hideModal(): void {
    if (this.modalRef) this.modalRef.hide();
  }

  onCancelar(): void {
    this.formCliente.reset();
    this.hideModal();
  }

  abrirModalNuevo(): void {
    this.editandoId.set(null);
    this.formCliente.reset({
      nombre: '',
      apellido_Paterno: '',
      apellido_Materno: '',
      correo: '',
      telefono: '',
      direccion: '',
      tipo_Cliente: 'Normal',
      estado: 'Activo',
    });
    this.showModal();
  }

  editar(c: ClienteDto): void {
    this.editandoId.set(c.id_Cliente);
    this.formCliente.reset({
      nombre: c.nombre,
      apellido_Paterno: c.apellido_Paterno ?? '',
      apellido_Materno: c.apellido_Materno ?? '',
      correo: c.correo ?? '',
      telefono: c.telefono ?? '',
      direccion: c.direccion ?? '',
      tipo_Cliente: c.tipo_Cliente,
      estado: c.estado,
    });
    this.showModal();
  }

  // ==============================
  // Crear / Actualizar
  // ==============================
  submit(): void {
    if (this.formCliente.invalid) {
      this.formCliente.markAllAsTouched();
      return;
    }

    const id = this.editandoId();
    const payload = this.formCliente.value as ClienteCreateRequest | ClienteUpdateRequest;

    this.cargando.set(true);

    if (id === null) {
      // Crear
      this.api.create(payload as ClienteCreateRequest).subscribe({
        next: (nuevo) => {
          Swal.fire('Listo', 'Cliente creado correctamente', 'success');
          // refrescamos lista en la página actual (opcional: reconsultar)
          this.clientes.unshift(nuevo);
          this.total++;
          this.cargando.set(false);
          this.hideModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = e?.error?.message || 'No se pudo crear el cliente';
          Swal.fire('Error', msg, 'error');
          console.error(e);
        }
      });
    } else {
      // Actualizar
      this.api.update(id, payload as ClienteUpdateRequest).subscribe({
        next: (actualizado) => {
          Swal.fire('Listo', 'Cliente actualizado correctamente', 'success');
          const i = this.clientes.findIndex(x => x.id_Cliente === actualizado.id_Cliente);
          if (i !== -1) this.clientes[i] = actualizado;
          this.cargando.set(false);
          this.hideModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = e?.error?.message || 'No se pudo actualizar el cliente';
          Swal.fire('Error', msg, 'error');
          console.error(e);
        }
      });
    }
  }

  // ==============================
  // Eliminar
  // ==============================
  eliminar(c: ClienteDto): void {
    const nombre = nombreCompleto({
      nombre: c.nombre,
      apellido_Paterno: c.apellido_Paterno,
      apellido_Materno: c.apellido_Materno
    });

    Swal.fire({
      title: '¿Eliminar cliente?',
      text: nombre,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
    }).then(res => {
      if (!res.isConfirmed) return;

      this.cargando.set(true);
      this.api.remove(c.id_Cliente).subscribe({
        next: () => {
          Swal.fire('Eliminado', 'Cliente eliminado correctamente', 'success');
          this.clientes = this.clientes.filter(x => x.id_Cliente !== c.id_Cliente);
          this.total--;
          this.cargando.set(false);

          // si se quedó vacía la página, retrocede una
          const vacia = (this.page - 1) * this.pageSize >= this.total && this.page > 1;
          if (vacia) this.buscar(this.page - 1);
        },
        error: (e) => {
          this.cargando.set(false);
          Swal.fire('Error', 'No se pudo eliminar el cliente', 'error');
          console.error(e);
        }
      });
    });
  }

  // ==============================
  // Paginación
  // ==============================
  puedeAtras(): boolean { return this.page > 1; }
  puedeAdelante(): boolean { return this.page * this.pageSize < this.total; }
  irAtras(): void { if (this.puedeAtras()) this.buscar(this.page - 1); }
  irAdelante(): void { if (this.puedeAdelante()) this.buscar(this.page + 1); }

  trackById = (_: number, c: ClienteDto) => c.id_Cliente;
}
