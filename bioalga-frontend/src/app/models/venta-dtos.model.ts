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
// ======= NUEVO: DTO para la lista (historial) =======
export interface VentaResumenDto {
  idVenta: number;
  fechaVenta: string;            // ISO
  clienteId?: number | null;
  clienteNombre?: string | null;

  subtotal: number;
  impuestos: number;
  total: number;

  metodoPago: MetodoPago;
  estatus: EstatusVenta;

  partidas: number;              // # de líneas
  unidades: number;              // suma de cantidades

  idUsuario: number;
  usuarioNombre?: string | null;
}

// ======= NUEVO: DTO de línea (detalle) =======
export interface VentaLineaDto {
  idDetalle: number;
  idProducto: number;
  productoNombre: string;
  codigoSku?: string | null;

  cantidad: number;

  precioUnitario: number;
  descuentoUnitario: number;
  ivaUnitario: number;

  // Calculados (opcionales en el front)
  importeBruto?: number;
  importeDescuento?: number;
  importeIva?: number;
  importeNeto?: number;
}

// ======= NUEVO: DTO de detalle completo =======
export interface VentaDetalleDto {
  idVenta: number;
  fechaVenta: string;

  clienteId?: number | null;
  clienteNombre?: string | null;

  subtotal: number;
  impuestos: number;
  total: number;
  efectivoRecibido?: number | null;
  cambio?: number | null;

  metodoPago: MetodoPago;
  estatus: EstatusVenta;

  idUsuario: number;
  usuarioNombre?: string | null;

  partidas: number;
  unidades: number;

  detalles: VentaLineaDto[];
}

// ======= EXTENSIÓN: más filtros para /api/ventas =======
// (TypeScript permite "declaration merging"; no toco tu interfaz original)
export interface VentaQueryParams {
  clienteId?: number;
  usuarioId?: number;
  estatus?: EstatusVenta;        // 'Pagada' | 'Cancelada'
  metodoPago?: MetodoPago;       // 'Efectivo' | 'Tarjeta' | 'Transferencia' | 'Mixto'

  // Alternativas de nombre si prefieres mandar 'desde'/'hasta'
  desde?: string | null;         // ISO date (YYYY-MM-DD o YYYY-MM-DDTHH:mm)
  hasta?: string | null;
}
