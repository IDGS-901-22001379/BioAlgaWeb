import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { UsuariosService } from '../../services/usuarios.service';
import { UsuarioDto, UsuarioCreateRequest, UsuarioUpdateRequest } from '../../models/usuario.models';
import Swal from 'sweetalert2';

declare var bootstrap: any; // Necesario para usar los modales de Bootstrap

@Component({
  selector: 'app-usuarios-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './usuarios.html',
  styleUrls: ['./usuarios.css'],
})
export class UsuariosPageComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(UsuariosService);

  cargando = signal(false);
  editandoId = signal<number | null>(null);

  usuarios: UsuarioDto[] = [];
  roles: any[] = []; // Lista de roles para el select del modal
  total = 0;
  page = 1;
  pageSize = 10;

  filtroNombre = '';
  filtroActivo: boolean | undefined = undefined;

  // Formulario del modal
  formUsuario = this.fb.group({
    nombre_Usuario: ['', [Validators.required, Validators.minLength(3)]],
    contrasena: [''],
    id_Rol: [1, [Validators.required]],
    id_Empleado: [null as number | null],
    activo: [true],
  });

  private modalRef: any; // referencia al modal Bootstrap

  get totalPages(): number {
    return Math.ceil(this.total / (this.pageSize || 1));
  }

  ngOnInit(): void {
    this.buscar();
    this.cargarRoles();
  }

  cargarRoles(): void {
    //  Puedes cambiar esto por un servicio real si tienes API de roles
    this.roles = [
      { id: 1, nombre: 'Administrador' },
      { id: 2, nombre: 'Usuario' },
      { id: 3, nombre: 'Empleado' }
    ];
  }

  // ==============================
  // Listar usuarios con filtros
  // ==============================
  buscar(page: number = this.page): void {
    this.cargando.set(true);
    this.api.buscar(this.filtroNombre, page, this.pageSize, this.filtroActivo)
      .subscribe({
        next: (res) => {
          this.usuarios = res.items;
          this.total = res.total;
          this.page = res.page;
          this.pageSize = res.pageSize;
          this.cargando.set(false);
        },
        error: (e) => {
          this.cargando.set(false);
          Swal.fire('Error', 'No se pudo cargar la lista de usuarios', 'error');
          console.error(e);
        }
      });
  }

  limpiarBusqueda(): void {
    this.filtroNombre = '';
    this.filtroActivo = undefined;
    this.buscar(1);
  }

  // ==============================
  // Modal: Abrir y Cerrar
  // ==============================
  private showModal(): void {
    const modalEl = document.getElementById('usuarioModal');
    if (modalEl) {
      this.modalRef = new bootstrap.Modal(modalEl, { backdrop: 'static' });
      this.modalRef.show();
    }
  }

  private hideModal(): void {
    if (this.modalRef) {
      this.modalRef.hide();
    }
  }

  abrirModalNuevo(): void {
    this.editandoId.set(null);
    this.formUsuario.reset({
      nombre_Usuario: '',
      contrasena: '',
      id_Rol: 1,
      id_Empleado: null,
      activo: true,
    });
    this.showModal();
  }

  editar(u: UsuarioDto): void {
    this.editandoId.set(u.id_Usuario);
    this.formUsuario.reset({
      nombre_Usuario: u.nombre_Usuario,
      contrasena: '',
      id_Rol: u.id_Rol,
      id_Empleado: u.id_Empleado ?? null,
      activo: u.activo,
    });
    this.showModal();
  }

  // ==============================
  // Crear o actualizar
  // ==============================
  submit(): void {
    if (this.formUsuario.invalid) {
      this.formUsuario.markAllAsTouched();
      return;
    }

    const id = this.editandoId();

    if (id === null) {
      // Crear usuario
      const body: UsuarioCreateRequest = {
        nombre_Usuario: this.formUsuario.value.nombre_Usuario!,
        contrasena: this.formUsuario.value.contrasena || '',
        id_Rol: Number(this.formUsuario.value.id_Rol),
        id_Empleado: this.formUsuario.value.id_Empleado ?? null,
        activo: !!this.formUsuario.value.activo,
      };

      if (!body.contrasena.trim()) {
        Swal.fire('Falta contraseña', 'Para crear un usuario, ingresa una contraseña.', 'warning');
        return;
      }

      this.cargando.set(true);
      this.api.create(body).subscribe({
        next: (nuevoUsuario) => {
          Swal.fire('Listo', 'Usuario creado correctamente ', 'success');
          // 🔹 Agregarlo directamente a la tabla sin recargar
          this.usuarios.push(nuevoUsuario);
          this.total++;
          this.hideModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = (e?.error as string) || 'No se pudo crear el usuario';
          Swal.fire('Error', msg, 'error');
          console.error(e);
        }
      });
    } else {
      // Editar usuario
      const body: UsuarioUpdateRequest = {
        nombre_Usuario: this.formUsuario.value.nombre_Usuario || undefined,
        contrasena: this.formUsuario.value.contrasena || undefined,
        id_Rol: this.formUsuario.value.id_Rol ?? undefined,
        id_Empleado: this.formUsuario.value.id_Empleado ?? undefined,
        activo: this.formUsuario.value.activo ?? undefined,
      };

      this.cargando.set(true);
      this.api.update(id, body).subscribe({
        next: (usuarioActualizado) => {
          Swal.fire('Listo', 'Usuario actualizado correctamente ', 'success');
          // 🔹 Reemplazar en la tabla sin recargar
          const index = this.usuarios.findIndex(u => u.id_Usuario === usuarioActualizado.id_Usuario);
          if (index !== -1) this.usuarios[index] = usuarioActualizado;
          this.hideModal();
        },
        error: (e) => {
          this.cargando.set(false);
          const msg = (e?.error as string) || 'No se pudo actualizar el usuario';
          Swal.fire('Error', msg, 'error');
          console.error(e);
        }
      });
    }
  }

  // ==============================
  // Eliminar
  // ==============================
  eliminar(u: UsuarioDto): void {
    Swal.fire({
      title: '¿Eliminar usuario?',
      text: `Se eliminará el usuario: ${u.nombre_Usuario}`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
    }).then(res => {
      if (res.isConfirmed) {
        this.cargando.set(true);
        this.api.remove(u.id_Usuario).subscribe({
          next: () => {
            Swal.fire('Eliminado', 'Usuario eliminado correctamente ', 'success');
            // 🔹 Sacarlo de la tabla directamente
            this.usuarios = this.usuarios.filter(x => x.id_Usuario !== u.id_Usuario);
            this.total--;
          },
          error: (e) => {
            this.cargando.set(false);
            Swal.fire('Error', 'No se pudo eliminar el usuario', 'error');
            console.error(e);
          }
        });
      }
    });
  }

  // ==============================
  // Paginación
  // ==============================
  puedeAtras(): boolean { return this.page > 1; }
  puedeAdelante(): boolean { return this.page * this.pageSize < this.total; }
  irAtras(): void { if (this.puedeAtras()) this.buscar(this.page - 1); }
  irAdelante(): void { if (this.puedeAdelante()) this.buscar(this.page + 1); }

  trackById = (_: number, u: UsuarioDto) => u.id_Usuario;
}
