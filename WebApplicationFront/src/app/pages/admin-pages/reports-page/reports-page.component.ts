import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable, Subject, takeUntil } from 'rxjs';
import { ReportService } from '../../../service/report/report.service';
import { ToastService } from '../../../service/toast/toast.service';

@Component({
  selector: 'app-reports-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './reports-page.component.html',
  styleUrl: './reports-page.component.scss',
})
export class ReportsPageComponent implements OnInit, OnDestroy {
  public selectedReportType: string = 'sales';
  public selectedTimeSpan: string = 'all';
  public reportName: string = 'report';
  public from!: string;
  public to!: string;

  private unsubscribe$ = new Subject<void>();
  constructor(
    private reportService: ReportService,
    private toastService: ToastService
  ) {}

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
        this.subscription(
          this.reportService.generateReportSalesByCategoryOverTime(from, to)
        );
        break;
      case 'stock':
        this.subscription(
          this.reportService.generateExcelStockReport(from, to)
        );
        break;
      case 'orders':
        this.subscription(this.reportService.generateOrdersReport(from, to));
        break;
      case 'usersProfit':
        this.subscription(
          this.reportService.generateProfitFromUsersReport(from, to)
        );
        break;
      case 'popylarProducts':
        let fromYear = '';
        let toYear = '';
        if (this.selectedTimeSpan == 'all') {
          fromYear = '2000-01-01';
          toYear = new Date(new Date().setDate(new Date().getDate() + 1))
            .toISOString()
            .split('T')[0];
        } else if (this.selectedTimeSpan == 'current') {
          fromYear = new Date(new Date().getFullYear(), 1, 1)
            .toISOString()
            .split('T')[0];
          toYear = new Date(new Date().getFullYear(), 11, 31)
            .toISOString()
            .split('T')[0];
        } else {
          fromYear = new Date(new Date().getFullYear() - 1, 1, 1)
            .toISOString()
            .split('T')[0];
          toYear = new Date(new Date().getFullYear() - 1, 11, 31)
            .toISOString()
            .split('T')[0];
        }
        this.subscription(
          this.reportService.generatePopylarProductsBySeasonsReport(
            fromYear,
            toYear
          )
        );
        break;
    }
  }

  private subscription(observable: Observable<Blob>) {
    observable.pipe(takeUntil(this.unsubscribe$)).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = this.reportName + '.xlsx';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      },
      error: (err) => {
        this.toastService.showError(
          'Не удалось сгенерировать отчет. Попробуйте еще раз.'
        );
      },
    });
  }

  public onReportTypeChange(event: any) {
    const selectedOption = event.target.options[event.target.selectedIndex];
    this.reportName = selectedOption.text;
  }
}
