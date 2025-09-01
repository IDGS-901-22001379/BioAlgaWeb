// src/app/models/pedido-dtos.model.ts

import { PagedResponse } from './paged-response.model';

/** Enum igual al backend (se serializa como string por JsonStringEnumConverter) */
export enum EstatusPedido {
  Borrador = 'Borrador',
  Confirmado = 'Confirmado',
  Preparacion = 'Preparacion',
  Listo = 'Listo',
  Facturado = 'Facturado',
  Entregado = 'Entregado',
  Cancelado = 'Cancelado',
}

/** -------- Lectura: Línea -------- */
export interface PedidoLineaDto {
  idDetalle: number;
  idProducto: number;
  productoNombre: string;
  cantidad: number;
  precioUnitario: number;
  /** Conveniencia para UI: cantidad * precioUnitario (lo manda el backend) */
  importe: number;
}

/** -------- Lectura: Item de listado -------- */
export interface PedidoListItemDto {
  idPedido: number;
  estatus: EstatusPedido;
  fechaPedido: string;       // ISO string (ej: 2025-08-31T12:34:56)
  fechaRequerida?: string;   // ISO o undefined

  idCliente: number;
  clienteNombre: string;

  subtotal: number;
  impuestos: number;
  total: number;
  anticipo: number;

  /** Conveniencia que envía el backend: total - anticipo */
  saldoPendiente: number;
}

/** -------- Lectura: Detalle completo -------- */
export interface PedidoDto extends PedidoListItemDto {
  idUsuario: number;
  notas?: string;
  lineas: PedidoLineaDto[];
}

/** -------- Creación: Línea -------- */
export interface PedidoLineaCreateRequest {
  idProducto: number;
  cantidad: number;
  /** Si se envía, el backend lo respeta (según permisos); si no, congela en Confirmación */
  precioUnitarioOverride?: number | null;
}

/** -------- Creación: Cabecera -------- */
export interface PedidoCreateRequest {
  idCliente: number;
  fechaRequerida?: string; // ISO
  anticipo: number;
  notas?: string;
  lineas: PedidoLineaCreateRequest[];
}

/** -------- Actualización: Cabecera (Borrador) -------- */
export interface PedidoUpdateHeaderRequest {
  idPedido: number;
  idCliente?: number;
  fechaRequerida?: string; // ISO
  anticipo?: number;
  notas?: string;
}

/** -------- Reemplazo total de líneas (Borrador) -------- */
export interface PedidoReplaceLinesRequest {
  idPedido: number;
  lineas: PedidoLineaCreateRequest[];
}

/** -------- Edición/Añadido de UNA línea (Borrador) -------- */
export interface PedidoLineaEditRequest {
  idPedido: number;
  idDetalle?: number; // si viene => edita, si no => agrega
  idProducto: number;
  cantidad: number;
  precioUnitarioOverride?: number | null;
}

/** -------- Confirmar Pedido -------- */
export interface PedidoConfirmarRequest {
  idPedido: number;
  reservarStock: boolean;
}

/** -------- Cambio de estatus -------- */
export interface PedidoCambioEstatusRequest {
  idPedido: number;
  nuevoEstatus: EstatusPedido;
}

/** -------- Query de búsqueda (conveniencia para servicio Angular) -------- */
export type PedidoSortBy = 'FechaPedido' | 'Total' | 'Estatus';
export type SortDir = 'ASC' | 'DESC';

export interface PedidoQueryParams {
  q?: string;
  estatus?: EstatusPedido;
  page?: number;
  pageSize?: number;
  sortBy?: PedidoSortBy;
  sortDir?: SortDir;
}

/** -------- Respuestas paginadas -------- */
export type PedidoListResponse = PagedResponse<PedidoListItemDto>;
