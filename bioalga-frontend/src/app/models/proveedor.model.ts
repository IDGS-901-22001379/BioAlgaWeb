// src/app/models/proveedor.model.ts

/** ===========================
 *  DTOs que vienen del backend
 *  (mantengo los nombres EXACTOS del JSON del API)
 *  =========================== */
export interface ProveedorDto {
  id_Proveedor: number;
  nombre_Empresa: string;
  contacto?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  rfc?: string | null;
  pais?: string | null;
  ciudad?: string | null;
  codigo_Postal?: string | null;
  estatus: 'Activo' | 'Inactivo';
  created_At: string; // ISO string
}

/** Payload para crear */
export interface ProveedorCreateRequest {
  nombre_Empresa: string;
  contacto?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  rfc?: string | null;
  pais?: string | null;
  ciudad?: string | null;
  codigo_Postal?: string | null;
}

/** Payload para actualizar */
export interface ProveedorUpdateRequest extends ProveedorCreateRequest {
  estatus: 'Activo' | 'Inactivo';
}

/** Parámetros de búsqueda/paginación */
export interface ProveedorQueryParams {
  q?: string;
  estatus?: 'Activo' | 'Inactivo';
  pais?: string;
  ciudad?: string;
  page?: number;      // default 1 en backend
  pageSize?: number;  // default 10 en backend
  sortBy?: 'Nombre_Empresa' | 'Pais' | 'Ciudad' | 'Estatus' | 'Created_At';
  sortDir?: 'asc' | 'desc';
}

/** Si ya tienes un PagedResponse<T> global, úsalo.
 *  Lo dejo aquí por claridad, pero comenta si ya lo tienes.
 */
export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
}
