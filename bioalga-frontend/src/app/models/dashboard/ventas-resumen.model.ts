export interface VentasResumen {
  /** ISO date (yyyy-MM-dd) que viene del backend */
  dia: string;
  anio: number;
  mes: number;
  semana: number;
  totalVentas: number;
  subtotal: number;
  impuestos: number;
  numTickets: number;
}
