import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SaasService } from '../../core/services/saas.service';
import { NotificationService } from '../../core/services/notification.service';
import { ApiResponse } from '../../core/models/auth.model';

@Component({
  selector: 'app-book-demo',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './book-demo.component.html',
  styleUrls: ['./book-demo.component.scss']
})
export class BookDemoComponent implements OnInit {
  demoForm!: FormGroup;
  isSubmitting: boolean = false;
  isSubmitted: boolean = false;
  successMessage: string = '';

  availableModules = [
    { id: 'Attendance', label: 'Attendance & Auto Clock-Out' },
    { id: 'Payroll', label: 'Salary & Payroll Processing' },
    { id: 'Payslips', label: 'Automated PDF Payslips' },
    { id: 'Leave', label: 'Leave & Comp-Off Management' },
    { id: 'Lifecycle', label: 'Employee Lifecycle & Vault' },
    { id: 'Timesheets', label: 'Project Timesheets & Billing' },
    { id: 'Inventory', label: 'Hardware & Asset Inventory' },
    { id: 'Onboarding', label: 'Automated Onboarding Emails' }
  ];

  companySizes = [
    '1 - 25 Employees',
    '26 - 100 Employees',
    '101 - 500 Employees',
    '500+ Employees'
  ];

  selectedModules: string[] = ['Attendance', 'Payroll'];

  constructor(
    private fb: FormBuilder,
    private saasService: SaasService,
    private notification: NotificationService
  ) {}

  ngOnInit(): void {
    this.demoForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.minLength(10)]],
      companyName: ['', [Validators.required, Validators.minLength(2)]],
      companySize: ['26 - 100 Employees', Validators.required],
      preferredDate: [''],
      message: ['']
    });
  }

  toggleModule(modId: string): void {
    const idx = this.selectedModules.indexOf(modId);
    if (idx > -1) {
      this.selectedModules.splice(idx, 1);
    } else {
      this.selectedModules.push(modId);
    }
  }

  onSubmit(): void {
    if (this.demoForm.invalid) {
      this.demoForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const payload = {
      ...this.demoForm.value,
      modulesOfInterest: this.selectedModules,
      type: 'DemoRequest' as const
    };

    this.saasService.submitContactInquiry(payload).subscribe({
      next: (res: ApiResponse<boolean>) => {
        this.isSubmitting = false;
        this.isSubmitted = true;
        this.successMessage = res.message || 'Demo request scheduled successfully!';
        this.notification.success('Demo request received! Our solution architect will connect with you.');
      },
      error: (err: any) => {
        this.isSubmitting = false;
        this.notification.error(err.error?.message || 'Failed to submit demo request.');
      }
    });
  }
}
