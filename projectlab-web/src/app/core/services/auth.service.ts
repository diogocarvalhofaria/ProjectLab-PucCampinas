import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface UserPayload {
  id: string;
  name: string;
  ra: string;
  role: string;
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'http://localhost:8000/api/Auth';
  private userSubject = new BehaviorSubject<UserPayload | null>(null);

  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.loadUserFromStorage();
  }

  private loadUserFromStorage() {
    const storedUser = localStorage.getItem('user_data');
    if (storedUser) {
      this.userSubject.next(JSON.parse(storedUser));
    }
  }

  login(ra: string, password: string): Observable<UserPayload> {
    return this.http
      .post<UserPayload>(`${this.apiUrl}/login`, { ra, password })
      .pipe(
        tap((response) => {
          localStorage.setItem('user_data', JSON.stringify(response));
          localStorage.setItem('auth_token', response.token);
          this.userSubject.next(response);
        })
      );
  }

  logout() {
    localStorage.removeItem('user_data');
    localStorage.removeItem('auth_token');
    this.userSubject.next(null);
    this.router.navigate(['/login']);
  }

  get currentUserValue(): UserPayload | null {
    return this.userSubject.value;
  }

  setupPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/setup-password`, {
      token: token,
      newPassword: newPassword,
    });
  }

  getCurrentUser(): UserPayload | null {
    const userJson = localStorage.getItem('user_data');
    if (userJson) {
      return JSON.parse(userJson);
    }
    return null;
  }
}
