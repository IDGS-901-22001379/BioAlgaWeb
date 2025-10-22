export type TipoMovimiento = 'Ingreso' | 'Egreso';

export interface CajaMovimientoDto {
  id_Mov: number;
  id_Turno: number;
  fecha: string;      // ISO
  tipo: TipoMovimiento;
  concepto: string;
  monto: number;
  referencia?: string | null;
}

export interface CrearCajaMovimientoRequest {
  id_Turno: number;
  tipo: TipoMovimiento;   // Ingreso | Egreso
  concepto: string;
  monto: number;
  referencia?: string | null;
}

export interface ActualizarCajaMovimientoRequest {
  tipo: TipoMovimiento;
  concepto: string;
  monto: number;
  referencia?: string | null;
}

export interface CajaMovimientoQueryParams {
  idTurno: number;
  tipo?: TipoMovimiento | null;
  q?: string | null;        // busca por concepto
  page?: number;
  pageSize?: number;
}
