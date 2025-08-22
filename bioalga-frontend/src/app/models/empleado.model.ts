// src/app/models/empleado.model.ts

// ====== DTO que devuelve el backend ======
export interface EmpleadoDto {
  id_Empleado: number;
  nombre: string;
  apellido_Paterno?: string | null;
  apellido_Materno?: string | null;
  curp?: string | null;
  rfc?: string | null;
  correo?: string | null;
  telefono?: string | null;
  puesto?: string | null;
  salario: number;
  fecha_Ingreso?: string | null; // ISO string (date)
  fecha_Baja?: string | null;    // ISO string (date)
  estatus: 'Activo' | 'Inactivo' | 'Baja';
  created_At: string;            // ISO datetime
  updated_At: string;            // ISO datetime

  // Campo de conveniencia que calculamos en backend; si no llega, la UI puede ignorarlo
  nombre_Completo?: string;
}

// ====== Crear ======
export interface EmpleadoCreateRequest {
  nombre: string;
  apellido_Paterno?: string | null;
  apellido_Materno?: string | null;
  curp?: string | null;
  rfc?: string | null;
  correo?: string | null;
  telefono?: string | null;
  puesto?: string | null;
  salario: number;
  fecha_Ingreso?: string | null; // 'YYYY-MM-DD'
  estatus: 'Activo' | 'Inactivo' | 'Baja';
}

// ====== Actualizar ======
export interface EmpleadoUpdateRequest extends EmpleadoCreateRequest {
  id_Empleado: number;
  fecha_Baja?: string | null;
}

// ====== Parámetros de búsqueda/paginación (coinciden con backend) ======
export interface EmpleadoQueryParams {
  q?: string;
  puesto?: string;
  estatus?: 'Activo' | 'Inactivo' | 'Baja';
  page?: number;        // default 1
  pageSize?: number;    // default 10
  sortBy?: 'nombre' | 'fecha_ingreso' | 'salario' | 'puesto' | 'estatus';
  sortDir?: 'ASC' | 'DESC';
}

// ====== Respuesta paginada genérica ======
export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  // totalPages es opcional en nuestro backend actual
  totalPages?: number;
}
