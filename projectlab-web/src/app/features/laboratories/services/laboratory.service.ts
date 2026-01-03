import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  LaboratoryResponse,
  PaginatedResult,
} from '../models/laboratory.model';

@Injectable({
  providedIn: 'root',
})
export class LaboratoryService {
  private readonly API_URL = 'http://localhost:8000/api/Laboratory';

  constructor(private http: HttpClient) {}

  findAll(): Observable<PaginatedResult<LaboratoryResponse>> {
    return this.http.get<PaginatedResult<LaboratoryResponse>>(
      'http://localhost:8000/api/Laboratory/search'
    );
  }

  findById(id: string): Observable<LaboratoryResponse> {
    return this.http.get<LaboratoryResponse>(`${this.API_URL}/${id}`);
  }

  create(lab: any): Observable<any> {
    return this.http.post('http://localhost:8000/api/Laboratory', lab);
  }
}
