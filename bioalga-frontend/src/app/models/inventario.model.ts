// src/app/models/inventario.model.ts

export type TipoMovimiento = 'Entrada' | 'Salida' | 'Ajuste';
export type OrigenMovimiento = 'Compra' | 'Venta' | 'Pedido' | 'Ajuste' | 'Devolucion';

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

// Para consultas de Kardex / stock (cuando hagamos los endpoints)
export interface KardexItem {
  fecha: string;
  tipo: TipoMovimiento;          // Entrada/Salida/Ajuste
  cantidad: number;
  saldo?: number;                // opcional si backend lo calcula
  origen: string;                // "Compra #12" etc.
}

export interface StockResponse {
  id_Producto: number;
  stock: number;
}
