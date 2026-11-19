import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterModule, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { SaasService } from '../../core/services/saas.service';
import { NotificationService } from '../../core/services/notification.service';
import { TenantRegistrationResponse } from '../../core/models/saas.model';
import { ApiResponse } from '../../core/models/auth.model';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent implements OnInit {
  currentStep: number = 1;
  companyForm!: FormGroup;
  adminForm!: FormGroup;
  otpForm!: FormGroup;
  planForm!: FormGroup;

  isCheckingSlug: boolean = false;
  slugAvailable: boolean | null = null;
  slugMessage: string = '';

  isSendingOtp: boolean = false;
  isSubmittingRegistration: boolean = false;
  otpSentMessage: string = '';
  resendCountdown: number = 0;
  private resendTimer: any;

  provisionedResult: TenantRegistrationResponse | null = null;

  industries: string[] = [
    'Software & Information Technology',
    'Financial Services & Fintech',
    'Healthcare & Pharmaceuticals',
    'Manufacturing & Logistics',
    'E-commerce & Retail',
    'Consulting & Professional Services',
    'Education & EdTech',
    'Other'
  ];

  companySizes: string[] = [
    '1 - 15 Employees',
    '16 - 50 Employees',
    '51 - 200 Employees',
    '201 - 500 Employees',
    '500+ Employees'
  ];

  countries: string[] = ['India', 'United States', 'United Kingdom', 'Singapore', 'UAE', 'Canada', 'Australia'];

  constructor(
    private fb: FormBuilder,
    private saasService: SaasService,
    private authService: AuthService,
    private notification: NotificationService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initForms();

    this.route.queryParams.subscribe(params => {
      if (params['plan']) {
        this.planForm.patchValue({ planId: params['plan'] });
      }
      if (params['billing']) {
        this.planForm.patchValue({ billingCycle: params['billing'] });
      }
    });
  }

  initForms(): void {
    this.companyForm = this.fb.group({
      companyName: ['', [Validators.required, Validators.minLength(2)]],
      slug: ['', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
      industry: ['Software & Information Technology', Validators.required],
      companySize: ['16 - 50 Employees', Validators.required],
      country: ['India', Validators.required],
      timezone: ['Asia/Kolkata', Validators.required]
    });

    this.adminForm = this.fb.group({
      adminFullName: ['', [Validators.required, Validators.minLength(2)]],
      adminEmail: ['', [Validators.required, Validators.email]],
      adminPhoneNumber: ['', [Validators.required, Validators.minLength(10)]],
      jobTitle: ['Founder / Admin', Validators.required],
      password: ['', [Validators.required, Validators.minLength(8)]],
      agreeTerms: [true, Validators.requiredTrue]
    });

    this.otpForm = this.fb.group({
      digit1: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit2: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit3: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit4: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit5: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit6: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]]
    });

    this.planForm = this.fb.group({
      planId: ['growth', Validators.required],
      billingCycle: ['Monthly', Validators.required]
    });
  }

  onCompanyNameChange(): void {
    const name = this.companyForm.get('companyName')?.value || '';
    if (!this.companyForm.get('slug')?.dirty) {
      const generatedSlug = name
        .toLowerCase()
        .replace(/[^a-z0-9]/g, '-')
        .replace(/-+/g, '-')
        .replace(/^-|-$/g, '');
      this.companyForm.patchValue({ slug: generatedSlug });
      this.checkSlug();
    }
  }

  checkSlug(): void {
    const slug = this.companyForm.get('slug')?.value;
    if (!slug || this.companyForm.get('slug')?.invalid) {
      this.slugAvailable = null;
      return;
    }

    this.isCheckingSlug = true;
    this.saasService.checkSlugAvailability(slug).subscribe({
      next: (res: ApiResponse<boolean>) => {
        this.isCheckingSlug = false;
        this.slugAvailable = res.data;
        this.slugMessage = res.message;
      },
      error: () => {
        this.isCheckingSlug = false;
        this.slugAvailable = true;
        this.slugMessage = 'Workspace domain available.';
      }
    });
  }

  nextToAdmin(): void {
    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();
      return;
    }
    this.currentStep = 2;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  nextToOtp(): void {
    if (this.adminForm.invalid) {
      this.adminForm.markAllAsTouched();
      return;
    }

    this.sendVerificationOtp();
  }

  sendVerificationOtp(): void {
    const email = this.adminForm.get('adminEmail')?.value;
    this.isSendingOtp = true;

    this.authService.sendOtp({ email, purpose: 'TenantRegistration' }).subscribe({
      next: (res: ApiResponse<string>) => {
        this.isSendingOtp = false;
        this.otpSentMessage = res.message;
        this.currentStep = 3;
        this.startResendTimer();
        this.notification.success(`Verification code dispatched to ${email}`);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      },
      error: (err: any) => {
        this.isSendingOtp = false;
        const msg = err.error?.message || 'Failed to dispatch verification code.';
        this.notification.error(msg);
      }
    });
  }

  startResendTimer(): void {
    this.resendCountdown = 60;
    clearInterval(this.resendTimer);
    this.resendTimer = setInterval(() => {
      this.resendCountdown--;
      if (this.resendCountdown <= 0) {
        clearInterval(this.resendTimer);
      }
    }, 1000);
  }

  onDigitInput(event: any, nextIndex: number): void {
    const input = event.target as HTMLInputElement;
    if (input.value && nextIndex <= 6) {
      const nextInput = document.getElementById('otp-' + nextIndex);
      nextInput?.focus();
    }
  }

  getEnteredOtp(): string {
    const v = this.otpForm.value;
    return `${v.digit1}${v.digit2}${v.digit3}${v.digit4}${v.digit5}${v.digit6}`;
  }

  verifyOtpAndProceed(): void {
    const otp = this.getEnteredOtp();
    if (otp.length !== 6) {
      this.notification.warning('Please enter the full 6-digit verification code.');
      return;
    }

    const email = this.adminForm.get('adminEmail')?.value;
    this.authService.verifyOtp({ email, otp, purpose: 'TenantRegistration' }).subscribe({
      next: (res: ApiResponse<boolean>) => {
        if (res.success) {
          this.currentStep = 4;
          this.notification.success('Email confirmed. Please review your initial subscription tier.');
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }
      },
      error: (err: any) => {
        this.notification.error(err.error?.message || 'Invalid or expired verification code.');
      }
    });
  }

  submitFinalRegistration(): void {
    this.isSubmittingRegistration = true;

    const payload = {
      ...this.companyForm.value,
      ...this.adminForm.value,
      otpCode: this.getEnteredOtp(),
      planId: this.planForm.get('planId')?.value,
      billingCycle: this.planForm.get('billingCycle')?.value
    };

    this.saasService.registerTenant(payload).subscribe({
      next: (res: ApiResponse<TenantRegistrationResponse>) => {
        this.isSubmittingRegistration = false;
        if (res.success && res.data) {
          this.provisionedResult = res.data;
          this.currentStep = 5;
          this.notification.success('Your HR Atlas organisation workspace is ready!');
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }
      },
      error: (err: any) => {
        this.isSubmittingRegistration = false;
        const msg = err.error?.message || 'Registration failed. Please check form values.';
        this.notification.error(msg);
      }
    });
  }

  goToDashboard(): void {
    if (this.provisionedResult?.workspaceUrl) {
      window.location.href = this.provisionedResult.workspaceUrl;
    } else {
      this.router.navigate(['/']);
    }
  }
}
