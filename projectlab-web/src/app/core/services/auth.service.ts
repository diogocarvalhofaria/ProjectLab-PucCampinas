import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

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
    this.loadUserFromToken();
  }

  private loadUserFromToken() {
    const token = localStorage.getItem('auth_token');

    localStorage.removeItem('user_data');

    if (token) {
      try {
        const decoded: any = jwtDecode(token);

        const user: UserPayload = {
          id: decoded.nameid || decoded.sub || '',
          name: decoded.unique_name || decoded.name || 'Usuário',
          role:
            decoded.role ||
            decoded[
              'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
            ] ||
            '',
          ra: decoded.ra || '',
          token: token,
        };

        this.userSubject.next(user);
      } catch (error) {
        console.error('Token inválido ou expirado', error);
        this.logout();
      }
    }
  }

  login(ra: string, password: string): Observable<UserPayload> {
    return this.http
      .post<UserPayload>(`${this.apiUrl}/login`, { ra, password })
      .pipe(
        tap((response) => {
          localStorage.setItem('auth_token', response.token);

          localStorage.removeItem('user_data');

          this.userSubject.next(response);
        })
      );
  }

  setupPassword(token: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/setup-password`, {
      token: token,
      newPassword: newPassword,
    });
  }

  logout() {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_data');
    this.userSubject.next(null);
    this.router.navigate(['/login']);
  }

  private getDecodedToken(): any {
    const token = localStorage.getItem('auth_token');
    if (!token) return null;
    try {
      return jwtDecode(token);
    } catch (e) {
      return null;
    }
  }

  public isAdmin(): boolean {
    const decoded = this.getDecodedToken();
    if (!decoded) return false;
    const role =
      decoded.role ||
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    return role === 'Admin';
  }

  public getUserIdFromToken(): string {
    const decoded = this.getDecodedToken();
    if (!decoded) return '';
    return decoded.nameid || decoded.sub || '';
  }

  get currentUserValue(): UserPayload | null {
    return this.userSubject.value;
  }
}
