import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { SaasService } from '../../core/services/saas.service';
import { SubscriptionPlan } from '../../core/models/saas.model';
import { ApiResponse } from '../../core/models/auth.model';

interface FaqItem {
  question: string;
  answer: string;
  isOpen: boolean;
}

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './pricing.component.html',
  styleUrls: ['./pricing.component.scss']
})
export class PricingComponent implements OnInit {
  isAnnual: boolean = true;
  currency: 'INR' | 'USD' = 'INR';
  isLoading: boolean = false;
  plans: SubscriptionPlan[] = [];

  faqs: FaqItem[] = [
    {
      question: 'How does the 14-day free trial work?',
      answer: 'You get immediate access to all Growth tier features for 14 days without entering credit card details. You can invite your team, test real payroll runs, setup attendance policies, and evaluate the full system.',
      isOpen: true
    },
    {
      question: 'Can we change plans or employee seat limits later?',
      answer: 'Yes, absolutely. You can upgrade, downgrade, or add employee seat packs at any time directly from the Organisation Usage settings inside your HR Atlas dashboard with prorated billing.',
      isOpen: false
    },
    {
      question: 'Is employee and payroll data strictly isolated?',
      answer: 'Yes. Every organisation is isolated at the database level with cryptographic TenantId tokens and security boundaries. We maintain strict compliance with SOC 2, ISO 27001, and GDPR mandates.',
      isOpen: false
    },
    {
      question: 'Do you charge extra for PDF payslips or auto clock-out?',
      answer: 'No. Automated PDF payslip generation and auto clock-out rules are built-in features included in the plan subscription. There are zero hidden fees per payslip.',
      isOpen: false
    },
    {
      question: 'What payment methods do you accept?',
      answer: 'We accept all major credit cards, debit cards, NetBanking, UPI (India), ACH, and wire transfers for Enterprise annual billing.',
      isOpen: false
    }
  ];

  constructor(private saasService: SaasService, private router: Router) {}

  ngOnInit(): void {
    this.loadPlans();
  }

  loadPlans(): void {
    this.isLoading = true;
    this.saasService.getSubscriptionPlans().subscribe({
      next: (res: ApiResponse<SubscriptionPlan[]>) => {
        if (res.success && res.data) {
          this.plans = res.data;
        }
        this.isLoading = false;
      },
      error: () => {
        // Fallback default plans if API not yet running
        this.plans = [
          {
            id: 'starter',
            name: 'Starter',
            tagline: 'Ideal for agile startups and emerging teams scaling up their HR operations.',
            monthlyPriceINR: 1499,
            annualPriceINR: 1199,
            monthlyPriceUSD: 29,
            annualPriceUSD: 22,
            maxSeats: 15,
            isPopular: false,
            features: [
              'Up to 15 Active Employees',
              'Employee Lifecycle & Digital Profiles',
              'Attendance Tracking & Auto Clock-Out',
              'Leave & Comp-Off Management',
              'Company Broadcasts & Announcements',
              'Real-Time Web Notifications',
              'Standard Email Support (24-48h)'
            ],
            highlightBadges: ['Self-Serve', 'Quick Setup']
          },
          {
            id: 'growth',
            name: 'Growth',
            tagline: 'Complete workforce automation for scaling companies with advanced payroll & tracking.',
            monthlyPriceINR: 3999,
            annualPriceINR: 3199,
            monthlyPriceUSD: 79,
            annualPriceUSD: 62,
            maxSeats: 50,
            isPopular: true,
            features: [
              'Up to 50 Active Employees',
              'Everything in Starter, plus:',
              'Salary Structuring & Payroll Engine',
              'One-Click Automated PDF Payslips',
              'Timesheets & Project Billable Tracking',
              'Inventory & Hardware Asset Management',
              'Automated Employee Onboarding Workflows',
              'Priority Chat & Email Support'
            ],
            highlightBadges: ['Most Popular', 'Best Value']
          },
          {
            id: 'enterprise',
            name: 'Enterprise',
            tagline: 'Tailored governance, dedicated infrastructure, and unlimited scale for enterprises.',
            monthlyPriceINR: 9999,
            annualPriceINR: 7999,
            monthlyPriceUSD: 199,
            annualPriceUSD: 159,
            maxSeats: 1000,
            isPopular: false,
            features: [
              'Custom & Unlimited Employee Seats',
              'Everything in Growth, plus:',
              'Multi-Branch & Custom Policy Engines',
              'Dedicated Account Manager & 99.9% SLA',
              'Custom Role-Based Access Control (RBAC)',
              'Audit Logs & Compliance Reporting',
              'REST API & Custom Webhook Integrations',
              'Assisted Data Migration & Training'
            ],
            highlightBadges: ['Unlimited Scale', 'Custom SLA']
          }
        ];
        this.isLoading = false;
      }
    });
  }

  toggleBilling(annual: boolean): void {
    this.isAnnual = annual;
  }

  toggleCurrency(cur: 'INR' | 'USD'): void {
    this.currency = cur;
  }

  toggleFaq(index: number): void {
    this.faqs[index].isOpen = !this.faqs[index].isOpen;
  }

  selectPlan(planId: string): void {
    this.router.navigate(['/signup'], {
      queryParams: {
        plan: planId,
        billing: this.isAnnual ? 'Annual' : 'Monthly'
      }
    });
  }
}
