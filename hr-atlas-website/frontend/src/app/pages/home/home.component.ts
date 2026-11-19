import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  activePreviewTab: 'attendance' | 'payroll' | 'lifecycle' | 'inventory' = 'attendance';
  calculatorSeats: number = 35;

  get hoursSavedPerMonth(): number {
    return Math.round(this.calculatorSeats * 0.85);
  }

  get costSavingsINR(): number {
    return Math.round(this.calculatorSeats * 450);
  }

  setPreviewTab(tab: 'attendance' | 'payroll' | 'lifecycle' | 'inventory'): void {
    this.activePreviewTab = tab;
  }
}

