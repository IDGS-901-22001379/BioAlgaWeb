import type { MetodoPago } from './venta-pagos.model';

export interface CorteMetodoDto {
  metodo: MetodoPago | string; // si viene de ventas puede ser string
  monto: number;
}

export interface CorteResumenDto {
  // Metadatos del turno
  id_Turno: number;
  id_Caja: number;
  id_Usuario: number;
  apertura: string;        // ISO
  cierre_Usado: string;    // ISO: cierre real o "ahora"

  // Ventas
  num_Tickets: number;
  ventas_Totales: number;

  // Métodos de pago
  ventas_Por_Metodo: CorteMetodoDto[];

  // Efectivo de ventas (efectivo_recibido - cambio)
  ventas_Efectivo_Neto: number;

  // Movimientos manuales
  entradas_Efectivo: number;
  salidas_Efectivo: number;

  // Devoluciones
  devoluciones_Total: number;

  // Fondo y arqueo
  saldo_Inicial: number;
  efectivo_Esperado: number;
}
