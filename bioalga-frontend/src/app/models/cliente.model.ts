// models/cliente.model.ts

// === DTO que llega desde el backend ===
// NOTA: los nombres mantienen el subrayado porque el backend
// serializa en camelCase pero conservando los underscores.
export interface ClienteDto {
  id_Cliente: number;
  nombre: string;
  apellido_Paterno?: string | null;
  apellido_Materno?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  tipo_Cliente: 'Normal' | 'Mayoreo' | 'Especial' | 'Descuento';
  estado: 'Activo' | 'Inactivo';
  fecha_Registro: string; // ISO string
}

// === Requests para crear/actualizar ===
export interface ClienteCreateRequest {
  nombre: string;
  apellido_Paterno?: string | null;
  apellido_Materno?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  tipo_Cliente?: 'Normal' | 'Mayoreo' | 'Especial' | 'Descuento' | null; // default "Normal"
  estado?: 'Activo' | 'Inactivo' | null;                                   // default "Activo"
}

export interface ClienteUpdateRequest {
  nombre?: string;
  apellido_Paterno?: string | null;
  apellido_Materno?: string | null;
  correo?: string | null;
  telefono?: string | null;
  direccion?: string | null;
  tipo_Cliente?: 'Normal' | 'Mayoreo' | 'Especial' | 'Descuento' | null;
  estado?: 'Activo' | 'Inactivo' | null;
}

// === Respuesta paginada (la usa tu tabla) ===
export interface PagedResponse<T> {
  total: number;
  items: T[];
  page: number;
  pageSize: number;
}

// === (Opcional) helper para mostrar nombre completo en la UI ===
export const nombreCompleto = (c: Pick<ClienteDto,
  'nombre' | 'apellido_Paterno' | 'apellido_Materno'>) =>
  [c.nombre, c.apellido_Paterno, c.apellido_Materno]
    .filter(Boolean)
    .join(' ');
