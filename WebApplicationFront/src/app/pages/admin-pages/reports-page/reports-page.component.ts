import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../service/admin/admin.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-reports-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.scss',
})
export class ReportsPageComponent implements OnDestroy {
  selectedReportType: string = 'sales';
  public from!: Date;
  public to!: Date;
  private unsubscribe$ = new Subject<void>();

  constructor(private adminService: AdminService) {}

  public generateReport(): void {
    switch (this.selectedReportType) {
      case 'sales':
        this.generateExcelProductSalaringReport();
        break;
      case 'popularity':
        this.generateWordReport();
        break;
    }
  }

  private generateWordReport() {
    this.adminService
      .generateWordReport(this.from, this.to)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (blob: Blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = 'report.docx';
          document.body.appendChild(a);
          a.click();
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
        },
        error: (err) => {
          console.error('Ошибка при генерации отчета:', err);
          alert('Не удалось сгенерировать отчет. Попробуйте еще раз.');
        },
      });
  }

  private generateExcelProductSalaringReport() {
    this.adminService
      .generateReportSalesByCategoryOverTime(this.from, this.to)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (blob: Blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = 'report.xlsx';
          document.body.appendChild(a);
          a.click();
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
        },
        error: (err) => {
          console.error('Ошибка при генерации отчета:', err);
          alert('Не удалось сгенерировать отчет. Попробуйте еще раз.');
        },
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
