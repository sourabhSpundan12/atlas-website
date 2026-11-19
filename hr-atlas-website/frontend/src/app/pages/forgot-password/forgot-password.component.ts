import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { ApiResponse } from '../../core/models/auth.model';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {
  currentStep: number = 1;
  emailForm!: FormGroup;
  otpForm!: FormGroup;
  passwordForm!: FormGroup;

  isLoading: boolean = false;
  errorMessage: string = '';
  resendCountdown: number = 0;
  private resendTimer: any;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private notification: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.emailForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });

    this.otpForm = this.fb.group({
      digit1: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit2: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit3: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit4: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit5: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit6: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]]
    });

    this.passwordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordsMatchValidator });
  }

  passwordsMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  sendResetOtp(): void {
    if (this.emailForm.invalid) {
      this.emailForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    const email = this.emailForm.get('email')?.value;

    this.authService.sendOtp({ email, purpose: 'PasswordReset' }).subscribe({
      next: (res: ApiResponse<string>) => {
        this.isLoading = false;
        this.currentStep = 2;
        this.startResendTimer();
        this.notification.success(`Verification OTP sent to ${email}`);
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Could not send verification code.';
        this.notification.error(this.errorMessage);
      }
    });
  }

  startResendTimer(): void {
    this.resendCountdown = 60;
    clearInterval(this.resendTimer);
    this.resendTimer = setInterval(() => {
      this.resendCountdown--;
      if (this.resendCountdown <= 0) clearInterval(this.resendTimer);
    }, 1000);
  }

  onDigitInput(event: any, nextIndex: number): void {
    const input = event.target as HTMLInputElement;
    if (input.value && nextIndex <= 6) {
      document.getElementById('f-otp-' + nextIndex)?.focus();
    }
  }

  getEnteredOtp(): string {
    const v = this.otpForm.value;
    return `${v.digit1}${v.digit2}${v.digit3}${v.digit4}${v.digit5}${v.digit6}`;
  }

  confirmOtp(): void {
    const otp = this.getEnteredOtp();
    if (otp.length !== 6) {
      this.notification.warning('Please enter all 6 digits.');
      return;
    }
    this.currentStep = 3;
  }

  resetPassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const email = this.emailForm.get('email')?.value;
    const otp = this.getEnteredOtp();
    const newPassword = this.passwordForm.get('newPassword')?.value;

    this.authService.forgotPassword({ email, otp, newPassword }).subscribe({
      next: (res: ApiResponse<boolean>) => {
        this.isLoading = false;
        if (res.success) {
          this.currentStep = 4;
          this.notification.success('Password updated successfully! Please sign in.');
        }
      },
      error: (err: any) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Password update failed. Please check OTP code.';
        this.notification.error(this.errorMessage);
      }
    });
  }
}
