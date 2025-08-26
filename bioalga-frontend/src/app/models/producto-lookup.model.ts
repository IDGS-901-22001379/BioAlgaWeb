// src/app/models/producto-lookup.model.ts

export interface ProductoLookupDto {
  id_Producto: number;
  nombre: string;
  sku: string;
  codigo_Barras?: string | null;
  estatus: 'Activo' | 'Inactivo' | string;
  codigoBarras?: string | null;
  precio?: number;  // si tu /lookup ya lo trae
  stock?: number;
}
