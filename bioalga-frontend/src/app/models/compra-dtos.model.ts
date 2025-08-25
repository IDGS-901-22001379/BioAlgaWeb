// src/app/models/compra-dtos.model.ts

export interface CrearCompraDto {
  proveedor_Id?: number | null;
  proveedor_Texto?: string | null;
  notas?: string | null;
  id_Usuario: number;   // lo enviarás desde el login/estado
}

export interface AgregarRenglonDto {
  id_Producto: number;
  cantidad: number;
  costo_Unitario: number;
  iva_Unitario?: number;   // default 0 en backend
}

export interface ConfirmarCompraResponse {
  id_Compra: number;
  movimientosCreados: number;
  subtotal: number;
  impuestos: number;
  total: number;
}

export interface CompraQueryParams {
  q?: string | null;
  desde?: string | null;   // ISO (yyyy-MM-dd) o ISO datetime
  hasta?: string | null;
  page?: number;           // default 1
  pageSize?: number;       // default 10
}
