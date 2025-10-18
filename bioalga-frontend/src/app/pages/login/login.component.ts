// login.component.ts
// Componente de Login con diseño tipo neón (mantiene tu lógica)
// ----------------------------------------------------------------
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',   // plantilla con el diseño tipo neón
  styleUrls: ['./login.css']     // estilos del login (fondo oscuro, líneas neón, tarjeta, etc.)
})
export class LoginComponent {
  // Inyección de dependencias
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  // Estado de carga para deshabilitar botón y cambiar texto
  loading = false;

  // Form reactivo con validaciones
  form = this.fb.group({
    nombreUsuario: ['', [Validators.required, Validators.minLength(3)]],
    contrasena: ['', [Validators.required, Validators.minLength(6)]],
  });

  // Envío del formulario
  submit() {
    // Si el formulario es inválido, marcamos controles y no continuamos
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;

    // Llamada al servicio de autenticación
    this.auth.login(this.form.value as any).subscribe({
      next: (user) => {
        this.loading = false;

        // Notificación de éxito
        Swal.fire({
          icon: 'success',
          title: '¡Bienvenido!',
          text: `Hola ${user.nombre_Usuario}, has iniciado sesión correctamente.`,
          confirmButtonText: 'Continuar'
        }).then(() => this.router.navigateByUrl('/inicio')); // Redirección posterior al login
      },
      error: (err) => {
        this.loading = false;

        // Normalizamos el mensaje de error
        let mensaje = 'Ocurrió un error. Inténtalo de nuevo.';
        const status = err?.status;

        if (status === 401 || status === 403) {
          mensaje = 'Usuario o contraseña inválidos.';
        } else if (status === 0) {
          mensaje = 'No se pudo conectar con el servidor. ¿Está encendido el backend?';
        } else if (typeof err?.error === 'string' && err.error.trim()) {
          mensaje = err.error; // Texto plano del backend
        } else if (err?.error?.title) {
          mensaje = err.error.title; // Problemas formateados por middleware
        }

        // Notificación de error
        Swal.fire({
          icon: 'error',
          title: 'No se pudo iniciar sesión',
          text: mensaje,
          confirmButtonText: 'Entendido'
        });
      }
    });
  }
}
