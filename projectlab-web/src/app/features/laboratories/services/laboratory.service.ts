import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  LaboratoryResponse,
  PaginatedResult,
} from '../models/laboratory.model';
import { HttpParams } from '@angular/common/http';

export type LaboratoryInput = Omit<LaboratoryResponse, 'id'>;

export interface SearchParams {
  keyword?: string;
  building?: string;
  page?: number;
  size?: number;
}

@Injectable({
  providedIn: 'root',
})
export class LaboratoryService {
  private readonly API_URL = 'http://localhost:8000/api/Laboratory';

  constructor(private http: HttpClient) {}

  findAll(
    paramsInput: SearchParams
  ): Observable<PaginatedResult<LaboratoryResponse>> {
    let params = new HttpParams()
      .set('Page', paramsInput.page || 1)
      .set('Size', paramsInput.size || 10); // 10 itens por página padrão

    if (paramsInput.keyword)
      params = params.set('Keyword', paramsInput.keyword);
    if (paramsInput.building)
      params = params.set('Building', paramsInput.building);

    return this.http.get<PaginatedResult<LaboratoryResponse>>(
      `${this.API_URL}/search`,
      { params }
    );
  }
  findById(id: string): Observable<LaboratoryResponse> {
    return this.http.get<LaboratoryResponse>(`${this.API_URL}/${id}`);
  }

  create(lab: LaboratoryInput): Observable<object> {
    return this.http.post('http://localhost:8000/api/Laboratory', lab);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`http://localhost:8000/api/Laboratory/${id}`);
  }

  update(id: string, lab: LaboratoryInput): Observable<void> {
    return this.http.put<void>(
      `http://localhost:8000/api/Laboratory/${id}`,
      lab
    );
  }
}
