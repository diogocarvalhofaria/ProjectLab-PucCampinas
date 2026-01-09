import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  EnderecoViaCep,
  PaginatedResult,
  UserRequest,
  UserResponse,
} from '../models/user.model';

export interface SearchParams {
  keyword?: string;
  role?: string;
  page?: number;
  size?: number;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = 'http://localhost:8000/api/User';

  constructor(private http: HttpClient) {}

  findAll(
    paramsInput: SearchParams
  ): Observable<PaginatedResult<UserResponse>> {
    let params = new HttpParams()
      .set('Page', paramsInput.page || 1)
      .set('Size', paramsInput.size || 9);

    if (paramsInput.keyword) {
      params = params.set('Keyword', paramsInput.keyword);
    }

    if (paramsInput.role) {
      params = params.set('Role', paramsInput.role);
    }

    return this.http.get<PaginatedResult<UserResponse>>(
      `${this.apiUrl}/search`,
      { params }
    );
  }
  create(user: UserRequest): Observable<void> {
    return this.http.post<void>(this.apiUrl, user);
  }

  update(id: string, user: UserRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, user);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  consultarCep(cep: string): Observable<EnderecoViaCep> {
    const cepLimpo = cep.replace(/\D/g, '');
    return this.http.get<EnderecoViaCep>(
      `${this.apiUrl}/consult-cep/${cepLimpo}`
    );
  }
}
