export interface LaboratoryResponse {
  id: string;
  name: string;
  building: string;
  capacity: number;
}

export interface LaboratoryRequest {
  name: string;
  building: string;
  capacity: number;
}

export interface PaginatedResult<T> {
  results: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
  totalPages: number;
  previousPage: boolean;
  nextPage: boolean;
}
