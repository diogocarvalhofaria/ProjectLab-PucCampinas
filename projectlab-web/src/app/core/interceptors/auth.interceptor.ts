import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // TENTATIVA 1: Pega o 'auth_token'
  let token = localStorage.getItem('auth_token');

  // TENTATIVA 2: Se não achar, tenta 'accessToken' (comum em alguns templates)
  if (!token) {
    token = localStorage.getItem('accessToken');
  }

  // TENTATIVA 3: Se não achar, tenta pegar de dentro do 'user_data'
  if (!token) {
    const userData = localStorage.getItem('user_data');
    if (userData) {
      const parsed = JSON.parse(userData);
      token = parsed.token; // ou parsed.accessToken, verifique o json
    }
  }

  // DEBUG: Isso vai aparecer no console do navegador (F12)
  console.log('🔍 Interceptor rodando! Token encontrado:', token);

  if (token) {
    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(cloned);
  }

  return next(req);
};
