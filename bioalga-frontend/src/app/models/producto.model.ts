export type TipoProducto =
  | 'Componente' | 'Sensor' | 'Actuador' | 'Módulo' | 'Accesorio'
  | 'Cable' | 'Alimentación' | 'Herramienta' | 'Microcontrolador'
  | 'Biorreactor' | 'Alga' | 'Otro';

export interface ProductoDto {
  id_Producto: number;
  nombre: string;
  descripcion?: string | null;
  tipo: TipoProducto | string;     // el backend envía enum como string
  id_Categoria?: number | null;
  id_Marca?: number | null;
  id_Unidad?: number | null;
  proveedor_Preferente_Id?: number | null;
  codigo_Sku: string;
  codigo_Barras?: string | null;
  estatus: 'Activo' | 'Inactivo';
  created_At: string;  // ISO
  updated_At: string;  // ISO

  // opcionales (si decides traer precios vigentes embebidos)
  precioNormal?: number | null;
  precioMayoreo?: number | null;
  precioDescuento?: number | null;
  precioEspecial?: number | null;
}

export interface CrearProductoDto {
  nombre: string;
  descripcion?: string | null;
  tipo: TipoProducto | string;
  idCategoria?: number | null;
  idMarca?: number | null;
  idUnidad?: number | null;
  proveedorPreferenteId?: number | null;
  codigoSku: string;
  codigoBarras?: string | null;
  estatus?: 'Activo' | 'Inactivo';
}

export interface ActualizarProductoDto extends CrearProductoDto {
  estatus: 'Activo' | 'Inactivo';
}
