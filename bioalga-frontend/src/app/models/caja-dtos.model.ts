// Enums idénticos al backend
export enum TipoCajaMovimiento {
  Ingreso = 'Ingreso',
  Egreso = 'Egreso',
}

// Apertura
export interface CajaAperturaCreate {
  fondoInicial: number;
}

export interface CajaAperturaDto {
  idCajaApertura: number;
  fechaApertura: string; // ISO
  idUsuario: number;
  fondoInicial: number;
  activa: boolean;
}

// Movimientos
export interface CajaMovimientoCreate {
  idCajaApertura: number;
  tipo: TipoCajaMovimiento;
  concepto: string;
  montoEfectivo: number;
  idVenta?: number | null; // opcional si viene de una venta
}

export interface CajaMovimientoDto {
  idCajaMovimiento: number;
  idCajaApertura: number;
  fecha: string; // ISO
  tipo: TipoCajaMovimiento;
  concepto: string;
  montoEfectivo: number;
  idVenta?: number | null;
  idUsuario: number;
}

// Corte
export interface CajaCorteCreate {
  idCajaApertura: number;
  totalEfectivoContado: number;
}

export interface CajaCorteDto {
  idCajaCorte: number;
  idCajaApertura: number;
  fechaCorte: string; // ISO
  totalEfectivoEsperado: number;
  totalEfectivoContado: number;
  diferencia: number;
  idUsuario: number;
}

// (Opcional) filtros para listados/reportes
export interface CajaQueryParams {
  idUsuario?: number;
  activa?: boolean;
  fechaDesde?: string | null;
  fechaHasta?: string | null;
  page?: number;
  pageSize?: number;
  sortBy?: string;  // 'fecha_apertura'
  sortDir?: 'asc' | 'desc';
}
