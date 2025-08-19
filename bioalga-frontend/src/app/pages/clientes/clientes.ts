// pages/clientes/clientes.ts
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ClientesService } from '../../services/clientes.service';
import { ClienteDto, ClienteCreateRequest, ClienteUpdateRequest } from '../../models/cliente.model';
import Swal from 'sweetalert2';

declare var bootstrap: any; // usar modales Bootstrap como en Usuarios

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

  // UI state
  cargando = signal(false);
  editandoId = signal<number | null>(null);
  private modalRef: any;

  // Data
  clientes: ClienteDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;

  // Filtros (coinciden con backend: q y tipo_Cliente)
  filtroNombre = '';
  filtroTipo: string | undefined = undefined; // "Normal" | "Premium" | undefined

  // Form del modal
  formCliente = this.fb.group({
    nombre: ['', [Validators.required, Validators.minLength(2)]],
    apellido: [''],
    correo: ['', [Validators.email]],
    telefono: [''],
    direccion: [''],
    tipo_Cliente: ['Normal', [Validators.required]],
  });

  get totalPages(): number {
    return Math.ceil(this.total / (this.pageSize || 1));
  }

  ngOnInit(): void {
    this.buscar(1);
  }

  // ==============================
  // Listar con filtros + paginación
  // ==============================
  buscar(page: number = this.page): void {
    this.cargando.set(true);
    this.api.buscar(this.filtroNombre, this.filtroTipo, page, this.pageSize).subscribe({
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
    this.buscar(1);
  }

  // ==============================
  // Modal: abrir / cerrar
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

  abrirModalNuevo(): void {
    this.editandoId.set(null);
    this.formCliente.reset({
      nombre: '',
      apellido: '',
      correo: '',
      telefono: '',
      direccion: '',
      tipo_Cliente: 'Normal',
    });
    this.showModal();
  }

  editar(c: ClienteDto): void {
    this.editandoId.set(c.id_Cliente);
    this.formCliente.reset({
      nombre: c.nombre,
      apellido: c.apellido ?? '',
      correo: c.correo ?? '',
      telefono: c.telefono ?? '',
      direccion: c.direccion ?? '',
      tipo_Cliente: c.tipo_Cliente,
    });
    this.showModal();
  }

  // ==============================
  // Crear o actualizar
  // ==============================
  submit(): void {
    if (this.formCliente.invalid) {
      this.formCliente.markAllAsTouched();
      return;
    }

    const id = this.editandoId();
    const payload = this.formCliente.value as ClienteCreateRequest | ClienteUpdateRequest;

    if (id === null) {
      // Crear
      this.cargando.set(true);
      this.api.create(payload as ClienteCreateRequest).subscribe({
        next: (nuevo) => {
          Swal.fire('Listo', 'Cliente creado correctamente', 'success');
          // agregar a la tabla sin recargar
          this.clientes.unshift(nuevo);
          this.total++;
          this.cargando.set(false);
          this.hideModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = e?.error ?? 'No se pudo crear el cliente';
          Swal.fire('Error', msg, 'error');
          console.error(e);
        }
      });
    } else {
      // Actualizar
      this.cargando.set(true);
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
          const msg = e?.error ?? 'No se pudo actualizar el cliente';
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
    Swal.fire({
      title: '¿Eliminar cliente?',
      text: `${c.nombre} ${c.apellido ?? ''}`.trim(),
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
          // si la página queda vacía, retrocede una
          const quedaVacia = (this.page - 1) * this.pageSize >= this.total && this.page > 1;
          if (quedaVacia) this.buscar(this.page - 1);
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
