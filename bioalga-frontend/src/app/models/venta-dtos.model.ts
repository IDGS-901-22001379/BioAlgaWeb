// Enums idénticos al backend
export enum MetodoPago {
  Efectivo = 'Efectivo',
  Tarjeta = 'Tarjeta',
  Transferencia = 'Transferencia',
  Mixto = 'Mixto',
}

export enum EstatusVenta {
  Pagada = 'Pagada',
  Cancelada = 'Cancelada',
}

// Linea de venta (request)
export interface VentaLineaCreate {
  idProducto: number;
  cantidad: number;
  precioUnitario: number;
  descuentoUnitario: number;
  ivaUnitario: number;
}

// Crear venta (request)
export interface VentaCreateRequest {
  clienteId?: number | null;
  metodoPago: MetodoPago;
  efectivoRecibido?: number | null; // requerido si metodoPago=Efectivo (validado en backend)
  lineas: VentaLineaCreate[];
}

// Venta (response)
export interface VentaDto {
  idVenta: number;
  clienteId?: number | null;
  fechaVenta: string; // ISO

  subtotal: number;
  impuestos: number;
  total: number;

  metodoPago: MetodoPago;
  efectivoRecibido?: number | null;
  cambio?: number | null;

  estatus: EstatusVenta | 'Pagada' | 'Cancelada'; // compatibilidad
  lineas: VentaLineaCreate[]; // se reutiliza el shape de línea
}

// Parámetros de búsqueda/listado (cuando hagas GET /api/ventas)
export interface VentaQueryParams {
  q?: string | null;
  fechaDesde?: string | null; // 'YYYY-MM-DD'
  fechaHasta?: string | null;
  page?: number;
  pageSize?: number;
  sortBy?: string;   // 'fecha_venta'
  sortDir?: 'asc' | 'desc';
}
