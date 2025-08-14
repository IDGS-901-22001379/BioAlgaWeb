export interface UsuarioDto {
  id_Usuario: number;
  nombre_Usuario: string;
  rol: string;
  id_Rol: number;
  id_Empleado?: number | null;
  nombre_Empleado?: string | null;
  apellido_Paterno?: string | null;
  apellido_Materno?: string | null;
  activo: boolean;
  ultimo_Login?: string | null;
  fecha_Registro: string;
}

export interface UsuarioCreateRequest {
  nombre_Usuario: string;
  contrasena: string;
  id_Rol: number;
  id_Empleado?: number | null;
  activo: boolean;
}

export interface UsuarioUpdateRequest {
  nombre_Usuario?: string;
  contrasena?: string;
  id_Rol?: number;
  id_Empleado?: number | null;
  activo?: boolean;
}

export interface PagedResponse<T> {
  total: number;
  items: T[];
  page: number;
  pageSize: number;
}
