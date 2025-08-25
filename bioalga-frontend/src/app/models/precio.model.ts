export type TipoPrecio = 'Normal' | 'Mayoreo' | 'Descuento' | 'Especial';

export interface PrecioDto {
  id_Precio: number;
  id_Producto: number;
  tipo_Precio: TipoPrecio | string;
  precio: number;
  vigente_Desde: string; // ISO
  vigente_Hasta?: string | null; // ISO
  activo: boolean;
}

export interface CrearPrecioDto {
  tipoPrecio: TipoPrecio | string;
  precio: number;
  vigenteDesde?: string | null; // ISO
  vigenteHasta?: string | null; // ISO
  activo?: boolean;
}

export interface ActualizarPrecioDto {
  precio: number;
  vigenteHasta?: string | null;
  activo?: boolean;
}
