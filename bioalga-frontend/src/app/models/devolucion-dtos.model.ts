// Línea de devolución (request)
export interface DevolucionLineaCreate {
  idProducto: number;
  cantidad: number;
  precioUnitario: number;
  ivaUnitario: number;
}

// Crear devolución (request)
export interface DevolucionCreateRequest {
  idVenta: number;
  motivo: string;            // 'defecto', 'garantía', etc.
  reingresaInventario: boolean; // true=Entrada, false=Merma/Ajuste
  lineas: DevolucionLineaCreate[];
}

// Devolución (response)
export interface DevolucionDto {
  idDevolucion: number;
  idVenta: number;
  fecha: string; // ISO
  motivo: string;
  reingresaInventario: boolean;

  subtotal: number;
  impuestos: number;
  total: number;

  lineas: DevolucionLineaCreate[];
}

// Parámetros de búsqueda/listado
export interface DevolucionQueryParams {
  idVenta?: number;
  fechaDesde?: string | null;
  fechaHasta?: string | null;
  page?: number;
  pageSize?: number;
  sortBy?: string;  // 'fecha'
  sortDir?: 'asc' | 'desc';
}
