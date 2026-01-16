import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../../shared/services/toast.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toast = inject(ToastService);

  let token = localStorage.getItem('auth_token');

  if (!token) {
    token = localStorage.getItem('accessToken');
  }

  if (!token) {
    const userData = localStorage.getItem('user_data');
    if (userData) {
      try {
        const parsed = JSON.parse(userData);
        token = parsed.token;
      } catch (e) {
        console.error('Erro ao ler user_data', e);
      }
    }
  }

  const authReq = token
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        console.warn('Token expirado ou inválido (401). Realizando logout...');

        localStorage.clear();
        toast.error('Sua sessão expirou. Por favor, faça login novamente.');

        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};
