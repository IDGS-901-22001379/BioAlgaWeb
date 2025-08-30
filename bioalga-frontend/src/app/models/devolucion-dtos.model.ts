// src/app/models/devolucion-dtos.model.ts
export interface DevolucionLineaCreate {
  idProducto: number;
  cantidad: number;
  /** Importe TOTAL de la línea (precio ya con IVA/lo que corresponda) */
  importeLineaTotal: number;
}

export interface DevolucionCreateRequest {
  motivo: string;
  regresaInventario: boolean;
  /** opcional, folio/ticket o texto libre */
  referencia?: string | null;
  /** opcional */
  notas?: string | null;
  lineas: DevolucionLineaCreate[];
}

export interface DevolucionDetalleDto {
  idProducto: number;
  productoNombre?: string | null;
  cantidad: number;
  importeLineaTotal: number;
}

export interface DevolucionDto {
  idDevolucion: number;
  fecha: string;                 // ISO string
  motivo: string;
  referencia?: string | null;
  notas?: string | null;
  regresaInventario: boolean;
  totalDevuelto: number;
  usuarioNombre?: string | null;
  detalles: DevolucionDetalleDto[];
}

/** filtros/paginación */
export type DevolucionSortBy = 'fecha' | 'idDevolucion' | 'totalDevuelto';

export interface DevolucionQueryParams {
  q?: string;
  page?: number;
  pageSize?: number;
  sortBy?: DevolucionSortBy;
  sortDir?: 'asc' | 'desc';
}

/** genérico que ya usas en otros módulos */
export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
