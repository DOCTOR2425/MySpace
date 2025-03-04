import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-reports-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.scss',
})
export class ReportsPageComponent {
  selectedReportType: string = 'sales';
  startDate: string = '';
  endDate: string = '';
  reportData: any = null;

  generateReport() {
    // Здесь можно добавить логику для генерации отчета
    this.reportData = {
      type: this.selectedReportType,
      startDate: this.startDate,
      endDate: this.endDate,
      data: [
        { product: 'Молоток', sales: 150 },
        { product: 'Отвертка', sales: 230 },
        { product: 'Дрель', sales: 75 },
      ],
    };
  }
}
