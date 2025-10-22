export type MetodoPago = 'Efectivo' | 'Tarjeta' | 'Transferencia' | 'Otro';

export interface VentaPagoDto {
  id_Pago: number;
  id_Venta: number;
  metodo: MetodoPago;
  monto: number;
  creado_En: string; // ISO
}

export interface CrearVentaPagoRequest {
  id_Venta: number;
  metodo: MetodoPago;
  monto: number;
}
