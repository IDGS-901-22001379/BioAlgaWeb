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
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = false;

  form = this.fb.group({
    nombreUsuario: ['', [Validators.required, Validators.minLength(3)]],
    contrasena: ['', [Validators.required, Validators.minLength(6)]],
  });

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.auth.login(this.form.value as any).subscribe({
      next: (user) => {
        this.loading = false;

        // ✅ Éxito
        Swal.fire({
          icon: 'success',
          title: '¡Bienvenido!',
          text: `Hola ${user.nombre_Usuario}, has iniciado sesión correctamente.`,
          confirmButtonText: 'Continuar'
        }).then(() => this.router.navigateByUrl('/inicio'));
      },
      error: (err) => {
        this.loading = false;

        // Mensaje claro según el código
        let mensaje = 'Ocurrió un error. Inténtalo de nuevo.';
        const status = err?.status;

        if (status === 401 || status === 403) {
          mensaje = 'Usuario o contraseña inválidos.';
        } else if (status === 0) {
          mensaje = 'No se pudo conectar con el servidor. ¿Está encendido el backend?';
        } else if (typeof err?.error === 'string' && err.error.trim()) {
          mensaje = err.error; // texto que envía el backend
        } else if (err?.error?.title) {
          mensaje = err.error.title;
        }

        // ❌ Error
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
