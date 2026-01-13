import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ReservationRequest,
  ReservationResponse,
  ReservedTime,
} from '../model/reservation.model';

@Injectable({
  providedIn: 'root',
})
export class ReservationService {
  // Ajuste a porta se necessário (no seu log parecia ser 8000 ou 5198)
  private apiUrl = 'http://localhost:8000/api/Reservation';

  constructor(private http: HttpClient) {}

  create(reservation: ReservationRequest): Observable<ReservationResponse> {
    return this.http.post<ReservationResponse>(this.apiUrl, reservation);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getMyReservations(): Observable<ReservationResponse[]> {
    return this.http.get<ReservationResponse[]>(
      `${this.apiUrl}/my-reservations`
    );
  }

  cancel(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/cancel`);
  }

  getReservedTimes(
    laboratoryId: string,
    date: string
  ): Observable<ReservedTime[]> {
    const params = new HttpParams()
      .set('laboratoryId', laboratoryId)
      .set('date', date);

    return this.http.get<ReservedTime[]>(`${this.apiUrl}/reservation-time`, {
      params,
    });
  }

  searchReservations(
    keyword: string = '',
    page: number = 1,
    size: number = 10
  ) {
    const params = new HttpParams()
      .set('Keyword', keyword)
      .set('Page', page.toString())
      .set('Size', size.toString());

    return this.http.get<any>(`${this.apiUrl}/search`, { params });
  }
}
