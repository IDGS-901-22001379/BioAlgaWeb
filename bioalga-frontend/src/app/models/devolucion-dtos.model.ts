// src/app/models/devolucion-dtos.model.ts

// ============== READ / RESPONSE ==============
export interface DevolucionDto {
  idDevolucion: number;
  fechaDevolucion: string;        // viene como ISO string desde .NET
  idUsuario: number;
  usuarioNombre: string;
  motivo: string;
  regresaInventario: boolean;
  totalDevuelto: number;
  ventaId?: number | null;
  referenciaVenta?: string | null;
  notas?: string | null;
  detalles: DevolucionDetalleDto[];
}

export interface DevolucionDetalleDto {
  idDetalle: number;
  idProducto: number;
  productoNombre: string;
  cantidad: number;
  importeLineaTotal: number;
  idDetalleVenta?: number | null;
}

// Versión compacta para listados (si la usas en tablas)
export interface DevolucionListItemDto {
  idDevolucion: number;
  fechaDevolucion: string;
  usuarioNombre: string;
  motivo: string;
  regresaInventario: boolean;
  totalDevuelto: number;
  ventaId?: number | null;
  referenciaVenta?: string | null;
}

// ============== CREATE / REQUEST ==============
export interface DevolucionCreateRequest {
  ventaId?: number | null;
  referenciaVenta?: string | null;
  usuarioNombre: string;
  motivo: string;
  regresaInventario: boolean;
  notas?: string | null;
  lineas: DevolucionLineaCreate[];
}

export interface DevolucionLineaCreate {
  idProducto: number;
  productoNombre: string;
  cantidad: number;
  idDetalleVenta?: number | null; // si ligas a un renglón de venta
  precioUnitario?: number | null; // obligatorio cuando no hay idDetalleVenta
}

// ============== QUERY PARAMS (GET /api/devoluciones) ==============
export interface DevolucionQueryParams {
  desde?: string;               // ISO string (yyyy-MM-dd o completa)
  hasta?: string;
  q?: string;
  regresaInventario?: boolean;
}

// ============== Helpers opcionales ==============
export const defaultDevolucionCreate = (): DevolucionCreateRequest => ({
  ventaId: null,
  referenciaVenta: null,
  usuarioNombre: '',
  motivo: '',
  regresaInventario: true,
  notas: null,
  lineas: []
});

export const nuevaLineaDevolucion = (): DevolucionLineaCreate => ({
  idProducto: 0,
  productoNombre: '',
  cantidad: 1,
  idDetalleVenta: null,
  precioUnitario: null
});
