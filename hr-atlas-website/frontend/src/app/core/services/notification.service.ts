import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface ToastMessage {
  id: string;
  type: 'success' | 'danger' | 'warning' | 'info';
  title?: string;
  message: string;
  duration?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private toastSubject = new Subject<ToastMessage>();
  public toasts$ = this.toastSubject.asObservable();

  show(type: 'success' | 'danger' | 'warning' | 'info', message: string, title?: string, duration: number = 5000): void {
    const id = Math.random().toString(36).substring(2, 9);
    this.toastSubject.next({ id, type, message, title, duration });
  }

  success(message: string, title: string = 'Success'): void {
    this.show('success', message, title);
  }

  error(message: string, title: string = 'Error'): void {
    this.show('danger', message, title);
  }

  info(message: string, title: string = 'Information'): void {
    this.show('info', message, title);
  }

  warning(message: string, title: string = 'Warning'): void {
    this.show('warning', message, title);
  }
}

