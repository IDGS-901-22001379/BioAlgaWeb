export interface CajaTurnoDto {
  idTurno: number;
  idCaja: number;
  idUsuario: number;
  apertura: string;          // ISO
  cierre?: string | null;    // ISO
  saldoInicial: number;
  saldoCierre?: number | null;
  observaciones?: string | null;
}

// Request para abrir (sin IDs)
export interface AbrirTurnoRequest {
  nombreUsuario: string;
  nombreCaja: string;
  saldoInicial: number;
  observaciones?: string | null;
  descripcionCaja?: string | null; // opcional si el backend lo soporta
}

// Request para cerrar (id va en la ruta)
export interface CerrarTurnoRequest {
  saldoCierre: number;
  observaciones?: string | null;
}

// Query de búsqueda
export interface CajaTurnoQueryParams {
  idCaja?: number;
  idUsuario?: number;
  desde?: string;  // ISO date/time
  hasta?: string;  // ISO date/time
  page?: number;
  pageSize?: number;
}
