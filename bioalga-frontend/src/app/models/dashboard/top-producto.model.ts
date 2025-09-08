export interface TopProducto {
  idProducto: number;
  nombre: string;
  totalUnidades: number;
  /** Ingreso total por producto (usado en vw_top_productos_ingreso) */
  ingresoTotal: number;
}
