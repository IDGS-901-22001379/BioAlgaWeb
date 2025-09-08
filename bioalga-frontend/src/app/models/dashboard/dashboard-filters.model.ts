export interface DashboardFilters {
  /** ISO date: '2025-09-07' */
  from?: string;
  /** ISO date: '2025-09-30' */
  to?: string;
  /** Limitar resultados (ej: top 5) */
  top?: number;
}
