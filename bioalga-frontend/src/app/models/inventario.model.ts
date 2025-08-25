// src/app/models/inventario.model.ts

// Tipos base
export type TipoMovimiento = 'Entrada' | 'Salida' | 'Ajuste';
export type OrigenMovimiento = 'Compra' | 'Venta' | 'Pedido' | 'Ajuste' | 'Devolucion';

// Entidad de movimiento (cuando listamos movimientos crudos)
export interface InventarioMovimiento {
  id_Movimiento: number;
  id_Producto: number;
  tipo_Movimiento: TipoMovimiento;
  cantidad: number;
  fecha: string;                 // ISO
  origen_Tipo: OrigenMovimiento;
  origen_Id?: number | null;
  id_Usuario: number;
  referencia?: string | null;
}

// Respuesta de Kardex (ordenado por fecha, con saldo si el backend lo calcula)
export interface KardexItem {
  fecha: string;                 // ISO
  tipo: TipoMovimiento;          // Entrada/Salida/Ajuste
  cantidad: number;              // Salida suele venir negativa
  saldo?: number;                // opcional si backend lo calcula
  origen: string;                // "Compra #12", "Ajuste #—", etc.
}

// Respuesta de stock actual
export interface StockResponse {
  id_Producto: number;
  stock: number;
}

// ===== Ajustes manuales (Agregar/Quitar) =====

// Payload para agregar/quitar stock manualmente
export interface AjusteInventarioDto {
  id_Producto: number;
  cantidad: number;              // > 0
  id_Usuario: number;            // quién realiza el ajuste
  motivo?: string | null;        // referencia/motivo opcional
}

// Respuesta del backend al registrar un ajuste
export interface MovimientoResultDto {
  id_Movimiento: number;
  id_Producto: number;
  tipo_Movimiento: TipoMovimiento;    // 'Entrada' | 'Salida'
  cantidad: number;
  fecha: string;                      // ISO
  origen_Tipo: OrigenMovimiento;      // 'Ajuste' en estos endpoints
  origen_Id?: number | null;
  referencia?: string | null;
  stock_Despues: number;              // stock resultante tras el movimiento
}
