import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { NotificationService, ToastMessage } from '../../../core/services/notification.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container" *ngIf="toasts.length > 0">
      <div *ngFor="let toast of toasts" class="toast-card" [ngClass]="'toast-' + toast.type">
        <div class="toast-icon">
          <i class="fa-solid" [ngClass]="{
            'fa-circle-check': toast.type === 'success',
            'fa-triangle-exclamation': toast.type === 'warning',
            'fa-circle-exclamation': toast.type === 'danger',
            'fa-circle-info': toast.type === 'info'
          }"></i>
        </div>
        <div class="toast-body">
          <div class="toast-title" *ngIf="toast.title">{{ toast.title }}</div>
          <div class="toast-msg">{{ toast.message }}</div>
        </div>
        <button class="toast-close" (click)="removeToast(toast.id)">
          <i class="fa-solid fa-xmark"></i>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: 90px;
      right: 24px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 12px;
      max-width: 400px;
      width: calc(100vw - 48px);
    }
    .toast-card {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 16px;
      background: #ffffff;
      border-radius: 12px;
      box-shadow: 0 10px 25px rgba(0, 0, 0, 0.12), 0 4px 10px rgba(0, 0, 0, 0.05);
      border-left: 5px solid transparent;
      animation: slideIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
    }
    .toast-success { border-left-color: #10b981; }
    .toast-danger { border-left-color: #ef4444; }
    .toast-warning { border-left-color: #f59e0b; }
    .toast-info { border-left-color: #3b82f6; }
    .toast-icon {
      font-size: 1.25rem;
      margin-top: 2px;
    }
    .toast-success .toast-icon { color: #10b981; }
    .toast-danger .toast-icon { color: #ef4444; }
    .toast-warning .toast-icon { color: #f59e0b; }
    .toast-info .toast-icon { color: #3b82f6; }
    .toast-body { flex: 1; }
    .toast-title { font-weight: 700; font-size: 0.95rem; color: #0f172a; margin-bottom: 2px; }
    .toast-msg { font-size: 0.88rem; color: #475569; line-height: 1.4; }
    .toast-close {
      background: none; border: none; color: #94a3b8; cursor: pointer; font-size: 1rem;
      &:hover { color: #0f172a; }
    }
    @keyframes slideIn {
      from { transform: translateX(100%); opacity: 0; }
      to { transform: translateX(0); opacity: 1; }
    }
  `]
})
export class ToastComponent implements OnInit, OnDestroy {
  toasts: ToastMessage[] = [];
  private sub?: Subscription;

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.sub = this.notificationService.toasts$.subscribe(toast => {
      this.toasts.push(toast);
      const duration = toast.duration || 5000;
      setTimeout(() => this.removeToast(toast.id), duration);
    });
  }

  removeToast(id: string): void {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}

