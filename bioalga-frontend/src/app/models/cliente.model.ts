// models/cliente.model.ts

export interface ClienteDto {
  id_Cliente: number;
  nombre: string;
  apellido?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  tipo_Cliente: string;   // "Normal" | "Premium" (ej.)
  estado: string;         // "Activo" | "Inactivo"
  fecha_Registro: string; // ISO string
}

export interface ClienteCreateRequest {
  nombre: string;
  apellido?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  tipo_Cliente?: string | null; // default "Normal"
  estado?: string | null;       // default "Activo"
}

export interface ClienteUpdateRequest {
  nombre?: string;
  apellido?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  tipo_Cliente?: string | null;
  estado?: string | null;
}

export interface PagedResponse<T> {
  total: number;
  items: T[];
  page: number;
  pageSize: number;
}
