export interface LoginRequest {
  nombreUsuario: string;
  contrasena: string;
}

export interface LoginResponse {
  id_Usuario: number;
  nombre_Usuario: string;
  rol: string;
  // si luego agregas token JWT, añádelo aquí: token?: string;
}
