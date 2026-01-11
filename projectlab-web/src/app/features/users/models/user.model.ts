export interface UserResponse {
  id: string;
  name: string;
  email: string;
  role: string;
  phoneNumber: string;
  ra: string;
  cep: string;
  logradouro: string;
  bairro: string;
  cidade: string;
  estado: string;
}

export interface UserRequest {
  name: string;
  email: string;
  role: string;
  phoneNumber: string;
  cep: string;
  logradouro: string;
  bairro: string;
  cidade: string;
  estado: string;
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

export interface EnderecoViaCep {
  logradouro: string;
  bairro: string;
  localidade: string;
  uf: string;
  cep?: string;
  complemento?: string;
}
