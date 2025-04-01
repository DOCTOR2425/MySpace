import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../service/admin/admin.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-reports-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.scss',
})
export class ReportsPageComponent implements OnInit, OnDestroy {
  public selectedReportType: string = 'sales';
  public from!: string;
  public to!: string;

  private unsubscribe$ = new Subject<void>();
  constructor(private adminService: AdminService) {}

  public ngOnInit(): void {}

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public generateReport(): void {
    let from: string = '';
    let to: string = '';
    if (this.from == undefined) from = '1970-01-01';
    else from = this.from;
    if (this.to == undefined) {
      to = new Date(new Date().setDate(new Date().getDate() + 1))
        .toISOString()
        .split('T')[0];
    } else {
      to = this.to;
    }
    switch (this.selectedReportType) {
      case 'sales':
        this.generateExcelProductSalaringReport(from, to);
        break;
      case 'stock':
        this.generateExcelStockReport(from, to);
        break;
    }
  }

  private generateExcelStockReport(from: string, to: string) {
    this.adminService
      .generateExcelStockReport(from, to)
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

  private generateExcelProductSalaringReport(from: string, to: string) {
    this.adminService
      .generateReportSalesByCategoryOverTime(from, to)
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
}
