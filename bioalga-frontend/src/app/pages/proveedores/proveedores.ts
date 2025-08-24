import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import Swal from 'sweetalert2';

import { ProveedoresService } from '../../services/proveedores.service';
import {
  ProveedorDto,
  ProveedorCreateRequest,
  ProveedorUpdateRequest,
  PagedResponse
} from '../../models/proveedor.model';

declare var bootstrap: any;

@Component({
  selector: 'app-proveedores-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './proveedores.html',
  styleUrls: ['./proveedores.css'],
})
export class ProveedoresPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(ProveedoresService);

  // UI
  cargando = signal(false);
  editId: number | null = null;
  private modalRef: any;

  // Data
  proveedores: ProveedorDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;

  get totalPages() { return Math.max(1, Math.ceil(this.total / this.pageSize)); }

  // ===== Filtros (igual que clientes) =====
  // Texto libre: "nombre / correo" (entra como q al backend)
  filtroNombreCorreo = '';
  filtroEstatus: 'Activo' | 'Inactivo' | undefined;
  filtroPais: string | undefined;
  filtroCiudad: string | undefined;

  // búsqueda en vivo (como clientes)
  private search$ = new Subject<string>();

  // Form (modal)
  formProveedor = this.fb.group({
    nombre_Empresa: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(120)]],
    contacto: [''],
    correo: ['', [Validators.email]],
    telefono: [''],
    direccion: [''],
    rfc: [''],
    pais: [''],
    ciudad: [''],
    codigo_Postal: [''],
    estatus: ['Activo' as 'Activo' | 'Inactivo']
  });

  constructor() {
    // Búsqueda con debounce, igual a clientes
    this.search$.pipe(debounceTime(250), distinctUntilChanged())
      .subscribe(() => this.buscar(1));
  }

  ngOnInit() {
    this.buscar(1);
  }

  // ======== BÚSQUEDA / LISTADO ========
  onNombreCorreoChange(v: string) {
    this.filtroNombreCorreo = v ?? '';
    this.search$.next(this.filtroNombreCorreo);
  }

  limpiarBusqueda() {
    this.filtroNombreCorreo = '';
    this.filtroEstatus = undefined;
    this.filtroPais = undefined;
    this.filtroCiudad = undefined;
    this.buscar(1);
  }

  buscar(p: number) {
    this.cargando.set(true);
    this.page = p;

    this.api.buscar({
      q: this.filtroNombreCorreo?.trim() || undefined,
      estatus: this.filtroEstatus,
      pais: this.filtroPais,
      ciudad: this.filtroCiudad,
      page: this.page,
      pageSize: this.pageSize,
      sortBy: 'Nombre_Empresa',
      sortDir: 'asc'
    }).subscribe({
      next: (res: PagedResponse<ProveedorDto>) => {
        this.proveedores = res.items;
        this.total = res.total;
        this.page = res.page;
        this.pageSize = res.pageSize;
        this.cargando.set(false);
      },
      error: (e) => {
        this.cargando.set(false);
        Swal.fire('Error', 'No se pudo cargar la lista de proveedores', 'error');
        console.error(e);
      }
    });
  }

  // ======== MODAL ========
  private showModal() {
    const el = document.getElementById('proveedorModal');
    if (!el) return;
    this.modalRef = new bootstrap.Modal(el, { backdrop: 'static' });
    this.modalRef.show();
  }
  private hideModal() { this.modalRef?.hide(); }
  onCancelar() { this.formProveedor.reset(); this.hideModal(); }

  abrirModalNuevo() {
    this.editId = null;
    this.formProveedor.reset({
      nombre_Empresa: '',
      contacto: '',
      correo: '',
      telefono: '',
      direccion: '',
      rfc: '',
      pais: '',
      ciudad: '',
      codigo_Postal: '',
      estatus: 'Activo'
    });
    this.showModal();
  }

  editar(p: ProveedorDto) {
    this.editId = p.id_Proveedor;
    this.formProveedor.reset({
      nombre_Empresa: p.nombre_Empresa,
      contacto: p.contacto ?? '',
      correo: p.correo ?? '',
      telefono: p.telefono ?? '',
      direccion: p.direccion ?? '',
      rfc: p.rfc ?? '',
      pais: p.pais ?? '',
      ciudad: p.ciudad ?? '',
      codigo_Postal: p.codigo_Postal ?? '',
      estatus: p.estatus
    });
    this.showModal();
  }

  // ======== CREAR / ACTUALIZAR (SweetAlert + refresco inmediato) ========
  submit() {
    if (this.formProveedor.invalid) {
      this.formProveedor.markAllAsTouched();
      return;
    }

    const base: ProveedorCreateRequest = {
      nombre_Empresa: this.formProveedor.value.nombre_Empresa!,
      contacto: this.formProveedor.value.contacto || undefined,
      correo: this.formProveedor.value.correo || undefined,
      telefono: this.formProveedor.value.telefono || undefined,
      direccion: this.formProveedor.value.direccion || undefined,
      rfc: this.formProveedor.value.rfc || undefined,
      pais: this.formProveedor.value.pais || undefined,
      ciudad: this.formProveedor.value.ciudad || undefined,
      codigo_Postal: this.formProveedor.value.codigo_Postal || undefined,
    };

    this.cargando.set(true);

    // Crear
    if (this.editId === null) {
      this.api.crear(base).subscribe({
        next: (nuevo) => {
          Swal.fire('Listo', 'Proveedor creado correctamente', 'success');
          // mostrarlo sin recargar: insertamos al inicio
          this.proveedores.unshift(nuevo);
          this.total++;
          // si la página se excede del tamaño, opcional reconsultar la primera página
          if (this.proveedores.length > this.pageSize) this.buscar(1);
          this.cargando.set(false);
          this.hideModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = e?.error?.message || 'No se pudo crear el proveedor';
          Swal.fire('Error', msg, 'error');
          console.error(e);
        }
      });
      return;
    }

    // Actualizar
    const upd: ProveedorUpdateRequest = {
      ...base,
      estatus: this.formProveedor.value.estatus as ('Activo' | 'Inactivo')
    };
    this.api.actualizar(this.editId, upd).subscribe({
      next: (actualizado) => {
        Swal.fire('Listo', 'Proveedor actualizado correctamente', 'success');
        const i = this.proveedores.findIndex(x => x.id_Proveedor === actualizado.id_Proveedor);
        if (i !== -1) this.proveedores[i] = actualizado;
        this.cargando.set(false);
        this.hideModal();
      },
      error: (e) => {
        this.cargando.set(false);
        const msg = e?.error?.message || 'No se pudo actualizar el proveedor';
        Swal.fire('Error', msg, 'error');
        console.error(e);
      }
    });
  }

  // ======== ELIMINAR (SweetConfirm + refresh local) ========
  eliminar(p: ProveedorDto) {
    Swal.fire({
      title: '¿Eliminar proveedor?',
      text: p.nombre_Empresa,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
    }).then(res => {
      if (!res.isConfirmed) return;

      this.cargando.set(true);
      this.api.eliminar(p.id_Proveedor).subscribe({
        next: () => {
          Swal.fire('Eliminado', 'Proveedor eliminado correctamente', 'success');
          this.proveedores = this.proveedores.filter(x => x.id_Proveedor !== p.id_Proveedor);
          this.total--;
          this.cargando.set(false);

          // si se quedó vacía la página, retrocede una
          const vacia = (this.page - 1) * this.pageSize >= this.total && this.page > 1;
          if (vacia) this.buscar(this.page - 1);
        },
        error: (e) => {
          this.cargando.set(false);
          Swal.fire('Error', 'No se pudo eliminar el proveedor', 'error');
          console.error(e);
        }
      });
    });
  }

  // ======== Paginación ========
  puedeAtras() { return this.page > 1; }
  puedeAdelante() { return this.page * this.pageSize < this.total; }
  irAtras() { if (this.puedeAtras()) this.buscar(this.page - 1); }
  irAdelante() { if (this.puedeAdelante()) this.buscar(this.page + 1); }

  trackById = (_: number, p: ProveedorDto) => p.id_Proveedor;
}
