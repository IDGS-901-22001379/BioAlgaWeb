export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
