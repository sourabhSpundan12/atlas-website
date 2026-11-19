import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SaasService } from '../../core/services/saas.service';
import { NotificationService } from '../../core/services/notification.service';
import { ApiResponse } from '../../core/models/auth.model';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent implements OnInit {
  contactForm!: FormGroup;
  isSubmitting: boolean = false;
  isSubmitted: boolean = false;
  responseMsg: string = '';

  departments = [
    'Enterprise Sales & Pricing',
    'Customer Support & Helpdesk',
    'Custom Implementation & Integrations',
    'Channel Partnerships'
  ];

  constructor(
    private fb: FormBuilder,
    private saasService: SaasService,
    private notification: NotificationService
  ) {}

  ngOnInit(): void {
    this.contactForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, Validators.minLength(10)]],
      companyName: ['', [Validators.required]],
      companySize: ['16 - 50 Employees'],
      department: ['Enterprise Sales & Pricing', Validators.required],
      message: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  onSubmit(): void {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const val = this.contactForm.value;
    const payload = {
      fullName: val.fullName,
      email: val.email,
      phoneNumber: val.phoneNumber,
      companyName: val.companyName,
      companySize: val.companySize,
      message: `[Dept: ${val.department}] ${val.message}`,
      type: 'ContactEnquiry' as const
    };

    this.saasService.submitContactInquiry(payload).subscribe({
      next: (res: ApiResponse<boolean>) => {
        this.isSubmitting = false;
        this.isSubmitted = true;
        this.responseMsg = res.message || 'Message sent successfully!';
        this.notification.success('Enquiry submitted! Our team will get back to you shortly.');
      },
      error: (err: any) => {
        this.isSubmitting = false;
        this.notification.error(err.error?.message || 'Failed to submit enquiry.');
      }
    });
  }
}
