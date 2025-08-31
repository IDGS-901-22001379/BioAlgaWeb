// Línea de la devolución a crear
export interface DevolucionLineaCreate {
  idProducto: number;
  cantidad: number;
  /** Importe TOTAL de la línea (precio ya con IVA/lo que corresponda) */
  importeLineaTotal: number;
}

// Request para crear devolución
export interface DevolucionCreateRequest {
  motivo: string;
  regresaInventario: boolean;
  /** opcional, folio/ticket o texto libre */
  referenciaVenta?: string | null;
  /** opcional */
  notas?: string | null;
  lineas: DevolucionLineaCreate[];
}

// Detalle incluido en el DTO
export interface DevolucionDetalleDto {
  idProducto: number;
  productoNombre?: string | null;
  cantidad: number;
  importeLineaTotal: number;
}

// DTO principal (mismos nombres que backend)
export interface DevolucionDto {
  idDevolucion: number;
  fechaDevolucion: string;
  motivo: string;
  referenciaVenta?: string | null;
  notas?: string | null;
  regresaInventario: boolean;
  totalDevuelto: number;
  usuarioNombre?: string | null;
  detalles: DevolucionDetalleDto[];
}

// Listado (el repo usa sortBy compacto: fecha_desc|fecha_asc|total_desc|total_asc)
export type DevolucionSortBy = 'fecha_desc' | 'fecha_asc' | 'total_desc' | 'total_asc';

export interface DevolucionQueryParams {
  q?: string;
  page?: number;
  pageSize?: number;
  sortBy?: DevolucionSortBy;
  // sortDir ya NO se usa en el repo, pero lo dejamos opcional por compatibilidad
  sortDir?: 'asc' | 'desc';
}

// Respuesta paginada genérica
export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
