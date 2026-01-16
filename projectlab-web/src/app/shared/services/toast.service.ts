import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'warning' | 'info';
}

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  toasts$ = new BehaviorSubject<Toast[]>([]);

  show(
    message: string,
    type: 'success' | 'error' | 'warning' | 'info' = 'info'
  ) {
    const currentToasts = this.toasts$.value;
    const id = Date.now();

    const newToast: Toast = { id, message, type };

    this.toasts$.next([...currentToasts, newToast]);

    setTimeout(() => {
      this.remove(id);
    }, 4000);
  }

  success(message: string) {
    this.show(message, 'success');
  }

  error(message: string) {
    this.show(message, 'error');
  }

  warning(message: string) {
    this.show(message, 'warning');
  }

  info(message: string) {
    this.show(message, 'info');
  }

  remove(id: number) {
    const currentToasts = this.toasts$.value;
    this.toasts$.next(currentToasts.filter((t) => t.id !== id));
  }
}
