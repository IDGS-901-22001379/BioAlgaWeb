export interface CajaTurnoDto {
  id_Turno: number;
  id_Caja: number;
  id_Usuario: number;

  apertura: string;       // ISO
  cierre?: string | null; // ISO

  saldo_Inicial: number;
  saldo_Cierre?: number | null;
  observaciones?: string | null;

  // ⬇️ agregar como opcionales para que no rompa si el backend no los manda
  caja_Nombre?: string | null;
  usuario_Nombre?: string | null;
}

// Abrir turno
export interface AbrirTurnoRequest {
  id_Caja: number;
  id_Usuario: number;     // el usuario logueado por defecto
  saldo_Inicial: number;
  observaciones?: string | null;
}

// Cerrar turno
export interface CerrarTurnoRequest {
  id_Turno: number;       // redundante si viene por ruta, pero útil en formularios
  saldo_Cierre: number;
  observaciones?: string | null;
}

// Búsqueda / listado de turnos
export interface CajaTurnoQueryParams {
  idCaja?: number | null;
  idUsuario?: number | null;
  desde?: string | null;  // ISO
  hasta?: string | null;  // ISO
  page?: number;
  pageSize?: number;
}
