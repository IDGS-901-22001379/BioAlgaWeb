// src/app/models/compra.model.ts

export interface DetalleCompra {
  id_Detalle: number;
  id_Producto: number;
  producto: string | null;  // backend envía "" si no mapea; aquí permitimos null
  sku: string | null;
  cantidad: number;
  costo_Unitario: number;
  iva_Unitario: number;
}

export interface Compra {
  id_Compra: number;
  proveedor_Id: number | null;
  proveedor_Texto: string | null;
  fecha_Compra: string;      // ISO string
  subtotal: number;
  impuestos: number;
  total: number;
  id_Usuario: number;
  notas: string | null;
  estado: 'Borrador' | 'Confirmada' | string;
  detalles: DetalleCompra[];
}
