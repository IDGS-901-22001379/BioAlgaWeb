// DTOs de Caja
export interface CajaDto {
  id_Caja: number;
  nombre: string;
  descripcion?: string | null;
}

export interface CajaCreateRequest {
  nombre: string;
  descripcion?: string | null;
}

export interface CajaUpdateRequest {
  nombre: string;
  descripcion?: string | null;
}
