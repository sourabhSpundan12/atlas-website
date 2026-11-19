import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

interface FeatureModule {
  id: string;
  name: string;
  category: string;
  badge: string;
  icon: string;
  summary: string;
  highlights: string[];
  workflow: string[];
}

@Component({
  selector: 'app-features',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './features.component.html',
  styleUrls: ['./features.component.scss']
})
export class FeaturesComponent {
  selectedCategory: string = 'all';

  modules: FeatureModule[] = [
    {
      id: 'lifecycle',
      name: 'Employee Lifecycle Management',
      category: 'core',
      badge: 'Core Foundation',
      icon: 'fa-solid fa-users-viewfinder',
      summary: 'Centralized single source of truth for all employee data from pre-hire documentation to exit interviews.',
      highlights: [
        'Centralized digital employee profiles with custom data fields',
        'Secure document repository for government IDs, education, and contracts',
        'Probation tracking with automated 30/60/90-day confirmation reviews',
        'Promotion, salary hike, and department transfer audit history',
        'Structured digital offboarding and asset recovery checklists'
      ],
      workflow: ['Job Offer Accepted', 'Digital Profile Created', 'Document Verification', 'Role & Team Assignment', 'Lifecycle Milestones']
    },
    {
      id: 'attendance',
      name: 'Attendance and Auto Clock-Out',
      category: 'operations',
      badge: 'Automation Favorite',
      icon: 'fa-regular fa-clock',
      summary: 'Accurate time tracking with automated intelligent auto clock-out to eliminate forgotten punches and fake overtime.',
      highlights: [
        'Browser-based single-click punch in and punch out',
        'Configurable Auto Clock-Out rule based on company shift limits (e.g. 10 hrs)',
        'Shift management with flexible, rotational, and split schedules',
        'Late arrival and early departure deduction algorithms',
        'Seamless daily reconciliation with leave and holiday calendars'
      ],
      workflow: ['Shift Start Check-In', 'Active Hours Tracker', 'Threshold Exceeded Alert', 'Auto Clock-Out Execution', 'Timesheet Sync']
    },
    {
      id: 'leave',
      name: 'Leave and Comp-Off Management',
      category: 'operations',
      badge: 'Policy Compliant',
      icon: 'fa-solid fa-calendar-days',
      summary: 'Configurable multi-tier leave policies, balance accruals, and automated compensatory-off (comp-off) tracking.',
      highlights: [
        'Custom leave types: Casual, Sick, Earned, Paternity/Maternity, Unpaid',
        'Automatic Compensatory Off (Comp-Off) credits for weekend and holiday work',
        'Configurable sandwich rules, carry-forward limits, and encashment',
        'Multi-level hierarchical approval workflows with mobile-friendly approvals',
        'Real-time team absence calendar preventing understaffing'
      ],
      workflow: ['Leave Application', 'Balance & Rule Check', 'Manager Review', 'Auto Balance Deducted', 'Calendar Updated']
    },
    {
      id: 'timesheets',
      name: 'Timesheets and Project Tracking',
      category: 'operations',
      badge: 'Productivity',
      icon: 'fa-solid fa-business-time',
      summary: 'Track billable hours across multiple client projects with task-level granularity and manager approval chains.',
      highlights: [
        'Weekly and monthly timesheet logging mapped to client projects',
        'Billable vs non-billable hour classification',
        'Task-level activity descriptions and burn-rate tracking',
        'Timesheet submission locks and manager one-click sign-off',
        'Exportable CSV and PDF reports ready for client billing and invoicing'
      ],
      workflow: ['Assign Project/Task', 'Log Daily Hours', 'Submit Weekly Sheet', 'Manager Verification', 'Billable Invoicing']
    },
    {
      id: 'payroll',
      name: 'Salary and Payroll Engine',
      category: 'finance',
      badge: 'Zero-Error Payroll',
      icon: 'fa-solid fa-calculator',
      summary: 'Full-fledged payroll processing with automated tax calculation, statutory deductions, and bank payout files.',
      highlights: [
        'Customizable CTC structures: Basic, HRA, Special Allowance, Bonuses',
        'Dual Tax Regime support (Old Regime vs New Regime tax calculation)',
        'Automated statutory compliance: PF, ESI, Professional Tax (PT), and TDS',
        'Loss of Pay (LOP) integration directly linked with attendance punches',
        'One-click bank transfer batch file generation (NEFT/RTGS/IMPS formats)'
      ],
      workflow: ['Import Attendance & LOP', 'Compute Tax & Deductions', 'Run Payroll Batch', 'Approve Payouts', 'Bank File Dispatched']
    },
    {
      id: 'payslips',
      name: 'Automated PDF Payslips',
      category: 'finance',
      badge: 'Instant Delivery',
      icon: 'fa-solid fa-file-pdf',
      summary: 'Password-protected, compliant PDF payslip generation delivered automatically to employee email inboxes.',
      highlights: [
        'Automated bulk PDF generation for hundreds of employees in seconds',
        'Password-protection options (e.g. PAN + Birth Year combination)',
        'Custom branding with company logo, tax breakdown, and YTD balances',
        'Direct automated email dispatch upon payroll publication',
        'Employee self-service archive to download historical payslips anytime'
      ],
      workflow: ['Payroll Finalized', 'Generate Cryptographic PDFs', 'Secure Password Injection', 'Email Dispatch', 'Archive in Portal']
    },
    {
      id: 'inventory',
      name: 'Inventory and Asset Management',
      category: 'operations',
      badge: 'Hardware Governance',
      icon: 'fa-solid fa-boxes-stacked',
      summary: 'Track physical hardware, laptops, monitors, accessories, and digital software licenses across remote teams.',
      highlights: [
        'Hardware lifecycle tracking from purchase to retirement',
        'Serial number, model, warranty, and vendor invoice logging',
        'Digital asset allocation agreements with employee signature acknowledgment',
        'Asset maintenance history and repair status tracking',
        'Automated asset return checklists during offboarding'
      ],
      workflow: ['Procure Asset', 'Tag & Register', 'Assign to Employee', 'Digital Handover Ack', 'Offboarding Reclamation']
    },
    {
      id: 'onboarding-emails',
      name: 'Employee Onboarding Emails',
      category: 'engagement',
      badge: 'First Impressions',
      icon: 'fa-solid fa-envelope-open-text',
      summary: 'Automate warm, professional onboarding email cadences, portal invitations, and manager preparations.',
      highlights: [
        'Automated Day-0 welcome email with secure first-time login credentials',
        'Pre-boarding document upload instructions and checklist links',
        'Manager preparation checklist sent 3 days before start date',
        'Company culture guides and team introduction announcements',
        'Scheduled 30-day onboarding feedback survey dispatch'
      ],
      workflow: ['Candidate Hired', 'Trigger Welcome Sequence', 'Credentials Dispatched', 'Day-1 Instructions', 'Check-in Survey']
    },
    {
      id: 'broadcasts',
      name: 'Corporate Broadcasts',
      category: 'engagement',
      badge: 'Company Culture',
      icon: 'fa-solid fa-bullhorn',
      summary: 'Publish organizational announcements, policy updates, CEO letters, and holiday notices with read confirmations.',
      highlights: [
        'Targeted broadcasts: Entire company, specific branches, or departments',
        'Priority tagging: General notice, Important update, or Emergency alert',
        'Mandatory read-receipt tracking to prove policy delivery',
        'Rich media formatting with attachments, links, and PDF documents',
        'Historical broadcast archive accessible on employee home feeds'
      ],
      workflow: ['Draft Announcement', 'Select Target Audience', 'Set Priority Level', 'Broadcast Published', 'Read Acknowledgments']
    },
    {
      id: 'notifications',
      name: 'Real-Time Notifications',
      category: 'engagement',
      badge: 'Instant Sync',
      icon: 'fa-solid fa-bell',
      summary: 'Instantaneous multi-channel alerts for leave approvals, auto clock-out triggers, payslip releases, and birthdays.',
      highlights: [
        'In-app notification center with instant unread badges',
        'Push notifications for critical manager approval bottlenecks',
        'Real-time alerts when auto clock-out or shift anomalies occur',
        'Notification preference matrix allowing employees to customize channels',
        'Automated birthday and work anniversary celebratory banners'
      ],
      workflow: ['System Event Triggered', 'Evaluate User Subscriptions', 'Deliver In-App Alert', 'Push Notification', 'Mark as Read']
    }
  ];

  get filteredModules(): FeatureModule[] {
    if (this.selectedCategory === 'all') return this.modules;
    return this.modules.filter(m => m.category === this.selectedCategory);
  }

  filterCategory(cat: string): void {
    this.selectedCategory = cat;
  }
}

